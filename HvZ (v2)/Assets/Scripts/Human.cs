using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Youki Iimori
/// This class is a child class of vehicle. It manages 
/// render lines and steering forces for the human. No 
/// known errors
/// </summary>
public class Human : Vehicle
{
    //field for the closest zombie
    private GameObject closestZombie;

    public bool stuck = false;

    /// <summary>
    /// method to calculate the total steering forces
    /// </summary>
    public override void CalcSteeringForces()
    {
        ultimateForce = Vector3.zero; //vector for "ultimate force"
        closestZombie = zombieList[0]; //set closestzombie to a default

        //loop through zombie list
        for (int i = 1; i < zombieList.Count; i++)
        {
            //set the vector to check for the closest zombie equal to distance between you and zombie
            Vector3 zombieVector = zombieList[i].transform.position - gameObject.transform.position;
            //get magnitude of that zombie
            float zombieVectorMag = zombieVector.magnitude;
            //get a vector for the current CLOSEST zombie 
            Vector3 closestZombieComparison = closestZombie.transform.position - gameObject.transform.position;
            //get magnitude for current closest zombie's vector
            float closestZombieMagComparison = closestZombieComparison.magnitude;
            //compare the current closest zombie versus the zombie in the list you're checkign against
            if (zombieVectorMag < closestZombieMagComparison)
            {
                //if the zombie in the list is smaller than the current closest, set the closest equal to the current zombie
                closestZombie = zombieList[i];
            }
        }
        //vector and magnitude of said vector for the closest zombie
        Vector3 closestZombieVector = closestZombie.transform.position - gameObject.transform.position;
        float closestZombieMag = closestZombieVector.magnitude;

        if (DetectEdge() == false)
        {
            //if the closest zombie is less than 5 units away, evade it
            if (closestZombieMag < 5)
            {
                ultimateForce += Evade(closestZombie) * evadeScale;
            }
            //otherwise, apply friction 
            else
            {
                Wander(); //wander if the zombie isn't close by
                ApplyFriction(.5f); //if the human is greater than 5 units away, apply friction
            }
        }

        Separate(humanList);
        
        //avoid the edge if detected
        if(DetectEdge() == true)
        {
            ultimateForce += AvoidEdge();
        }
       //obstacle avoidance 
        for (int i = 0; i < obstacles.Length; i++)
        {
            ultimateForce += ObstacleAvoidance(obstacles[i]) * obstAvoidScale;
        }

        //scale the ultimate force to the human's maximum speed
        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);
        //apply U L T I M A T E force
        ApplyForce(ultimateForce);


    }

    /// <summary>
    /// detects the if the human is nearby the edge or not
    /// </summary>
    public override bool DetectEdge()
    {
        //variables for getting the human size for size adjustment
        float humanSizeX = radius/*GetComponent<Renderer>().bounds.size.x / 2*/;
        float humanSizeZ = radius/*GetComponent<Renderer>().bounds.size.z / 2*/;


        //if the human is at the edge, seek the center scaled with as much speed as possible
        if (transform.position.x > (terrainSizeX - 1.5f) - humanSizeX)
        {
            return true;
        }
        if (transform.position.x < (-terrainSizeX + 1.5f) + humanSizeX)
        {
            return true;
        }

        if (transform.position.z > (terrainSizeZ-1.5f) - humanSizeZ)
        {
            return true;
        }
        if (transform.position.z < (-terrainSizeZ+1.5f) + humanSizeZ)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// method to lock the human out of leaving the terrain
    /// </summary>
    public override Vector3 AvoidEdge()
    {
        //when avoid edge is called, seek center
        //variables for getting the human size for size adjustment
        float humanSizeX = radius /*GetComponent<Renderer>().bounds.size.x / 2*/;
        float humanSizeZ = radius /*GetComponent<Renderer>().bounds.size.z / 2*/;
        //if the human is off the terrain, set its transform.position equal the the same position but at the edge
        if (transform.position.x > terrainSizeX - humanSizeX)
        {
            transform.position = new Vector3(terrainSizeX - humanSizeX, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -terrainSizeX + humanSizeX)
        {
            transform.position = new Vector3(-terrainSizeX + humanSizeX, transform.position.y, transform.position.z);
        }

        if (transform.position.z > terrainSizeZ - humanSizeZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, terrainSizeZ - humanSizeZ);
        }
        if (transform.position.z < -terrainSizeZ + humanSizeZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -terrainSizeZ + humanSizeZ);
        }
        //seek center
        return Seek(reCenter) * maxSpeed;
    }



    /// <summary>
    /// method for rendering Debug Lines
    /// </summary>
    public void OnRenderObject()
    {
        //if debug lines are turned on
        if (debugLineProxy == true)
        {
            //draw forward vector
            mat2.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(gameObject.transform.position);
            GL.Vertex(gameObject.transform.position + (velocity));
            GL.End();
            //draw right vector
            mat3.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(gameObject.transform.position);
            GL.Vertex(gameObject.transform.position + (Quaternion.Euler(0, 90, 0) * velocity));
            GL.End();
            //vector to zombie
            mat5.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(gameObject.transform.position);
            GL.Vertex(closestZombie.transform.position + closestZombie.GetComponent<Zombie>().velocity * 5);
            GL.End();
        }
    }

}