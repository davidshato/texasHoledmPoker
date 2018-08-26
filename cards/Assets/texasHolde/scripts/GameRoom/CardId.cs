using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct CardId
{
	public CardId(CardConstants.Suite s, CardConstants.Card c, bool h)
	{
		suite = s;
		card = c;
		hidden = h;
	}

	public CardConstants.Suite suite;
	public CardConstants.Card card;
	public bool hidden;

	public override string ToString()
	{
		return suite.ToString() + ":" + card.ToString();
	}
}
