using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;



/*
 * Room manager Class is the main Game class 
 * this class responsible of the flow of the game and the main game scene GameRoom
 * this class is a NetworkBehaviour class it means that it includes function that control the server connection 
 * and the the trasformaion of the data to all the clients
 * it is a singelton that mean that only one instance of this class can be at the time
 * we implemented it this way to emphasize the important of this class and connection reasons
 * 
    * Game sequence:
    * StartDeck.
    * StartHand.
    *  - reset all player hands
    *  - reset all player bets
    * Betting phase:
    *  - Each player may bet an amount
    * Dealing phase:
    *  - Deal 2 cards to players
    *  Flop phase
    * Playing phase:
    *  - each player plays their hand
    * turn phase
    * Playing phase:
    *  - each player plays their hand
    * river phase
    * Show Down Phase:
    *   - compare hands of the players and pay winners
    *   - declaration of the winner and the loser 
    *   - split the money
    * go back to StartHand
    */
public class RoomManager : NetworkBehaviour//fix the flow of the Game that every player has the same amount of money invested
{
   public AudioSource buttonSound;
    public AudioSource shuffleSound;
    public AudioSource winSound;
    [SyncVar]
    public double MainPot;
    public TextMeshProUGUI MainPotAmount;
    public HandRanker HandAnalizer;
    private int playerCount = 0;

    public static RoomManager singleton;

    public Sprite[] cardSprites;
    public Deck deck;
    public PlayerPanel communityCard;
    public List<CardId> cardsInTable;
    public List<texasPlayer> players = new List<texasPlayer>();
    public texasPlayer currentTurnPlayer;
    public int currentPlayerIndex;
    public texasPlayer localPlayer;

    public Text moneyAmount;
    public Text betAmount;
    public Text paidAmount;
    public Text infoText;

    [SyncVar]
    public CardConstants.GameTurnState turnState;

    public PlayerPanel[] playerPanels = new PlayerPanel[5];
    public Button buttonStay;
    public Button buttonHitMe;
    public Button buttonDoubleDown;
    public Button buttonSplit;
    public Button buttonPlaceBet;
    public Button buttonRaiseBet;
    public bool everybodyChecked = true;
    public List<texasPlayer> CheckedPlayers = new List<texasPlayer>();
    private bool nextHandWaiting;

    public int maxTempBet = 0;
    void Start()
    {
        cardSprites = Resources.LoadAll<Sprite>("cards");//loding the card images

    }//unity constractor

    private void Awake()
    {
        singleton = this;
        cardSprites = Resources.LoadAll<Sprite>("cards");//loding the card images

    }//

    public override void OnStartClient()
    {
        cardSprites = Resources.LoadAll<Sprite>("cards");//loding the card images
        ClientHandleState(turnState, turnState.ToString(), MainPot);
    }//when client start he will get the broadCast of the game status and the main pot

    public Sprite GetCardSprite(CardConstants.Suite suite, CardConstants.Card card)
    {

        int suiteOffset = (int)suite * 14;
        int cardOffset = (int)card;
        int offset = suiteOffset + cardOffset;
        return cardSprites[offset];
    }//return the sprit of the card (image of the card) from the array of sprits 

    public Sprite GetCardSprite(CardId cardId)
    {
        return GetCardSprite(cardId.suite, cardId.card);
    }//getter for the card sprit

    public int GetScore(CardId cardId)
    {
        return GetScore(cardId.suite, cardId.card);
    }//return the card score

    public int GetScore(CardConstants.Suite suite, CardConstants.Card card)
    {
        return CardConstants.CardValues[(int)card];
    }//return the card score

    public int GetScoreLowAce(CardId cardId)
    {
        return GetScoreLowAce(cardId.suite, cardId.card);
    }

    public int GetScoreLowAce(CardConstants.Suite suite, CardConstants.Card card)
    {
        return CardConstants.CardValuesLowAce[(int)card];
    }

    public CardId GetRandomCard()
    {
        return deck.GetTopCard();
    }

    public void AddPlayer(texasPlayer player)
    {

        players.Add(player);

        if (players.Count == 1)//checking if the number of the clients is the minimum for play
        {
            ServerNextState("ServerState_StartDeck");//starting the game
        }
        else
        {

          
            foreach (var other in players)
            {
                if (other == player)
                    continue;

                foreach (var card in other.handCards)
                {
                    var msg = new CardConstants.CardMessage();
                    msg.playerId = other.netId;
                    msg.cardId = card;

                    player.connectionToClient.Send(CardConstants.CardMsgId, msg);
                }
            }
        }
    }//add player is the function that add the player and check the minimum amount of player to began the Game

