using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



//looby scene is a function that assigen a player to a room and assign a player to a sit
public class LobbyScene : NetworkManager
{
    const int maxPlayers = 4;
    texasPlayer[] playerSlots = new texasPlayer[4];
    DbPlayer db = new DbPlayer();
    public playerinfo info;



    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        // find empty player slot
        for (int slot = 0; slot < maxPlayers ; slot++)
        {
            if (playerSlots[slot] == null)
            {
                var playerObj = (GameObject)GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                var player = playerObj.GetComponent<texasPlayer>();

                player.playerId = slot;
                playerSlots[slot] = player;

                Debug.Log("Adding player in slot " + slot);
                NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
                return;
            }
        }

        //TODO: graceful  disconnect
        conn.Disconnect();
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController playerController)
    {
        // remove players from slots
        var player = playerController.gameObject.GetComponent<texasPlayer>();
        playerSlots[player.playerId] = null;
       RoomManager.singleton.RemovePlayer(player);

        base.OnServerRemovePlayer(conn, playerController);
    }//a function that remove a player from the room

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        foreach (var playerController in conn.playerControllers)
        {

            var player = playerController.gameObject.GetComponent<texasPlayer>();
            playerSlots[player.playerId] = null;
            PlayerData playerData = db.GetPlayerByUserName(player.name);
            playerData.Money = player.money;
            db.UpdatePlayer(playerData);
            RoomManager.singleton.RemovePlayer(player);
        }

        base.OnServerDisconnect(conn);
    }//a function for the case that server disconnect

    public override void OnStartClient(NetworkClient client)
    {
        client.RegisterHandler(CardConstants.CardMsgId, OnCardMsg);
    }//assigen the msg type and the delegte function for the msg pass throw the network

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        ClientScene.AddPlayer(conn, 0);
    }

    void OnCardMsg(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<CardConstants.CardMessage>();

        var other = ClientScene.FindLocalObject(msg.playerId);
        var player = other.GetComponent<texasPlayer>();
        player.MsgAddCard(msg.cardId);
    }//the function that pass the message throw the clients
}
