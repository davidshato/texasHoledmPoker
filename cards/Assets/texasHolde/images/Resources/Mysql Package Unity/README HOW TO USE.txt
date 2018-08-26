This is how to use the class

1 - for connect you have the functions Connect, Connectv, Connectb
	connect = string return, return the state of connection.
	connectv = void only for connect.
	connectb = bool return true for connected and false for others.

2 - for Read data and show you have Read
	read = string return.
		return a value from a query, 
		like that: MysqlClass.Read("SELECT * FROM users","name"); that will return the first record for the row name.

3 - for query you have Execute
	Execute = void only for execute
	if executed prints in Debug.Log that was executed

4 - for test if the connection is open use the function is_Connected
	is_connected = bool return, return true if connection is Open and false if other state.

5 - for close the connection sucessfully use Close
	Close = void that close the connection.
	MysqlClass.Close(); // change the state to closed.

6 - for see the connection state use status
	status = string return the state of connection


INSTALATION

Hello, for install just copy the Plugins and the Scripts folder to your project.
Use like that in your scripts:

	MysqlClass.thefunction();

thanks.

CREDITS

Created by : Chinchila (joaovitim61@gmail.com)

in base of this package: http://forum.unity3d.com/threads/63364-Unity-with-MySQL

Thanxs and Enjoy (: