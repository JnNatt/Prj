using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionIcon : MonoBehaviour
{
    public float ScaleTarget = 1.25f;
    public float TimeDuration = .5f;
    
	// Update is called once per frame
	void Update ()
	{
	    var defaultScale = 1f;
        /*if (Mathf.Abs(transform.localScale.x - defaultScale) < 0.001f)
	    {
	        inverse = false;
	    }
	    else if (Mathf.Abs(transform.localScale.x - ScaleTarget) < 0.001f)
	    {
	        inverse = true;
	    }*/
	    var t = Time.time % TimeDuration;
	    var inverse = (int)(Time.time / TimeDuration) % 2 == 1;
	    var startScale = inverse ? ScaleTarget : defaultScale;
	    var d = ScaleTarget - defaultScale;
	    
	    var x = startScale + d * (1 - t / TimeDuration) * (inverse ? -1 : 1);
	    transform.localScale = Vector3.one * x;
	}
}
