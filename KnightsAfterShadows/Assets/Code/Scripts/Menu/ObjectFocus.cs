using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ObjectFocus : MonoBehaviour {
    [SerializeField] GameObject objectToSelect;
    private GameObject lastSelected;

    private void Start() {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(objectToSelect);
            Debug.Log("Focus reset");
        }
    }

    void Update() {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(objectToSelect);
            Debug.Log("Focus reset");
        }
    }
}
