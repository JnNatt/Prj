using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TimelineScaleManager : MonoBehaviour
{
    public ScaleGroup scaleGroupPref;

    public RectTransform TimeScaleGroup;

    public RectTransform TimePointGroup;

    public RectTransform Line;

    
    [SerializeField]
    private float offset = 400;

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

    void GenerateScale()
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
            float position = i * _itemSize + 45f;
            var rectTransform = (RectTransform) item.transform;
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2(0, -position);
            rectTransform.offsetMin = new Vector2(0, rectTransform.offsetMin.y);
            rectTransform.offsetMax = new Vector2(0, rectTransform.offsetMax.y);
            scaleSetList.Add(item);

        }

        _lastItemIndex = scaleSetList.Count - 1;

        _itemsToRecycleAfter = scaleSetList.Count - _itemsVisible;

        TimeScaleGroup.sizeDelta = new Vector2(0, _itemSize * scaleData.Count); //Set TimeScale size

        _scrollView.onValueChanged.AddListener((Vector2 position) =>
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

    #region Recycle
    private float _lastPosition;

    private int _itemsTotal;
    private int _itemsVisible;

    private int _itemsToRecycleBefore;
    private int _itemsToRecycleAfter;

    private int _currentItemIndex;
    private int _lastItemIndex;

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

        int displacedRows = Mathf.FloorToInt(Mathf.Abs(GetContentPosition() - _lastPosition) / _itemSize);

        if (displacedRows == 0)
        {
            return;
        }

        ScrollDirection direction = GetScrollDirection();

        for (int i = 0; i < displacedRows; i++)
        {
            switch (direction)
            {
                case ScrollDirection.NEXT:

                    NextItem();

                    break;

                case ScrollDirection.PREVIOUS:

                    PreviousItem();

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

    private float GetContentPosition()
    {
        return _scrollView.content.anchoredPosition.y;
    }
    private ScrollDirection GetScrollDirection()
    {
        return _lastPosition > GetContentPosition() ? ScrollDirection.PREVIOUS : ScrollDirection.NEXT;
    }

    private void NextItem()
    {
        if (_itemsToRecycleBefore >= (scaleSetList.Count - _itemsVisible) / 2 && _lastItemIndex < _itemsTotal - 1)
        {
            _lastItemIndex++;

            RecycleItem(ScrollDirection.NEXT);
        }
        else
        {
            _itemsToRecycleBefore++;
            _itemsToRecycleAfter--;
        }
    }

    private void PreviousItem()
    {
        if (_itemsToRecycleAfter >= (scaleSetList.Count - _itemsVisible) / 2 && _lastItemIndex > scaleSetList.Count - 1)
        {
            RecycleItem(ScrollDirection.PREVIOUS);

            _lastItemIndex--;
        }
        else
        {
            _itemsToRecycleBefore--;
            _itemsToRecycleAfter++;
        }
    }

    private void RecycleItem(ScrollDirection direction)
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

                ItemLoaded(firstItem, _lastItemIndex);
                break;

            case ScrollDirection.PREVIOUS:
                
                lastItemT.anchoredPosition = new Vector2(lastItemT.anchoredPosition.x, firstItemT.anchoredPosition.y + targetPosition);
                
                lastItem.transform.SetAsFirstSibling();

                scaleSetList.RemoveAt(scaleSetList.Count - 1);
                scaleSetList.Insert(0, lastItem);

                ItemLoaded(lastItem, _lastItemIndex - scaleSetList.Count);
                break;
        }

        Canvas.ForceUpdateCanvases();
    }

    private void ItemLoaded(ScaleGroup item, int index)
    {
        var data = scaleData[index];
        SetData(item, data.Start, data.Gap);
    }

    #endregion
}
