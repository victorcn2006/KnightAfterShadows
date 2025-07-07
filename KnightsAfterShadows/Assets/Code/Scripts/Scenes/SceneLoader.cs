using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour {
    public SceneField sceneToLoad;
    public void LoadScene() {
        SceneManager.LoadScene(sceneToLoad);
    }
    /*
    public void LoadSceneWithControlType(string controlType) {
        ControlTypeSelection.selectedControl = controlType; //It can be Gamepad or Keyboard
        SceneManager.LoadScene(sceneToLoad);
    }
    */
    public void Exit() {
        Debug.Log("Exit...");
        Application.Quit();
    }
}
