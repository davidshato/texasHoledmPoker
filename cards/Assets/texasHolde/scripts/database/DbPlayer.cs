using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using MySql.Data.MySqlClient;
using System;


//class that work with the players table in the data  derived from DataBase class
public class DbPlayer : DataBase {


    public DbPlayer() : base()
    { }

    public void InsertPlayer(PlayerData player)//insert users data in to the data base
    {
        string query = "INSERT INTO players(firstname,lastname,email,username,password,money,date,status,playerImage)";
        query += "VALUES(@fname,@lname,@email,@name,@password,@money,@date,@status,@profileImage)";

        using (MySqlCommand cmd = new MySqlCommand(query))
        {
            cmd.Parameters.AddWithValue("@fname", player.FirstName);
            cmd.Parameters.AddWithValue("@lname", player.LastName);
            cmd.Parameters.AddWithValue("@email", player.Email);
            cmd.Parameters.AddWithValue("@name", player.UserName);
            cmd.Parameters.AddWithValue("@password", player.Password);
            cmd.Parameters.AddWithValue("@money", player.Money);
            cmd.Parameters.AddWithValue("@date", player.Date);
            cmd.Parameters.AddWithValue("@status", player.Status);
            cmd.Parameters.AddWithValue("@profileImage", player.PlayerImage);

            base.ExecuteSimpleQuery(cmd);
        }

    }//insert the player information in to the data base 

    public void UpdatePlayer(PlayerData player)//update users data in to the data base
    {
        string query = "UPDATE players SET firstname=@fname,lastname=@lname,email=@email,username=@name,password=@password,money=@money,status=@status,date=@date WHERE username=@username";


        using (MySqlCommand cmd = new MySqlCommand(query))
        {
            cmd.Parameters.AddWithValue("@fname", player.FirstName);
            cmd.Parameters.AddWithValue("@lname", player.LastName);
            cmd.Parameters.AddWithValue("@email", player.Email);
            cmd.Parameters.AddWithValue("@name", player.UserName);
            cmd.Parameters.AddWithValue("@password", player.Password);
            cmd.Parameters.AddWithValue("@money", player.Money);
            cmd.Parameters.AddWithValue("@date", DateTime.Today.ToString());
            cmd.Parameters.AddWithValue("@status", player.Status);
            cmd.Parameters.AddWithValue("@username", player.UserName);
            base.ExecuteSimpleQuery(cmd);
        }

    }

    public int getPlayerNumber()//return the amount of users in the data base
    {
        int result;
        string cmdstr = "Select Count(*) From players";

        using (MySqlCommand cmd = new MySqlCommand(cmdstr))
        {
            result = ExecuteScalarIntQuary(cmd);
        }

        return result;

    }

    public PlayerData GetPlayerByUserName(string UserName)//return a player class from data base if player==null player is no in data base
    {
        DataSet ds = new DataSet();
        string query = "SELECT * FROM players WHERE username=@username";
        

        using (MySqlCommand cmd = new MySqlCommand(query))
        {
            cmd.Parameters.AddWithValue("@username", UserName);
            ds = GetMultipleQuary(cmd);
        }
        DataTable dt = new DataTable();
        try
        {
            dt = ds.Tables[0];

        }
        catch { }

            
        if (dt.Rows.Count!= 0)
        {
            PlayerData player = new PlayerData();
            DataRow dr = dt.Rows[0];
            player.SetPlayer(dr["firstname"].ToString(), dr["lastname"].ToString(), dr["email"].ToString(), dr["username"].ToString(), dr["password"].ToString(), double.Parse(dr["money"].ToString()), dr["status"].ToString(),dr["date"].ToString(),int.Parse(dr["playerImage"].ToString()));
            return player;
        }



        return null;
    }


    public bool ExistUser(string UserName)//return a player class from data base if player==null player is no in data base
    {
        DataSet ds = new DataSet();
        string query = "SELECT * FROM players WHERE username=@username";


        using (MySqlCommand cmd = new MySqlCommand(query))
        {
            cmd.Parameters.AddWithValue("@username", UserName);
            ds = GetMultipleQuary(cmd);
        }
        DataTable dt = new DataTable();
        try
        {
            dt = ds.Tables[0];

        }
        catch { }


        if (dt.Rows.Count != 0)
            return true;
        return false;
            
        



       
    }

    public bool isUserConnected(string UserName)//return a player class from data base if player==null player is no in data base
    {
        DataSet ds = new DataSet();
        string query = "SELECT * FROM conection WHERE userName=@username";


        using (MySqlCommand cmd = new MySqlCommand(query))
        {
            cmd.Parameters.AddWithValue("@username", UserName);
            ds = GetMultipleQuary(cmd);
        }
        DataTable dt = new DataTable();
        try
        {
            dt = ds.Tables[0];

        }
        catch { }


        if (dt.Rows.Count != 0)
            return true;
        return false;






    }


    public void addUserConnection(String userName)//adding the player information to the connection table in the data base 
    {

        string query = "INSERT INTO conection(userName,Date)";
        query += "VALUES(@username,@date)";

        using (MySqlCommand cmd = new MySqlCommand(query))
        {
            cmd.Parameters.AddWithValue("@username", userName);
            cmd.Parameters.AddWithValue("@date", DateTime.Today);
         

            base.ExecuteSimpleQuery(cmd);
        }


    }

    public void deleteUserConnection(String userName)//delete player info from the connection table
    {
        string query = "DELETE FROM `conection` WHERE userName=@username";
      

        using (MySqlCommand cmd = new MySqlCommand(query))
        {
            cmd.Parameters.AddWithValue("@username", userName);
            base.ExecuteSimpleQuery(cmd);
        }

    }


}
