using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Youki Iimori
/// A class which loads the Zombie Scene (IE the game scene)
/// </summary>
public class LoadScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SceneManager.LoadScene("zombieScene");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
