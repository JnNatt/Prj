using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavigationHelper : MonoBehaviour {

    EventSystem system;

    void Start()
    {
        system = EventSystem.current;

    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            var selected = system.currentSelectedGameObject;
            if (!selected) return;
            Selectable current = selected.GetComponent<Selectable>();
            if (!current) return;
            Selectable next = null;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                next = current.FindSelectableOnUp();
            }
            else
            {
                next = current.FindSelectableOnDown();
            }

            if (next != null)
            {

                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret

                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
            //else Debug.Log("next nagivation element not found");

        }
    }
}
