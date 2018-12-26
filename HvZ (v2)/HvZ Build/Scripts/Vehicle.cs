using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Youki Iimori
/// an abstract class that holds the defaults for vehicle behavior
/// and is inherited by humans and zombies
/// </summary>
public abstract class Vehicle : MonoBehaviour
{
    //variables for scaling vectors 
    public const float obstAvoidScale = 300;
    public const float evadeScale = 5f;
    public const float pursueScale = .5f;
    // Vectors necessary for force-based movement
    protected Vector3 vehiclePosition;
    public Vector3 acceleration;
    public Vector3 direction;
    public Vector3 velocity;

    //material references
    public Material mat1;
    public Material mat2;
    public Material mat3;
    public Material mat4;
    public Material mat5;

    //references for child classes
    protected List<GameObject> humanList;
    protected List<GameObject> zombieList;
    protected List<GameObject> hunterList;
    protected GameObject reCenter;
    protected bool debugLineProxy;
    protected GameObject targetObject;

    // Floats
    public float mass;
    public float maxSpeed;
    public float maxForce = 10;
    float angle = 10; //start the angle at 10 degrees
    //the ultimate force
    protected Vector3 ultimateForce;

    //variables for the terrain (used in lower classes)
    protected GameObject terrain;
    protected float terrainSizeX;
    protected float terrainSizeZ;
    protected Camera cameraVar;

    //obstacle avoidance
    public float radius;
    public float safeDistance;
    public GameObject[] obstacles;

    // SceneManager reference
    protected GameObject sceneManager;

    //velocity property
    public Vector3 Velocity
    {
        get { return velocity; }
    }

    // Use this for initialization
    protected void Start()
    {
        sceneManager = GameObject.Find("Scene Manager");

        //find the center force
        reCenter = GameObject.Find("CenterForce");

        //variables to grab the humanList and zombielist
        humanList = sceneManager.GetComponent<SceneManager2>().humanList;
        zombieList = sceneManager.GetComponent<SceneManager2>().zombieList;
        hunterList = sceneManager.GetComponent<SceneManager2>().hunterList;

        //code to grab the terrain size and store it
        terrain = GameObject.Find("Terrain");
        terrainSizeX = terrain.GetComponent<Renderer>().bounds.size.x / 2;
        terrainSizeZ = terrain.GetComponent<Renderer>().bounds.size.z / 2;

        //set the vehicle position variable to the transform.position
        vehiclePosition = transform.position;

        //obstacle avoidance
        radius = 1.5f;
        safeDistance = 3;
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

    }

    // Update is called once per frame
    protected void Update()
    {
        //grabs the debugLine setter to make sure that all debug liens wil lbe shown, even for new objects that don't get the lines at start
        debugLineProxy = sceneManager.GetComponent<SceneManager2>().debugLinesOn;

        // The stuff remaining from how we moved vehicles before
        acceleration.y = 0; //make sure the objects stay on the terrain.
        velocity += acceleration * Time.deltaTime;
        vehiclePosition += velocity * Time.deltaTime;

        //set the direction equal to normalized velocity
        direction = velocity.normalized;

        // Zero out acceleration so we start fresh each frame
        acceleration = Vector3.zero;

        // "Draw" the object at its position
        transform.position = vehiclePosition;

        //implement rotation 
        transform.LookAt(transform.position + velocity);
       
        //as long as humans exist, call the method to calculate steering forces
        if (humanList.Count != 0 && zombieList.Count != 0)
        {
            CalcSteeringForces();
        }
    }

    /// <summary>
    ///  ApplyForce 
    ///  Receive an incoming force, divide by mass, and apply to the cumulative accel vector
    /// </summary>
    /// <param name="force"></param>
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    /// <summary>
    /// method to apply gravity
    /// I don't think it does anything but I'm leaving it in to be safe
    /// </summary>
    /// <param name="force"></param>
    public void ApplyGravityForce(Vector3 force)
    {
        //add the force given (gravity) to the acceleration
        acceleration += force;
    }


    /// <summary>
    /// method to apply friction, takes in the friction coefficient
    /// </summary>
    /// <param name="coeff"></param>
    public void ApplyFriction(float coeff)
    {
        //the friction vector is the reverse of the velocity
        Vector3 friction = velocity * -1;
        //normalize it
        friction.Normalize();
        //multiply it by the coefficient of friction, then add it to acceleration
        Vector3 frictionAdd = friction * coeff;
        acceleration += frictionAdd;
    }