    public void RemovePlayer(texasPlayer player)
    {
        players.Remove(player);
    }//remove the player from the Game
   

    // enters the state immediately
    [Server]
    void ServerEnterGameState(CardConstants.GameTurnState newState, string message, double mainPotinfo)
    {

        Debug.Log("Server Enter state:" + newState);
        turnState = newState;
        RpcGameState(newState, message, mainPotinfo);
    }//this function responsible for the status of the Game and broadcasting it to the client

    // will transition to a new state via the function after a delay
    [Server]//this function works only on the server
    void ServerNextState(string funcName)//this function invokes all the game states function and control the game 
    {
        Debug.Log("Server next state:" + funcName);
        Invoke(funcName, 2.0f);
    }//this function invokes the function in the server in dealay

    [ClientRpc]//from server to clients
    private void RpcGameState(CardConstants.GameTurnState newState, string message, double mainPotinfo)
    {
        ClientHandleState(newState, message, mainPotinfo);
    }//broadcasting the game state and main pot at the start of every state or turn


    [ClientRpc]
    public void RpcsendPotInfo(double potinfo)
    {
        ClientsendPotInfo(potinfo);
    }//broadcast the main pot to all clients

    [Client]
    private void ClientsendPotInfo(double pot)
    {

        MainPotAmount.text = pot.ToString();

    }//updating the main pot amount visualy at the client side


    [Client]//run only on clients side
    void ClientHandleState(CardConstants.GameTurnState newState, string message, double mainPotinfo)
    {

        turnState = newState;
        string msg = "Client TurnState:" + newState + " : " + message;
        MainPot = mainPotinfo;
        MainPotAmount.text = mainPotinfo.ToString();
        infoText.text = message;
        Debug.Log(msg);

        ClientDisableAllButtons();

        switch (newState)
        {
            case CardConstants.GameTurnState.ShufflingDeck:
                {
                    ClientState_StartDeck();
                    break;
                }

            case CardConstants.GameTurnState.WaitingForBets:
                {
                    ClientState_WaitingForBets();
                    break;
                }
            case CardConstants.GameTurnState.Flop:
                {

                    break;
                }
            case CardConstants.GameTurnState.Turn:
                {

                    break;
                }

            case CardConstants.GameTurnState.River:
                {

                    break;
                }
            case CardConstants.GameTurnState.DealingCards:
                {
                    ClientState_DealingCards();
                    break;
                }

            case CardConstants.GameTurnState.PlayingPlayerHand:
                {

                    ClientState_PlayHands();
                    break;
                }


            case CardConstants.GameTurnState.Complete:
                {
                    ClientState_Complete();
                    break;
                }
        }
    }//this function responsible to updateing the state of the turn at the client side and starting a function according to the game status

    [Client]
    public void ClientDisableAllButtons()
    {
        buttonStay.interactable = false;
        buttonHitMe.interactable = false;
        buttonDoubleDown.interactable = false;
        buttonSplit.interactable = false;
        buttonPlaceBet.interactable = false;
        buttonRaiseBet.interactable = false;
        
    }//disable all the button

    [Client]
    public void DisableRaise()
    {
        buttonPlaceBet.interactable =false;
        buttonRaiseBet.interactable = false;
        buttonStay.interactable = false;
        buttonDoubleDown.interactable = false;
        buttonHitMe.interactable = true;
        buttonSplit.interactable = true;
    }//disable afet raise

    [Client]
    public void EnablePlayHandButtons()
    {
        buttonSound.Play();
        buttonPlaceBet.interactable = true;
        buttonRaiseBet.interactable = true;
        buttonStay.interactable = true;
        buttonHitMe.interactable = true;
        buttonDoubleDown.interactable = true;
        buttonSplit.interactable = true;
    }//enables all the button

    [ClientRpc]//from server to clients
    private void RpcDisable(int buttonsState)
    {
        if (buttonsState == 1)
            DisableRaise();
        if (buttonsState == 2)
            ClientDisableAllButtons();

    }

    // ------------------------ Client State Functions -------------------------------

