using UnityEngine;
using UnityEngine.UI;

public class LessonSelectItem : MonoBehaviour
{
    public string LessonName;
    [SerializeField] private float fadeDuration = .5f;
    protected string sceneName = "";

    protected virtual void Start ()
    {
        var button = GetComponent<Button>();
        if (!button)
        {
            button = gameObject.AddComponent<Button>();
        }

        button.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(sceneName, fadeDuration);
        });
    }
}