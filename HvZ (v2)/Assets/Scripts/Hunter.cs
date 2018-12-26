using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Youki Iimori
/// A child class of vehicle, it controls the forces of third "hunter" class I added
/// No known errors
/// </summary>
public class Hunter : Vehicle {

    //field for the closest zombie
    private GameObject closestZombie;

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
   

        //seeks the PSG so the human is constantly moving 
        //ultimateForce += Seek(psg);
        if(zombieList.Count == 0)
        {
            Wander();
        }
        //hunt down the closest zombie, assuming it doesn't detect an edge
        else if (DetectEdge() == false)
        {
            ultimateForce += Seek(closestZombie) * 2;
        }

        else if (DetectEdge() == true)
        {
            ultimateForce += AvoidEdge();
        }
        

        //separate from both humans and fellow hunters, considering both are allies
        Separate(humanList);
        Separate(hunterList);

        //obstacle avoidance 
        for (int i = 0; i < obstacles.Length; i++)
        {
            //call obstacleavoidance passing in each obstacle in the list, then scale it by a set amount
            ultimateForce += ObstacleAvoidance(obstacles[i]) * obstAvoidScale;
        }

        //scale the ultimate force to the human's maximum speed
        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);
        //apply U L T I M A T E force
        ApplyForce(ultimateForce);


    }

    /// <summary>
    /// DetectEdge is a method simply for detecting the edge of the map.
    /// </summary>
    public override bool DetectEdge()
    {
        //variables for getting the human size for size adjustment
        float humanSizeX = radius;
        float humanSizeZ = radius;


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
        //ultimateForce += Seek(reCenter) * maxSpeed;
        //variables for getting the human size for size adjustment
        float humanSizeX = radius;
        float humanSizeZ = radius;
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
        return Seek(reCenter) * maxSpeed;
    }


    /// <summary>
    /// method for rendering Debug Lines
    /// Because the hunter uses seek instead of pursue, it 
    /// doesn't have a debug line for pursue
    /// </summary>
    public void OnRenderObject()
    {
        //if debug lines are turned on
        if (debugLineProxy == true)
        {
            mat1.SetPass(0);
            //code to draw the line seeking the nearest zombie
            //draw one line
            GL.Begin(GL.LINES); //start to draw lines
            GL.Vertex(gameObject.transform.position); //first endpoint of line
            GL.Vertex(closestZombie.transform.position); //second endpoint of line
            GL.End(); //finish drawing line
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
        }
    }

}

