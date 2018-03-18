using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignupGroup : MonoBehaviour
{
    public InputField username, email, password;
    public Button registerBtn;
    public Text errorTextEmail, errorTextUsername, errorTextPassword;

    public event OnRegister OnRegisterE;
    public delegate void OnRegister(string email, string username, string password);

    void Awake()
    {
        errorTextEmail.gameObject.SetActive(false);
        errorTextUsername.gameObject.SetActive(false);
        errorTextPassword.gameObject.SetActive(false);
        registerBtn.onClick.AddListener(() =>
        {
            if (OnRegisterE != null)
            {
                OnRegisterE(email.text, username.text, password.text);
            }
        });
    }
}
