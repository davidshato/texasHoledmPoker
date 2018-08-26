//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using MySql.Data;
//using MySql.Data.MySqlClient;
//using System.Linq;
//using System;
//using UnityEngine.SceneManagement;
//using System.Data;

//public class query : MonoBehaviour {


//	public InputField username;
//	public InputField password;
//	public InputField email;
//	public InputField firstname;
//	public InputField lastname;
//	public InputField confirmpass;
//	public Text userMSG;
//	public Text passMSG;
//	public Text emailMSG;
//	string userQUERY="";
//	string emailQuery="";
//	string DataToInsert ="";


//	public void onclick()
//	{
		
//		userQUERY= "select * from players where username=" + "'" + username.text + "'";
//		emailQuery="select * from players where email="+"'"+email.text+"'";

//		if (user_exists (userQUERY)==1 && email_exists (emailQuery)==1)
//		{
//			DataToInsert = "INSERT INTO players (firstname,lastname,email,username,password,confirm) VALUES('"+firstname.text+"','"+lastname.text+"','"+email.text+"','"+username.text+"','"+password.text+"','"+confirmpass.text+"')";
//				if(insert_query(DataToInsert)==1)
//					SceneManager.LoadScene("LOGIN");
//		}



			
//	}

//	public int user_exists(string query)
//	{
//		string quer=query;
//		if (quer == "") 
//		{ 
//			Debug.Log ("query is empty");
//			return 0;
//		}
			
//		String MySQLConnectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=th";
//		MySqlConnection DatabaseConnection = new MySqlConnection (MySQLConnectionString);
//		MySqlCommand  commandDatabase = new MySqlCommand (quer,DatabaseConnection);
//		commandDatabase.CommandTimeout = 60;

//		try
//		{
//			DatabaseConnection.Open();
//			MySqlDataReader myReader=commandDatabase.ExecuteReader();
//			if(myReader.HasRows)
//			{
//				userMSG.text=username.text+" exists";
//				return 0;
//			}
		


//		}
//		catch(Exception e)
//		{
//			Debug.Log("Query error: "+e.Message);
//			return 0;
//		}

//		return 1;
//	}

//	public int email_exists(string query)
//	{


//		string quer=query;
//		if (quer == "") 
//		{ 
//			Debug.Log ("query is empty");
//			return 0;
//		}

//		String MySQLConnectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=th";
//		MySqlConnection DatabaseConnection = new MySqlConnection (MySQLConnectionString);
//		MySqlCommand  commandDatabase = new MySqlCommand (quer,DatabaseConnection);
//		commandDatabase.CommandTimeout = 60;

//		try
//		{
//			DatabaseConnection.Open();
//			MySqlDataReader myReader=commandDatabase.ExecuteReader();
//			if(myReader.HasRows)
//			{
//				emailMSG.text=emailMSG.text+" exists";
//				return 0;
//			}



//		}
//		catch(Exception e)
//		{
//			Debug.Log("Query error: "+e.Message);
//			return 0;
//		}
//		return 1;	
//	}



//	public int insert_query(string query)
//	{
//		string quer = query;
//		if (quer == "") 
//		{
//			Debug.Log ("query is empty");
//			return 0;
//		}


//		String MySQLConnectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=th";
//		MySqlConnection DatabaseConnection = new MySqlConnection (MySQLConnectionString);
//		MySqlCommand  commandDatabase = new MySqlCommand (quer,DatabaseConnection);
//		commandDatabase.CommandTimeout = 60;

//		try
//		{
//			DatabaseConnection.Open();
//			MySqlDataReader myReader=commandDatabase.ExecuteReader();
		
//		}
//		catch(Exception e)
//		{
//			Debug.Log("Query error: "+e.Message);
//			return 0;
//		}

//		return 1;

			
//	}





//}
