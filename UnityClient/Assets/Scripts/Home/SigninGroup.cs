using UnityEngine;
using UnityEngine.UI;

public class SigninGroup : MonoBehaviour
{
    public InputField username, password;
    public Button loginBtn;
    public Text errorText;

    public event OnLogin OnLoginE;
    public delegate void OnLogin(string username, string password);
    
    void Awake()
    {
        errorText.gameObject.SetActive(false);
        loginBtn.onClick.AddListener(() =>
        {
            if (OnLoginE != null)
            {
                OnLoginE(username.text, password.text);
            }
        });
    }
}
