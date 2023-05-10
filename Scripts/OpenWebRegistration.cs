using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWebRegistration : MonoBehaviour
{
    public string registrationUrl =  "https:// you_web_page_registration.com";

    public void OpenRegistration()
    {
        Application.OpenURL(registrationUrl);
    }
}
