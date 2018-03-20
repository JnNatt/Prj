using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class TimePoint : MonoBehaviour
{
    [SerializeField] private Image pointSelect;
    [SerializeField] private Button point;
    [SerializeField] private Text titleWest;
    [SerializeField] private Text titleThai;
    [SerializeField] private Button west;
    [SerializeField] private Button thai;
    [SerializeField] private RectTransform lineW;
    [SerializeField] private RectTransform lineTh;

    [SerializeField] private bool isTitlePoint;

    public string titleText;
    [SerializeField] public Sprite WestSprite;
    [SerializeField] public Sprite ThaiSprite;

    public TimepointData dataW, dataTh;

    private float offsetMin, offsetMax, baseLineWidth;
    private Vector2 baseAnchor;

    private void Start()
    {
        if (isTitlePoint) return;
        
        point.onClick.AddListener(OnSelect);
        west.onClick.AddListener(() => OnIconClick(west));
        thai.onClick.AddListener(() => OnIconClick(thai));
    }

    private void OnIconClick(Button icon)
    {
        if (!isSelected)
        {
            OnSelect();
        }
        else
        {
            
        }
    }

    public void Init()
    {
        if (isTitlePoint) return;

        var iconRect = (RectTransform)thai.transform;
        offsetMin = iconRect.offsetMin.x;
        offsetMax = iconRect.offsetMax.x;
        baseLineWidth = lineTh.sizeDelta.x;
        baseAnchor = iconRect.anchoredPosition;
    }

    public static event Action<TimePoint> OnSelectE;
    public static event Action OnDeselectE;
    private bool isSelected;
    public void OnSelect()
    {
        if (!isSelected)
        {
            if (OnSelectE != null)
            {
                OnSelectE(this);
            }
            SetSelect(true);
            OnSelectE += Deselect;
        }
    }

    private void Deselect(TimePoint owner)
    {
        OnSelectE -= Deselect;
        SetSelect(false);
        if (OnDeselectE != null)
        {
            OnDeselectE();
        }
    }
    void OnEnable()
    {
        if (isSelected)
            OnSelectE += Deselect;
    }
    void OnDisable()
    {
        if (isSelected)
            OnSelectE -= Deselect;
    }

    public void SetData(TimepointData data)
    {
        if (data == null) return;
        if (data.type == TimelineType.Th)
        {
            dataTh = data;
            SetThai(null, data.title);
        }
        else
        {
            dataW = data;
            SetWest(null, data.title);
        }
    }

    public void ResetData()
    {
        if (isTitlePoint) return;
        dataW = null;
        dataTh = null;
        west.gameObject.SetActive(false);
        thai.gameObject.SetActive(false);
        titleWest.gameObject.SetActive(false);
        titleThai.gameObject.SetActive(false);
        lineW.gameObject.SetActive(false);
        lineTh.gameObject.SetActive(false);
        SetSelect(false);
    }

    public void SetIconOffset(float offset)
    {
        if(isTitlePoint) return;
        var thaiRect = (RectTransform) thai.transform;
        var westRect = (RectTransform) west.transform;
        thaiRect.anchoredPosition = new Vector2(baseAnchor.x - offset, baseAnchor.y);
        lineTh.sizeDelta = new Vector2(baseLineWidth + offset, lineTh.sizeDelta.y);
        westRect.anchoredPosition = new Vector2(-baseAnchor.x + offset, baseAnchor.y);
        lineW.sizeDelta = new Vector2(baseLineWidth + offset, lineW.sizeDelta.y);
    }

    public void SetThai(Sprite icon, string title)
    {
        Sprite sprite = icon != null ? icon : ThaiSprite;
        SetDetail(thai, sprite, titleThai, title);
    }

    public void SetWest(Sprite icon, string title)
    {
        if (isTitlePoint) return;
        Sprite sprite = icon != null ? icon : WestSprite;
        SetDetail(west, sprite, titleWest, title);
    }

    private void SetDetail(Button icon, Sprite sprite, Text text, string title)
    {
        if (sprite)
        {
            icon.GetComponent<Image>().sprite = sprite;
            icon.gameObject.SetActive(true);
        }

        text.text = title;
        text.gameObject.SetActive(true);
    }

    private void SetSelect(bool select)
    {
        isSelected = select;
        if (dataTh != null) lineTh.gameObject.SetActive(select);
        if (dataW != null) lineW.gameObject.SetActive(select);
        pointSelect.gameObject.SetActive(select);
    }
}
