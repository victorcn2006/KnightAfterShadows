using TMPro;
using UnityEngine;

public class DisplayUsername : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (Systems.instance != null && !string.IsNullOrEmpty(Systems.instance.CurrentUsername))
        {
            textMesh.text = "Welcome, " + Systems.instance.CurrentUsername + "!";
        }
        else
        {
            textMesh.text = "Not logged in";
        }
    }
}
