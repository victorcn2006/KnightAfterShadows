using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private Button deleteButton;

    private int _userId;

    public void Setup(User user)
    {
        _userId = user.id;
        usernameText.text = user.username;
        
        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(OnDeleteClicked);
    }

    private void OnDeleteClicked()
    {
        if (SQLiteReader.instance.DeleteUser(_userId))
        {
            // Successfully deleted from DB, now remove from UI
            Destroy(this.gameObject);
        }
    }
}
