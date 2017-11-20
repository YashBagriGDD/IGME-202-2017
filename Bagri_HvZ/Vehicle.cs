using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Vehicles (for Autonomous Agents)

public abstract class Vehicle : MonoBehaviour 
{
	// Vectors for force-based movement
	public Vector3 velocity;
	public Vector3 direction;
	public Vector3 acceleration;
	public Vector3 position;

	// Floats (for AA)
	public float mass;			// Use now
	public float maxSpeed;		// Use now
	public float maxForce;		// Use later
	public float radius;		// Use later for collisions
    public float viewDistance;  // For determining how far the vehicle can "see" ahead
    public float angle;

    //Reference Game Manager
    public ExerciseManager manager;

    // Use this for initialization
    public virtual void Start() {
        // Grab the transform's position 
        position = transform.position;

        //Get reference to scene manager
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ExerciseManager>();
    }

	// Update is called once per frame
	public virtual void Update () 
	{
        CalcSteeringForces();

        UpdatePosition();

        SetTransform();
	}

    protected abstract void CalcSteeringForces();

    /// <summary>
    /// Calculates Velocity and Direction,
    /// </summary>
    void UpdatePosition() {
        // Calculate movement
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        position += velocity * Time.deltaTime;
        direction = velocity.normalized;

        // "Draw" object at its position
        transform.position = position;
        transform.rotation = Quaternion.Euler(direction);

        // Start fresh each frame
        acceleration = Vector3.zero;
    }

    //Sets the game object's forward transform (so it faces the correct direction) and position
    void SetTransform() {
        transform.forward = direction;

        // Grab the transform's position - useful for testing!
        position = transform.position;
    }

    // void ApplyForce()
    // Apply an incoming force to this vehicle's acceleration
    public void ApplyForce(Vector3 force)
	{
		acceleration += force / mass;
	}

	// Seek method
    // Params: Target's location
    // Returns: Seeking steering force
	// Move at max speed toward a target
	// Assume we already know the targets location
	public Vector3 Seek (Vector3 targetPosition)
	{
		// Find desired velocity (vector from me to my target)
		Vector3 desiredVelocity = targetPosition - position;

		// Scale my desired velocity to max speed
		desiredVelocity.Normalize();
		desiredVelocity = desiredVelocity * maxSpeed;

		// Find the seeking steering force (desired - current)
		Vector3 seekingForce = desiredVelocity - velocity;

		// Return the seeking steering force
		return seekingForce;
	}

    // Flee method
    // Params: Target's location
    // Returns: Fleeing steering force
    // Move at max speed away from target
    // Assume we already know the targets location
    public Vector3 Flee(Vector3 targetPosition) {
        // Find desired velocity (vector from me to my target)
        Vector3 desiredVelocity = position - targetPosition;

        // Scale my desired velocity to max speed
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        // Find the seeking steering force (desired - current)
        Vector3 fleeingForce = desiredVelocity - velocity;

        // Return the seeking steering force
        return fleeingForce;
    }

    /// <summary>
    /// Pursue method takes an object and will seek the future position of that object based off where they're facing
    /// </summary>
    /// <param name="target">The object to get the future position of</param>
    /// <returns>Vector3 of the force of seeking that future position</returns>
    public Vector3 Pursue(GameObject target){
        Vector3 futurePos = target.transform.position + (target.transform.forward * 10);
        return Seek(futurePos);
    }

    /// <summary>
    /// Pursue method takes an object and will flee from the future position of that object based off where they're facing
    /// </summary>
    /// <param name="target">The object to get the future position of</param>
    /// <returns>Vector3 of the force of fleeing from that future position</returns>
    public Vector3 Evade(GameObject target){
        Vector3 futurePos = target.transform.position + (target.transform.forward * 10);
        return Flee(futurePos);
    }

