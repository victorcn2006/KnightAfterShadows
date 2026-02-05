using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuInicio : MonoBehaviour {
    [SerializeField] private GameObject enemigoGameObject;
    private Animator enemigoAnimator;
    // Start is called before the first frame update
    void Start() {
        enemigoAnimator = enemigoGameObject.GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update() {

    }
    public void Jugar() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Exit() {
        enemigoAnimator.SetTrigger("Activado");
        
        Invoke("Salir", 3.5f);
    }
    private void Salir() {
        Application.Quit();
        Debug.Log("Saliendo....");
    }
}
