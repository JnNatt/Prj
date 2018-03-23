using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class TimelineManager : MonoBehaviour
{

    public static TimelineInfo timelineInfo;
    private static readonly List<TimePoint> timelineList = new List<TimePoint>();
    public static List<TimePoint> TimelineList { get { return timelineList; } }

    public Dropdown categoryDropdown;
    public ScrollRect scrollView;
    public TimelineScaleManager timelineScaleManager;
    public Button mapBtn;
    public TimePoint timePointPrefab;

    public DetailPopup popup;

	void Start ()
	{
        SceneLoader.Instance.CameraBlack();
	    //Time.timeScale = 0;
	    if (timelineInfo == null)
	    {
            Debug.Log("Getting data from server...");
	        NetworkManager.Instance.GetTimelineInfo(info =>
	        {
                Debug.Log("received!");
	            timelineInfo = info;
                Init();
	        });
        }
	    else
	    {
	        Debug.Log("Already got data from the server.");
            Init();
        }
	}

    void Init()
    {
        try
        {
            timelineScaleManager.Init();
            categoryDropdown.ClearOptions();
            if (timelineInfo != null && timelineInfo.Categories != null)
            {
                timelineScaleManager.InitTimepoint(timelineInfo.Categories);
                var list = timelineInfo.Categories.Select(cat => cat.name).ToList();
                categoryDropdown.AddOptions(list);
            }
            categoryDropdown.onValueChanged.AddListener(index =>
            {
                Debug.Log(index);
                JumpToCategory(index);
            });
            timelineScaleManager.OnTimepointClickE += ShowPopup;

            JumpToCategory(0);

            SceneLoader.Instance.FadeIn(.5f);
        }
        catch (Exception e)
        {
            Debug.LogError("There is some error in callback function : " + e.Message + e.StackTrace);
            throw;
        }
        
    }

    private void JumpToCategory(int index)
    {
        timelineScaleManager.JumpToCategory(index);
    }

    private void ShowPopup(TimepointData data)
    {
        popup.SetData(data);
        popup.ShowPopup();
    }
}
