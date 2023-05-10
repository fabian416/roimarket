using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageUser : MonoBehaviour
{
    public static ManageUser Instance;

    public User CurrentUser;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

public class User
{
    public string email;
}

