using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;


//the player class is responsible for the player action is spacial class that is a network Behaviour 
//includ spcial unity function that send command to the game server 
//and hook function 
//rpc function 
//command function 
//
public class texasPlayer : NetworkBehaviour
{
    //enum for the player state in the game
    public enum PlayerState
    {
        none,
        fold,
        check,
        allIn

    }


    public PlayerPanel playerPanel;
    [SyncVar]
    public string userName;

    public PlayerState playerState;
    public CardConstants.HandRank PlayerHandRank;
    public int totalHandValue;

    [SyncVar(hook = "OnMoney")]
    public int money = 1000;

    [SyncVar(hook = "" +
        "OnBet")]
    public int currentBet = 0;
    public int tmpBet = 0;

    [SyncVar]
    public bool bettingOnCurrentHand = false;

    [SyncVar]
    public int cardScore;

    [SyncVar]
    public bool isDealer;

    [SyncVar]
    public int playerId;

    public List<CardId> handCards = new List<CardId>();


    public override void OnStartLocalPlayer()
    {
        RoomManager.singleton.localPlayer = this;
        SetPlayerName();
    }//on the start of the local player set the player info

    [Client]
    void SetPlayerName()
    {
        userName = PlayerPrefs.GetString("userName");
        CmdSendNameToServer(userName);
    }//set the player info on the client side

    [Command]
    void CmdSendNameToServer(string nameToSend)
    {
        RpcSetPlayerName(nameToSend);
    }//send the server a command with the player info

    [ClientRpc]
    void RpcSetPlayerName(string name)
    {
        playerPanel.userNameText.text = name;
    }//borde cast the player info to all the client in the game

    public override void OnStartClient()
    {
        RoomManager.singleton.moneyAmount.text = money.ToString();
        RoomManager.singleton.betAmount.text = currentBet.ToString();

        playerPanel = RoomManager.singleton.playerPanels[playerId];
        playerPanel.gameObject.SetActive(true);
        playerPanel.userNameText.text = userName;
    }//when the client start set his money and assigen him a seat at the table

    // hook function for money when there will be any change with the money this function will be invoke
    void OnMoney(int amount)
    {
        money = amount;
        setMoneyAmount(amount);
      //  RoomManager.singleton.moneyAmount.text = amount.ToString();
       
    }

    // hook function for currentBet when there will be any change with the currentBet this function will be invoke
    void OnBet(int amount)
    {
        currentBet = amount;
        RoomManager.singleton.betAmount.text = amount.ToString();
        playerPanel.BetText.text = amount.ToString();
    }

    public override void OnNetworkDestroy()
    {
        if (playerPanel != null)
        {
            playerPanel.ClearCards();
            playerPanel.gameObject.SetActive(false);
        }
    }//function for when the networke is destroy

    public override void OnStartServer()
    {
        RoomManager.singleton.AddPlayer(this);

    }//when the server start adding the player to the room


    [Server]
    public void ServerAddCard(CardId newCard)
    {
        RpcAddCard(newCard);
    }//work on the server and broade cast the card for all the client

    [ClientRpc]//from server to client 
    void RpcAddCard(CardId newCard)//from server to clients 
    {

        if (isLocalPlayer)
        {

            // this was already done for host player
            handCards.Add(newCard);
        }
        else
        {
            newCard.hidden = true;
            handCards.Add(newCard);
        }

        playerPanel.AddCard(newCard, cardScore);
    }//adding the card to the clients if the player is local host the card will be visable else hidden


    public void MsgAddCard(CardId cardId)
    {
        handCards.Add(cardId);
        CalculateScore();
        playerPanel.AddCard(cardId, cardScore);
    }//the the adding of the card to the player hand and to panel
    

    [Server]
    public void ServerClearCards()
    {
        handCards.Clear();
        cardScore = 0;
        playerState = PlayerState.none;

        RpcClearCards();
    }//clear the player hand in the server

    [ClientRpc]
    private void RpcClearCards()
    {
        if (!isServer)
        {
            // this was already done for host player
            handCards.Clear();
            playerState = PlayerState.none;
            RoomManager.singleton.communityCard.ClearCards();
            RoomManager.singleton.cardsInTable.Clear();
        }
        playerPanel.ClearCards();
        cardScore = 0;
        RoomManager.singleton.paidAmount.text = "0";
    }//brodcast and clear the card at the clients

    [Client]
    public void ShowCards()
    {
        var card = handCards[0];
        card.hidden = false;
        handCards[0] = card;

        playerPanel.ShowCard(0);


    }//client side show the cards

    [Server]
    public void ServerClearBet()
    {
        currentBet = 0;
        tmpBet = 0;
        bettingOnCurrentHand = false;
    }//clear the bet at the server


    void CalculateScore()//the hand Rank function 
    {
        int newScore = 0;
        foreach (var cardId in handCards)
        {
            if (cardId.hidden)
                continue;

            newScore += RoomManager.singleton.GetScore(cardId);
        }

        if (newScore > 21)
        {
            newScore = 0;
            foreach (var cardId in handCards)
            {
                if (cardId.hidden)
                    continue;

                newScore += RoomManager.singleton.GetScoreLowAce(cardId); ;
            }
        }
        cardScore = newScore;
    }

    [Server]
    public void ServerPayout(int amount)//collect the money 
    {

        Debug.Log("Payout " + this + " amount:" + amount);
        money += amount;
        RpcWin(amount);
    }

