using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {

	[SerializeField] private List<UnitCard> _deck = new List<UnitCard>();
	[SerializeField] private List<UnitInfo> _unitInfo = new List<UnitInfo> ();

	[SerializeField] private List<UnitCard> _inUseDeck = new List<UnitCard>();
	[SerializeField] private List<UnitCard> _inHandDeck = new List<UnitCard>();

	public List<UnitCard> InUse {
		get { return _inUseDeck; }
	}

	public List<UnitCard> InHand {
		get { return _inHandDeck; }
	}

	public void Start() {

		Object[] objects = Resources.LoadAll ("Units/");

		for (int i = 0; i < objects.Length; i++) {
			var info = (UnitInfo) objects [i];

			_unitInfo.Add (info);

			for (int j = 0; j < info.count; j++) {
				_deck.Add (info.UnityType);
			}
		}

	}

	public List<UnitCard> DeckUnitCards {
		get { return _deck; }
	}

	public UnitInfo GetInfoMatchingEnum(UnitCard type) {
		return _unitInfo.Find ((t) => t.UnityType == type);
	}

	public void CloneDeck ()
	{
		List<UnitCard> cloned = new List<UnitCard> ();

		for (int i = 0; i < _deck.Count; i++) {
			cloned.Add (_deck [i]);
		}

		_inUseDeck = cloned;
	}
}
