using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRanker : MonoBehaviour {

    private int[] suits;//like ranks
    private CardConstants.Card [] ranks;//if hand j,q,k,ace so in ranks[0]=ace,ranks[1]=k,reanks[2]=q ...
    private int totalSuits;//num of different suits
    private int totalRanks;//num of different ranks in hand
    private int[] rankcount;//shows array, if player have 3 king in hand,rankcount[0]=3
    private int totalValue;

    public int[] Suits
    {
        get
        {
            return suits;
        }

        set
        {
            suits = value;
        }
    }

    public CardConstants.Card[] Ranks
    {
        get
        {
            return ranks;
        }

        set
        {
            ranks = value;
        }
    }

    public int TotalSuits
    {
        get
        {
            return totalSuits;
        }

        set
        {
            totalSuits = value;
        }
    }

    public int TotalRanks
    {
        get
        {
            return totalRanks;
        }

        set
        {
            totalRanks = value;
        }
    }

    public int[] Rankcount
    {
        get
        {
            return rankcount;
        }

        set
        {
            rankcount = value;
        }
    }

    public int TotalValue
    {
        get
        {
            return totalValue;
        }

        set
        {
            totalValue = value;
        }
    }

    public void setHandRanker(List<CardId> cards)
    {

        Dictionary<CardConstants.Card, int> rank = new Dictionary<CardConstants.Card, int>();
        Dictionary<CardConstants.Suite, int> suit = new Dictionary<CardConstants.Suite, int>();

        foreach (CardId card in cards)
        {
            if (rank.ContainsKey(card.card))//counting the Amount of different cards in hand 
                rank[card.card] += 1;
            else
                rank.Add(card.card, 1);


            if (suit.ContainsKey(card.suite))//counting the Amount of the different suits in hand
                suit[card.suite] += 1;
            else
                suit.Add(card.suite, 1);

        }




        //    Debug.Log("rank");
        //foreach (KeyValuePair<CardConstants.Card, int> kvp in rank)
        //{

        //    Debug.Log("Key =" + kvp.Key + ", Value = " + kvp.Value);
        //}

        //Debug.Log("suit");
        //foreach (KeyValuePair<CardConstants.Suite, int> kvp in suit)
        //{
        //    Debug.Log("Key =" + kvp.Key + ", Value = " + kvp.Value);
        //}


                List<CardConstants.Card> handvalues = new List<CardConstants.Card>();
                foreach (var item in rank)
                {
                    handvalues.Add(item.Key);
                }
                handvalues.Sort();
                handvalues.Reverse();

                ranks = handvalues.ToArray();

        //Debug.Log("Ranks");
        //for (int i = 0; i < ranks.Length; i++)
        //{
        //    Debug.Log(ranks[i]);
        //}


                List<int> handsuits = new List<int>();
                foreach (var item in suit)
                {
                    handsuits.Add(item.Value);
                }
                handsuits.Sort();
                handsuits.Reverse();

                suits = handsuits.ToArray();

        //Debug.Log("Suits");
        //for (int i = 0; i < suits.Length; i++)
        //{
        //    Debug.Log(suits[i]);
        //}


    
            totalRanks = ranks.Length;
        //Debug.Log("Total Ranks: " + totalRanks);

        this.totalSuits = suits.Length;


            int value = 0;
            foreach (CardId card in cards)
            {
                   value += (int)card.card + 1;

             }
                this.totalValue = value;
        //Debug.Log("Total Value: " + totalValue);


        List<int> values = new List<int>();


        foreach (KeyValuePair<CardConstants.Card, int> item in rank)
        {
            values.Add(item.Value);
            
        }


        values.Sort();
        values.Reverse();


        int size = values.Count;
        this.rankcount = new int[size];
        for (int i = 0; i < size; i++)
            this.rankcount[i] = values[i];
        //Debug.Log("rankCount");
        //foreach (int i in rankcount)
        //    Debug.Log(i);








    }
}
