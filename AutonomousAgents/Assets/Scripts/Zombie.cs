using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Vehicle {

    public GameObject zombieTarget;

    public float seekWeight;

    // Use this for initialization
    public override void Start () {
        base.Start();
	}

    protected override void CalcSteeringForces() {
        //Create a new ultimate force thats zeroed out
        Vector3 ultimateForce = Vector3.zero;

        //Add the seek force to the ultimate force, wighted by the seek weight
        ultimateForce += (Seek(zombieTarget.transform.position) * seekWeight);

        //Clamp the ultimate force by the maximum force
        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);

        //Apply the ultimate force to the zombie's acceleration
        ApplyForce(ultimateForce);
    }
}
