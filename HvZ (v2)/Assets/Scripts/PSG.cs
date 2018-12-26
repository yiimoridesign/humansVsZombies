using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PSG : MonoBehaviour
{

    private GameObject terrain;
    private float terrainSizeX;
    private float terrainSizeZ;
    // Use this for initialization
    public List<GameObject> human;
    void Start()
    {
        //find the scene manager and get the human list
        human = GameObject.Find("Scene Manager").GetComponent<SceneManager2>().humanList;
        //find the terrain adn its dimensions
        terrain = GameObject.Find("Terrain");
        terrainSizeX = terrain.GetComponent<Renderer>().bounds.size.x;
        terrainSizeZ = terrain.GetComponent<Renderer>().bounds.size.z;

    }

    // Update is called once per frame
    void Update()
    {
        if (human.Count != 0)
        {
            //loop through human list
            for (int i = 0; i < human.Count; i++)
            {
                //make a vector + its magnitude from the position of the psg and human positions
                Vector3 distance = transform.position - human[i].transform.position;
                float distanceMag = distance.magnitude;
                //if the human is withing two units, the PSG will transport itself to a new random place on the terrain. 
                if (distanceMag < 2)
                {
                    gameObject.transform.position = new Vector3(Random.Range(-terrainSizeX / 2, terrainSizeX / 2), 1, Random.Range(-terrainSizeZ / 2, terrainSizeZ / 2));
                }
            }
        }
    }
}