    [Client]
    public void ClientState_StartDeck()
    {
        deck.ReturnAllCards();//create Deack
        deck.Shuffle();
    }//work on the client side restarting the game deck to the start position and suffle it 

    [Client]
    void ClientState_WaitingForBets()
    {
        buttonPlaceBet.interactable = true;
        buttonRaiseBet.interactable = true;
    }//in this stage of game we need to enable the client only 2 button


    [Client]
    void ClientState_DealingCards()
    {
    }

    [Client]
    void ClientState_PlayHands()
    {

       
    }

    

    [Client]
    void ClientState_Complete()
    {
        
    }

    // ------------------------ Server State Functions -------------------------------

    [Server]
    public void ServerState_StartDeck()
    {
        ServerEnterGameState(CardConstants.GameTurnState.ShufflingDeck, "Shuffling", MainPot);//runs the Game on the client side
        deck.ReturnAllCards();//restoring all the card deck to a full card deck
        deck.Shuffle();
        ServerClearHands();

        ServerNextState("ServerState_WaitingForBets");
    }//server state function start deck restore all the cards to the deck suffle it and clear all the player hand from the table 

    [Server]
    void ServerState_WaitingForBets()
    {
        ServerEnterGameState(CardConstants.GameTurnState.WaitingForBets, "Waiting for bets", MainPot);
        
    }//in this stage of the game we wating for the client respods after we updating the game status


    [Server]
    void ServerState_DealingCards()
    {
        ServerEnterGameState(CardConstants.GameTurnState.DealingCards, "Dealing cards", MainPot);

        // deal first card
        foreach (var player in players)
        {
            
            if (player.bettingOnCurrentHand == false)
                continue;

            ServerDealCard(player, false);

        }

        // deal second card
        foreach (var player in players)
        {
           
            if (player.bettingOnCurrentHand == false)
                continue;

            ServerDealCard(player, false);
        }
 

        //dealFlop
        ServerNextState("ServerDealFlop");


    }//deal the cards to the clients if they make a bet at this and invokeing the next turn state flop function

    [Server]
    void ServerState_PlayHands()
    {
        ServerEnterGameState(CardConstants.GameTurnState.PlayingPlayerHand, "Playing hands", MainPot);

        // currentPlayerIndex will be incremented in ServerNextPlayer
        currentPlayerIndex = -1;
        ServerNextPlayer();
    }//this funtion is play hand function change the status to play hand and call the funcion that control the turns of the player



    // ---- server player actions -----

    private void GetMaxTempBet()
    {
        int max = 0;
        foreach (texasPlayer player in players)
        {
            if (player.tmpBet > max)
                max = player.tmpBet;
        }

        maxTempBet = max;
    }//getting the max bet at a turn 

    [Server]
    public void ServerCheckAllBets()//check if all the players place bet and ready to start the game
    {
        
        foreach (var player in players)
        {
            if (!player.bettingOnCurrentHand)
                return;

            if (player.bettingOnCurrentHand&&player.playerState!=texasPlayer.PlayerState.fold)
            {
                playerCount += 1;
                
            }
        }

        foreach (texasPlayer player in players)
        {
            RpcDisable(2);
        }
        
        GetMaxTempBet();

       
        foreach (texasPlayer player in players)
        {
           
            if (player.tmpBet < maxTempBet)
            {
                if (isLocalPlayer)
                {
                    RpcDisable(1);//enable only call fold buttons
                    return;
                }
                else
                {
                    ClientDisableAllButtons();

                }
            }
            else
            {
                continue;
            }
        }

        

        if (playerCount == 0)
        {
            // no players bet on this hand. reset it
            ServerNextState("ServerState_WaitingForBets");
        }
        else
        {

            playerCount = 0;
            ServerNextState("ServerState_DealingCards");
           
        }
    }

    [Server]
    void ServerDealCard(texasPlayer player, bool hideCard)
    {
        var newCard = deck.GetTopCard();
        newCard.hidden = hideCard;
        player.ServerAddCard(newCard);
    }//funtion in the server in purpos is to drow cards from the deck and deal it to the players


    [Server]
    void ServerDealFlop()
    {

        ServerEnterGameState(CardConstants.GameTurnState.Flop, "Flop", MainPot);
        for (int i = 0; i < 3; i++)
        {
            var newCard = deck.GetTopCard();
            newCard.hidden = false;
            cardsInTable.Add(newCard);
            communityCard.AddCard(newCard, 90);
            RpcdealFlop(newCard);
        }
        ServerNextState("ServerState_PlayHands");
        


    }//deal flop funciotn 

