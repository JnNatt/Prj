using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAvartar : MonoBehaviour
{
    [SerializeField] private Image picture;
    [SerializeField] private Text name;

    [SerializeField] private List<Sprite> characterPictures;
    void Awake ()
    {
        picture.sprite = characterPictures[PlayerPrefs.GetInt("charId")];
        name.text = PlayerPrefs.GetString("playername");
    }

}
