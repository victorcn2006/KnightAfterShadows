using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarrouselManager : MonoBehaviour {
    //===========STRINGS===================
    [Header("All the strings you are going to change LANGUAGE")]
    public List<string> languages;
    [Header("All the speed options for text")]
    public List<string> speedText;
    [Header("The two options to enable the FullScreen")]
    public List<string> fullScreenText;
    [Header("Resolution Options")]
    public List<string> resolutions;
    [Header("Quality Options")]
    public List<string> qualities;
    //===========OPTION BUTTONS============
    [Header("All the buttons of every section")]
    public List<GameObject> buttons;
    //===========TEXT COMPONENTS===========
    [Header("All the text componentes that need to be changed dynamically")]
    public List<TextMeshProUGUI> gameTexts;

    private int languageIndex = 0;
    private int speedIndex = 0;
    private int fullScreenIndex = 0;
    private int resolutionIndex = 0;
    private int qualitiesIndex = 0;

    private bool inputHandled = false;

    public void Update() {
        if (Systems.instance == null || Systems.instance.inputManager == null)
        {
            // Por si no está inicializado Systems o InputManager
            return;
        }

        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current == null || !buttons.Contains(current))
        {
            inputHandled = false;
            return;
        }

        bool leftNav = Systems.instance.inputManager.leftNavigation;
        bool rightNav = Systems.instance.inputManager.rightNavigation;
        if (!inputHandled && (leftNav || rightNav))
        {
            inputHandled = true;

            int direction = leftNav ? 1 : -1;

            switch (current.name)
            {
                case "LanguageSelectorButton":
                    languageIndex += direction;
                    if (languageIndex < 0) languageIndex = languages.Count - 1;
                    if (languageIndex >= languages.Count) languageIndex = 0;

                    string selectedLanguage = languages[languageIndex];

                    foreach (var text in gameTexts)
                    {
                        if (text.name == "LanguageText")
                        {
                            text.text = selectedLanguage;
                            PlayerPrefs.SetString("Language", selectedLanguage);
                            PlayerPrefs.Save();
                            Systems.instance.gameManager.UpdatePlayerPrefs("Language");
                            text.ForceMeshUpdate();
                        }
                    }
                    break;

                case "SpeedSelectorButton":
                    speedIndex += direction;
                    if (speedIndex < 0) speedIndex = speedText.Count - 1;
                    if (speedIndex >= speedText.Count) speedIndex = 0;

                    string selectedSpeed = speedText[speedIndex];

                    foreach (var text in gameTexts){
                        if (text.name == "SpeedSelectorText"){
                            text.text = selectedSpeed;
                            PlayerPrefs.SetString("TextSpeed", selectedSpeed);
                            PlayerPrefs.Save();
                            Systems.instance.gameManager.UpdatePlayerPrefs("TextSpeed");
                            text.ForceMeshUpdate();
                        }
                    }
                    break;

                case "FullScreenButton":
                    fullScreenIndex += direction;
                    if (fullScreenIndex < 0) fullScreenIndex = fullScreenText.Count - 1;
                    if (fullScreenIndex >= fullScreenText.Count) fullScreenIndex = 0;

                    string selectedFullScreen = fullScreenText[fullScreenIndex];
                    foreach (var text in gameTexts)
                    {
                        if (text.name == "FullScreenText")
                        {
                            text.text = selectedFullScreen;
                            if (selectedFullScreen == "On")
                                PlayerPrefs.SetInt("FullScreen", 1);
                            else if (selectedFullScreen == "Off")
                                PlayerPrefs.SetInt("FullScreen", 0);
                            PlayerPrefs.Save();
                            Systems.instance.gameManager.UpdatePlayerPrefs("FullScreen");
                            text.ForceMeshUpdate();
                        }
                    }
                    break;

                case "ResolutionButton":
                    resolutionIndex += direction;
                    if (resolutionIndex < 0) resolutionIndex = resolutions.Count - 1;
                    if (resolutionIndex >= resolutions.Count) resolutionIndex = 0;

                    string selectedResolution = resolutions[resolutionIndex];
                    foreach (var text in gameTexts)
                    {
                        if (text.name == "ResolutionText")
                        {
                            text.text = selectedResolution;
                            PlayerPrefs.SetString("Resoltion", selectedResolution);
                            PlayerPrefs.Save();
                            Systems.instance.gameManager.UpdatePlayerPrefs("Resolution");
                            text.ForceMeshUpdate();
                        }
                    }
                    break;

                case "QualityButton":
                    qualitiesIndex += direction;
                    if (qualitiesIndex < 0) qualitiesIndex = qualities.Count - 1;
                    if (qualitiesIndex >= qualities.Count) qualitiesIndex = 0;

                    string selectedQuality = qualities[qualitiesIndex];
                    foreach (var text in gameTexts)
                    {
                        if (text.name == "QualityText")
                        {
                            text.text = selectedQuality;
                            PlayerPrefs.SetString("Quality", selectedQuality);
                            PlayerPrefs.Save();
                            Systems.instance.gameManager.UpdatePlayerPrefs("Quality");
                            text.ForceMeshUpdate();
                        }
                    }
                    break;
            }
        }

        if (!Systems.instance.inputManager.leftNavigation && !Systems.instance.inputManager.rightNavigation)
        {
            inputHandled = false;
        }
    }
}
