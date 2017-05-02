using UnityEngine;
using System.Collections;
using DG.Tweening;
using DragDrop;

public class Tile : MonoBehaviour, IDropHandler, IVisitable {

	public enum TileType {
		None,
		Earth,
		Water,
		Sand,
		Rock,
		Void
	}

	public delegate void TerrainTilePlaced(Tile tile, TerrainCard terrain);
	public TerrainTilePlaced OnTerrianTilePlaced;

	public delegate void UnitPlaced(Unit unit, Tile tile);
	public UnitPlaced OnUnitPlaced;

	public delegate void DoneAddingPoints();
	public DoneAddingPoints OnDoneAddingPoints;

	public GameController.ObjectMouseInfo OnMouseInfoListener;

	[SerializeField] private Sprite _grassSprite;
	[SerializeField] private Sprite _waterSprite;
	[SerializeField] private Sprite _sandSprite;
	[SerializeField] private Sprite _rockSprite;
	[SerializeField] private Sprite _voidSprite;

	[SerializeField] private Sprite _rampPlayer1;
	[SerializeField] private Sprite _rampPlayer2;

	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private TileType _tileType = TileType.None;

	[Header("UI")]
	[SerializeField] private float _appearTime;

	[Header("Game")]
	[SerializeField] private int _captureAmount;
	[SerializeField] private int _amountAddedToScore;

	[Header("Navigation")]
	[SerializeField] private Tile _north;
	[SerializeField] private Tile _south;
	[SerializeField] private Tile _northEast;
	[SerializeField] private Tile _northWest;
	[SerializeField] private Tile _southEast;
	[SerializeField] private Tile _southWest;

	[Header("Points Sprites")]
	[SerializeField] private Sprite[] _pointSprites;
	[SerializeField] private float _pointScaleDuration;

	[Header("Unit")]
	[SerializeField] private Unit _unit;

	public Tile North {
		get {
			return this._north;
		}
		set {
			_north = value;
		}
	}

	public Tile South {
		get {
			return this._south;
		}
		set {
			_south = value;
		}
	}

	public Tile NorthEast {
		get {
			return this._northEast;
		}
		set {
			_northEast = value;
		}
	}

	public Tile NorthWest {
		get {
			return this._northWest;
		}
		set {
			_northWest = value;
		}
	}

	public Tile SouthEast {
		get {
			return this._southEast;
		}
		set {
			_southEast = value;
		}
	}

	public Tile SouthWest {
		get {
			return this._southWest;
		}
		set {
			_southWest = value;
		}
	}

	public int AmountToAdd {
		get { return _captureAmount; }
		set { _captureAmount = value; }
	}

	public Tile.TileType TerrainType {
		get { return _tileType; }
		set { _tileType = value; }
	}

	public Unit Unit { 
		get { return _unit; }
	}

	public int AmountAdded {
		set { _amountAddedToScore = value; }
	}

	public void OnMouseEnter() {
		_spriteRenderer.material.SetFloat ("_OutlineAmount", 1);
		_spriteRenderer.material.SetFloat ("_ReplacementPassThrough", 1);

		var amount = "";
		var moreInfo = "";
		if (_unit == null) {
			amount = _captureAmount + "";

			switch (_tileType) {
			case TileType.Earth:
				moreInfo = "Grass terrain type";
				break;
			case TileType.Water:
				moreInfo = "Water terrain type";
				break;			
			case TileType.Rock:
				moreInfo = "Rock terrain type";	

				break;
			}

		} else {
			amount = _amountAddedToScore + "";
			moreInfo = "value: " + _captureAmount + "\nunit: " + _unit.Info.name + "\n"+ "\npower: " + _unit.Info.Strength + "\n" + _unit.Info.UnitDescription; 
		}

		OnMouseInfoListener (_tileType.ToString(), amount.ToString(), moreInfo);
	}

	public void OnMouseExit() {
		_spriteRenderer.material.SetFloat ("_OutlineAmount", 0);

		if (_unit != null) {			
			_spriteRenderer.material.SetFloat ("_ReplacementPassThrough", 0);		
		}

		OnMouseInfoListener ("","","");
	}

	public void Start() {
		_spriteRenderer = GetComponent<SpriteRenderer> ();
		_spriteRenderer.enabled = false;
	}

	public void RandomizeTerrian ()
	{
		_tileType = TileType.Earth;

		Show ();

	}

	public void Show() {

		switch (_tileType) {
		case TileType.Earth:
			_spriteRenderer.sprite = _grassSprite;
			break;
		case TileType.Water:
			_spriteRenderer.sprite = _waterSprite;
			break;
		case TileType.Sand:
			_spriteRenderer.sprite = _sandSprite;
			break;
		case TileType.Rock:
			_spriteRenderer.sprite = _rockSprite;
			break;
		case TileType.Void:
			_spriteRenderer.sprite = _voidSprite;
			break;
		}

		_spriteRenderer.enabled = true;
		transform.localScale = Vector3.zero;

		transform.DOScale (new Vector3(1f, 1f, 1f), _appearTime).SetEase(Ease.OutElastic);

	}

