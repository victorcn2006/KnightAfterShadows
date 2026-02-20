using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
[RequireComponent(typeof(BirdPool))]
public class BirdFlockController : MonoBehaviour
{
    private const float RESTART_DELAY = 600f; //10 minutes

    private PlayableDirector _director;
    private BirdPool _birdPool;

    private void Awake(){
        _director = GetComponent<PlayableDirector>();
        _birdPool = GetComponent<BirdPool>();
    }

    private void OnEnable() => _director.stopped += OnTimelineStopped;
    private void OnDisable() => _director.stopped -= OnTimelineStopped;

    private void Start(){
        _birdPool.SpawnAll();
        _director.Play();
    }

    private void OnTimelineStopped(PlayableDirector director){
        if(director == _director){
            _birdPool.ReturnAll();
            StartCoroutine(RestartTimeline());
        }
    }

    private IEnumerator RestartTimeline(){
        yield return new WaitForSeconds(RESTART_DELAY);
        _birdPool.SpawnAll();
        _director.Play();
    }
}
