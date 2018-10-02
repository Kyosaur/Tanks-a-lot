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

        NetworkManager.singleton.networkAddress = "127.0.0.1";
        NetworkManager.singleton.networkPort = 7777;

        NetworkManager.singleton.StartClient();
           
        
    }

    public void SinglePlayer()
    {
        if (!NetworkClient.active && !NetworkServer.active && NetworkManager.singleton.matchMaker == null)
        {
            //SceneManager.LoadScene("Forest");
            //NetworkManager.singleton.ServerChangeScene("Forest");

            NetworkManager.singleton.networkAddress = "127.0.0.1";
            NetworkManager.singleton.networkPort = 7777;

            NetworkManager.singleton.StartHost();
   
        }
    }

    public void MultiplayerJoin()
    {

    }

    public void MultiplayerStart()
    {
        if (NetworkManager.singleton.matchMaker == null) NetworkManager.singleton.StartMatchMaker();

        NetworkManager.singleton.matchMaker.CreateMatch("Test match", 10, true, "", "", "", 0, 0, NetworkManager.singleton.OnMatchCreate);
    }

    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}
