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

    private RectTransform _rectT;
    private Vector2 _originalPosition;
	void Start ()
	{
	    _rectT = (RectTransform) transform;
	    _originalPosition = popupObj.anchoredPosition;

	    HidePopup(true);
	}

    public void SetData(TimepointData data)
    {
        titleText.text = TimelineManager.timelineInfo.Categories.First(cat => cat.id == data.category).name;
        detailText.text = data.description;
        //TODO : set picture
        //TODO : set audio
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
        var startPoint = popupObj.anchoredPosition;
        var endPoint = show ? _originalPosition : Vector2.zero;
        var distance = endPoint.y - startPoint.y;
        var speed = distance / (time > 0 ? time : 1);

        while (t > 0)
        {
            popupObj.anchoredPosition += Vector2.up * speed * Time.deltaTime;
            yield return null;
            t -= Time.deltaTime;
        }
        popupObj.anchoredPosition = endPoint;
        _currentTransition = null;
    }

    private IEnumerator HideCheck()
    {
        var inputModule = (CustomStandaloneInputModule) EventSystem.current.currentInputModule;
        while (true)
        {
            if (inputModule.input.GetMouseButton(0))
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
