using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


//the info panel in the lobby scene hold the player information
public class playerinfo : MonoBehaviour {

    public Sprite[] avatars;
    public Sprite[] Status;

  
    public TextMeshProUGUI userName;
    public TextMeshProUGUI chipsAmount;
    public Image status;


    public Image playersImage;
    private DbPlayer Data;
    public PlayerData info;



	// Use this for initialization
	void Start () {

        Data = new DbPlayer();
        info = Data.GetPlayerByUserName(PlayerPrefs.GetString("userName"));
        
        userName.text = info.UserName;
        chipsAmount.text = info.Money.ToString();
       
       if(info.Status=="A")
        status.sprite = Status[0];

        if (info.Status == "B")
            status.sprite = Status[1];

        if (info.Status == "C")
            status.sprite = Status[2];

        if (info.Status == "D")
            status.sprite = Status[3];

        if (info.Status == "E")
            status.sprite = Status[4];


        playersImage.sprite =avatars[info.PlayerImage];

    }//on start of the object fill in all of the player data
	
	// Update is called once per frame
	void Update () {
		
	}

    public void logOut()//log the player out of the game 
    {
        
        Data.deleteUserConnection(info.UserName);
        SceneManager.LoadScene("LoginPage");



    }
}
