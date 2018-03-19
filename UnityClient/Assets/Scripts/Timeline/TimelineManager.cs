using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class TimelineManager : MonoBehaviour
{

    public static TimelineInfo timelineInfo;
    private static readonly List<TimePoint> timelineList = new List<TimePoint>();

    public Dropdown categoryDropdown;
    public ScrollRect scrollView;
    public TimelineScaleManager timelineScaleManager;
    public Button mapBtn;
    public TimePoint timePointPrefab;


	void Start ()
	{
        SceneLoader.Instance.CameraBlack();
	    Time.timeScale = 0;
	    if (timelineInfo == null)
	    {
	        NetworkManager.Instance.GetTimelineInfo(info =>
	        {
                Debug.Log("received!");
	            timelineInfo = info;
                Init();
	        });
        }
	    else
	    {
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
                var list = timelineInfo.Categories.Select(cat => cat.name).ToList();
                categoryDropdown.AddOptions(list);
            }
            categoryDropdown.onValueChanged.AddListener(index =>
            {
                Debug.Log(index);
                //JumpToCategory(index);
            });

            //JumpToCategory(0);

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
        throw new NotImplementedException();
    }

    /*public void LoadCategory(int index)
    {
        //This logic is based on assumption that the list is ordered by order number
        var list = timelineInfo.Categories[index].timeline;
        int order = -1;
        int i = 0;
        TimePoint current = null;
        foreach (var data in list)
        {
            if (order != data.order)
            {
                order = data.order;
                current = GetOrCreateTimePoint(i);
                current.ResetData();
                i++;
            }
            current.SetData(data);
            current.gameObject.SetActive(true);
        }
        for (int j = i; j < timelineList.Count; j++)
        {
            timelineList[j].gameObject.SetActive(false);
        }

        scrollView.verticalNormalizedPosition = 1;
    }*/

    private TimePoint GetOrCreateTimePoint(int index)
    {
        Assert.IsFalse(index > timelineList.Count);
        TimePoint result;
        if (index == timelineList.Count)
        {
            result = Instantiate(timePointPrefab, scrollView.content);
            timelineList.Add(result);
        }
        else
        {
            result = timelineList[index];
        }
        return result;
    }
}
