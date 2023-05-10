using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using UnityEngine.UI;



public class JsonWriter
{
    private StringBuilder sb;
    private int indent;

    public JsonWriter()
    {
        sb = new StringBuilder();
    }

    public void WriteObjectStart()
    {
        sb.Append("{\n");
        indent++;
    }

    public void WriteObjectEnd()
    {
        RemoveTrailingComma();
        indent--;
        sb.Append(new string(' ', indent * 2));
        sb.Append("}");
    }

    public void WriteArrayStart()
    {
        sb.Append("[\n");
        indent++;
    }

    public void WriteArrayEnd()
    {
    indent--;
    RemoveTrailingComma();
    sb.Append(new string(' ', indent * 2));
    sb.Append("]");
    }

    public void AppendComma()
    {
    sb.Append(",\n");
    }


    public void WriteString(string name, string value)
    {
        sb.Append(new string(' ', indent * 2));
        sb.Append("\"");
        sb.Append(name);
        sb.Append("\": \"");
        sb.Append(value);
        sb.Append("\",\n");
    }

    public void WritePropertyName(string name)
    {
    sb.Append(new string(' ', indent * 2));
    sb.Append("\"");
    sb.Append(name);
    sb.Append("\": ");
    }


    public void WriteInt(string name, int value)
    {
        sb.Append(new string(' ', indent * 2));
        sb.Append("\"");
        sb.Append(name);
        sb.Append("\": ");
        sb.Append(value);
        sb.Append(",\n");
    }

    public void RemoveTrailingComma()
    {
        if (sb[sb.Length - 2] == ',')
        {
            sb.Remove(sb.Length - 2, 1);
        }
    }

    public override string ToString()
    {
        return sb.ToString();
    }
}


[Serializable]
public class PaymentResponse
{
    public string status;
    public string message;
    public string init_point;
    public string mp_error;
}

public class CartItem
{
    public Product product;
    public int product_id;
    public int quantity;
}

[Serializable]
public class SerializableCartItem
{
    public int product_id;
    public int quantity;
}

public class PaymentManager : MonoBehaviour
{
    //public WebViewObject webViewObject;
    
    private Action<bool> _onComplete;

    /*private void Start()
    {
        webViewObject.OnWebViewClosed += OnWebViewClosed;
    }
    /*
    private void OnWebViewClosed()
    {
        Debug.Log("WebView closed");
        _onComplete(true);
    }
    */
    public IEnumerator MakePayment(string userEmail, List<CartItem> cartItems, Action<bool> onComplete)
    {
    Debug.Log("MakePayment called");

    _onComplete = onComplete;

    string url = "https://54.94.186.91/api/pay";

    List<SerializableCartItem> serializableCartItems = cartItems.ConvertAll(item => new SerializableCartItem { product_id = item.product_id, quantity = item.quantity });

    JsonWriter jsonWriter = new JsonWriter();
    jsonWriter.WriteObjectStart();
    jsonWriter.WriteString("payer_email", userEmail);
    jsonWriter.WritePropertyName("cart_items"); 
    jsonWriter.WriteArrayStart();

    for (int i = 0; i < serializableCartItems.Count; i++)
    {
        jsonWriter.WriteObjectStart();
        jsonWriter.WriteInt("product_id", serializableCartItems[i].product_id);
        jsonWriter.WriteInt("quantity", serializableCartItems[i].quantity);
        jsonWriter.WriteObjectEnd();
        if (i < serializableCartItems.Count -1)
        {
            jsonWriter.AppendComma();
        }
    
    }

    jsonWriter.WriteArrayEnd();
    jsonWriter.WriteObjectEnd();
    jsonWriter.RemoveTrailingComma();

    string jsonRequestBody = jsonWriter.ToString();
    Debug.Log("JSON Request Body: " + jsonRequestBody);

    UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequestBody);
    www.uploadHandler = new UploadHandlerRaw(bodyRaw);
    www.downloadHandler = new DownloadHandlerBuffer();
    www.SetRequestHeader("Content-Type", "application/json");
    www.certificateHandler = new AcceptAllCertificates();

    Debug.Log("Request sent");
    yield return www.SendWebRequest();

    if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
    {
        Debug.LogError(www.error);
        Debug.LogError("Response: " + www.downloadHandler.text);
        PaymentResponse paymentResponse = JsonUtility.FromJson<PaymentResponse>(www.downloadHandler.text);
        Debug.LogError("Error de Mercado Pago: " + paymentResponse.mp_error);
        onComplete(false);
    }
    else
    {
        Debug.Log("Pago exitoso");
        Debug.Log("Response: " + www.downloadHandler.text);
        PaymentResponse paymentResponse = JsonUtility.FromJson<PaymentResponse>(www.downloadHandler.text);
        string initPoint = paymentResponse.init_point;

        Debug.Log("init_point URL: " + initPoint);
        
        Application.OpenURL(initPoint);
        //webViewObject.LoadURL(initPoint);
        //webViewObject.SetVisibility(true);
    }
}

}

public class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Devuelve true para aceptar todos los certificados, incluso los autofirmados
        return true;
    }
}