    public Vector3 AvoidObstacle(){
        Vector3 totalVel = Vector3.zero;

        //Go through each obstacle in array
        foreach (var obstacle in manager.obstacles) {
            //Get the Vector from the center to the obstacle's center
            Vector3 vecToObstacleCenter = obstacle.transform.position - transform.position;
            //Dot product the vector with the forward vector
            float forwardDot = Vector3.Dot(transform.forward, vecToObstacleCenter);
            //Check to see if the dot product put the object in front of the vehicle and if its in the view range
            if (forwardDot > 0 && (Vector3.SqrMagnitude(vecToObstacleCenter) < (viewDistance *viewDistance))) {
                //Dot to find which side object is relative to vehicle
                float rightDot = Vector3.Dot(transform.right, vecToObstacleCenter);
                //If the dot is less than the sum of the two radii, potential collision
                if(rightDot < (radius + obstacle.GetComponent<Tree>().radius)) {
                    // Find desired velocity (Opposite of the dot product with the right vector)
                    Vector3 desiredVelocity;
                    if (rightDot < 0)
                        desiredVelocity = Vector3.left;
                    else
                        desiredVelocity = Vector3.left;

                    // Scale my desired velocity to max speed
                    desiredVelocity = desiredVelocity * maxSpeed;

                    // Find the avoiding force
                    Vector3 avoidForce = desiredVelocity - velocity;

                    // Add avoid force
                    totalVel += avoidForce;
                }
            }
        }

        //Do the same as obstacles for Zombies
        foreach (var obstacle in manager.zombieList) {
            Vector3 vecToObstacleCenter = obstacle.transform.position - transform.position;
            float forwardDot = Vector3.Dot(transform.forward, vecToObstacleCenter);
            if (forwardDot > 0 && (Vector3.SqrMagnitude(vecToObstacleCenter) < (viewDistance * viewDistance))) {
                float rightDot = Vector3.Dot(transform.right, vecToObstacleCenter);
                if (rightDot < (radius + obstacle.GetComponent<Zombie>().radius)) {
                    // Find desired velocity (Opposite of the dot product with the right vector)
                    Vector3 desiredVelocity;
                    if (rightDot < 0)
                        desiredVelocity = Vector3.left;
                    else
                        desiredVelocity = Vector3.left;

                    // Scale my desired velocity to max speed
                    desiredVelocity = desiredVelocity * maxSpeed;

                    // Find the avoiding force
                    Vector3 avoidForce = desiredVelocity - velocity;

                    // Add avoid force
                    totalVel += avoidForce;
                }
            }
        }

        //Return the total forces of the avoidForce
        return totalVel;
    }

    /// <summary>
    /// Finds the closest edge to the object and seeks the center inverse to the distance from an edge
    /// </summary>
    /// <returns>Seeking force for the center</returns>
    public Vector3 StayInBounds(){
        float distanceFromEdge;
        //Set distance from object to positive ex edge
        distanceFromEdge = Mathf.Abs(197 - transform.position.x);
        //Compare to negative x edge
        if (distanceFromEdge > Mathf.Abs(3 - transform.position.x))
            distanceFromEdge = Mathf.Abs(3 - transform.position.x);
        //Compare to negative Z edge
        if (distanceFromEdge > Mathf.Abs(3 - transform.position.z))
            distanceFromEdge = Mathf.Abs(3 - transform.position.z);
        //Compare to positive z edge
        if (distanceFromEdge > Mathf.Abs(197 - transform.position.z))
            distanceFromEdge = Mathf.Abs(197 - transform.position.z);

        //Have it seek the center
        return (Seek(new Vector3(100, transform.position.y, 100)) / distanceFromEdge * 10);
    }

    /// <summary>
    /// Haves an agent wander in relation to it's view
    /// </summary>
    /// <returns>Vector3 of the seeking force from the wander</returns>
    public Vector3 Wander() {
        //Generate a random number to serve as the angle for the view
        float ranAngle = Random.Range(-6f, 6f);
        // update the wander point angle
        angle += ranAngle * Time.deltaTime;

        // project a point in front of the object
        Vector3 circleCenter = transform.position + transform.forward;

        // extended vector is the rotating vector centered at the circle's center, and rotates around
        Vector3 extendedVector = circleCenter + (transform.forward * 0.3f);

        // rotate the extended vector by the wandering point angle
        Vector3 extendedDirection = Quaternion.Euler(0f, angle, 0f) * extendedVector;

        //Seek the direction
        Vector3 wanderForce = Seek(extendedDirection);

        return wanderForce;
    }

    /// <summary>
    /// Checks to see if the two Game Object's bounding circles are colliding
    /// </summary>
    /// <param name="a">Game Object 1</param>
    /// <param name="b">Game Object 2</param>
    /// <param name="aRad">Radius of Game Object 1</param>
    /// <param name="bRad">Radius of Game Object 2</param>
    /// <returns></returns>
    public bool BoundingCircle(GameObject a, GameObject b, float aRad, float bRad) {
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
