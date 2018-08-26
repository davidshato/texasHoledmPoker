using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour {

    DbPlayer db;
    public InputField userName;
    public InputField Password;
    public Text Errors;

    public void doQuit()//close the application
    {
        #if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
        #else
             Application.Quit();
        #endif

    }


    public void LoginClick()//function that check the user name and the password after a user tries to Login
    {
        db = new DbPlayer();


        if (db.ExistUser(userName.text))
        {
            if (!db.isUserConnected(userName.text))
            {
                PlayerData info = db.GetPlayerByUserName(userName.text);

                if (info.Password == Password.text)
                {
                    PlayerPrefs.SetString("userName", info.UserName);
                    PlayerPrefs.SetInt("ImageOffset",info.PlayerImage);
                    PlayerPrefs.Save();
                    db.addUserConnection(info.UserName);

                    SceneManager.LoadScene("lobbytest");



                }
                else
                {
                    Errors.text = "Worng Password";

                }
            }
            else
            {
                Errors.text = "This User Allready Conntected";
            }

        }
        else
        {
            Errors.text = "User Dont Exist In The Data Base";
        }





    }

    public void OnRegisterClick()//change the scene to Register page
    {

        SceneManager.LoadScene("Register");

    }

}
