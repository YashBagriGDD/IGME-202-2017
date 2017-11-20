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

    public List<GameObject> humanList = new List<GameObject>();
    public List<GameObject> zombieList = new List<GameObject>();
    public GameObject[] obstacles;
    GameObject purpleTarget;
    public int humansAlive;

    //Debug Line Materials
    public bool showLines;

    // Use this for initialization
    void Start () {
        //Grab All Obstacles in the scene
        obstacles = GameObject.FindGameObjectsWithTag("Tree");

        //Instantiate humans, zombies and targets into scene
        //Humans
        for (int i = 0; i < numOfHumans; i++) {
            GameObject spawnHuman = Instantiate(human, new Vector3(Random.Range(-0f, 200f), 7.5f, Random.Range(0f, 200f)), Quaternion.identity);
            humanList.Add(spawnHuman);
        }
        //Zombies
        for (int i = 0; i < numOfZombies; i++) {
            GameObject spawnZombie = Instantiate(zombie, new Vector3(Random.Range(0, 200f), 7.5f, Random.Range(0f, 200f)), Quaternion.identity);
            zombieList.Add(spawnZombie);
        }
        

        //Set human target and zombie target
        foreach (GameObject human in humanList) {
            human.GetComponent<Human>().zombieChaser = zombieList[0];
        }
        foreach (GameObject zombie in zombieList) {
            zombie.GetComponent<Zombie>().zombieTarget = humanList[Random.Range(0, humanList.Count)];
        }

        //Set debug lines to show
        showLines = true;
    }
	
	// Update is called once per frame
	void Update () {
        //toggle showing lines on 'd' key
        if (Input.GetKeyDown(KeyCode.D)) {
            showLines = !showLines;
        }

        //Set Zombie to closest human
        foreach (GameObject human in humanList) {
            //for each human go through each zombie
            foreach (GameObject zombie in zombieList) {
                GameObject zombieTarget = zombie.GetComponent<Zombie>().zombieTarget;
                if (zombieTarget == null && humansAlive > 0) {
                    zombieTarget = humanList[0];
                }

                //Find the distnace for each zombie for each human
                float distToHuman = Vector3.Distance(human.transform.position, zombie.transform.position);
                float distToTarget = Vector3.Distance(zombie.transform.position, zombieTarget.transform.position);

                //Zombie chases the closest human. Human flees closest zombie
                if (distToHuman <distToTarget) {
                    zombie.GetComponent<Zombie>().zombieTarget = human;
                    human.GetComponent<Human>().zombieChaser = zombie;
                }
            }
        }

        humansAlive = humanList.Count;
	}
}