    [ClientRpc]//from server to client 
    void RpcdealFlop(CardId newCard)//from server to clients 
    {
        if (!isServer)
        {
            // this was already done for host player
            communityCard.AddCard(newCard, 90);
            cardsInTable.Add(newCard);
        }
    }//broadcast the the cards in the flop to all the clients

    [Server]
    public void ServerDealturn()
    {      
            var newCard = deck.GetTopCard();
            newCard.hidden = false;
            Debug.Log(newCard.suite);
            cardsInTable.Add(newCard);
            communityCard.AddCard(newCard, 90);
            RpcdealTurn(newCard);
            ServerNextState("ServerState_PlayHands");
    }//deal turn function

    [ClientRpc]//from server to client 
    void RpcdealTurn(CardId newCard)//from server to clients 
    {
        if (!isServer)
        {
            cardsInTable.Add(newCard);
            // this was already done for host player
            communityCard.AddCard(newCard, 90);
        }
    }//broadcast the turn card to all the clients

    [Server]
    void ServerDealRiver()
    {
        ServerEnterGameState(CardConstants.GameTurnState.River, "River", MainPot);
        var newCard = deck.GetTopCard();
        newCard.hidden = false;
        Debug.Log(newCard.suite);
        cardsInTable.Add(newCard);
        communityCard.AddCard(newCard, 90);
        RpcdealRiver(newCard);
        ServerNextState("ServerState_PlayHands");
     
    }//deal river card

    [ClientRpc]//from server to client 
    void RpcdealRiver(CardId newCard)//from server to clients 
    {
        if (!isServer)
        {
            cardsInTable.Add(newCard);
            // this was already done for host player
            communityCard.AddCard(newCard, 90);
        }
    }//broadcast the turn card to all the clients



    [Server]
    void ServerClearHands()//clearing the card in the server then clearring the card at the client
    {
       
        communityCard.ClearCards();
        cardsInTable.Clear();
        foreach (var player in players)
        {
            player.ServerClearCards();
            player.ServerClearBet();
        }
        currentTurnPlayer = null;
    }

    [Server]
    public void ServerNextPlayer()//control the game tempo in this function need to check player state fold/all in /check
    {
        
        if (currentTurnPlayer != null)
        {
            currentTurnPlayer.RpcYourTurn(false);
        }

        currentPlayerIndex += 1;
        while (currentPlayerIndex < players.Count)
        {
            currentTurnPlayer = players[currentPlayerIndex];
            if (currentTurnPlayer != null)
            {
               
                
                    if (currentTurnPlayer.bettingOnCurrentHand&& currentTurnPlayer.playerState != texasPlayer.PlayerState.fold && currentTurnPlayer.playerState != texasPlayer.PlayerState.allIn)
                    {
                        currentTurnPlayer.RpcYourTurn(true);

                        break;

                    }
                
              
            }
            currentPlayerIndex += 1;
           
            
        }
        

        
        if (currentPlayerIndex >= players.Count)
        {


            
            players[currentPlayerIndex-1].RpcYourTurn(false);//disable the buttons

            //if (AllFolded())//no player is in this bet
            //{
            //    foreach (texasPlayer player in players)
            //    {
            //        if (player.playerState != texasPlayer.PlayerState.fold)
            //        {
            //            player.ServerPayout((int)MainPot);
            //            break;
            //        }

            //    }
            //    ServerNextHand();
            //    return;
            //}




            if (RoomManager.singleton.cardsInTable.Count == 4)//River 
            {
                ServerNextState("ServerDealRiver");
            }
            else
                if (RoomManager.singleton.cardsInTable.Count == 3)//Turn 
            {
                ServerNextState("ServerDealturn");
            }
            else
            {
                if (RoomManager.singleton.cardsInTable.Count == 5)//Show Down
                {
                    ServerNextState("ShowDown");
                }
            }

            // all players have played their hands

            
        }
    }

    public bool EveryBodyCheck()
    {
        if (CheckedPlayers.Count >= 1 && CheckedPlayers.Count < players.Count)
        {
            everybodyChecked = false;
            return everybodyChecked;

        }

        everybodyChecked = true;
        return everybodyChecked;

    }

