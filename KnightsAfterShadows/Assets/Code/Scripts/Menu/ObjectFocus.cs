using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ObjectFocus : MonoBehaviour {
    [SerializeField] GameObject objectToSelect;
    private GameObject lastSelected;

    private void Start() {
        if (EventSystem.current == null)
            return;
        EventSystem.current.SetSelectedGameObject(objectToSelect);
        lastSelected = objectToSelect;  
    }

    void Update() {
        if (EventSystem.current == null)
            return;

        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null && currentSelected != lastSelected){
            lastSelected = currentSelected;
        }
        // On left mouse button click, set the selected UI object
        if (Systems.instance.inputManager.leftClick.triggered){
            if (EventSystem.current.currentSelectedGameObject == null){
                EventSystem.current.SetSelectedGameObject(lastSelected);
            }
        }
    }
}
