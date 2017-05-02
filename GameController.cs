using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	[SerializeField] private GameState _state;

	public delegate void ObjectMouseInfo(string name, string amount, string description);

	[SerializeField] private Player _playerOne;
	[SerializeField] private Player _playerTwo;

	[Header("Tiles")]
	[SerializeField] private List<int> _possiblePoints;
	[SerializeField] private List<Tile> _terrainTiles;

	[Header("Description")]
	[SerializeField] private Text _infoName;
	[SerializeField] private Text _infoStrength;
	[SerializeField] private Text _infoDescription;

	[Header("Game Over")]
	[SerializeField] private GameObject _gameOverParent;
	[SerializeField] private Image _gameOverBackground;
	[SerializeField] private Text _gameOverText;

	[Header("Camera Fade")]
	[SerializeField] private Image _cameraFade;


	private GameState[] _sequence;
	private int _seqeunceIndex = -1;

	public static float _delay;
	private System.Random rng = new System.Random();

	public Player PlayerOne {
		get { return _playerOne; }
	}

	public Player PlayerTwo {
		get { return _playerTwo; }
	}

	public void Start() {

		Shuffle (_possiblePoints);

		_terrainTiles = GameObject.FindObjectsOfType<Tile> ().ToList();

		_sequence = new GameState[]{ 
			new WaitState(1.5f),
			new DistributeTerrainCards(), 
			new WaitState(3),
			new DistributeUnitCards (PlayerOne),
			new ChooseUnitsState (PlayerOne),
			new WaitState(1),
			new DistributeUnitCards(PlayerTwo),
			new ChooseUnitsState (PlayerTwo),
			new DistributeTilePoints(),
			new ChooseTerrain(PlayerOne),
			new ChooseTerrain(PlayerTwo),
			new ChooseTerrain(PlayerOne),
			new ChooseTerrain(PlayerTwo),
			new ChooseTerrain(PlayerOne),
			new ChooseTerrain(PlayerTwo),
			new ChooseTerrain(PlayerOne),
			new ChooseTerrain(PlayerTwo),
			new ChooseTerrain(PlayerOne),
			new ChooseTerrain(PlayerTwo),
			new ChooseTerrain(PlayerOne),
			new ChooseTerrain(PlayerTwo),
			new ChooseTerrain(PlayerOne),
			new ChooseTerrain(PlayerTwo),
			new ChooseTerrain(PlayerOne),
			new ChooseTerrain(PlayerTwo),
			new ChooseTerrain(PlayerTwo),
			new PlayerTurn(PlayerOne),
			// 		new EndGameState()
			// 		new RestartGame()
		};

		AdvanceState ();

		var tiles = GameObject.FindObjectsOfType<Tile> ();

		tiles.ToList ().ForEach ((t) => {
			t.OnUnitPlaced = this.OnUnitPlacedOnGrid;
			t.OnTerrianTilePlaced = this.OnTerrainTilePlacedOnGrid;
			t.OnDoneAddingPoints = this.OnDoneAddingPoints;
			t.OnMouseInfoListener = this.OnMouseInfoUpdated;
		});

		PlayerOne.Units.ToList ().ForEach ((t) => t.OnMouseInfoListener = this.OnMouseInfoUpdated);
		PlayerTwo.Units.ToList ().ForEach ((t) => t.OnMouseInfoListener = this.OnMouseInfoUpdated);

		PlayerOne.TerrainCards.ToList ().ForEach ((t) => t.OnMouseInfoListener = this.OnMouseInfoUpdated);
		PlayerTwo.TerrainCards.ToList ().ForEach ((t) => t.OnMouseInfoListener = this.OnMouseInfoUpdated);

		FadeIn ();
	}

	public void Shuffle<T>(IList<T> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}

	public void SetState (GameState newState)
	{
		Debug.Log ("Changing to state" + newState.GetType () + " from " + _state.GetType ());

		if (_state != null) {
			_state.OnDisable (this);
		}

		_state = newState;

		newState.OnEnable (this);

	}

	public void Update() {
		_state.OnUpdate (this);
	}

	public void AdvanceState() {
		_seqeunceIndex++;

		_seqeunceIndex = _seqeunceIndex % _sequence.Length;

		SetState (_sequence [_seqeunceIndex]);
	}

	public void DistributeTerrainCards ()
	{
		PlayerOne.TerrainCards.ToList ().ForEach (RandomizeAndDisableTerrainCard);	

		PlayerOne.TerrainCards.ToList ().ForEach (TweenCardsIntoPosition);

		_delay = 0;

		PlayerTwo.TerrainCards.ToList ().ForEach (RandomizeAndDisableTerrainCard);	

		PlayerTwo.TerrainCards.ToList ().ForEach (TweenCardsIntoPositionTwo);
	}

	void TweenCardsIntoPosition (TerrainCard terrainCard)
	{
		_delay+=0.1f;

		terrainCard.TweenIntoPosition (new Vector2(0, 3), _delay);
	}

	void TweenCardsIntoPositionTwo (TerrainCard terrainCard)
	{
		_delay+=0.1f;

		terrainCard.TweenIntoPosition (new Vector2(0, -3), _delay);
	}

	public void RandomizeAndDisableTerrainCard (TerrainCard terrainCard)
	{
		terrainCard.RandomizeType ();

		terrainCard.Disable ();
	}

	public bool WasContinueButtonPressed ()
	{
		return Input.GetMouseButtonUp(0);
	}

	public void FadeIn() {
		_cameraFade.DOColor(new Color(1,1,1,0), 1f).SetDelay(0.5f);
	}

	public void FadeOutIn ()
	{
		_cameraFade.DOColor(Color.white, 0.3f).OnComplete(()=> {
			_cameraFade.DOColor(new Color(1,1,1,0), 1f).SetDelay(2f);
		});
	}

	public void DistributePoints ()
	{
		var tiles = GameObject.FindObjectsOfType<Tile> ();

		_delay = 0;

		if (tiles != null && tiles.Length > 0) {
			tiles.ToList ().ForEach ((tile) => {
				int points = _possiblePoints [0];
			
				_possiblePoints.RemoveAt (0);
			
				tile.CreatePoints (points);
			
				_delay += 0.2f;

			}
			);
		}
	}

	public void DistributeUnitCards (Player player) {
		player.Deck.CloneDeck ();

		Shuffle (player.Deck.InUse);

		player.DistributeUnitCards ();
	}

	public void OnDoneChoosingCards()
	{
		AdvanceState ();
	}

	public void UpdateUnitUI (UnitInfo unitInfo)
	{
		if (_state is ChooseUnitsState) {
			((ChooseUnitsState)_state).UpdateUnitUI (unitInfo);
		}
	}
		
	public void RedistributeUnitCard (UnitCardSelection clickedUnitCard, Player player)
	{
		int randomIndex = UnityEngine.Random.Range(0, player.Deck.InUse.Count);

		UnitCard unit = player.Deck.InUse[randomIndex];

		player.Deck.InUse.RemoveAt(randomIndex);
		player.Deck.InHand.Add(unit);
		player.Deck.InUse.Add (clickedUnitCard.UnitInfo.UnityType);

		clickedUnitCard.UnitInfo = player.Deck.GetInfoMatchingEnum(unit);

		clickedUnitCard.AnimateNewCardSelected ();

		clickedUnitCard.wasClickedOn = false;
	}

	public void SetUnitBackgroundActive (bool active, Player player)
	{
		player.SetBackgroundActive (active);
	}

	public bool AreAllTerrainCardsDistributed() 
	{
		return !_terrainTiles.ToList ().Any ((t) => t.TerrainType == Tile.TileType.None); 
	}

	public void EnableTerrainCardDragging ()
	{
		PlayerOne.TerrainCards.ToList ().ForEach ((v) => {
			v.EnableColor();
			v.Enable ();
		});

		PlayerTwo.TerrainCards.ToList ().ForEach ((v) => {
			v.EnableColor();
			v.Enable ();
		});
	}

	public void OnTerrainTilePlacedOnGrid(Tile tile, TerrainCard terrain) {
		if (_state is ChooseTerrain) {
			AdvanceState ();
		}
	}

	public void OnUnitPlacedOnGrid (Unit unit, Tile tile)
	{
		if (_state is PlayerTurn) {
			((PlayerTurn)_state).DisablePlacingCards ();
		}
	}

	public void DistributeAllTerrainCards() {
		PlayerOne.TerrainCards.ToList ().ForEach ((t) => t.Hide ());	
		PlayerTwo.TerrainCards.ToList ().ForEach ((t) => t.Hide ());	

		_terrainTiles.ForEach ((t) => t.RandomizeTerrian());

		_seqeunceIndex = 26;

		SetState (_sequence [26]);
	}

	public void OnDoneAddingPoints ()
	{
		if (_state is PlayerTurn) {
			ChangeToOtherPlayersTurn ();
		} else if (_state is EndRound) {
			ContinuteWithEndOfRound ();
		}
	}

	public void ChangeToOtherPlayersTurn() {
		if (_state is PlayerTurn) {
			if (((PlayerTurn)_state).Player.Equals(PlayerOne)) {
				if (!PlayerOne.IsOutOfCards ()) {
					SetState (new PlayerTurn (PlayerTwo));
				} else {
					SetState (new EndRound (PlayerTwo));
				}
			} 
			else 
			{
				if (!PlayerTwo.IsOutOfCards ()) {
					SetState (new PlayerTurn (PlayerOne));
				} else {
					SetState (new EndRound (PlayerOne));
				}

			}
		}
	}

	void ContinuteWithEndOfRound ()
	{
		if (((EndRound)_state).Player.IsOutOfCards ()) {
			SetState (new TallyPointsAtEndOfRound (((EndRound)_state).Player));
		} else {
			SetState (new EndRound (((EndRound)_state).Player, false));
		}

	}

	public void CompleteRound() {
		if (_state is PlayerTurn) {
			if (((PlayerTurn)_state).Player == PlayerOne) {
				SetState (new EndRound (PlayerTwo));
			} 
			else 
			{
				SetState (new EndRound (PlayerOne));				
			}
		} else if (_state is EndRound) {
			SetState (new TallyPointsAtEndOfRound (((EndRound)_state).Player));				
		}
	}

	public IEnumerator RoundOver (Player lastPlayerToPlay)
	{
		if (PlayerOne.Score > PlayerTwo.Score) {
			PlayerOne.OnRoundWin ();
		} else {
			PlayerTwo.OnRoundWin ();
		}

		if (PlayerOne.RoundsWon >= 2) {
			SetState (new GameOver (PlayerOne));
		} 

		if (PlayerTwo.RoundsWon >= 2) {
			SetState (new GameOver (PlayerTwo));
		} 
		PlayerOne.HideUnitCards ();
		PlayerTwo.HideUnitCards ();

		yield return new WaitForSeconds (4);

		_terrainTiles.ForEach ((t) => t.ResetForNewRound ());

		PlayerOne.ResetScore ();
		PlayerTwo.ResetScore ();

		if (lastPlayerToPlay == PlayerOne) {
			SetState (new PlayerTurn (PlayerTwo));
		} else {
			SetState (new PlayerTurn (PlayerOne));
		}
	}

	public IEnumerator OnGameOver (Player winner)
	{
		_gameOverParent.SetActive (true);

		_gameOverText.enabled = false;

		if (winner == PlayerOne) {
			_gameOverText.text = "Player One is Victorious";
		} else {
			_gameOverText.text = "Player Two is Victorious";
		}

		_gameOverBackground.color = Color.clear;

		Tween tween = _gameOverBackground.DOColor (Color.white, 0.7f);

		yield return tween.WaitForCompletion ();

		_gameOverText.enabled = true;

		yield return new WaitForSeconds (5);

		SceneManager.LoadScene (1);
	}

	public void OnMouseInfoUpdated (string name, string strength, string description)
	{
		_infoName.text = name;
		_infoStrength.text = strength;
		_infoDescription.text = description;
	}
}

