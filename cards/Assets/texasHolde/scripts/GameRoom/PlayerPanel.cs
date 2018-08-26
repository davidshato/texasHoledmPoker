using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerPanel : MonoBehaviour
{
	public Text userNameText;
	public Text BetText;
    public Image playerImage;
	public CardSlot[] cardSlots;
	public int nextSlot = 0;
	public int score = 0;
	public Image cardPanelImage;
    public AudioClip MusicClip;
    public AudioSource MusicSource;

    

    private void Awake()
	{
        
		cardSlots = GetComponentsInChildren<CardSlot>();//get into cardslots the cardId object from the scene
		ClearCards();
	}

	void Start()
	{
        //this.gameObject.SetActive(false);
        MusicSource = GetComponent<AudioSource>();
        MusicSource.clip = MusicClip;
		
	}

	public void ColorImage(Color c)
	{
		cardPanelImage.color = c;
	}//setting the panel color

	public void ShowCard(int slotId)
	{
		var cardSlot = cardSlots[slotId];
		cardSlot.cardId.hidden = false;
		cardSlot.GetComponent<Image>().sprite =RoomManager.singleton.GetCardSprite(cardSlot.cardId);

	}//show the player cards

	public void SetScore(int cardScore)
	{
		score = cardScore;
		
	}//seeting the card score

	public void AddCard(CardId cardId, int cardScore)
	{
        
		var cardSlot = cardSlots[nextSlot];
		nextSlot += 1;

		var findCard = cardId;
		if (cardId.hidden)
		{
			// this is the back of the card
			findCard.suite = CardConstants.Suite.Diamonds;
			findCard.card = CardConstants.Card.Joker;
		}

		cardSlot.GetComponent<Image>().sprite = RoomManager.singleton.GetCardSprite(findCard);
		cardSlot.cardId = cardId;
		cardSlot.gameObject.SetActive(true);
        MusicSource.Play();
		
	}//adding a card visualy 

	public void ClearCards()
	{
		foreach (var card in cardSlots)
		{
			card.gameObject.SetActive(false);
			card.cardId = new CardId();
		}
		nextSlot = 0;
		score = 0;
		
	}//clearing the card visaly

	
}