    public CardConstants.HandRank EvaluatPlayerHand(List<CardId> finalHand)
    {

        HandAnalizer = new HandRanker();
        HandAnalizer.setHandRanker(finalHand);


        if (IsRoyalFlush(finalHand))
            return CardConstants.HandRank.royalFlush;
        if (IsStarightFlush(finalHand))
            return CardConstants.HandRank.strightFlush;
        if (HandAnalizer.Rankcount[0] == 4)
            return CardConstants.HandRank.fourOfAKind;
        if (HandAnalizer.Rankcount[0] == 3 && HandAnalizer.Rankcount[1] == 2)
            return CardConstants.HandRank.fullHouse;
        if (HandAnalizer.Suits[0] >= 5)
            return CardConstants.HandRank.flush;
        if (IsStright(finalHand))
            return CardConstants.HandRank.stright;
        if (HandAnalizer.Rankcount[0] == 3)
            return CardConstants.HandRank.threeOfaKind;
        if (HandAnalizer.Rankcount[0] == 2 && HandAnalizer.Rankcount[1] == 2)
            return CardConstants.HandRank.twoPairs;
        if (HandAnalizer.Rankcount[0] == 2)
            return CardConstants.HandRank.onePair;
        else
            return CardConstants.HandRank.highCard;

    }//this function  determine hand value after the anlizer work on the player hand 

    private bool IsRoyalFlush(List<CardId> cards)
    {

        int count = 0;

        for (int j = 0; j < 4; j++)
        {

            count = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].card == CardConstants.Card.King && (CardConstants.Suite)j == cards[i].suite)
                    count++;

                if (cards[i].card == CardConstants.Card.Queen && (CardConstants.Suite)j == cards[i].suite)
                    count++;

                if (cards[i].card == CardConstants.Card.Jack && (CardConstants.Suite)j == cards[i].suite)
                    count++;

                if (cards[i].card == CardConstants.Card.Ten && (CardConstants.Suite)j == cards[i].suite)
                    count++;

