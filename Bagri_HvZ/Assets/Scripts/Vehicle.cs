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

    // Use this for initialization
    public virtual void Start() {
        // Grab the transform's position 
        position = transform.position;
    }

	// Update is called once per frame
	void Update () 
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
        Vector3 desiredVelocity =position - targetPosition;

        // Scale my desired velocity to max speed
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        // Find the seeking steering force (desired - current)
        Vector3 fleeingForce = desiredVelocity - velocity;

        // Return the seeking steering force
        return fleeingForce;
    }
}
