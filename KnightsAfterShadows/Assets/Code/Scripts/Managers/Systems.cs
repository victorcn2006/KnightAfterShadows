using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Systems : MonoBehaviour{
    public static Systems instance { get; private set; }

    public InputManager inputManager;
    public GameManager gameManager;
    public ScreenManager screenManager;
    private void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else{ 
            Destroy(this.gameObject);
            return;
        }
    }
}
