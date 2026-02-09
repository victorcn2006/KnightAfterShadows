using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabSelectorVisuals : MonoBehaviour
{
    [System.Serializable]
    public class TabVisual
    {
        public Button button;
        public Image[] indicatorImages; // The RB/LB images for this tab
    }

    [SerializeField] private TabVisual[] tabVisuals;
    private GameObject lastSelected;


    private void Start()
    {
        // Disable all images initially
        DisableAllImages();

        // Select first button by default
        if (tabVisuals.Length > 0 && tabVisuals[0].button != null)
        {
            tabVisuals[0].button.Select();
        }
    }

    private void DisableAllImages()
    {
        foreach (var tab in tabVisuals)
        {
            foreach (var img in tab.indicatorImages)
            {
                if (img != null)
                    img.enabled = false;
            }
        }
    }

    private void Update()
    {
        // Check if the selected object has changed
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != lastSelected)
        {
            lastSelected = currentSelected;
            UpdateVisuals(currentSelected);
        }
    }

    private void UpdateVisuals(GameObject selectedObject)
    {
        // Disable all images first
        DisableAllImages();

        // Find which tab is selected and enable its images
        for (int i = 0; i < tabVisuals.Length; i++)
        {
            if (tabVisuals[i].button != null && tabVisuals[i].button.gameObject == selectedObject)
            {
                foreach (var img in tabVisuals[i].indicatorImages)
                {
                    if (img != null)
                        img.enabled = true;
                }
                break;
            }
        }
    }
}