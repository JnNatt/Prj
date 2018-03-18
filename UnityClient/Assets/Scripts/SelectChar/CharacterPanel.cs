using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private InputField charName;
    [SerializeField] private GameObject selection;

    public Characters CharId;
    public string CharName { get { return charName.text; } }

    public static CharacterPanel SelectedChar;
    public static event Action<CharacterPanel> OnSelectE; 

	void Start () {
	    btn.onClick.AddListener(() =>
	    {
	        SelectedChar = this;
            selection.SetActive(true);
	        if (OnSelectE != null)
	        {
	            OnSelectE(this);
	        }
	    });
	}

    void OnEnable()
    {
        OnSelectE += OnSelect;
    }

    void OnDisable()
    {
        OnSelectE -= OnSelect;
    }

    private void OnSelect(CharacterPanel panel)
    {
        if (panel != this)
        {
            selection.SetActive(false);
        }
    }
}
