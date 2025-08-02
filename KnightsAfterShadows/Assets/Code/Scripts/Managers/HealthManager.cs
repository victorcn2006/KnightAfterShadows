using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour{
    [Header("Prefab with the heart, the Transform from layout element and the Animator Controller")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartContainer;
    [SerializeField] private Animator animator;

    [Header("Health properties")]
    [SerializeField] private float totalHearts;

    private void Awake() {
        if (animator == null){
            Debug.LogError("Animator not assigned on HealthManager!");
        }
    }

    private void Start() {
        InstantiateHearts();
        Damage();
    }
    private void InstantiateHearts(){
        for (int i = 0; i < totalHearts; i++){
            GameObject heart = Instantiate(heartPrefab);
            heart.transform.SetParent(heartContainer, false);
        }
    }
    private void Damage(){
        animator.SetBool("Damage", true);
    }
    private void Heal(){ 
    
    }
}
