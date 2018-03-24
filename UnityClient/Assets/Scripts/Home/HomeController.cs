using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HomeController : MonoBehaviour
{
    public GameObject MenuGroup, objGroup;
    public SigninGroup SigninGroup;
    public SignupGroup SignupGroup;
    public Button SigninBtn, SignupBtn, BackBtn;

    void Awake()
    {
        SigninGroup.username.text = String.Empty;
        SigninGroup.password.text = String.Empty;
        SignupGroup.username.text = String.Empty;
        SignupGroup.email.text = String.Empty;
        SignupGroup.password.text = String.Empty;
        ShowMenuPage();

        SigninGroup.OnLoginE += OnLogin;
        SignupGroup.OnRegisterE += OnRegister;
    }
    public void ShowSignupPage()
    {
        MenuGroup.SetActive(false);
        SignupGroup.gameObject.SetActive(true);
        objGroup.SetActive(true);
    }

    public void ShowSigninPage()
    {
        MenuGroup.SetActive(false);
        SigninGroup.gameObject.SetActive(true);
        objGroup.SetActive(true);
    }

    public void ShowMenuPage()
    {
        SigninGroup.gameObject.SetActive(false);
        SignupGroup.gameObject.SetActive(false);
        objGroup.SetActive(false);
        MenuGroup.SetActive(true);
    }

    private void OnLogin(string username, string password)
    {
        SigninGroup.errorText.gameObject.SetActive(false);
        SigninGroup.loginBtn.interactable = false;
        NetworkManager.Instance.Login(username, password, error =>
        {
            if (String.IsNullOrEmpty(error))
            {
                SceneLoader.LoadScene("SelectChar", .75f);
            }
            else
            {
                SigninGroup.errorText.text = error;
                SigninGroup.errorText.gameObject.SetActive(true);
                SigninGroup.loginBtn.interactable = true;
            }
        });
    }

    private void OnRegister(string email, string username, string password)
    {
        SignupGroup.ResetErrorText();
        SignupGroup.registerBtn.interactable = false;
        NetworkManager.Instance.Register(email, username, password, error =>
        {
            if (error != null)
            {
                SignupGroup.registerBtn.interactable = true;
                if (!String.IsNullOrEmpty(error.error))
                {
                    SignupGroup.errorText.text = error.error;
                    SignupGroup.errorText.gameObject.SetActive(true);
                }
                else
                {
                    if (error.Email.Any())
                    {
                        SignupGroup.errorTextEmail.text = error.Email.First();
                        SignupGroup.errorTextEmail.gameObject.SetActive(true);
                    }
                    if (error.Username.Any())
                    {
                        SignupGroup.errorTextUsername.text = error.Username.First();
                        SignupGroup.errorTextUsername.gameObject.SetActive(true);
                    }
                    if (error.Password.Any())
                    {
                        SignupGroup.errorTextPassword.text = error.Password.First();
                        SignupGroup.errorTextPassword.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                OnLogin(username, password);
            }
        });
    }
}
