using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFadeIn : MonoBehaviour
{
    public float Duration = 1f;

	void Start () {
		SceneLoader.Instance.FadeIn(Duration);
	}
}
