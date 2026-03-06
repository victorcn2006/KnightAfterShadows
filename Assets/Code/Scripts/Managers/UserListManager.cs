using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserListManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject userItemPrefab; // A simple prefab with a TextMeshPro component
    [SerializeField] private Transform contentParent; // The Content object of the ScrollView
    [SerializeField] private Button listUsersButton;

    private void Start()
    {
        if (listUsersButton != null)
        {
            listUsersButton.onClick.AddListener(DisplayUsers);
        }
    }

    public void DisplayUsers()
    {
        // Clear previous items
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Get all users from the ORM
        List<User> users = SQLiteReader.instance.GetAllUsers();

        // Instantiate a prefab for each user
        foreach (var user in users)
        {
            GameObject newItem = Instantiate(userItemPrefab, contentParent);
            
            // Setup the new UI item
            UserItemUI itemUI = newItem.GetComponent<UserItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(user);
            }
            else
            {
                Debug.LogWarning("Prefab does not have a UserItemUI component!");
            }
        }
    }
}
