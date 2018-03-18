using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static GameObject cameraFade;

    private static SceneLoader instance;
	// Use this for initialization
	void Awake () {
	    if (instance && instance != this)
	    {
	        Destroy(instance);
	    }
	    instance = this;
        DontDestroyOnLoad(this);
	}

    public static SceneLoader Instance
    {
        get
        {
            if (!instance)
            {
                var sceneLoader = new GameObject("SceneLoader", typeof(SceneLoader)).GetComponent<SceneLoader>();
                instance = sceneLoader;
            }
            return instance;
        }
    }

    public static GameObject CameraFadeAdd()
    {
        return CameraFadeAdd(null);
    }

    public static GameObject CameraFadeAdd(Texture2D texture)
    {
        if (cameraFade)
        {
            return null;
        }

        cameraFade = new GameObject("Camera Fade", typeof(Canvas), typeof(RawImage));
        Canvas canvas = cameraFade.GetComponent<Canvas>();
        canvas.sortingOrder = 999;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        RawImage img = cameraFade.GetComponent<RawImage>();
        if (!texture)
        {
            img.color = new Color(0f, 0f, 0f, 0);
        }
        else
        {
            img.texture = texture;
            img.color = new Color(.5f, .5f, .5f, 0);
        }
        return cameraFade;
    }

    public static void LoadScene(string sceneName, float time = 1f) { Instance.ChangeScene(sceneName, time);}

    public void ChangeScene(string sceneName)
    {
        ChangeScene(sceneName, 1f);
    }
    public void ChangeScene(string sceneName, float time)
    {
        CameraFadeTo(1f, time, () =>
        {
            SceneManager.LoadScene(sceneName);
            Time.timeScale = 1f;
        });
    }

    public void FadeIn(float time = 1f)
    {
        if (!cameraFade) CameraFadeAdd();
        Time.timeScale = 0f;
        CameraFadeTo(1f, 0f, delegate
        {
            CameraFadeTo(0f, 1f, delegate
            {
                Time.timeScale = 1f;
            });
        });
    }

    public void CameraFadeTo(float amount, float time, Action callBack = null)
    {
        if (!cameraFade) CameraFadeAdd();
        /*var tween = LeanTween.alpha((RectTransform)cameraFade.transform, amount, time).setIgnoreTimeScale(true);
        if (callBack != null)
        {
            tween.setOnComplete(callBack);
        }*/
        StartCoroutine(_TweenAlpha(amount, time, callBack));
    }

    public void CameraBlack()
    {
        if (!cameraFade) CameraFadeAdd();

        RawImage img = cameraFade.GetComponent<RawImage>();
        img.color = Color.black;
    }

    private IEnumerator _TweenAlpha(float amount, float time, Action callBack)
    {
        RawImage img = cameraFade.GetComponent<RawImage>();
        
        if (time > 0)
        {
            var count = time;
            var initialValue = img.color.a;
            var d = amount - img.color.a;
            do
            {
                yield return 0;
                count -= Time.unscaledDeltaTime;
                var c = new Color(img.color.r, img.color.g, img.color.b, initialValue + d * (1 - count / time));
                img.color = c;
            } while (count > 0);
        }
        else
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, amount);
        }
        
        if (callBack != null)
        {
            callBack();
        }
    }
}
