using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebigCard : MonoBehaviour {

    public RoomManager cd;
    public PlayerPanel[] pp;
    public PlayerPanel table;
    public HandRanker HandRanker;
    public List<CardId> finalhand = new List<CardId>();


    void Start()
    {
        HandRanker = GetComponent<HandRanker>();

    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 28), "Start"))
        {
            foreach(PlayerPanel p in pp)
             {
                CardId card = cd.deck.GetTopCard();
                p.AddCard(card, (int)card.suite);
                finalhand.Add(card);

            }

            foreach (PlayerPanel p in pp)
            {
                CardId card = cd.deck.GetTopCard();
                p.AddCard(card, (int)card.suite);
                finalhand.Add(card);
            }

            for (int i = 0; i < table.cardSlots.Length; i++)
            {
                CardId card = cd.deck.GetTopCard();
                table.AddCard(card,90);
                finalhand.Add(card);
            }

            
            HandRanker.setHandRanker(finalhand);
        




        }






    }




    public void Onclick()
    {

        Debug.Log("David");

    }
}
