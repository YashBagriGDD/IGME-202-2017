using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Vehicle {

    public GameObject zombieTarget;

    public float seekWeight;

    //Debug Line Variables
    public Material green;
    public Material blue;
    public Material red;
    public Material black;

    // Use this for initialization
    public override void Start () {
        base.Start();
	}

    public override void Update() {

        //Check to see if current target is null (in the case the collision deleting the human is the target)
        if (zombieTarget == null && manager.humansAlive > 0) {
            zombieTarget = manager.humanList[0];
        }

        base.Update();
    }

    protected override void CalcSteeringForces() {
        //Create a new ultimate force thats zeroed out
        Vector3 ultimateForce = Vector3.zero;

        //Check to see if humans are present in the scene
        if (manager.humansAlive > 0) {
            //Add the seek force to the ultimate force, wighted by the seek weight
            ultimateForce += (Pursue(zombieTarget) * seekWeight);
        }
        //Wander if none
        else
            ultimateForce += Wander();

        //Stay in bounds
        ultimateForce += StayInBounds();

        //Clamp the ultimate force by the maximum force
        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);

        //Apply the ultimate force to the zombie's acceleration
        ApplyForce(ultimateForce);
    }

    private void OnRenderObject() {
        //Only show when bool is true
        if (manager.showLines) {
            if (zombieTarget != null) {
                //Line to target
                black.SetPass(0);
                GL.Begin(GL.LINES);
                GL.Vertex(transform.position);
                GL.Vertex(zombieTarget.transform.position);
                GL.End();
            }

            //Forward Line
            green.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(transform.position);
            GL.Vertex(transform.position + (transform.forward * 7));
            GL.End();

            //Right Line
            blue.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(transform.position);
            GL.Vertex(transform.position + (transform.right * 7));
            GL.End();

            //Future Pos
            red.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(transform.position + (transform.forward * 10));
            GL.Vertex(transform.position + ((transform.forward * 10) + (transform.forward * .8f)));
            GL.End();
        }
    }
}