    /// <summary>
    /// method to seek out a target vector
    /// takes in a position then returns a vector3 force
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    public Vector3 Seek(Vector3 targetPosition)
    {
        //step 1: Find Desired velocity
        Vector3 desiredVelocity = targetPosition - vehiclePosition;

        //step 2: Scale velocity to maximum speed
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        //step 3: calculate seeking steering force
        Vector3 seekingForce = desiredVelocity - velocity;

        //keep the object from flying up
        transform.position = new Vector3(transform.position.x, .5f, transform.position.z);

        //step 4: return force
        return seekingForce;
    }

    /// <summary>
    /// alternate version of the previous method that only takes in a target gameobject, then passes
    /// in that gameobject's position for the original seek
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public Vector3 Seek(GameObject target)
    {
        return Seek(target.transform.position);
    }

    /// <summary>
    /// method to flee from target position vector
    /// it takes in a vector to flee from and returns a vector
    /// for the force enacted
    /// </summary>
    /// <param name="fleePosition"></param>
    /// <returns></returns>
    public Vector3 Flee(Vector3 fleePosition)
    {
        //Step 1:
        //get a vector from the thing you're fleeing from to you
        Vector3 desiredVelocity = vehiclePosition - fleePosition;

        //Step 2: Scale to max speed so you run away at M A X S P E E D
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        //step3: calculate fleeing steering force?
        Vector3 fleeingForce = desiredVelocity - velocity;

        //keep the object from flying up
        transform.position = new Vector3(transform.position.x, .5f, transform.position.z);

        //step 4: return force
        return fleeingForce;
    }

    /// <summary>
    /// alternate version of the previous method that only takes in target gameobject, 
    /// then passes that gameobject's position in to run away
    /// </summary>
    /// <param name="monster"></param>
    /// <returns></returns>
    public Vector3 Flee(GameObject monster)
    {
        //keep the object from flying up
        transform.position = new Vector3(transform.position.x, .5f, transform.position.z);
        return Flee(monster.transform.position);
    }

    /// <summary>
    /// method to Evade rather than flee
    /// takes in a vector3 for the position to evade, and the vector3 for 
    /// the velocity of the object to evade
    /// </summary>
    /// <returns></returns>
    public Vector3 Evade(Vector3 evadePosition, Vector3 evadeVelocity)
    {
        //keep the object from flying up
        transform.position = new Vector3(transform.position.x, .5f, transform.position.z);
        //make a vector out of the position to evade and the velocity
        Vector3 evasionVector = evadePosition + evadeVelocity;
        //return that vector scaled by 2
        return Flee(evasionVector) * 2f;
    }

    /// <summary>
    /// overloaded method to evade a specific monster, taking in the gameobject
    /// and return a vector to evade it
    /// </summary>
    /// <param name="monster"></param>
    /// <returns></returns>
    public Vector3 Evade(GameObject monster)
    {
        return Evade(monster.transform.position, monster.GetComponent<Vehicle>().velocity);
    }
    /// <summary>
    /// method to pursue rather than seek, once again
    /// taking the position of the thing to pursue, and the
    /// velocity of the thing to pursue
    /// </summary>
    /// <param name="pursuePosition"></param>
    /// <param name="pursueVelocity"></param>
    /// <returns></returns>
    public Vector3 Pursue(Vector3 pursuePosition, Vector3 pursueVelocity)
    {
        //keep the object from flying up
        transform.position = new Vector3(transform.position.x, .5f, transform.position.z);
        //make a vector to pursue out of the things passed in
        Vector3 pursueVector = pursuePosition + pursueVelocity;
        return Seek(pursueVector) * 2f;
    }
   /// <summary>
   /// overloaded method to pursue using a gameobject
   /// and returning a vector
   /// </summary>
   /// <param name="human"></param>
   /// <returns></returns>
    public Vector3 Pursue(GameObject human)
    {
        return Pursue(human.transform.position, human.GetComponent<Vehicle>().velocity);
    }

    /// <summary>
    /// abstract method to calculate the total steering forces
    /// </summary>
    public abstract void CalcSteeringForces();