[System.Serializable]
public class GameState {

	public virtual void OnEnable(GameController controller) {

	}

	public virtual void OnUpdate(GameController controller) {

	}

	public virtual void OnDisable(GameController controller) {

	}
}

[System.Serializable]
public class DistributeTerrainCards : GameState {

	public override void OnEnable (GameController controller)
	{
		base.OnEnable (controller);

		controller.DistributeTerrainCards ();

		controller.AdvanceState ();
	}

	public override void OnUpdate (GameController controller)
	{
		base.OnUpdate (controller);
	}

	public override void OnDisable (GameController controller)
	{
		base.OnDisable (controller);
	}

}

[System.Serializable]
public class DistributeUnitCards : GameState {

	[SerializeField] private Player _player;

	public DistributeUnitCards (Player _player)
	{
		this._player = _player;
	}

	public override void OnEnable (GameController controller)
	{
		base.OnEnable (controller);

		controller.StartCoroutine (Distribute (controller));

	}

	public IEnumerator Distribute(GameController controller) {
		controller.FadeOutIn ();

		yield return new WaitForSeconds (0.3f);

		controller.SetUnitBackgroundActive (true, _player);

		yield return new WaitForSeconds (1f);

		controller.DistributeUnitCards (_player);

		controller.AdvanceState ();
	}

