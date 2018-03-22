using UnityEngine;
using UnityEngine.UI;

public class SelectCharController : MonoBehaviour
{
    public Text warningTxt;
    public Button okBtn;

    [SerializeField] private string nextScene;

	void Start ()
	{
	    warningTxt.gameObject.SetActive(false);
        CharacterPanel.OnSelectE += (panel) =>
	    {
            warningTxt.gameObject.SetActive(false);
	    };
        okBtn.onClick.AddListener(() =>
        {
            var player = CharacterPanel.SelectedChar;
            if (!CheckPlayer(player))
            {
                warningTxt.gameObject.SetActive(true);
                return;
            }
            //TODO: implement logic to call to the server and save player setting
            PlayerPrefs.SetInt("charId", (int)player.CharId);
            PlayerPrefs.SetString("playername", player.CharName);
            SceneLoader.LoadScene(nextScene);
        });
	}

    bool CheckPlayer(CharacterPanel player)
    {
        if (player == null) return false;

        return true;
    }
}
