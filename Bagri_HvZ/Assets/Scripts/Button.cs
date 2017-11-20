using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    //Reference to Game Manager
    ExerciseManager manager;

	// Use this for initialization
	void Start () {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ExerciseManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //On Mouse click will spawn a human and add it to the human list
    void OnMouseDown() {
        GameObject spawnHuman = Instantiate(manager.human, new Vector3(Random.Range(-0f, 200f), 7.5f, Random.Range(0f, 200f)), Quaternion.identity);
        manager.humanList.Add(spawnHuman);
    }
}
