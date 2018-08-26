using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//adapter for the info of the player 
public class PlayerData :MonoBehaviour
{
    private string firstName;
    private string lastName;
    private string email;
    private string userName;
    private string password;
    private double money;
    private string status;
    private string date;
    private int playerImage;

    void Start()
    {
        firstName = "";
        lastName = "";
        email = "";
        userName = "";
        password = "";
        money = 0;
        status = "";
        date = "";



    }

    //register
    public void SetPlayer(string firstName, string lastName, string email, string userName, string password,int playerImage)
    {
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this.userName = userName;
        this.password = password;
        this.money =1000;
        this.playerImage = playerImage;
        this.status = "A";
        this.date = DateTime.Today.ToString();
        
    }

    //select
    public void SetPlayer(string firstName, string lastName, string email, string userName, string password, double money, string status, string date,int profileImage) 
    {
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this.userName = userName;
        this.password = password;
        this.money = money;
        this.status = status;
        this.date = date;
        this.playerImage = profileImage;
    }

 

    public string FirstName
    {
        get
        {
            return firstName;
        }

        set
        {
            firstName = value;
        }
    }

    public string LastName
    {
        get
        {
            return lastName;
        }

        set
        {
            lastName = value;
        }
    }

    public string Email
    {
        get
        {
            return email;
        }

        set
        {
            email = value;
        }
    }

    public string UserName
    {
        get
        {
            return userName;
        }

        set
        {
            userName = value;
        }
    }

    public string Password
    {
        get
        {
            return password;
        }

        set
        {
            password = value;
        }
    }

    public double Money
    {
        get
        {
            return money;
        }

        set
        {
            money = value;
        }
    }

    public string Status
    {
        get
        {
            return status;
        }

        set
        {
            status = value;
        }
    }

    public string Date
    {
        get
        {
            return date;
        }

        set
        {
            date = value;
        }
    }

    public int PlayerImage
    {
        get
        {
            return playerImage;
        }

        set
        {
            playerImage = value;
        }
    }
}
