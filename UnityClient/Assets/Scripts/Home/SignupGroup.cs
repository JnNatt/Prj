using UnityEngine;
using UnityEngine.UI;

public class SignupGroup : MonoBehaviour
{
    public InputField username, email, password;
    public Button registerBtn;
    public Text errorTextEmail, errorTextUsername, errorTextPassword, errorText;

    public event OnRegister OnRegisterE;
    public delegate void OnRegister(string email, string username, string password);

    void Awake()
    {
        ResetErrorText();
        registerBtn.onClick.AddListener(() =>
        {
            if (OnRegisterE != null)
            {
                OnRegisterE(email.text, username.text, password.text);
            }
        });
    }

    public void ResetErrorText()
    {
        errorTextEmail.gameObject.SetActive(false);
        errorTextUsername.gameObject.SetActive(false);
        errorTextPassword.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
    }
}
