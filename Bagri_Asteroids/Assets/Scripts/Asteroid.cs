using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ----- Class header comment -----
// Asteroid Class
// Placed on the asteroids prefabs
// Pushes asteroids and wraps them on screen. Handles collisions for the bullet and player.

public class Asteroid : MonoBehaviour {

    //Objects to be inserted in inspector
    public GameObject smallAsteroid;
    public AudioClip die;
    public AudioClip shipCrash;

    //References
    GameManager gameManager;
    GameObject player;

    //Raddi for collisions
    public float shipR = .5f;
    public float asteroidR = 2f;
    public float bulletR = .1f;
    public float smallR = .5f;

    float thisAstR;

    // Use this for initialization
    void Start () {
        //Set references
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Ship");

        //Push the asteroid
        GetComponent<Rigidbody2D>().AddForce(transform.up * 70f);

        //Set this asteroid's raddi
        if (tag.Equals("Large Asteroid"))
            thisAstR = asteroidR;
        else
            thisAstR = smallR;
    }
	
	// Update is called once per frame
	void Update () {
        playerCol();
        bulletCol();
        Wrap();
	}

    /// <summary>
    /// Checks if this asteroid is colliding with the player
    /// </summary>
    void playerCol() {
        if (BoundingCircle(gameObject, player, thisAstR, shipR)) {
            AudioSource.PlayClipAtPoint(shipCrash, Camera.main.transform.position);

            gameManager.DecreseLife();
        }
    }

    /// <summary>
    /// Check if this asteroid is colliding with any bullets on screen
    /// </summary>
    void bulletCol() {
        //Grab all bullets on screen
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

        //Loop through each on and check if they're colliding
        foreach (GameObject item in bullets) {
            if (BoundingCircle(gameObject, item, thisAstR, bulletR)) {
                //Destroy Bullet
                Destroy(item.gameObject);

                //If Large spawn small asteroids
                if (tag.Equals("Large Asteroid")) {
                    // Spawn small asteroids
                    Instantiate(smallAsteroid, new Vector3(transform.position.x - .5f, transform.position.y - .5f, 0), Quaternion.Euler(0, 0, 90));

                    Instantiate(smallAsteroid, new Vector3(transform.position.x + .5f, transform.position.y + .0f, 0), Quaternion.Euler(0, 0, 0));

                    Instantiate(smallAsteroid, new Vector3(transform.position.x + .5f, transform.position.y - .5f, 0), Quaternion.Euler(0, 0, 270));

                    //Increase Asteroid count
                    gameManager.asteroidsRemaining += 2;
                }
                //Small Asteroid Destroyed
                else
                    gameManager.asteroidsRemaining--;

                //Play Sound
                AudioSource.PlayClipAtPoint(die, Camera.main.transform.position);
                //Increase Score
                gameManager.IncreaseScore();
                //Destroy this object
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Wrap this game object if it goes off screen
    /// </summary>
    void Wrap() {
        // Teleport the game object
        if (transform.position.x > 14.5f)
            transform.position = new Vector3(-14.5f, transform.position.y, 0);
        else if (transform.position.x < -14.5f)
            transform.position = new Vector3(14.5f, transform.position.y, 0);
        else if (transform.position.y > 6)
            transform.position = new Vector3(transform.position.x, -6, 0);
        else if (transform.position.y < -6)
            transform.position = new Vector3(transform.position.x, 6, 0);
    }

    /// <summary>
    /// Checks to see if the two Game Object's bounding circles are colliding
    /// </summary>
    /// <param name="a">Game Object 1</param>
    /// <param name="b">Game Object 2</param>
    /// <param name="aRad">Radius of Game Object 1</param>
    /// <param name="bRad">Radius of Game Object 2</param>
    /// <returns></returns>
    bool BoundingCircle(GameObject a, GameObject b, float aRad, float bRad) {
        // Get positions
        Vector3 aPos = a.transform.position;
        Vector3 bPos = b.transform.position;

        // Check if distance between is less than the radii added together
        if ((bPos - aPos).magnitude < (aRad + bRad)) {
            return true;
        }
        else return false;
    }
}
