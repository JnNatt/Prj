using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeSceneBtn : MonoBehaviour, IPointerClickHandler
{
    public string SceneName;
    public float fadeOutDuration = .5f;

    public bool UnityButton = true;
    public event Action OnClickE;
	void Start () {
	    if (UnityButton)
	    {
	        GetComponent<Button>().onClick.AddListener(() =>
	        {
	            SceneLoader.LoadScene(SceneName, fadeOutDuration);
	        });
        }
	    else
	    {
	        OnClickE += () =>
	        {
	            SceneLoader.LoadScene(SceneName, fadeOutDuration);
	        };
	    }
	}

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (OnClickE != null)
        {
            OnClickE();
        }
    }
}
