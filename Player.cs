using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class Player : MonoBehaviour {

	[SerializeField] private int _score;
	[SerializeField] private int _roundsWon;
	[SerializeField] private Sprite _colorRamp;

	[SerializeField] private Deck _deck;

	[Header("Units")]
	[SerializeField] private Unit[] _units;
	[SerializeField] private GameObject _unitParent;

	[Header("Unit Selection Cards")]
	[SerializeField] private GameObject _unitsBackground;
	[SerializeField] private UnitCardSelection[] _unitCards;

	[Header("Terrain Cards")]
	[SerializeField] private TerrainCard[] _terrainCards;

	[Header("Unit UI")]
	[SerializeField] private Text _UnitNameText;
	[SerializeField] private Text _unitDescriptionText;
	[SerializeField] private Text _unitStrengthText;
	[SerializeField] private Vector2 _unitUIOffset = new Vector2 (0, 3);

	[Header("Score UI")]
	[SerializeField] private Text _scoreText;
	[SerializeField] private Image _roundWon1;
	[SerializeField] private Image _roundWon2;
	[SerializeField] private Sprite _roundWonSprite;

	[HeaderAttribute("Turn")]
	[SerializeField] private Image _turnUI;

	public Texture2D ColorTexture
	{
		get { return _colorRamp.texture; }
	}

	public Unit[] Units {
		get { return _units; }
	}

	public Deck Deck {
		get { return _deck; }
		set { _deck = value; }
	}

	public TerrainCard[] TerrainCards {
		get { return _terrainCards; }
	}

	public Text ScoreText {
		get { return _scoreText; }
	}

	public int Score {
		get { return _score; }
	}

	public int RoundsWon {
		get { return _roundsWon; }
	}

	public void Start() 
	{
		_units.ToList ().ForEach ((unit) => {
			unit.Player = this;
			unit.SetInitialPosition();
		});
	}

	public void AddScore(int scoreToAdd) {

		_scoreText.transform.DOKill (false);

		_scoreText.transform.localScale = Vector3.one;

		_scoreText.transform.DOScale (new Vector3(1.4f, 1.4f, 1.4f), 0.5f).SetLoops(2, LoopType.Yoyo);

		_score += scoreToAdd;
	}

	public void ShowUnitCards()
	{
		_unitParent.SetActive (true);

		GameController._delay = 0;

		for (int i = 0; i < _unitCards.Length; i++) {
			Unit unit = _units [i];

			unit.Info = _unitCards [i].UnitInfo;
			unit.Show ();
			unit.TweenIntoPosition (_unitUIOffset);
		}

		if (_unitCards.Length < _units.Length) {
			_units [_units.Length - 1].gameObject.SetActive (false);
		}

		_turnUI.enabled = true;
	}

	public void HideUnitCards()
	{
		_unitParent.SetActive (true);

		GameController._delay = 0;

		for (int i = 0; i < _unitCards.Length; i++) {
			Unit unit = _units [i];

			unit.Info = _unitCards [i].UnitInfo;
			unit.Hide ();
			unit.TweenOutOfPosition (-_unitUIOffset);
		}

		if (_unitCards.Length < _units.Length) {
			_units [_units.Length - 1].gameObject.SetActive (false);
		}

		_turnUI.enabled = false;
	}

	public void SetBackgroundActive (bool active)
	{
		_unitsBackground.SetActive (active);
	}

	public void DistributeUnitCards ()
	{
		
		_unitCards.ToList().ForEach((card) => { 

			int randomIndex = UnityEngine.Random.Range(0, Deck.InUse.Count);

			UnitCard unit = Deck.InUse[randomIndex];

			Deck.InUse.RemoveAt(randomIndex);
			Deck.InHand.Add(unit);

			card.UnitInfo = Deck.GetInfoMatchingEnum(unit);

			card.Show();

		});

	}

	public bool WereAnyUnitCardsClickedOn ()
	{
		return _unitCards.ToList().Any((card) => card.wasClickedOn);
	}

	public UnitCardSelection GetUnitCardClickedOn() {
		return _unitCards.ToList().Find((card) => card.wasClickedOn);
	}

	public void EnableTerrainCardDragging ()
	{
		_terrainCards.ToList ().ForEach ((v) => {
			v.EnableColor();
			v.Enable ();
		});	
	}

	public void DisableTerrainCardDragging ()
	{
		_terrainCards.ToList ().ForEach ((v) => {
			v.DisableColor();
			v.Disable ();
		});
	}

	public void ShowTerrainCardColors ()
	{
		_terrainCards.ToList ().ForEach ((v) => {
			v.EnableColor();
		});
	}

	public void HideTerrainCardColors ()
	{
		_terrainCards.ToList ().ForEach ((v) => {
			v.DisableColor();
		});
	}
		
	public void UpdateUnitUI (UnitInfo unitInfo)
	{
		_UnitNameText.text = unitInfo.UnitName;
		_unitStrengthText.text = unitInfo.Strength + "";
		_unitDescriptionText.text = unitInfo.UnitDescription;
	}

	public void Update() 
	{
		_scoreText.text = string.Format ("{0}", _score);
	}

	public void OnRoundWin ()
	{
		_roundsWon++;

		if (_roundsWon == 1) {
			_roundWon1.enabled = true;
			_roundWon1.transform.localScale = Vector3.zero;
			_roundWon1.sprite = _roundWonSprite;
			_roundWon1.transform.DOScale (Vector3.one, 0.3f).SetEase (Ease.OutElastic);
		} else {
			_roundWon2.enabled  = true;
			_roundWon2.sprite = _roundWonSprite;
			_roundWon2.transform.localScale = Vector3.zero;
			_roundWon2.transform.DOScale (Vector3.one, 0.3f).SetEase (Ease.OutElastic);
		}

	}

	public bool IsOutOfCards ()
	{
		return _units.ToList ().All ((t) => t.IsPlaced ());
	}

	public void ResetScore ()
	{
		_score = 0;
	}
}