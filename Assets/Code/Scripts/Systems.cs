using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Systems : MonoBehaviour
{
    public static Systems instance { get; private set; }

    private void Awake() {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }
}
