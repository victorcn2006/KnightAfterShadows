using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemManager : MonoBehaviour
{
    private GameObject lastFocus;

    private void Start()
    {
        lastFocus = EventSystem.current.firstSelectedGameObject;
    }
    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null) {
            EventSystem.current.SetSelectedGameObject(lastFocus);
        }
        else if (EventSystem.current.currentSelectedGameObject != lastFocus)
        {
            lastFocus = EventSystem.current.currentSelectedGameObject;
        }
    }
}
