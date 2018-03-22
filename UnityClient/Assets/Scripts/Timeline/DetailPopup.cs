using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DetailPopup : MonoBehaviour
{
    [SerializeField] private RectTransform blocker, popupObj;
    [SerializeField] private Text titleText;
    [SerializeField] private Text detailText;
    [SerializeField] private Image detailImage;

    [SerializeField] private Button audioButton;
    [SerializeField] private ScrollRect scrollRect;

    public float delayTime;

    private Image _fade;
    private Vector2 _originalPosition;
    private Color _originalColor;
	void Start ()
	{
	    _fade = blocker.GetComponent<Image>();
	    _originalPosition = popupObj.anchoredPosition;
	    _originalColor = _fade.color;

	    HidePopup(true);
	}

    public void SetData(TimepointData data)
    {
        titleText.text = TimelineManager.timelineInfo.Categories.First(cat => cat.id == data.category).name;
        detailText.text = data.description;
        var image = PictureMapper.GetDetailPic(data.id);
        if (image)
        {
            detailImage.sprite = image;
            detailImage.gameObject.SetActive(true);
        }
        /*else
        {
            detailImage.gameObject.SetActive(false);
        }*/
        //TODO : set audio
        Canvas.ForceUpdateCanvases();
    }

    public void ShowPopup()
    {
        blocker.gameObject.SetActive(true);
        _currentTransition = Translate(true, delayTime);
        scrollRect.verticalNormalizedPosition = 1;
        StartCoroutine(_currentTransition);
        StartCoroutine(HideCheck());
    }

    public void HidePopup(bool instantly = false)
    {
        if (_currentTransition != null)
        {
            StopCoroutine(_currentTransition);
            _currentTransition = null;
        }
        blocker.gameObject.SetActive(false);
        _currentTransition = Translate(false, instantly ? 0 : delayTime);
        StartCoroutine(_currentTransition);
    }

    private IEnumerator _currentTransition;
    private IEnumerator Translate(bool show, float time)
    {

        var t = time;
        var timeGap = (time > 0 ? time : 1);
        var startPoint = popupObj.anchoredPosition;
        var endPoint = show ? _originalPosition : Vector2.zero;
        var distance = endPoint.y - startPoint.y;
        var speed = distance / timeGap;

        var startAlpha = _fade.color.a;
        var endAlpha = show ? _originalColor.a : 0f;
        distance = endAlpha - startAlpha;
        var fadeSpeed = distance / timeGap;

        while (t > 0)
        {
            popupObj.anchoredPosition += Vector2.up * speed * Time.deltaTime;
            _fade.color = new Color(0, 0, 0, _fade.color.a + fadeSpeed * Time.deltaTime);
            yield return null;
            t -= Time.deltaTime;
        }
        popupObj.anchoredPosition = endPoint;
        _fade.color = new Color(0, 0, 0, endAlpha);
        _currentTransition = null;
    }

    private IEnumerator HideCheck()
    {
        var inputModule = (CustomStandaloneInputModule) EventSystem.current.currentInputModule;
        while (true)
        {
            if (inputModule.input.GetMouseButtonDown(0))
            {
                var hover = CustomStandaloneInputModule.GetPointerEventData().hovered;
                var hidePopup = hover.Contains(blocker.gameObject);
                if (hidePopup)
                {
                    break;
                }
            }
            yield return null;
        }
        HidePopup();
    }
}
