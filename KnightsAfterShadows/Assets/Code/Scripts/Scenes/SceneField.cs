using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] //Now we can see the class in the inspector
public class SceneField {

    [SerializeField]
    private Object m_SceneAsset; //Saves a reference to the SceneAsset

    [SerializeField]
    private string m_SceneName = "";
    //Scene getter to print the actual m_SceneName
    public string SceneName {
        get { return m_SceneName; }
    }

    // Converts SceneField to a string 
    public static implicit operator string(SceneField sceneField) {
        return sceneField.SceneName;
    }
}
