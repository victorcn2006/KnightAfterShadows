using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMODUnity;

public class ActionButtonManager : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public enum ACTIONS { 
        PLAY,
        OPTIONS,
        RETURN,
        EXIT
    }
    [SerializeField] private string sceneToLoad;
    [SerializeField] private ACTIONS action;
    [SerializeField] private EventReference clickSound;
    [SerializeField] private EventReference hoverSound;

    private Button button;

    private void OnEnable() { 
        button = GetComponent<Button>();
        button.onClick.AddListener(CheckAction);
    }

    private void OnDisable() {
        button.onClick.RemoveListener(CheckAction);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHoverSound();
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayHoverSound();
    }

    private void PlayHoverSound()
    {
        if (!hoverSound.IsNull)
        {
            RuntimeManager.PlayOneShot(hoverSound);
        }
    }

    private void CheckAction() {
        if (!clickSound.IsNull)
        {
            RuntimeManager.PlayOneShot(clickSound);
        }

        switch (action) {
            case ACTIONS.PLAY:
                InventoryManager.instance.SaveCurrentInventory();
                SceneManager.LoadScene(sceneToLoad);
                break;
            case ACTIONS.OPTIONS:
                InventoryManager.instance.SaveCurrentInventory();
                SceneManager.LoadScene(sceneToLoad);
                break;
            case ACTIONS.RETURN:
                InventoryManager.instance.SaveCurrentInventory();
                SceneManager.LoadScene(sceneToLoad);
                break;
            case ACTIONS.EXIT:
                InventoryManager.instance.SaveCurrentInventory();
                Application.Quit();
                Debug.Log("Exit");
                break;
        }

    }
}