                if (cards[i].card == CardConstants.Card.Ace && (CardConstants.Suite)j == cards[i].suite)
                    count++;

            }

            if (count >= 4)
                return true;
        }
        return false;

    }//check for royal flush 

    private bool IsStarightFlush(List<CardId> cards)
    {

        int count = 0;
       


        for (int i = 0; i < 4; i++)
        {

            count = 0;
            CardId preCard = cards[0];

            for (int j = 1; j < cards.Count; j++)
            {

                if (preCard.card - cards[j].card == 1 && preCard.suite == cards[j].suite)
                {
                    preCard = cards[j];
                    count++;
                }

            }


            if (count >= 4)
                return true;


        }
        return false;

    }//check for stright flush

    private bool IsStright(List<CardId> cards)
    {
        int count = 0;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].card == CardConstants.Card.Two)
                count++;
            if (cards[i].card == CardConstants.Card.Three)
                count++;
            if (cards[i].card == CardConstants.Card.Four)
                count++;
            if (cards[i].card == CardConstants.Card.Five) 
                count++;
            if (cards[i].card == CardConstants.Card.Ace)
                count++;
        }

        if (count == 5)
            return true;

        count = 0;
        for (int i = 0; i < cards.Count - 1; i++)
        {
            if ((int)cards[i].card - (int)cards[i + 1].card == 1)
                count++;
            else
               if (i > 2 && count < 1)
                return false;

            if (count >= 4)
                return true;

        }


        if (count >= 4)
            return true;
        else
            return false;
        
    }//check for stright

    public CardConstants.HandRank MaxRankeInTable()//Hand val like royal flush ,one pair etc....
    {
        CardConstants.HandRank MaxHand = CardConstants.HandRank.none;

        foreach (texasPlayer p in players)
            if (p.PlayerHandRank > MaxHand && p.playerState!= texasPlayer.PlayerState.fold)
                MaxHand = p.PlayerHandRank;

        return MaxHand;

    }//return the max hand Rank in the table 


    private int MaxTotalValue()//the total score od the player hand in numbers High Card
    {
        int max = 0;

        foreach (texasPlayer p in players)
        {
            if (p.totalHandValue > max && p.playerState != texasPlayer.PlayerState.fold)
                max = p.totalHandValue;
        }

        return max;

    }




    public void ShowDown()
    {
        ServerEnterGameState(CardConstants.GameTurnState.Complete, "Show Down", MainPot);
        foreach (texasPlayer player in players)
        {
            if (player.playerState == texasPlayer.PlayerState.fold)
                continue;
            else
            {
                List<CardId> finalhand = new List<CardId>();

                foreach (CardId card in player.handCards)
                {
                    finalhand.Add(card);
                }

                foreach (CardId card in cardsInTable)
                {
                    finalhand.Add(card);
                }

                player.PlayerHandRank = EvaluatPlayerHand(finalhand);
                player.totalHandValue = HandAnalizer.TotalValue;
            }
        }



        List<texasPlayer> tmpwinners = new List<texasPlayer>();
        List<texasPlayer> Winners = new List<texasPlayer>();
        CardConstants.HandRank winningHand = MaxRankeInTable();//Hand Val Max
        int maximunHandscore = MaxTotalValue();//Max total value in numbers

        foreach (texasPlayer p in players)
        {

            if (p.PlayerHandRank == winningHand && p.playerState != texasPlayer.PlayerState.fold)
                tmpwinners.Add(p);
        }


        if (tmpwinners.Count > 1)
        {
            foreach (texasPlayer p in tmpwinners)
            {
                if (p.totalHandValue == maximunHandscore)
                    Winners.Add(p);
            }

            int numWinners = Winners.Count;
            foreach (texasPlayer p in Winners)
            {
                p.ServerPayout((int)MainPot/Winners.Count);
                Debug.Log(""+" "+Winners.Count);
            }

           
            foreach (texasPlayer player in players)
            {
                if (!Winners.Contains(player))
                {
                    player.ServerLose(player.tmpBet);
                }

            }

           

        }
        else
        {
            tmpwinners[0].ServerPayout((int)MainPot);

            foreach (texasPlayer player in players)
            {
                if (tmpwinners[0] != player)
                {
                    player.ServerLose(player.tmpBet);
                }

            }
        }

       
        ServerNextState("ServerNextHand");


    }//Show down funtion declear the winner after evaluating ever hand in the table and comparison of the player one against the other at the end start game again





    public bool AllFolded()
    {
        Debug.Log("All Folded");
        int count = 0;
        foreach (texasPlayer player in players)
        {
            if (player.playerState == texasPlayer.PlayerState.fold)
                count++;
        }

        
        if (count+1== players.Count)
            return true;
        return false;

    }//check all the player has been folded

    [Server]
    public void ServerNextHand()//the Game restart 
    {
        

        nextHandWaiting = true;
        MainPot = 0;
        ServerClearHands();
        ServerEnterGameState(CardConstants.GameTurnState.ShufflingDeck, "Start Game", MainPot);
        ServerNextState("ServerState_StartDeck");
        
       

    }//stating the game after show down function


    ////// Client UI hooks ////////
    //this funtions connect the game buttons to the local player


    public void UiPlaceBet()
    {
        buttonSound.Play();
        localPlayer.CmdPlaceBet();
    }

    public void UiRaiseBet()
    {
        buttonSound.Play();
        localPlayer.CmdRaiseBet();
    }

    public void UiCall()
    {
        buttonSound.Play();
        localPlayer.CmdCall();
    }

    public void UiAllIn()//AllIn
    {
        buttonSound.Play();
        localPlayer.CmdAllIn();
    }

    public void UiFold()//Fold
    {
        buttonSound.Play();
        localPlayer.CmdFold();
    }

    public void UiCheck()//check
    {
        buttonSound.Play();
        localPlayer.CmdCheck();
    }

  



    public void exitRoom()
    {
        buttonSound.Play();

        if (isServer&&players.Count>1)
        {
            return;

        }



        DbPlayer db = new DbPlayer();
        PlayerData Info = db.GetPlayerByUserName(localPlayer.playerPanel.userNameText.text);
        Info.Money += localPlayer.money;
        db.UpdatePlayer(Info);
        RemovePlayer(localPlayer);
     
        if (isServer)
        {
            LobbyScene.singleton.StopHost();
            Destroy(LobbyScene.singleton.gameObject);
            return;
        }
        

        if (isClient)
        {
            LobbyScene.singleton.StopClient();
            Destroy(localPlayer);
            return;
        }
    }//exit the room after saving the player new score in the data base to the lobby scene
}
