using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


//this class controls the button host and start game
public class NetworkPanel : MonoBehaviour {

    public NetworkManager manager;
    // Use this for initialization
    void Start () {
        manager = GetComponent<NetworkManager>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public void StartHost()
    {
        
        if (!NetworkServer.active&&!NetworkClient.active)
        {
            manager.StartHost();
        }
      
    }

    public void StartClient()
    {
        
          manager.StartClient();
    }


}
