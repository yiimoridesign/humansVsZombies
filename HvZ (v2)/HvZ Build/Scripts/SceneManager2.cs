using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Youki Iimori
/// A class to manage anything done globally
/// IE, human lists, zombie lists, spawning, points, etc
/// </summary>
public class SceneManager2 : MonoBehaviour
{

    // Use this for initialization
    public GameObject human;
    public GameObject zombie;
    public GameObject hunter;
    public List<GameObject> zombieList;
    public List<GameObject> humanList;
    public List<GameObject> hunterList;
    public GameObject[] humanID;
    public bool debugLinesOn;
    public float points = 100;

    //terrain size stuff
    public GameObject terrain;
    private float terrainSizeX;
    private float terrainSizeZ;
    void Start()
    {
        //starting from .5 seconds, call PointUp every second
        InvokeRepeating("PointUp", .5f, 1f);
        debugLinesOn = false; //set debuglines to off by default

        //get the terrain renderer
        terrainSizeX = terrain.GetComponent<Renderer>().bounds.size.x / 2;
        terrainSizeZ = terrain.GetComponent<Renderer>().bounds.size.z / 2;

        //zombie instantiation at random location
        for (int i = 0; i < 3; i++)
        {
            zombieList.Add(Instantiate(zombie, new Vector3(Random.Range(-terrainSizeX, terrainSizeX), .5f, Random.Range(-terrainSizeZ, terrainSizeZ)), Quaternion.identity));
        }

        //human instantiation loop at random location
        for (int i = 0; i < 7; i++)
        {
            humanList.Add(Instantiate(human, new Vector3(Random.Range(-terrainSizeX, terrainSizeX), .5f, Random.Range(-terrainSizeZ, terrainSizeZ)), Quaternion.identity));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //on D keypress, change debug lines from on to off and vice versa
        if (Input.GetKeyDown(KeyCode.D))
        {
            debugLinesOn = !debugLinesOn;
        }
        Zombify(); //call zombify once per frame
        if (Input.GetKeyDown(KeyCode.H))
        {
            //as long as you have enough points and you click H, spawn a hunter
            if (points >= 75)
            {
                SpawnHunter();
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            //as long as you have enough points and you click s, you spawn a human
            if (points >= 10)
            {
                points -= 10;
                humanList.Add(Instantiate(human, new Vector3(Random.Range(-terrainSizeX, terrainSizeX), 1f, Random.Range(-terrainSizeZ, terrainSizeZ)), Quaternion.identity));
            }
        }
        if(Input.GetKeyDown(KeyCode.Z))
        {
            //you can add 25 points and spawn a human
            points += 25;
            zombieList.Add(Instantiate(zombie, new Vector3(Random.Range(-terrainSizeX, terrainSizeX), 1.5f, Random.Range(-terrainSizeZ, terrainSizeZ)), Quaternion.identity));
        }
        //cull calls the method for to check if the hunter's collided with a zombie
        Cull();

        //if the humanList goes to 0, go to game over
        if(humanList.Count == 0)
        {
            GameLoss();
        }
        //if the zombie list goes to 0, goes to victory
        else if (zombieList.Count == 0)
        {
            GameWin();
        }
    }

    /// <summary>
    /// method to increment points
    /// </summary>
    public void PointUp()
    {
        points += 1;
    }
    /// <summary>
    /// spawns a hunter at mouse position if you have enough points, then adds it to the hunterlist
    /// </summary>
    public void SpawnHunter()
    {

        points -= 75;
        hunterList.Add(Instantiate(hunter, new Vector3(0, .5f, 0), Quaternion.identity));

    }

    /// <summary>
    /// method to check for hunter/zombie collision, then destroy both if a collision happens
    /// </summary>
    public void Cull()
    {
          //loop through zombies and hunters, then check for collision
        if (hunterList.Count != 0) //do this only if humans exist
        {
            //loop through hunter's list
            for (int i = 0; i < hunterList.Count; i++)
            {
                //loop through zombie's list
                for (int j = 0; j < zombieList.Count; j++)
                {
                    //if the hunter list is empty, return. (this is to ensure there are no exceptions thrown)
                    if (hunterList.Count == 0)
                    {
                        return;
                    }
                    //vector to check the distance between the current zombie and current hunter
                    Vector3 checkVector = zombieList[j].transform.position - hunterList[i].transform.position;
                    float vectorMag = checkVector.magnitude;
                    //if the magnitude of the vector above is less than the radius of the colliders for the two objects beign compoared
                    if (vectorMag < (hunterList[i].GetComponent<CapsuleCollider>().radius + zombieList[j].GetComponent<CapsuleCollider>().radius))
                    {
                        //if there's a collision, remove the zombie and hunter
                        GameObject deadHunter = hunterList[i];
                        hunterList.Remove(deadHunter);
                        Destroy(deadHunter);
                        GameObject deadZombie = zombieList[j]; //make a temporary dead human to store the human that's about to die in
                        zombieList.Remove(deadZombie); //remove that human from the list so stuff doesn't explode
                        Destroy(deadZombie); //destroy the gameobject
                        points += 25;
                        return; //end
                    }
                }

            }
        }

    }

    /// <summary>
    /// method to check for collision and make zombies from humans on collision
    /// </summary>
    /// <returns></returns>
    public void Zombify()
    {
        //loop through zombies and humans, then check for collision
        if (humanList.Count != 0) //do this only if humans exist
        {
            //loop through human's list
            for (int i = 0; i < humanList.Count; i++)
            {
                //loop through zombie's list
                for (int j = 0; j < zombieList.Count; j++)
                {
                    //if the human list is empty, return. (this is to ensure there are no exceptions thrown)
                    if (humanList.Count == 0)
                    {
                        return;
                    }
                    //vector to check the distance between the current zombie and current human
                    Vector3 checkVector = zombieList[j].transform.position - humanList[i].transform.position;
                    float vectorMag = checkVector.magnitude;
                    //if the magnitude of the vector above is less than the radius of the colliders for the two objects beign compoared
                    if (vectorMag < (humanList[i].GetComponent<CapsuleCollider>().radius + zombieList[j].GetComponent<CapsuleCollider>().radius))
                    {
                        //if there's a collision, make a new zombie and remove the human
                        zombieList.Add(Instantiate(zombie, new Vector3(humanList[i].transform.position.x, zombieList[j].transform.position.y, humanList[i].transform.position.z), Quaternion.identity));
                        GameObject deadHuman = humanList[i]; //make a temporary dead human to store the human that's about to die in
                        humanList.Remove(deadHuman); //remove that human from the list so stuff doesn't explode
                        Destroy(deadHuman); //destroy the gameobject
                        points += 10;
                        return; //end
                    }
                }

            }
        }

    }

    /// <summary>
    /// displays points on a gui box
    /// </summary>
    void OnGUI()
    {
        GUI.color = Color.white; //change the color
        GUI.skin.box.fontSize = 20; //increase text size
        GUI.Box(new Rect(0f, 9f, 400, 150), "D: Toggle Debug Lines\nC: Change camera angle\nH: -75pts, spawn Hunter \nS: -10pts, spawn Human \nZ: +25pts, spawn Zombie \nPoints: " + points);
        GUI.Box(new Rect(0f, 160f, 400, 50), "Humans Remaining: " + humanList.Count + "\nZombies: " + zombieList.Count);
    }

    /// <summary>
    /// method to change scenes on loss or win
    /// </summary>
    public void GameLoss()
    {
            SceneManager.LoadScene("gameOver"); //load gameOver
    }

    public void GameWin()
    {
        SceneManager.LoadScene("victory"); //load victory scene
    }
}

