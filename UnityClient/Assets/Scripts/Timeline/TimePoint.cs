using System.Collections;
using System.Collections.Generic;
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

    public string titleText;
    public Sprite WestSprite, ThaiSprite;

    public TimepointData dataW, dataTh;

    private void Start()
    {
        point.onClick.AddListener(() =>
        {
            
        });
    }

    public void SetData(TimepointData data)
    {
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
        dataW = null;
        dataTh = null;
        west.gameObject.SetActive(false);
        thai.gameObject.SetActive(false);
        titleWest.gameObject.SetActive(false);
        titleThai.gameObject.SetActive(false);
    }

    public void SetIconOffset(bool offset)
    {
        var x = offset ? 125 : 400;
        thai.GetComponent<RectTransform>().offsetMax = new Vector2(x, 0);
        west.GetComponent<RectTransform>().offsetMax = new Vector2(-x, 0);
    }

    public void SetThai(Sprite icon, string title)
    {
        SetDetail(thai, icon, titleThai, title);
        thai.gameObject.SetActive(true);
        titleThai.gameObject.SetActive(true);
    }

    public void SetWest(Sprite icon, string title)
    {
        SetDetail(west, icon, titleWest, title);
        west.gameObject.SetActive(true);
        titleWest.gameObject.SetActive(true);
    }

    private void SetDetail(Button icon, Sprite sprite, Text text, string title)
    {
        icon.GetComponent<Image>().sprite = sprite;
        text.text = title;
    }

    
}
