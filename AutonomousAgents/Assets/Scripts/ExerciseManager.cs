using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExerciseManager : MonoBehaviour {

    //References to objects in the scene
    public GameObject human;
    public GameObject zombie;
    public GameObject target;

    //Manager number of agents spawned
    public int numOfHumans;
    public int numOfZombies;

    GameObject[] humanArray;
    GameObject[] zombieArray;
    GameObject purpleTarget;
    public int humansAlive;

	// Use this for initialization
	void Start () {
        //Instantiate humans, zombies and targets into scene
        //Target
        purpleTarget = Instantiate(target, new Vector3(Random.Range(0, 200f), 7.5f, Random.Range(-00f, 200f)), Quaternion.identity);
        //Humans
        for (int i = 0; i < numOfHumans; i++) {
            Instantiate(human, new Vector3(Random.Range(-0f, 200f), 7.5f, Random.Range(0f, 200f)), Quaternion.identity);
        }
        humanArray = GameObject.FindGameObjectsWithTag("Human");
        humansAlive = humanArray.Length;
        //Zombies
        for (int i = 0; i < numOfZombies; i++) {
            Instantiate(zombie, new Vector3(Random.Range(0, 200f), 7.5f, Random.Range(0f, 200f)), Quaternion.identity);
        }
        zombieArray = GameObject.FindGameObjectsWithTag("Zombie");
        
        //Set human target and zombie target
        foreach (GameObject human in humanArray) {
            human.GetComponent<Human>().humanTarget = purpleTarget;
            human.GetComponent<Human>().zombieChaser = zombieArray[0];
        }
        foreach (GameObject zombie in zombieArray) {
            zombie.GetComponent<Zombie>().zombieTarget = humanArray[Random.Range(0, humanArray.Length)];
        }
    }
	
	// Update is called once per frame
	void Update () {
        //Set Zombie to closest human
        zombieArray = GameObject.FindGameObjectsWithTag("Zombie");
        humanArray = GameObject.FindGameObjectsWithTag("Human");
        foreach (GameObject human in humanArray) {
            //for each human go through each zombie
            foreach (GameObject zombie in zombieArray) {
                //Find the distnace for each zombie for each human
                float distToHuman = Vector3.Distance(human.transform.position, zombie.transform.position);
                float distToTarget = Vector3.Distance(zombie.transform.position, zombie.GetComponent<Zombie>().zombieTarget.transform.position);

                //Zombie chases the closest human. Human flees closest zombie
                if (distToHuman <distToTarget) {
                    zombie.GetComponent<Zombie>().zombieTarget = human;
                    human.GetComponent<Human>().zombieChaser = zombie;
                }
            }
        }

        //If Human gets too close to target move to random spot
        foreach (GameObject human in humanArray) {
            if (Mathf.Abs(purpleTarget.transform.position.x - human.transform.position.x) < 6 && 
                Mathf.Abs(purpleTarget.transform.position.y - human.transform.position.y) < 6) {
                purpleTarget.transform.position = new Vector3(Random.Range(0f, 200f), 7.5f, Random.Range(-0f, 200f));
            }
        }
	}
}
