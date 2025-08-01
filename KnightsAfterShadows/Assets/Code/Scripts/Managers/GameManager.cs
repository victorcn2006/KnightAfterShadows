using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{
    private string language, textSpeed, quality, resolution;
    private bool isFullScreen;
    private void Awake() {
        language = PlayerPrefs.GetString("Language", "English");
        textSpeed = PlayerPrefs.GetString("TextSpeed", "Normal");
    }
    public void UpdatePlayerPrefs(string option) {
        switch (option){
            case "Language":
                language = PlayerPrefs.GetString("Language", "English");
                break;
            case "TextSpeed":
                textSpeed = PlayerPrefs.GetString("TextSpeed", "Normal");               
                break;
            case "FullScreen":
                isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
                Systems.instance.screenManager.ChangeFullScreen(isFullScreen);
                break;
            case "Resolution":
                resolution = PlayerPrefs.GetString("Resolution", "1920 x 1080");
                break;
            case "Quality":
                quality = PlayerPrefs.GetString("Quality", "High");
                break;
        }

    }
}
