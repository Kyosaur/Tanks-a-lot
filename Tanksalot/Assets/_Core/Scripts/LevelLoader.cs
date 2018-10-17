using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ConnectClient()
    {
        //SceneManager.LoadScene("Forest");

           
        
    }

    public void SinglePlayer()
    {

    }

    public void MultiplayerJoin()
    {

    }

    public void MultiplayerStart()
    {

    }

    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}
