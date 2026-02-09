using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    [System.Serializable]
    public class TabPanel
    {
        public Button button;
        public GameObject panel;
    }

    private GameObject lastSelected;



    [SerializeField] private TabPanel[] tabPanels;

    private void Start()
    {
        // Show first panel by default
        if (tabPanels.Length > 0)
            ShowPanel(tabPanels[0].panel);
    }

    private void ShowPanel(GameObject panelToShow)
    {
        // Hide all panels
        foreach (var tab in tabPanels)
        {
            if (tab.panel != null)
                tab.panel.SetActive(false);
        }

        // Show selected panel
        if (panelToShow != null)
            panelToShow.SetActive(true);
    }

    private void Update()
    {
        // Check if the selected object has changed
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != lastSelected)
        {
            lastSelected = currentSelected;
            CheckPanelSwitch(currentSelected);
        }
    }

    private void CheckPanelSwitch(GameObject selectedObject)
    {
        // Find which button is selected and show its panel
        for (int i = 0; i < tabPanels.Length; i++)
        {
            if (tabPanels[i].button != null && tabPanels[i].button.gameObject == selectedObject)
            {
                ShowPanel(tabPanels[i].panel);
                break;
            }
        }
    }
}
