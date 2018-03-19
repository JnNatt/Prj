using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineScaleManager : MonoBehaviour
{
    public ScaleGroup scaleGroupPref;

    public RectTransform TimeScaleGroup;

    public RectTransform TimePointGroup;

    public RectTransform Line;

    private ScrollRect scrollView;
    [SerializeField]
    private float offset = 400;

    [SerializeField] private string beforeHistoryFormat = "{0} ปีก่อนปัจจุบัน";
    [SerializeField] private string historyFormat = "ค.ศ.{0}";
    [SerializeField] private List<Abbreviation> abbreForNumber;

    [SerializeField] private int startPoint = -3000000;
    [SerializeField][Tooltip("If data is longer than endpoint, this will be ignored")] private int endPoint = 2025;
    [SerializeField] private List<ScaleElement> scaleSetting;
	void Start ()
	{
	    scrollView = GetComponent<ScrollRect>();

        GenerateScale();

	    LayoutRebuilder.ForceRebuildLayoutImmediate(TimeScaleGroup);
        var height = TimeScaleGroup.rect.height;
	    scrollView.content.sizeDelta = new Vector2(scrollView.content.sizeDelta.x, height);
	    Line.sizeDelta = new Vector2(Line.sizeDelta.x, height + offset);
    }

    void GenerateScale()
    {
        int current = startPoint;
        int count = scaleSetting.Count;
        bool limitHit = false;
        for (var i = 0; i < count; i++)
        {
            var scaleElement = scaleSetting[i];
            if (i == count - 1)
            {
                scaleElement.setCount = -1;
                while (!limitHit)
                {
                    CreateNewScaleSet(scaleElement, ref current, ref limitHit);
                }
            }
            else
            {
                for (int ii = 0; ii < scaleElement.setCount; ii++)
                {
                    if(CreateNewScaleSet(scaleElement, ref current, ref limitHit)) break;
                }
            }

            if (limitHit) break;
        }
    }

    private bool CreateNewScaleSet(ScaleElement scaleElement, ref int current, ref bool limitHit)
    {
        var set = Instantiate(scaleGroupPref, TimeScaleGroup);
        SetData(set, current, scaleElement.gapPerSet);
        current += scaleElement.gapPerSet;
        if (current >= endPoint)
        {
            limitHit = true;
            return true;
        }
        return false;
    }

    private void SetData(ScaleGroup scaleSet, int start, int gap)
    {
        var beforeHistory = start < 0;
        scaleSet.start = start;
        scaleSet.gap = gap;
        if (beforeHistory)
        {
            start = -start;
        }
        scaleSet.MainLabel.text = GetNumber(start, beforeHistory);
        var middle = start - gap / 2;
        scaleSet.MiddleLabel.text = GetNumber(middle, beforeHistory);
    }

    private string GetNumber(int data, bool beforeHistory)
    {
        string number = data.ToString();
        foreach (Abbreviation abbre in abbreForNumber)
        {
            var amount = abbre.amount;
            if (data >= amount)
            {
                number = (float)data / amount + abbre.abbre;
                break;
            }
        }
        return beforeHistory
            ? String.Format(beforeHistoryFormat, number)
            : String.Format(historyFormat, number);
    }

    [Serializable]
    public class Abbreviation
    {
        public int amount;
        public string abbre;
    }

    [Serializable]
    public class ScaleElement
    {
        public int gapPerSet;
        public int setCount = 1;
    }
}
