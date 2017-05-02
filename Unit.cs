using UnityEngine;
using System.Collections;
using DragDrop;
using DG.Tweening;

[RequireComponent(typeof(BoxCollider2D))]
public class Unit : Draggable {

	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private BoxCollider2D _collider;

	[SerializeField] private Player _player;

	[SerializeField] private UnitInfo _info;

	[SerializeField] private GameObject _instantiatedPrefab;
	[SerializeField] private Sprite _cardSprite;

	[SerializeField] private IVisitor _visitor;

	public GameController.ObjectMouseInfo OnMouseInfoListener;

	private Vector3 _startPosition;
	private bool _placed;

	public UnitInfo Info {

		get{ return _info; }

		set { 
			_info = value; 

			_visitor = _info.CreateIVisitorFromEnum ();

			_spriteRenderer.sprite = value.CardSprite;
		}

	}

	public IVisitor Visitor {
		get { return _visitor; }
	}

	public Player Player {
		get { return _player; }
		set { _player = value; }
	}

	public GameObject InstantiatedPrefab {
		get { return _instantiatedPrefab; }
	}

	public void Start() {

		_spriteRenderer = GetComponent<SpriteRenderer> ();
		_collider = GetComponent<BoxCollider2D> ();

	}

	public void SetInitialPosition() {
		_startPosition = transform.localPosition;
	}

	public void Show ()
	{
		_canDrag = true;
		_spriteRenderer.sprite = Info.CardSprite;
	}

	public void Disable() {
		_canDrag = false;
	}

	public void Hide ()
	{
		_spriteRenderer.sprite = _cardSprite;
	}

	public void OnMouseEnter() {		
		OnMouseInfoListener (Info.UnitName, Info.Strength.ToString(), Info.UnitDescription);
	}

	public void OnMouseExit() {		
		OnMouseInfoListener ("","","");
	}
			
	public void TweenIntoPosition (Vector3 offset)
	{
		GameController._delay += 0.05f;

		transform.DOLocalMove (_startPosition, .8f)
			.SetDelay (GameController._delay)
			.SetEase (Ease.OutCirc);

		transform.localPosition += offset;
	}

	public void TweenOutOfPosition (Vector3 offset)
	{
		if (!_placed) {
			GameController._delay += 0.05f;
			
			transform.DOLocalMove (transform.localPosition - offset, .8f)
				.SetDelay (GameController._delay)
				.SetEase (Ease.OutCirc);
		}		
	}

	public void PlaceUnitOnTile(Tile tile) {
		_instantiatedPrefab = (GameObject) GameObject.Instantiate (Info.Prefab, tile.transform.position, Quaternion.identity);

		transform.DOScale (Vector3.zero, 0.5f).OnComplete(()=> _spriteRenderer.enabled = false).SetEase(Ease.InElastic);

		_collider.enabled = false;

		_placed = true;
	}

	public override void HandleFailedDrop (Collider2D col)
	{
		transform.DOLocalMove (_startPosition, .4f).SetEase (Ease.OutCirc);
	}

	public bool IsPlaced ()
	{
		return _placed;
	}

	public bool CanOnlyBePlacedOnCertainTiles ()
	{
		return _info.OnlyPlaceOn != Tile.TileType.None;
	}

	public bool CanBePlacedOnTileType (Tile tile)
	{
		return tile.TerrainType == _info.OnlyPlaceOn;
	}
}
