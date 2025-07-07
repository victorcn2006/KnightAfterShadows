#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

//CustomPropertyDrawer type: SceneField to use attach to it
[CustomPropertyDrawer(typeof(SceneField))]

public class SceneFieldPropertyDrawer : PropertyDrawer { //PropertyDrawer allow us to draw in the Inspector.

    //Override the method OnGUI from the father PropertyDrawer, this method is called everytime we draw in the Inspector
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label) {
        if (_property == null || _property.serializedObject == null || _property.serializedObject.targetObject == null)
            return;
        EditorGUI.BeginProperty(_position, GUIContent.none, _property); // We say to Unity we are starting to draw in the Inspector a property

        //Saves to the variables sceneAsset and sceneName from SceneField
        SerializedProperty sceneAsset = _property.FindPropertyRelative("m_SceneAsset"); //sceneAsset is the object that only exists in the editor
        SerializedProperty sceneName = _property.FindPropertyRelative("m_SceneName"); //sceneName is the string of the scene

        //It creates a label in the inspector to put an input
        _position = EditorGUI.PrefixLabel(_position, GUIUtility.GetControlID(FocusType.Passive), _label);

        //Shows a field to add our scenes like a list where we can drop there our scenes
        if (sceneAsset != null)
        {
            sceneAsset.objectReferenceValue = EditorGUI.ObjectField(
                _position,
                sceneAsset.objectReferenceValue,
                typeof(SceneAsset),
                false
            );
            //If the user drops a scene, it saves the string to Load the Scene
            if (sceneAsset.objectReferenceValue != null)
            {
                sceneName.stringValue = (sceneAsset.objectReferenceValue as SceneAsset).name;
            }
        }

        EditorGUI.EndProperty();
    }
}
#endif
