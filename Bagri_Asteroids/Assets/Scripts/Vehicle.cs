using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ----- Class header comment -----
// Vehicle Class
// Placed on the ship prefab
// Will move the ship around the screen and shoot bullets

public class Vehicle : MonoBehaviour {
    // Vehicle's position will be used for calculations
    public Vector3 vehiclePosition;     // = new Vector3(0, 0, 0);
    public Vector3 direction;
    public Vector3 velocity;

    // For acceleration:
    public Vector3 acceleration;    // 0, 0, 0
    public float accelRate;         // small value
    public float maxSpeed;          // small value
    public float deccelRate;

    // For rotation:
    public float totalRotation;     // accumulation of all rotation
    public float anglePerFrame;

    //Screen wrap attributes
    private Renderer[] renderers;
    private bool isWrappingX = false;
    private bool isWrappingY = false;

    //Audio 
    public AudioClip shoot;

    //References
    public GameObject bullet;
    GameManager gameManager;

    // Use this for initialization
    void Start() {
        // Get the position from the Transform component
        vehiclePosition = transform.position;

        //Grab renderers for screen wrap
        renderers = GetComponentsInChildren<Renderer>();
        //Grab reference to GameManager
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        //Rotate ship
        RotateShip();

        //Move Ship
        Accelerate();
        Wrap();
        //Fire Bullet
        if (Input.GetKeyDown(KeyCode.Space)) {
            Shoot();
        }
        UpdateTransform();
    }

    /// <summary>
    /// Change Direction of ship with arrow keys.
    /// </summary>
    void RotateShip() {
        // CHANGE THE DIRECTION WITH ARROW KEYS
        if (Input.GetKey(KeyCode.LeftArrow)) {
            // Rotate positively
            totalRotation += anglePerFrame;
            direction = Quaternion.Euler(0, 0, anglePerFrame) * direction;
        }
        else if (Input.GetKey(KeyCode.RightArrow)) {
            // Rotate negatively
            totalRotation -= anglePerFrame;
            direction = Quaternion.Euler(0, 0, -anglePerFrame) * direction;
        }
    }

    /// <summary>
    /// Accelerates ship when accelerate button is pushed
    /// Deccel when it is up
    /// </summary>
    void Accelerate() {
        //Accelerate when pushing the "Up Arrow" key
        if (Input.GetKey(KeyCode.UpArrow)) {
            acceleration = accelRate * direction;
            velocity += acceleration;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            vehiclePosition += velocity;
        }
        //Deccelerate when its up
        else {
            velocity *= deccelRate;
            vehiclePosition += velocity;
        }
    }

    /// <summary>
    /// Updates the changes to the player's position to the game
    /// </summary>
    void UpdateTransform() {
        // Update the transform
        transform.position = vehiclePosition;
        transform.rotation = Quaternion.Euler(0, 0, totalRotation);
    }

    /// <summary>
    /// Wraps player to other side of the screen when not visible from a side
    /// </summary>
    void Wrap() {
        //Checks to renderers to see if on screen
        bool isVisible = CheckRenderers();

        //Resets bools if visible on screen both ways
        if (isVisible) {
            isWrappingX = false;
            isWrappingY = false;
            return;
        }

        if (isWrappingX && isWrappingY) {
            return;
        }

        //Gets viewport position of the player
        var cam = Camera.main;
        var viewportPosition = cam.WorldToViewportPoint(transform.position);

        //If off one of the sides wrap it to the other
        if (!isWrappingX && (viewportPosition.x > 1 || viewportPosition.x < 0)) {
            vehiclePosition.x = -vehiclePosition.x;

            isWrappingX = true;
        }

        if (!isWrappingY && (viewportPosition.y > 1 || viewportPosition.y < 0)) {
            vehiclePosition.y = -vehiclePosition.y;

            isWrappingY = true;
        }
    }

    /// <summary>
    /// Check to see if the renderers are on screen.
    /// </summary>
    /// <returns></returns>
    bool CheckRenderers() {
        foreach (Renderer renderer in renderers) {
            if (renderer.isVisible) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Spawns a bullet with same rotation as the player.
    /// </summary>
    void Shoot() {
        //Spawn
        Instantiate(bullet, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);

        //Play Sound
        AudioSource.PlayClipAtPoint(shoot, Camera.main.transform.position);
    }
}
