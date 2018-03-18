using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

public class TimelineScaleManager : MonoBehaviour
{
    public ScaleGroup scaleGroupPref;

    public RectTransform TimeScaleGroup;

    public RectTransform TimePointGroup;

    public RectTransform Line;

    private ScrollRect scrollView;
	void Start ()
	{
	    scrollView = GetComponent<ScrollRect>();

	    
    }
	
	// Update is called once per frame
    void LateUpdate()
    {
        var height = TimeScaleGroup.rect.height;
        //scrollView.content.sizeDelta = new Vector2(scrollView.content.sizeDelta.x, height);
        Line.sizeDelta = new Vector2(Line.sizeDelta.x, height);
    }
}
