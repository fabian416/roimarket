using System;
using UnityEngine;
using UnityEngine.UI;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;


public class CognitoAuthentication : MonoBehaviour
{
    public Button loginButton;
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;

    public string awsRegion = RegionEndpoint.SAEast1.SystemName;
    public string cognitoUserPoolId = "tu_user_pool_id";
    public string cognitoAppClientId = "tu_app_client_id";

    private AmazonCognitoIdentityProviderClient _provider;
    private CognitoUserPool _userPool;

    // Static propperty to store the user email 
    public static string UserEmail { get; private set; }


    private void Start()
        {
            _provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), RegionEndpoint.GetBySystemName(awsRegion));
            _userPool = new CognitoUserPool(cognitoUserPoolId, cognitoAppClientId, _provider);

            loginButton.onClick.AddListener(() => SignIn(usernameInputField.text, passwordInputField.text));
        }

    private async void SignIn(string username, string password)
{
    try
    {

        CognitoUser user = new CognitoUser(username, cognitoAppClientId, _userPool, _provider);
        AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest

        {
            Password = password
        });

        if (authResponse.AuthenticationResult != null)
        {
            Debug.Log("Autenticación exitosa");

            // Get the user email  
            GetUserRequest getUserRequest = new GetUserRequest();
            getUserRequest.AccessToken = authResponse.AuthenticationResult.AccessToken;
            GetUserResponse getUserResponse = await _provider.GetUserAsync(getUserRequest);

            string userEmail = string.Empty;

            foreach (AttributeType attribute in getUserResponse.UserAttributes)
            {
                if (attribute.Name == "email")
                {
                    userEmail = attribute.Value;
                    break;
                }
            }


            User currentUser = new User { email = userEmail };
            ManageUser.Instance.CurrentUser = currentUser;

            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.LogError("Error en la autenticación");
        }
    }
    catch (Exception e)
    {
        Debug.LogError($"Error al autenticar al usuario: {e.Message}");
    }
}




}
