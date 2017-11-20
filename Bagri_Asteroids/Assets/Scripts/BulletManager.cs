using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ----- Class header comment -----
// BulletManager Class
// Placed on the bullet prefab
// Pushes the bullet in the direction it is facing and  wraps it in the screen. Destroys after a set amount of time.

public class BulletManager : MonoBehaviour {

    //Reference to Ship
    Vehicle player;

    //Variables for movement
    Vector3 bulletPosition;
    Vector3 velocity;

    // Use this for initialization
    void Start () {
        //Destroy  the bullet after set amount of time
        Destroy(gameObject, 1.0f);

        //Push Bullet
        GetComponent<Rigidbody2D>().AddForce(transform.right * 400);
	}
	
	// Update is called once per frame
	void Update () {
        Wrap();
	}

    /// <summary>
    /// Wraps the object if it goes outside the screen
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
}
