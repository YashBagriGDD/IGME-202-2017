using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieButton : MonoBehaviour {

    //Reference to Game Manager
    ExerciseManager manager;

    // Use this for initialization
    void Start() {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ExerciseManager>();
    }

    // Update is called once per frame
    void Update() {

    }

    //On Click will spawn a Zombie and add it to the zombie list
    void OnMouseDown() {
        GameObject spawnZombie = Instantiate(manager.zombie, new Vector3(Random.Range(-0f, 200f), 7.5f, Random.Range(0f, 200f)), Quaternion.identity);
        manager.zombieList.Add(spawnZombie);
    }
}