	public override void OnUpdate (GameController controller)
	{
		base.OnUpdate (controller);
	}

	public override void OnDisable (GameController controller)
	{
		base.OnDisable (controller);
	}
}

[System.Serializable]
public class ChooseUnitsState : GameState {

	private int _cardsRedrawn;
	private Player _player;

	public ChooseUnitsState (Player _player)
	{
		this._player = _player;
	}

	public override void OnEnable (GameController controller)
	{
		base.OnEnable (controller);

		_player.ShowTerrainCardColors ();
	}

	public override void OnUpdate (GameController controller)
	{
		base.OnUpdate (controller);

		if (_player.WereAnyUnitCardsClickedOn ()) {

			_cardsRedrawn++;

			UnitCardSelection clickedUnitCard = _player.GetUnitCardClickedOn ();

			controller.RedistributeUnitCard (clickedUnitCard, _player);

		}

		if (_cardsRedrawn >= 3) {

			controller.SetUnitBackgroundActive (false, _player);

			controller.AdvanceState ();

		}
	}

	public override void OnDisable (GameController controller)
	{
		base.OnDisable (controller);

		_player.SetBackgroundActive (false);

		_player.HideTerrainCardColors ();
	}

	public void UpdateUnitUI (UnitInfo unitInfo)
	{
		_player.UpdateUnitUI (unitInfo);
	}
}

