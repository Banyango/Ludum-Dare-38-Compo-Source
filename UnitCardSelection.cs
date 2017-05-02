using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum UnitCard {
	Grunt,
	Soldier,
	Ship,
	Catapult,
	Castle,
	ElvenArcher,
	ElvenSwordsman,
	RockGolem,
	Mage,
	Wisp,
	Varl_G,
	Varl_R,
	Varl_W,
	Grunt_W,
	Grunt_R
}

public class UnitCardSelection : MonoBehaviour {

	public GameController Controller;
	public UnitInfo UnitInfo;

	public SpriteRenderer _cardSprite;

	public float DissolveAmount;

	public void Awake() {
		_cardSprite = GetComponent<SpriteRenderer> ();
	}

	public bool wasClickedOn {
		get;
		set;
	}

	public void OnMouseUp() {
		wasClickedOn = true;
	}

	public void OnMouseOver() {
		Controller.UpdateUnitUI (UnitInfo);
	}

	public void Show () {
		_cardSprite.sprite = UnitInfo.CardSprite;
	}

	public void AnimateNewCardSelected ()
	{
		DOTween.To (()=> {return DissolveAmount;}, (v) => {DissolveAmount = v;}, 1.0f, 0.8f).OnComplete(ChangeSpriteAndFadeIn);
	}

	void ChangeSpriteAndFadeIn ()
	{
		
		_cardSprite.sprite = UnitInfo.CardSprite;

		DOTween.To (()=> {return DissolveAmount;}, (v) => {DissolveAmount = v;}, 0.0f, 0.8f);
	}

	public void Update() {

		_cardSprite.material.SetFloat ("_DissolveAmount", DissolveAmount);


	}
}

