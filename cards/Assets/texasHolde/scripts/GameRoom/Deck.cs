using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Deck : MonoBehaviour
{
    public AudioSource shuffleSound;

    public List<CardId> readyCards = new List<CardId>();
	public List<CardId> usedCards = new List<CardId>();

	public void Awake()
	{
		foreach (var suite in CardConstants.Suites)
		{
			foreach (var card in CardConstants.Cards)
			{
				readyCards.Add(new CardId(suite, card, false));
			}
		}
		Shuffle();
	}

	public void Shuffle()
	{
        shuffleSound.Play();

        List<CardId> tmp = new List<CardId>();

		int max = readyCards.Count;
		while (max > 0)
		{
			int offset = UnityEngine.Random.Range(0, max);
			tmp.Add(readyCards[offset]);
			readyCards.RemoveAt(offset);
			max -= 1;
		}
		readyCards = tmp;
	}

	public void ReturnAllCards()
	{
		foreach (var cardId in usedCards)
		{
			readyCards.Add(cardId);
		}
		usedCards.Clear();
	}

	public CardId GetTopCard()
	{
		int top = readyCards.Count - 1;

		var cardId = readyCards[top];
		readyCards.RemoveAt(top);
		usedCards.Add(cardId);

		//Debug.Log("Got top:" + top + " " + cardId);
		return cardId;
	}
}