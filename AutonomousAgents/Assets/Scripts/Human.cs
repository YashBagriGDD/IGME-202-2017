using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Vehicle {

    public GameObject humanTarget;
    public GameObject zombieChaser;

    public float seekWeight;
    public float fleeWeight;

	// Use this for initialization
	public override void Start () {
        base.Start();
	}

    protected override void CalcSteeringForces() {
        // Create new ultimate force tbat's zeroed out
        Vector3 ultimateForce = Vector3.zero;

        // Add the Seek force to ultimate force, weighted by the Seek weight
        ultimateForce += (Seek(humanTarget.transform.position) * seekWeight);

        // If zombie is too close, add the Flee force to the ultimate force
        //      which is weighted by the Flee Wight
        if (Mathf.Abs(transform.position.x - zombieChaser.transform.position.x) < 30 &&
            Mathf.Abs(transform.position.y - zombieChaser.transform.position.y) < 30) {
            ultimateForce += (Flee(zombieChaser.transform.position) * fleeWeight);
        }

        // Clamp the ultimate force to the Maximum Force
        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);

        // Apply the Ultimate Force to the Human's Acceleration
        ApplyForce(ultimateForce);
    }
}
