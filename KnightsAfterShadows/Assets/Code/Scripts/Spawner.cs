using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Spawner : MonoBehaviour{
    public List<GameObject> charactersToSpawn;
    public float cooldown;

    private void Start() {
        //If you forgot to assign a GameObject to the list
        for (int i = 0; i < charactersToSpawn.Count; i++){
            if (charactersToSpawn[i] == null){
                Debug.Log("GameObject missing in the list by index" + i);
            }
        }
        //Starts the coroutine for spawning entities every 5 seconds
        StartCoroutine(SpawnCharactersRepeatedly());
    }
    private IEnumerator SpawnCharactersRepeatedly() {
        while (true)
        {
            yield return new WaitForSeconds(cooldown);  // Every 5 seconds
            yield return SpawnAllCharacters();  //Spawns everything
            
        }
    }
    private IEnumerator SpawnAllCharacters(){
        for (int i = 0; i < charactersToSpawn.Count; i++){
            GameObject character = charactersToSpawn[i];

            if (character != null){
                GameObject spawned = Instantiate(character, this.transform);
                spawned.transform.localPosition = Vector3.zero;
                spawned.transform.localRotation = Quaternion.identity;
            }

            yield return new WaitForSeconds(cooldown);
        }
    }
}