    /// <summary>
    /// abstract method to detect if there's an edge
    /// </summary>
    public abstract bool DetectEdge();

    /// <summary>
    /// abstract method to FORCE objects to go back onto the terrain if they hit the edge of the map
    /// </summary>
    public abstract Vector3 AvoidEdge();

    /// <summary>
    /// make the humans and zombies wander around
    /// </summary>
    public void Wander()
    {
        Vector3 circleCenter = transform.position + velocity * 3; //projects a circle 3 seconds ahead
        float circleRadius = 2; //arbitary radius
        Vector3 target; 
        
        //adds 5 degrees in either direction to the angle
        angle = angle + Random.Range(-5, 5);
        //get a target x and z using the angle and trig
        float targetX = Mathf.Cos(angle) * circleRadius + circleCenter.x;
        float targetZ = Mathf.Sin(angle) * circleRadius + circleCenter.z;

        //set the target to a new vector3
        target = new Vector3(targetX, 0, targetZ);
        //Debug.DrawLine(transform.position, target, Color.yellow);
        //keep the object from flying up
        transform.position = new Vector3(transform.position.x, .5f, transform.position.z);
        ultimateForce += Seek(target);
    }

    /// <summary>
    /// separation from other objects
    /// takes in a list of gameObjects to separate from
    /// then separates from them, returning a vector3
    /// </summary>
    /// <param name="Fellow"></param>
    /// <returns></returns>
    public Vector3 Separate(List<GameObject> Fellow)
    {
        //iterate through the list
        for (int i = 0; i < Fellow.Count; i++)
        {
            //get the proximity from you to the gameobject
            Vector3 proximity = Fellow[i].transform.position - transform.position;
            float proxMag = proximity.magnitude;
            if (proxMag < 0.5)
            {
                //if the proximity is less than .5, make it .5 so it isn't too small
                proxMag = 0.5f;
            }
            //if the proximity is less than your safe distance
            if (proxMag < safeDistance)
            {
                //keep the object from flying up
                transform.position = new Vector3(transform.position.x, .5f, transform.position.z);
                //return a scaled ultimate force fleeing your fellow members 
                return ultimateForce += Flee(Fellow[i]) * 1 / proxMag;
            }
        }
        //return ultimateForce;
        return new Vector3();
    }



    /// <summary>
    /// Obstacle Avoidance method
    ///Returns a steering force against one obstacle
    /// </summary>
    /// <param name="obstacle"></param>
    /// <returns></returns>
    protected Vector3 ObstacleAvoidance(GameObject obstacle)
    {
        // Info needed for obstacle avoidance
        Vector3 vecToCenter = obstacle.transform.position - vehiclePosition;
        float dotForward = Vector3.Dot(vecToCenter, transform.forward);
        float dotRight = Vector3.Dot(vecToCenter, transform.right);
        float radiiSum = obstacle.GetComponent<Obstacle>().radius + radius;

        // Step 1: Are there objects in front of me?  
        // If obstacle is behind, ignore, no need to steer - exit method
        // Compare dot forward < 0
        if (dotForward < 0)
        {
            return Vector3.zero;
        }

        // Step 2: Are the obstacles close enough to me?  
        // Do they fit within my "safe" distance
        // If the distance > safe, exit method
        if (vecToCenter.magnitude > safeDistance)
        {
            return Vector3.zero;
        }

        // Step 3:  Check radii sum against distance on one axis
        // Check dot right, 
        // If dot right is > radii sum, exit method
        if (radiiSum < Mathf.Abs(dotRight))
        {
            return Vector3.zero;
        }

        // NOW WE HAVE TO STEER!  
        // The only way to get to this code is if the obstacle is in my path
        // Determine if obstacle is to my left or right
        // Desired velocity in opposite direction * max speed
        Vector3 desiredVelocity;

        if (dotRight < 0)        // Left
        {
            desiredVelocity = transform.right * maxSpeed;
        }
        else                    // Right
        {
            desiredVelocity = -transform.right * maxSpeed;
        }

        // Debug line to obstacle
        // Helpful to see which obstacle(s) a vehicle is attempting to maneuver around
        Debug.DrawLine(transform.position, obstacle.transform.position, Color.green);

        // Return steering force
        Vector3 steeringForce = desiredVelocity - velocity;
        return steeringForce;
    }







}