	public void CreatePoints(int points) 
	{
		GameObject child = new GameObject ();

		child.name = "Points";
		child.transform.SetParent (transform, false);

		var spriteRenderer = child.AddComponent<SpriteRenderer> ();

		spriteRenderer.sprite = _pointSprites [points - 1];
		spriteRenderer.sortingOrder = 3;

		spriteRenderer.transform.localScale = new Vector2 (0, 1);
		spriteRenderer.transform.DOScale (Vector3.one, _pointScaleDuration).SetEase (Ease.OutCirc).SetDelay (GameController._delay);

		_captureAmount = points;
	}

	#region IDropHandler implementation

	public void HandleDrop (Draggable draggable)
	{
		if (draggable is TerrainCard) {

			if (_tileType != TileType.None) {
				draggable.HandleFailedDrop (null);
				return;
			}

			var terrainCard = (TerrainCard) draggable;

			_tileType = terrainCard.TerrainType;

			terrainCard.Hide ();

			Show ();

			OnTerrianTilePlaced (this, terrainCard);
		}

		if (draggable is Unit) {

			if (_unit != null) {				
				draggable.HandleFailedDrop (null);
				return;
			}

			if (((Unit)draggable).CanOnlyBePlacedOnCertainTiles () && !((Unit)draggable).CanBePlacedOnTileType (this)) {
				draggable.HandleFailedDrop (null);
				return;
			}

			var unit = (Unit) draggable;

			AddUnit (unit);

		}
	}

	public void HandleFailedDrop (Draggable draggable)
	{
		
	}

	#endregion

//	public void OnDrawGizmos() {
//		if (UnityEditor.Selection.activeGameObject != this.gameObject) return;
//
//		if (_north != null) {
//			Gizmos.color = Color.red;
//			Gizmos.DrawLine (transform.position, _north.transform.position);
//		}
//
//		if (_south != null) {
//			Gizmos.color = Color.blue;
//			Gizmos.DrawLine (transform.position, _south.transform.position);
//		}
//
//		if (_northEast != null) {
//			Gizmos.color = Color.yellow;
//			Gizmos.DrawLine (transform.position, _northEast.transform.position);
//		}
//
//		if (_northWest != null) {
//			Gizmos.color = Color.green;
//			Gizmos.DrawLine (transform.position, _northWest.transform.position);
//		}
//
//		if (_southEast != null) {
//			Gizmos.color = Color.gray;
//			Gizmos.DrawLine (transform.position, _southEast.transform.position);
//		}
//
//		if (_southWest != null) {
//			Gizmos.color = Color.white;
//			Gizmos.DrawLine (transform.position, _southWest.transform.position);
//		}
//	}

	#region IVisitable implementation

	public void Visit (IVisitor visitor)
	{
		visitor.Accept (this);	
	}

	#endregion

	public void AddUnit (Unit unit)
	{
		_unit = unit;

		_spriteRenderer.material.SetFloat ("_ReplacementPassThrough", 0f);
		_spriteRenderer.material.SetTexture ("_node_7325", unit.Player.ColorTexture);

		unit.PlaceUnitOnTile (this);

		StartCoroutine(AnimatePointsEarnedThenAdvaceTurn(unit));
	}

	public IEnumerator AnimatePointsEarnedThenAdvaceTurn(Unit unit) {

		OnUnitPlaced (unit, this);
		Tween tween = transform.GetChild (0).DOMove (unit.Player.ScoreText.transform.position, 1.3f);
		yield return tween.WaitForCompletion ();
		transform.GetChild (0).DOScale (Vector3.zero, 0.4f);
		// todo add modifiers here.
		var scoreToAdd = _captureAmount + unit.Info.Strength;

		if (unit.Visitor != null) {
			unit.Visitor.Accept (this);

			scoreToAdd += unit.Visitor.AmountToAdd ();
		}

		_amountAddedToScore = scoreToAdd;

		unit.Player.AddScore (scoreToAdd);

		OnDoneAddingPoints ();
	}

	public void ResetForNewRound ()
	{
		if (_unit != null && _unit.InstantiatedPrefab != null) {
			GameObject.Destroy (_unit.InstantiatedPrefab);
		}

		_amountAddedToScore = 0;

		_unit = null;

		var child = transform.GetChild (0).transform;

		child.localPosition = Vector3.zero;
		child.localScale = Vector3.one;

		_spriteRenderer.material.SetFloat ("_ReplacementPassThrough", 1);
	}

}

