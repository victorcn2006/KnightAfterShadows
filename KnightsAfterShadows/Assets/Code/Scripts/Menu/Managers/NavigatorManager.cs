using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NavigatorManager : MonoBehaviour{
    [Header("Next Page and Prev Page icons for navigation")]
    public List<GameObject> icons;

    [Header("All the four buttons for navigation")]
    public List<GameObject> buttons;

    [Header("The option layouts of every button")]
    public List<GameObject> layouts;

    private GameObject lastSelected;

    private void Update() {
        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current == null || current == lastSelected) return;
        //It only changes the layout if is one of the four buttons
        if (buttons.Contains(current)){
            lastSelected = current;

            SetActiveAll(icons, false);
            SetActiveAll(layouts, false);

            switch (current.name){
                case "GameOptions Button":
                    ActivateIcon("GameNextPageIcon");
                    ActivateLayout("GameOptions");
                    break;

                case "Graphics Button":
                    ActivateIcon("GraphicsNextPageIcon");
                    ActivateIcon("GraphicsReturnPageIcon");
                    ActivateLayout("GraphicsOptions");
                    break;

                case "Sound Button":
                    ActivateIcon("SoundNextPageIcon");
                    ActivateIcon("SoundReturnPageIcon");
                    ActivateLayout("SoundOptions");
                    break;

                case "Controls Button":
                    ActivateIcon("ControlsNextPageIcon");
                    ActivateIcon("ControlsReturnPageIcon");
                    ActivateLayout("ControlsOptions");
                    break;
            }
        }
    }
    private void SetActiveAll(List<GameObject> list, bool state) {
        foreach (var go in list){
            go.SetActive(state);
        }
    }
    private void ActivateIcon(string iconName) {
        foreach (var icon in icons){
            if (icon.name == iconName){
                icon.SetActive(true);
                break;
            }
        }
    }

    private void ActivateLayout(string layoutName) {
        foreach (var layout in layouts){
            if (layout.name == layoutName){
                layout.SetActive(true);
                break;
            }
        }
    }
}
        