    [Server]
    public void ServerLose(int amount)
    {
        // money was already subtracted
        RpcLose(amount);
    }//broadcast the losers client that they lost


    [ClientRpc]
    void RpcWin(int amount)//win Message 
    {
        
        if (isLocalPlayer)
        {
            RoomManager.singleton.winSound.Play();
            DisplayMoneyAmount(amount); 
            
        }
    }


    [Client]
    public void DisplayMoneyAmount(int amount)
    {
        var winMsg = "You Won " + amount;
        Debug.Log(winMsg);
        RoomManager.singleton.infoText.text = winMsg;
        RoomManager.singleton.paidAmount.text = amount.ToString();


    }//display the amount of winning money th the client and the winning m

    [Client]
    public void setMoneyAmount(int amount)
    {

        RoomManager.singleton.moneyAmount.text = amount.ToString();

    }

    [ClientRpc]
    void RpcLose(int amount)//lose Message
    {
        if (isLocalPlayer)
        {
            var lostMsg = "You Lost " + amount;
            Debug.Log(lostMsg);
            RoomManager.singleton.infoText.text = lostMsg;
            setMoneyAmount(money);
            //RoomManager.singleton.moneyAmount.text = money.ToString();
            RoomManager.singleton.paidAmount.text = (-amount).ToString();
        }
    }

    [ClientRpc]
    public void RpcYourTurn(bool isYourTurn)//control the turn and the buttons
    {
       
        // make player who is having current turn green
        Color c = new Color(1, 1, 1, 0.5f);
        if (isYourTurn)
            c = Color.green;

        playerPanel.GetComponent<PlayerPanel>().ColorImage(c);
        
        if (isYourTurn && isLocalPlayer)
        {
            
            RoomManager.singleton.EnablePlayHandButtons();
        }
        else
        {
           
            RoomManager.singleton.ClientDisableAllButtons();
        }
        
    }


    ////////// Commands /////////
    //commends funcion from the client to the server 
    //to place the bet fold etc.


    [Command]
    public void CmdPlaceBet()
    {
        //if (RoomManager.singleton.turnState != CardConstants.GameTurnState.WaitingForBets)
        //{
        //    Debug.LogError("cannot place bet now");
        //    return;
        //}

        Debug.Log("CmdPlaceBet");
        RoomManager.singleton.MainPot += currentBet;
        tmpBet += currentBet;
        currentBet = 0;
        
        RoomManager.singleton.RpcsendPotInfo(RoomManager.singleton.MainPot);
        bettingOnCurrentHand = true;

        if (RoomManager.singleton.turnState == CardConstants.GameTurnState.WaitingForBets)
        {
            RoomManager.singleton.ServerCheckAllBets();
        }
        else
            RoomManager.singleton.ServerNextPlayer();

    }




    [Command]
    public void CmdRaiseBet()
    {
       

        Debug.Log("CmdRaiseBet");
        currentBet += 10;
        money -= 10;
    }

    [Command]
    public void CmdCall()//Call function
    {
        
      
        Debug.Log("cmdCall");

        int callAmount = RoomManager.singleton.maxTempBet - this.tmpBet;
        this.money -= callAmount;
        tmpBet += callAmount;
        RoomManager.singleton.MainPot += callAmount;
        
        if (RoomManager.singleton.turnState == CardConstants.GameTurnState.WaitingForBets)
            RoomManager.singleton.ServerCheckAllBets();


    }

    [Command]
    public void CmdAllIn()//AllIn
    {
        if (RoomManager.singleton.turnState != CardConstants.GameTurnState.PlayingPlayerHand)
        {
            Debug.LogError("cannot double down now");
            return;
        }
        if (RoomManager.singleton.currentTurnPlayer != this)
        {
            Debug.LogError("not your turn");
            return;
        }
        Debug.Log("CmdAllIn");

        currentBet = money;
        money = 0;
        RoomManager.singleton.MainPot += currentBet;
        tmpBet = currentBet;
        currentBet = 0;
        playerState = PlayerState.allIn;
        RoomManager.singleton.RpcsendPotInfo(RoomManager.singleton.MainPot);
        RoomManager.singleton.ServerNextPlayer();

        // TODO
    }

    [Command]
    public void CmdFold()//Fold
    {
      

        Debug.Log("CmdFold");

        this.playerState = PlayerState.fold;
        RoomManager.singleton.ServerNextPlayer();
        // TODO
    }

    [Command]
    public void CmdCheck()//this will be the Check function
    {
        if (RoomManager.singleton.turnState != CardConstants.GameTurnState.PlayingPlayerHand)
        {
            Debug.LogError("cannot stay now");
            return;
        }
        if (RoomManager.singleton.currentTurnPlayer != this)
        {
            Debug.LogError("not your turn");
            return;
        }

        Debug.Log("CmdChecked");

        playerState = PlayerState.check;
        //RoomManager.singleton.CheckedPlayers.Add(this);
        RoomManager.singleton.ServerNextPlayer();
    }

    [Command]
    public void CmdNextHand()//the Player restarting the Game
    {
        if (RoomManager.singleton.turnState != CardConstants.GameTurnState.Complete)
        {
            Debug.LogError("cannot start new hand now");
            return;
        }

        RoomManager.singleton.ServerNextHand();
    }

}
