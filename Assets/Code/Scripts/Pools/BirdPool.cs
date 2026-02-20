using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdPool : MonoBehaviour
{
    [Header("Bird Prefab")]
    [SerializeField] private GameObject _birdPrefab;

    [Header("Spawn Points")]
    [Tooltip("Add the three spawn points for each bird")]
    [SerializeField] private RectTransform[] _spawnPoints;

    private GameObject[] _birds;


    private void Awake() {
        _birds = new GameObject[_spawnPoints.Length];
        for(int i = 0; i < _spawnPoints.Length; i++){
            _birds[i] = Instantiate(_birdPrefab, _spawnPoints[i], false);
            _birds[i].SetActive(false); //Start deactivated
        }
    }
    public void SpawnAll(){
        for(int i = 0; i < _birds.Length; i++){
            _birds[i].transform.localPosition = Vector3.zero;
            _birds[i].SetActive(true);
        }
    }
    public void ReturnAll()
    {
        foreach (GameObject bird in _birds)
            bird.SetActive(false);
    }
}

