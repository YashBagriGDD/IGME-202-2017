using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Vehicle {

    public GameObject zombieChaser;

    public float seekWeight;
    public float fleeWeight;

    //Debug Line Variables
    public Material green;
    public Material blue;
    public Material purple;

	// Use this for initialization
	public override void Start () {
        base.Start();
	}

    public override void Update() {
        //Check Collisions
        ZombieCol();
        //Call base movement methods
        base.Update();
    }

    protected override void CalcSteeringForces() {
        // Create new ultimate force tbat's zeroed out
        Vector3 ultimateForce = Vector3.zero;

        // If zombie is too close, add the Flee force to the ultimate force
        //      which is weighted by the Flee Wight
        if (Mathf.Abs(transform.position.x - zombieChaser.transform.position.x) < 30 &&
            Mathf.Abs(transform.position.z - zombieChaser.transform.position.z) < 30) {
            ultimateForce += (Evade(zombieChaser) * fleeWeight);
        }
        //Else wander around
        else
            ultimateForce += Wander();

        //Add forces for avoiding obstacles and staying in bounds
        ultimateForce += AvoidObstacle();
        ultimateForce += StayInBounds();

        // Clamp the ultimate force to the Maximum Force
        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);

        // Apply the Ultimate Force to the Human's Acceleration
        ApplyForce(ultimateForce);
    }

    void ZombieCol() {
        //Check against every zombie in the scene
        foreach (GameObject zombie in manager.zombieList) {
            //Check to see if this human is colliding with any zombies
            if (BoundingCircle(gameObject, zombie, radius, zombie.GetComponent<Zombie>().radius)) {
                //If colliding spawn new zombie and add it to the zombie list
                GameObject spawnZombie = Instantiate(manager.zombie, transform.position, Quaternion.Euler(direction));
                manager.zombieList.Add(spawnZombie);

                //Delete this human from the human list then delete this object
                manager.humanList.Remove(this.gameObject);
                GameObject.Destroy(this.gameObject);
            }
        }
    }

    private void OnRenderObject() {
        //Only show when bool is true
        if (manager.showLines) {
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
            purple.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(transform.position + (transform.forward * 10));
            GL.Vertex(transform.position + ((transform.forward * 10) + (transform.forward * .8f)));
            GL.End();
        }
    }
}