[System.Serializable]
public class DistributeTilePoints : GameState 
{
	public override void OnEnable (GameController controller)
	{
		base.OnEnable (controller);

		controller.DistributePoints ();

		controller.AdvanceState ();
	}

	public override void OnUpdate (GameController controller)
	{
		base.OnUpdate (controller);
	}

	public override void OnDisable (GameController controller)
	{
		base.OnDisable (controller);
	}
}

[System.Serializable]
public class ChooseTerrain : GameState 
{
	private Player _player;

	public ChooseTerrain (Player _player)
	{
		this._player = _player;
	}

	public override void OnEnable (GameController controller)
	{
		base.OnEnable (controller);

		_player.EnableTerrainCardDragging ();
	}

	public override void OnUpdate (GameController controller)
	{
		base.OnUpdate (controller);
	}

	public override void OnDisable (GameController controller)
	{
		base.OnDisable (controller);

		_player.DisableTerrainCardDragging ();
	}

}

[System.Serializable]
public class PlayerTurn : GameState 
{
	private Player _player;
	private bool _showUnits;
	private bool _hideUnits;

	public Player Player {
		get { return _player; }
	}

	public PlayerTurn (Player player, bool showUnits = true, bool hideUnits = true)
	{
		this._player = player;
		_showUnits = showUnits;
		_hideUnits = hideUnits;
	}

	public override void OnEnable (GameController controller)
	{
		base.OnEnable (controller);

		if (_showUnits) {
			_player.ShowUnitCards ();
		}

		if (_player.IsOutOfCards ()) {
			controller.CompleteRound ();
		}
	}

	public override void OnUpdate (GameController controller)
	{
		base.OnUpdate (controller);
	}

	public void DisablePlacingCards () 
	{
		_player.Units.ToList().ForEach((t) => t.Disable());
	}

	public override void OnDisable (GameController controller)
	{
		base.OnDisable (controller);

		if (_hideUnits) {
			_player.HideUnitCards ();
		}
	}
}

[System.Serializable]
public class WaitState : GameState 
{
	private float _timeToWait;

	public WaitState (float _timeToWait)
	{
		this._timeToWait = _timeToWait;
	}

	public override void OnEnable (GameController controller)
	{
		base.OnEnable (controller);

		controller.StartCoroutine (WaitThenMoveOn (controller));
	}
		
	public IEnumerator WaitThenMoveOn(GameController controller) {
		yield return new WaitForSeconds (_timeToWait);
		controller.AdvanceState ();
	}

	public override void OnUpdate (GameController controller)
	{
		base.OnUpdate (controller);
	}

	public override void OnDisable (GameController controller)
	{
		base.OnDisable (controller);

		controller.StopAllCoroutines ();
	}
}


[System.Serializable]
public class EndRound : GameState 
{
	private Player _player;
	private bool _showUnits;

	public Player Player {
		get { return _player; }
	}

	public EndRound (Player player, bool showUnits = true)
	{
		this._player = player;
		_showUnits = showUnits;
	}

	public override void OnEnable (GameController controller)
	{
		base.OnEnable (controller);

		if (_showUnits) {
			_player.ShowUnitCards ();
		}
	}

	public void DisablePlacingCards () 
	{
		_player.Units.ToList().ForEach((t) => t.Disable());
	}

}

[System.Serializable]
public class TallyPointsAtEndOfRound : GameState 
{
	private Player _player;

	public TallyPointsAtEndOfRound (Player _player)
	{
		this._player = _player;
	}
		
	public override void OnEnable (GameController controller)
	{
		base.OnEnable (controller);

		controller.StartCoroutine(controller.RoundOver (_player));

	}

}

[System.Serializable]
public class GameOver : GameState 
{
	private Player _winner;

	public GameOver (Player _winner)
	{
		this._winner = _winner;
	}

	public override void OnEnable (GameController controller)
	{
		base.OnEnable (controller);

		controller.StartCoroutine(controller.OnGameOver (_winner));
	}

}

