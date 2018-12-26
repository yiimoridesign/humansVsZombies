using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Youki Iimori
/// A basic class with a function limited to
/// loading "startscene"
/// </summary>
public class LoadHome : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SceneManager.LoadScene("startScene");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
