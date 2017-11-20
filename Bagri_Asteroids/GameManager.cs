using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ----- Class header comment -----
// GameManager class
// Placed on an empty object in the scene
// Sets the hud, handles death, score, setting the wave, and clearing asteroids

public class GameManager : MonoBehaviour {

    //Reference to asteroids
    public GameObject asteroid;

    //Variables for score and waves
    int score;
    int hiscore;
    public int asteroidsRemaining;
    int lives;
    int wave;
    public int waveIncrease;

    //HUD text variables
    public Text scoreText;
    public Text hiscoreText;
    public Text livesText;
    public Text wavesText;

	// Use this for initialization
	void Start () {
        //Retrieve hiscore information
        hiscore = PlayerPrefs.GetInt("hiscore", 0);
        //Start Game
        Begin();
	}
	
	// Update is called once per frame
	void Update () {
        //Quit on escape press
        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }
	}

    /// <summary>
    /// Sets the hud and game for the start and spawns asteroids
    /// </summary>
    void Begin() {
        //Set starting values
        score = 0;
        wave = 1;
        lives = 3;

        //Set HUD
        scoreText.text = "Score: " + score;
        hiscoreText.text = "HiScore: " + hiscore;
        livesText.text = "Lives: " + lives;
        wavesText.text = "Wave: " + wave;

        //Spawn Asteroids
        AsteroidSpawn();
    }

    /// <summary>
    /// Removes all asteroids and bullets currently spawned. 
    /// Randomly spawns asteroids in the view and gives them a random rotation
    /// Spawns number according to wave and resets player position
    /// </summary>
    void AsteroidSpawn() {
        //Removes asteroids in game (from previous waves/game)
        ClearAsteroids();

        //Determine how many asteroids to spawn;
        asteroidsRemaining = wave * waveIncrease;

        //Spawn accordingly
        for (int i = 0; i < asteroidsRemaining; i++) {
            //Spawn Asteroids
            Instantiate(asteroid, new Vector3(Random.Range(-14f, 14f),Random.Range(-6f, 6f),0), Quaternion.Euler(0, 0, Random.Range(-0.0f, 359.0f)));
        }

        //Set hud
        wavesText.text = "Wave: " + wave;

        //Reset Player position and velocity
        GameObject.FindGameObjectWithTag("Ship").GetComponent<Vehicle>().vehiclePosition = new Vector3(0, 0, 0);
        GameObject.FindGameObjectWithTag("Ship").GetComponent<Vehicle>().velocity = new Vector3(0, 0, 0);
    }


    /// <summary>
    /// Finds all asteroids and bullets on screen and removes them.
    /// </summary>
    void ClearAsteroids() {
        //Grabs all asteroids and deletes them
        GameObject[] asteroidsLarge =GameObject.FindGameObjectsWithTag("Large Asteroid");
        foreach (GameObject a in asteroidsLarge) {
            GameObject.Destroy(a);
        }
        GameObject[] asteroidsSmall = GameObject.FindGameObjectsWithTag("Small Asteroid");
        foreach (GameObject a in asteroidsSmall) {
            GameObject.Destroy(a);
        }

        //Grabs all bullets and destroys them
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject b in bullets) {
            GameObject.Destroy(b);
        }
    }

    /// <summary>
    /// Decrese life and change hud value
    /// Resets Player's position
    /// If lives less than 0 reset game
    /// </summary>
    public void DecreseLife() {
        //Descrease life and update UI to show
        lives--;
        livesText.text = "Lives: " + lives;

        //Reset Ship Position
        GameObject.FindGameObjectWithTag("Ship").GetComponent<Vehicle>().vehiclePosition = new Vector3(0, 0, 0);
        GameObject.FindGameObjectWithTag("Ship").GetComponent<Vehicle>().velocity = new Vector3(0, 0, 0);

        //Check to see if out of lives
        if (lives < 1) {
            Begin();
        }
    }

    /// <summary>
    /// Increases score and updates the hud
    /// Keeps track of hiscore and updates it accordingly
    /// If all asteroids gone, start new wave
    /// </summary>
    public void IncreaseScore() {
        score += 100;
        scoreText.text = "Score: " + score;

        //If Score goes above HiScore change HiScore
        if (score > hiscore) {
            hiscore = score;
            hiscoreText.text = "HiScore: " + hiscore;

            //Save hiscore
            PlayerPrefs.SetInt("hiscore", hiscore);
        }

        //Check to see if all asteroids are destroyed
        if (asteroidsRemaining < 1) {
            //New Wave
            wave++;
            AsteroidSpawn();
        }
    }


}
