using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logout : MonoBehaviour {

    private DbPlayer db;
    public void logout()
    {
        
        db.deleteUserConnection(PlayerPrefs.GetString("userName"));
        SceneManager.LoadScene("LoginPage");
    }
}
