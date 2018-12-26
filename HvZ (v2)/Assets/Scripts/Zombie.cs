using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Youki Iimori
/// This class is a class that deals with the forces for Zombie,
/// a child class of Vehicle
/// </summary>
public class Zombie : Vehicle
{
    // Use this for initialization
    private GameObject nearestHuman; 
    //gameobject for the nearest human

    // Update is called once per frame
    /// <summary>
    /// Method to calculate various steering forces on the object
    /// </summary>
    public override void CalcSteeringForces()
    {
        nearestHuman = humanList[0]; //set nearesthuman to a default
        for (int i = 1; i < humanList.Count; i++) //loop through humanlist
        {
            //set nearestVector based on the position of the current human and the current zombie
            Vector3 nearVector = humanList[i].transform.position - gameObject.transform.position;
            float nearVectorMag = nearVector.magnitude; //store the magnitude of above vector
            //set the CURRENT near vector equal to the nearest human vs the current transform position
            Vector3 currentNearVector = nearestHuman.transform.position - transform.position;
            if (nearVectorMag < currentNearVector.magnitude) //compare the new potential nearest human and the current nearest human
            {
                //if the new one is closer tha nthe old one, set the nearest human value to the new one
                nearestHuman = humanList[i];
            }
        }

        if (humanList.Count == 0)
        {
            //if there are no humans, wander
            Wander();
        }
        else if (DetectEdge() == false)
        {
            //add pursuing the nearest human to ultimate force
            ultimateForce += Pursue(nearestHuman) * pursueScale;
        }
        else if (DetectEdge() == true)
        {
            ultimateForce += AvoidEdge();
        }

        Separate(zombieList);

        //obstacle avoidance 
       for (int i = 0; i < obstacles.Length; i++)
        {
           ultimateForce += ObstacleAvoidance(obstacles[i]) * obstAvoidScale;
        }


        //clamp ultimate force
        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);

        //apply ultimateforce
        ApplyForce(ultimateForce);
    }



    /// <summary>

    /// This method detects if the zombie is close to the edge or not

    /// </summary>
    public override bool DetectEdge()
    {
        //variables for getting the human size for size adjustment
        float zombieSizeX = radius;
        float zombieSizeZ = radius;


        //if the human is at the edge, seek the center scaled with as much speed as possible
        if (transform.position.x > (terrainSizeX - 1.5f) - zombieSizeX)
        {
            return true;
        }
        if (transform.position.x < (-terrainSizeX + 1.5f) + zombieSizeX)
        {
            return true;
        }

        if (transform.position.z > (terrainSizeZ - 1.5f) - zombieSizeZ)
        {
            return true;
        }
        if (transform.position.z < (-terrainSizeZ + 1.5f) + zombieSizeZ)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// this is a method which both ensures that the zombie doesn't leave the stage, and uses seek to do the same.
    /// </summary>
    public override Vector3 AvoidEdge()
    {
        //when avoid edge is called, seek center
        //variables for getting the human size for size adjustment
        float zombieSizeX = radius;
        float zombieSizeZ = radius;
        //if the human is off the terrain, set its transform.position equal the the same position but at the edge
        if (transform.position.x > terrainSizeX - zombieSizeX)
        {
            transform.position = new Vector3(terrainSizeX - zombieSizeX, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -terrainSizeX + zombieSizeX)
        {
            transform.position = new Vector3(-terrainSizeX + zombieSizeX, transform.position.y, transform.position.z);
        }

        if (transform.position.z > terrainSizeZ - zombieSizeZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, terrainSizeZ - zombieSizeZ);
        }
        if (transform.position.z < -terrainSizeZ + zombieSizeZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -terrainSizeZ + zombieSizeZ);
        }
        return Seek(reCenter) * maxSpeed;
    }

    //code for debug lines
    public void OnRenderObject()
    {
        //if debug lines are turned on and humans exist
        if (debugLineProxy == true && humanList.Count != 0)
        {
            //GameObject human = GameObject.Find("Human");
            //set the material to be used for the first line
            mat1.SetPass(0);

            //code to draw the line seeking the nearest Human
            //draw one line
            GL.Begin(GL.LINES); //start to draw lines
            GL.Vertex(gameObject.transform.position); //first endpoint of line
            GL.Vertex(nearestHuman.transform.position); //second endpoint of line
            GL.End(); //finish drawing line

            //code to draw the debug line for the forward vector
            mat2.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(gameObject.transform.position);
            GL.Vertex(gameObject.transform.position + (velocity * 2));
            GL.End();

            //code for the debug line for the right vector
            mat3.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(gameObject.transform.position);
            GL.Vertex(gameObject.transform.position + (Quaternion.Euler(0, 90, 0) * (velocity * 2)));
            GL.End();

            mat4.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(gameObject.transform.position);
            GL.Vertex(nearestHuman.transform.position + nearestHuman.GetComponent<Human>().velocity * 5);
            GL.End();
        }
    }

}