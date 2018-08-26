using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Data;



public class DataBase 
{


    //the main data Base class make the connection with the data base 
	private const string database = "th";
    private const string datasource = "127.0.0.1";//"172.16.5.141";
    private const string myusername = "root";// "connect";
	private const string port = "3306";
    private const string pass = "";//"1234";
	protected MySqlConnection _conn=null;

	public DataBase()
	{
		String ConnectionString = "datasource=" + datasource + ";port=" + port + ";username=" + myusername + ";password="+pass+";database=" + database;
		_conn = new MySqlConnection(ConnectionString);
	}

	protected void connect()
	{
		if (_conn.State != ConnectionState.Open)
		{
			_conn.Open();
		}
	}//open connection to the data base

	protected void Disconnect()
	{
		_conn.Close();
	}//close the connection with the data base

	protected void ExecuteSimpleQuery(MySqlCommand cmd)
	{
		lock (_conn)
		{
			connect();
			cmd.Connection = _conn;
			try
			{
				cmd.ExecuteNonQuery();
			}
			finally {
				Disconnect();
			}

		}
	}//execute the insert update and delete function 

	protected int ExecuteScalarIntQuary(MySqlCommand cmd)
	{
		int ret = -1;
		lock (_conn)
		{
			connect();
			cmd.Connection = _conn;
			try
			{
				ret = int.Parse(cmd.ExecuteScalar().ToString());

			}
			finally {

				Disconnect();
			}
		}
		return ret;
	}//execute the scalar query

	protected DataSet GetMultipleQuary(MySqlCommand cmd)
	{
        DataSet dataset = new DataSet();
		lock (_conn)
		{
			connect();
			cmd.Connection = _conn;
			try
			{
				MySqlDataAdapter adapter = new MySqlDataAdapter();
				adapter.SelectCommand = cmd;
				adapter.Fill(dataset);
			}
			finally {
				Disconnect();
			}

		}
		return dataset;
    }//function for the select queries
}




