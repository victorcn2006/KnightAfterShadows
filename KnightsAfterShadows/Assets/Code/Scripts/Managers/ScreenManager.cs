using UnityEngine;
public class ScreenManager : MonoBehaviour{
    
    public void ChangeFullScreen(bool isFullScreen){ 
        Screen.fullScreen = isFullScreen;
    }
}
