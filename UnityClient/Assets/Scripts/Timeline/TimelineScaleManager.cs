using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TimelineScaleManager : MonoBehaviour
{
    public ScaleGroup scaleGroupPref;
    public TimePoint timepointTitlePrefab;
    public TimePoint timepointPrefab;

    public RectTransform TimeScaleGroup;

    public RectTransform TimePointGroup;

    public RectTransform Line;

    
    [SerializeField][Tooltip("Offset for the line of timeline's height")]
    private float offset = 400;
    [SerializeField][Tooltip("This is for determine how close must timepoint be to hide the label of time scale")]
    private float threshold = 100f;

    [SerializeField] private string beforeHistoryFormat = "{0} ปีก่อนปัจจุบัน";
    [SerializeField] private string historyFormat = "ค.ศ.{0}";
    [SerializeField] private List<Abbreviation> abbreForNumber;

    [SerializeField] private int startPoint = -3000000;
    [SerializeField][Tooltip("If data is longer than endpoint, this will be ignored")] private int endPoint = 2025;
    [SerializeField] private List<ScaleElement> scaleSetting;

    private readonly List<ScaleGroup> scaleSetList = new List<ScaleGroup>();
    private static readonly List<ScaleData> scaleData = new List<ScaleData>();
    private ScrollRect _scrollView;
    private float _itemSize;

    #region Data
        
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

    public class ScaleData
    {
        public int Start;
        public int Gap;

        public int Middle
        {
            get { return Start + Gap / 2; }
        }
        public int End
        {
            get { return Start + Gap; }
        }
    }

    public class TimepointDataSet
    {
        public TimepointData th;
        public TimepointData w;
        public float offset;

        public bool isSelected;

        public int order
        {
            get { return th != null ? th.order : w != null? w.order : -1; }
        }

        public float positionInTimeline;

        public ScaleData scale;

        public void SetData(TimepointData data)
        {
            if (data == null) return;
            if (data.type == TimelineType.Th)
                th = data;
            else
                w = data;
        }

        public void Deselect()
        {
            TimePoint.OnDeselectE -= Deselect;
            isSelected = false;
        }
    }

    #endregion

    void Awake ()
	{
	    _scrollView = GetComponent<ScrollRect>();
    }

    public void Init()
    {
        GenerateScale();

        LayoutRebuilder.ForceRebuildLayoutImmediate(TimeScaleGroup);
        var height = TimeScaleGroup.rect.height;
        _scrollView.content.sizeDelta = new Vector2(_scrollView.content.sizeDelta.x, height);
        Line.sizeDelta = new Vector2(Line.sizeDelta.x, height + offset);
    }

    #region Timepoint Management
    private readonly List<TimePoint> timepointList = new List<TimePoint>();
    private readonly List<TimePoint> timepointTitleList = new List<TimePoint>();
    private static readonly List<TimepointDataSet> timepointData = new List<TimepointDataSet>();
    private static readonly List<TimepointDataSet> timepointTitleData = new List<TimepointDataSet>();
    private readonly Dictionary<TimepointDataSet, TimePoint> titleMapping = new Dictionary<TimepointDataSet, TimePoint>();
    private readonly Dictionary<TimepointDataSet, TimePoint> timepointMapping = new Dictionary<TimepointDataSet, TimePoint>();

    private int prevTitle;
    private int nextTitle;
    private int prevTimepoint;
    private int nextTimepoint;

    private float _timepointGroupOffset;
    public void InitTimepoint(List<Category> categories)
    {
        //Add data
        for (int i = 0; i < categories.Count; i++)
        {
            var cat = categories[i];
            AddTimepointTitleData(cat.name, cat.startPoint);
            var order = -1;
            TimepointDataSet current = null;
            var iconOffset = 0f;
            for (int j = 0; j < cat.timeline.Count; j++)
            {
                var data = cat.timeline[j];
                if (order != cat.timeline[j].order)
                {
                    order = data.order;
                    current = AddTimepointData(data);
                    current.offset = iconOffset;
                    iconOffset += 200f;
                    if (iconOffset > 400f)
                    {
                        iconOffset = 0f;
                    }
                    /*var item = current;
                    TimePoint.OnSelectE += point =>
                    {
                        var temp = point.dataTh ?? point.dataW;
                        if (temp.order == item.order)
                        {
                            item.isSelected = true;
                            TimePoint.OnDeselectE += item.Deselect;
                        }
                    };*/
                }
                current.SetData(data);
            }
        }
        _timepointGroupOffset = TimePointGroup.anchoredPosition.y;
        timepointData.Sort((a, b) => Mathf.RoundToInt(b.positionInTimeline - a.positionInTimeline));
        timepointTitleData.Sort((a, b) => Mathf.RoundToInt(b.positionInTimeline - a.positionInTimeline));
        //Init visible timepoints and pool
        int timepointTitleIndex;
        for (timepointTitleIndex = 0; timepointTitleIndex < timepointTitleData.Count; timepointTitleIndex++)
        {
            var data = timepointTitleData[timepointTitleIndex];
            if (!IsTimepointVisibleInView(data))
            {
                break;
            }
            GenerateNewTimepointTitle(data);
        }
        int timepointIndex;
        for (timepointIndex = 0; timepointIndex < timepointData.Count; timepointIndex++)
        {
            var data = timepointData[timepointIndex];
            if (!IsTimepointVisibleInView(data))
            {
                break;
            }
            GenerateNewTimepoint(data);
        }
        prevTitle = 0;
        prevTimepoint = 0;

        nextTitle = timepointTitleIndex;
        nextTimepoint = timepointIndex;
    }
    private void AddTimepointTitleData(string title, int point)
    {
        ScaleData scale;
        var data = new TimepointDataSet
        {
            th = new TimepointData
            {
                title = title,
                order = point,
                type = TimelineType.Th
            },
            positionInTimeline = GetPositionOnTimeScale(point, out scale),
            scale = scale
        };
        timepointTitleData.Add(data);
    }

    private TimepointDataSet AddTimepointData(TimepointData data)
    {
        ScaleData scale;
        var dataSet = new TimepointDataSet
        {
            positionInTimeline = GetPositionOnTimeScale(data.order, out scale),
            scale = scale
        };
        dataSet.SetData(data);
        timepointData.Add(dataSet);
        return dataSet;
    }

    private float GetPositionOnTimeScale(int point)
    {
        ScaleData scale;
        return GetPositionOnTimeScale(point, out scale);
    }
    private float GetPositionOnTimeScale(int point, out ScaleData scaleSet)
    {
        ScaleData data = null;
        int i;
        for (i = 0; i < scaleData.Count; i++)
        {
            var scale = scaleData[i];
            if (scale.Start > point) break;
            if (scale.End > point)
            {
                data = scale;
                break;
            }
        }
        scaleSet = data;
        if (data == null) return 0;
        float d = Mathf.Abs(point - data.Start);
        float pointInScaleSet = d / data.Gap;

        return -(i * _itemSize + pointInScaleSet * _itemSize);
    }

    private TimePoint GenerateNewTimepointTitle(TimepointDataSet data)
    {
        var timepoint = GetTimepointFromPool(timepointTitleList, timepointTitlePrefab);
        TimepointLoaded(timepoint, data);
        if (titleMapping.ContainsKey(data)) titleMapping[data] = timepoint;
        else titleMapping.Add(data, timepoint);
        return timepoint;
    }
    private TimePoint GenerateNewTimepoint(TimepointDataSet data)
    {
        var timepoint = GetTimepointFromPool(timepointList, timepointPrefab);
        TimepointLoaded(timepoint, data);
        if (timepointMapping.ContainsKey(data)) timepointMapping[data] = timepoint;
        else timepointMapping.Add(data, timepoint);
        return timepoint;
    }
    private TimePoint GetTimepointFromPool(List<TimePoint> pool, TimePoint prefab)
    {
        TimePoint timepoint = null;
        if (pool.Count != 0)
        {
            timepoint = pool.FirstOrDefault(tp => !tp.gameObject.activeSelf);
        }
        if (timepoint == null)
        {
            timepoint = Instantiate(prefab, TimePointGroup);
            timepoint.Init();
            pool.Add(timepoint);
        }
        timepoint.gameObject.SetActive(true);
        return timepoint;
    }

    private bool IsTimepointVisibleInView(TimepointDataSet timepoint, float offset = 0f)
    {
        return IsTimepointNotAboveView(timepoint, offset) && IsTimepointNotBelowView(timepoint, offset);
    }

    private bool IsTimepointNotAboveView(TimepointDataSet timepoint, float offset = 0f)
    {
        return -timepoint.positionInTimeline >= _scrollView.content.anchoredPosition.y + _timepointGroupOffset - offset;
    }

    private bool IsTimepointNotBelowView(TimepointDataSet timepoint, float offset = 0f)
    {
        return -timepoint.positionInTimeline <=
               _scrollView.content.anchoredPosition.y + _timepointGroupOffset + _scrollView.viewport.rect.height + offset;
    }

    #endregion

    #region Scale Generation

    private void GenerateScale()
    {
        if (!scaleData.Any())
        {
            InitScaleData();
        }

        _itemSize = (scaleGroupPref.transform as RectTransform).rect.height;
        _itemsVisible = Mathf.CeilToInt(_scrollView.viewport.rect.height / _itemSize);
        _itemsTotal = scaleData.Count;
        int itemsToInstantiate = _itemsVisible;
        if (_itemsVisible == 1)
        {
            itemsToInstantiate = 5;
        }
        else if (itemsToInstantiate < _itemsTotal)
        {
            itemsToInstantiate *= 2;
        }

        if (itemsToInstantiate > _itemsTotal)
        {
            itemsToInstantiate = _itemsTotal;
        }

        for (int i = 0; i < itemsToInstantiate; i++)
        {
            var item = CreateNewScaleSet(scaleData[i]);
            float position = i * _itemSize;
            var rectTransform = (RectTransform)item.transform;
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2(0, -position);
            rectTransform.offsetMin = new Vector2(0, rectTransform.offsetMin.y);
            rectTransform.offsetMax = new Vector2(0, rectTransform.offsetMax.y);
            scaleSetList.Add(item);

        }

        _lastItemIndex = scaleSetList.Count - 1;

        _itemsToRecycleAfter = scaleSetList.Count - _itemsVisible;

        TimePointGroup.sizeDelta = TimeScaleGroup.sizeDelta = new Vector2(0, _itemSize * scaleData.Count); //Set TimeScale size

        _scrollView.onValueChanged.AddListener(position =>
        {
            Recycle();
        });

    }

    private void InitScaleData()
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
                    AddNewScaleData(scaleElement, ref current, ref limitHit);
                }
            }
            else
            {
                for (int ii = 0; ii < scaleElement.setCount; ii++)
                {
                    if (AddNewScaleData(scaleElement, ref current, ref limitHit)) break;
                }
            }

            if (limitHit) break;
        }
    }

    private bool AddNewScaleData(ScaleElement scaleElement, ref int current, ref bool limitHit)
    {
        var data = new ScaleData
        {
            Start = current,
            Gap = scaleElement.gapPerSet
        };
        scaleData.Add(data);
        current += scaleElement.gapPerSet;
        if (current >= endPoint)
        {
            limitHit = true;
            return true;
        }
        return false;
    }

    private ScaleGroup CreateNewScaleSet(ScaleData data)
    {
        var set = Instantiate(scaleGroupPref, TimeScaleGroup);
        SetData(set, data.Start, data.Gap);
        scaleMapping.Add(data, set);
        return set;
    }

    private void SetData(ScaleGroup scaleSet, int start, int gap)
    {
        var beforeHistory = start < 0;
        scaleSet.start = start;
        scaleSet.gap = gap;

        var middle = start + gap / 2;
        if (beforeHistory)
        {
            start = -start;
            middle = -middle;
        }

        scaleSet.MainLabel.text = GetNumber(start, beforeHistory);
        scaleSet.MiddleLabel.text = GetNumber(middle, beforeHistory);
        scaleSet.MainLabel.gameObject.SetActive(true);
        scaleSet.MiddleLabel.gameObject.SetActive(true);
    }

    private string GetNumber(int data, bool beforeHistory)
    {
        string number = data.ToString("##,###");
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

    #endregion

    #region Recycle
    private float _lastPosition;

    private int _itemsTotal;
    private int _itemsVisible;

    private int _itemsToRecycleBefore;
    private int _itemsToRecycleAfter;

    private int _currentItemIndex;
    private int _lastItemIndex;
    
    private float _timepointOffset = 125f;
    private readonly Dictionary<ScaleData, ScaleGroup> scaleMapping = new Dictionary<ScaleData, ScaleGroup>();

    private enum ScrollDirection
    {
        NEXT,
        PREVIOUS
    }

    void Recycle()
    {
        if (_lastPosition == -1)
        {
            _lastPosition = GetContentPosition();

            return;
        }

        ScrollDirection direction = GetScrollDirection();

        int displacedRows = Mathf.FloorToInt(Mathf.Abs(GetContentPosition() - _lastPosition) / _itemSize);

        if (displacedRows != 0)
        {
            for (int i = 0; i < displacedRows; i++)
            {
                switch (direction)
                {
                    case ScrollDirection.NEXT:

                        NextScaleItem();

                        break;

                    case ScrollDirection.PREVIOUS:

                        PreviousScaleItem();

                        break;
                }

                if (direction == ScrollDirection.NEXT)
                {
                    _lastPosition += _itemSize;
                }
                else
                {
                    _lastPosition -= _itemSize;
                }
            }
        }

        //Debug.Log("prev: " + prevTimepoint);
        //Debug.Log("next: " + nextTimepoint);

        switch (direction)
        {
            case ScrollDirection.NEXT:
                NextTimepointTitleItem();
                NextTimepointItem();
                break;
            case ScrollDirection.PREVIOUS:
                PreviousTimepointTitleItem();
                PreviousTimepointItem();
                break;
        }
    }

    private float GetContentPosition()
    {
        return _scrollView.content.anchoredPosition.y;
    }
    private ScrollDirection GetScrollDirection()
    {
        return _lastPosition > GetContentPosition() ? ScrollDirection.PREVIOUS : ScrollDirection.NEXT;
    }

    private void NextScaleItem()
    {
        if (_itemsToRecycleBefore >= (scaleSetList.Count - _itemsVisible) / 2 && _lastItemIndex < _itemsTotal - 1)
        {
            _lastItemIndex++;

            RecycleScaleItem(ScrollDirection.NEXT);
        }
        else
        {
            _itemsToRecycleBefore++;
            _itemsToRecycleAfter--;
        }
    }

    private void PreviousScaleItem()
    {
        if (_itemsToRecycleAfter >= (scaleSetList.Count - _itemsVisible) / 2 && _lastItemIndex > scaleSetList.Count - 1)
        {
            RecycleScaleItem(ScrollDirection.PREVIOUS);

            _lastItemIndex--;
        }
        else
        {
            _itemsToRecycleBefore--;
            _itemsToRecycleAfter++;
        }
    }

    private void NextTimepointTitleItem()
    {
        while (prevTitle < timepointTitleData.Count - 1 && !IsTimepointNotAboveView(timepointTitleData[prevTitle], _timepointOffset))
        {
            titleMapping[timepointTitleData[prevTitle]].gameObject.SetActive(false);
            prevTitle++;
        }

        while (nextTitle < timepointTitleData.Count && IsTimepointNotBelowView(timepointTitleData[nextTitle], _timepointOffset * 2))
        {
            RecycleTimepointTitleItem(ScrollDirection.NEXT);
            nextTitle++;
        }
    }

    private void PreviousTimepointTitleItem()
    {
        while (nextTitle > 1 && !IsTimepointNotBelowView(timepointTitleData[nextTitle - 1], _timepointOffset))
        {
            titleMapping[timepointTitleData[nextTitle - 1]].gameObject.SetActive(false);
            nextTitle--;
        }

        while (prevTitle > 0 && IsTimepointNotAboveView(timepointTitleData[prevTitle - 1], _timepointOffset))
        {
            prevTitle--;
            RecycleTimepointTitleItem(ScrollDirection.PREVIOUS);
        }
        
    }

    private void NextTimepointItem()
    {
        while (prevTimepoint < timepointData.Count && !IsTimepointNotAboveView(timepointData[prevTimepoint], _timepointOffset))
        {
            timepointMapping[timepointData[prevTimepoint]].gameObject.SetActive(false);
            prevTimepoint++;
        }
        while (nextTimepoint < timepointData.Count && IsTimepointNotBelowView(timepointData[nextTimepoint], _timepointOffset))
        {
            RecycleTimepointItem(ScrollDirection.NEXT);
            nextTimepoint++;
        }
    }

    private void PreviousTimepointItem()
    {
        while (nextTimepoint > 1 && !IsTimepointNotBelowView(timepointData[nextTimepoint - 1], _timepointOffset))
        {
            timepointMapping[timepointData[nextTimepoint - 1]].gameObject.SetActive(false);
            nextTimepoint--;
        }

        while (prevTimepoint > 0 && IsTimepointNotAboveView(timepointData[prevTimepoint - 1], _timepointOffset * 2))
        {
            prevTimepoint--;
            RecycleTimepointItem(ScrollDirection.PREVIOUS);
            
        }
    }

    private void RecycleScaleItem(ScrollDirection direction)
    {
        ScaleGroup firstItem = scaleSetList[0];
        ScaleGroup lastItem = scaleSetList[scaleSetList.Count - 1];

        float targetPosition = (_itemSize);
        var firstItemT = (RectTransform) firstItem.transform;
        var lastItemT = (RectTransform)lastItem.transform;

        switch (direction)
        {
            case ScrollDirection.NEXT:

                firstItemT.anchoredPosition = new Vector2(firstItemT.anchoredPosition.x, lastItemT.anchoredPosition.y - targetPosition);
                
                firstItem.transform.SetAsLastSibling();

                scaleSetList.RemoveAt(0);
                scaleSetList.Add(firstItem);

                ScaleLoaded(firstItem, _lastItemIndex);
                break;

            case ScrollDirection.PREVIOUS:
                
                lastItemT.anchoredPosition = new Vector2(lastItemT.anchoredPosition.x, firstItemT.anchoredPosition.y + targetPosition);
                
                lastItem.transform.SetAsFirstSibling();

                scaleSetList.RemoveAt(scaleSetList.Count - 1);
                scaleSetList.Insert(0, lastItem);

                ScaleLoaded(lastItem, _lastItemIndex - scaleSetList.Count);
                break;
        }

        Canvas.ForceUpdateCanvases();

    }

    private void RecycleTimepointItem(ScrollDirection direction)
    {
        switch (direction)
        {
            case ScrollDirection.NEXT:
                var timepoint = GenerateNewTimepoint(timepointData[nextTimepoint]);
                ((RectTransform) timepoint.transform).SetAsLastSibling();
                timepoint.gameObject.SetActive(true);
                break;
            case ScrollDirection.PREVIOUS:
                timepoint = GenerateNewTimepoint(timepointData[prevTimepoint]);
                ((RectTransform) timepoint.transform).SetAsFirstSibling();
                timepoint.gameObject.SetActive(true);
                break;
        }

        Canvas.ForceUpdateCanvases();
    }

    private void RecycleTimepointTitleItem(ScrollDirection direction)
    {
        switch (direction)
        {
            case ScrollDirection.NEXT:
                var timepoint = GenerateNewTimepointTitle(timepointTitleData[nextTitle]);
                ((RectTransform)timepoint.transform).SetAsLastSibling();
                timepoint.gameObject.SetActive(true);
                break;
            case ScrollDirection.PREVIOUS:
                timepoint = GenerateNewTimepointTitle(timepointTitleData[prevTitle]);
                ((RectTransform)timepoint.transform).SetAsFirstSibling();
                timepoint.gameObject.SetActive(true);
                break;
        }

        Canvas.ForceUpdateCanvases();
    }

    private void ScaleLoaded(ScaleGroup item, int index)
    {
        var data = scaleData[index];
        SetData(item, data.Start, data.Gap);
        scaleMapping[data] = item;
    }

    private void TimepointLoaded(TimePoint item, TimepointDataSet data)
    {
        item.ResetData();
        item.SetData(data.th);
        item.SetData(data.w);
        item.SetIconOffset(data.offset);
        if (data.isSelected)
        {
            item.OnSelect();
        }
        var rectT = (RectTransform)item.transform;
        rectT.anchoredPosition = new Vector2(0, data.positionInTimeline);
        //Only hide scale label if there is data on left side (Thai history)
        if (data.th != null)
        {
            var scale = scaleMapping[data.scale];
            if (Mathf.Abs(GetPositionOnTimeScale(data.scale.Start) - data.positionInTimeline + _timepointOffset) <= threshold)
            {
                scale.MainLabel.gameObject.SetActive(false);
            }
            else if (Math.Abs(GetPositionOnTimeScale(data.scale.Middle) - data.positionInTimeline + _timepointOffset) <= threshold)
            {
                scale.MiddleLabel.gameObject.SetActive(false);
            }
        }
        
    }

    #endregion
}
