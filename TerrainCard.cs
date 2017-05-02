using UnityEngine;
using System.Collections;
using DragDrop;
using DG.Tweening;

public class TerrainCard : Draggable {

	[Header("Sprites")]
	[SerializeField] private Sprite _grassCard;
	[SerializeField] private Sprite _sandCard;
	[SerializeField] private Sprite _rockCard;
	[SerializeField] private Sprite _waterCard;

	[Header("attributes")]
	[SerializeField] private Tile.TileType _type;
	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private Collider2D _collider;

	[SerializeField] private Sprite _ramp;

	public GameController.ObjectMouseInfo OnMouseInfoListener;

	public float _rampAmount = 1;
	private Vector3 _startPosition;

	public void Awake() {

		_spriteRenderer = GetComponent<SpriteRenderer> ();
		_collider = GetComponent<Collider2D> ();

		SetSpriteFromType ();
			
		_spriteRenderer.material.SetTexture ("_node_7325", _ramp.texture);

		_startPosition = transform.position;
	}

	public override void Update() {
		base.Update ();

		_spriteRenderer.material.SetFloat ("_ReplacementPassThrough", _rampAmount);
	}

	public void OnMouseEnter() {

		var moreInfo = "";

		switch (_type) {
		case Tile.TileType.Earth:
			moreInfo = "Grass terrain type";
			break;
		case Tile.TileType.Water:
			moreInfo = "Water terrain type";
			break;			
		case Tile.TileType.Rock:
			moreInfo = "Rock terrain type";				
			break;
		}
			
		if (OnMouseInfoListener != null) {
			if (_rampAmount > 0.4f) {
				OnMouseInfoListener (_type.ToString (), "", moreInfo);
			} else {
				OnMouseInfoListener ("", "", "");
			}
		}
	}

	public void OnMouseExit() {
		if (OnMouseInfoListener != null) {
			OnMouseInfoListener ("", "", "");
		}
	}

	public Tile.TileType TerrainType {
		get { return _type; }
	}

	public void RandomizeType()
	{
		var types = new Tile.TileType[]{ Tile.TileType.Earth, Tile.TileType.Rock, Tile.TileType.Water };

		_type = types [Random.Range (0, 3)];
	}

	void SetSpriteFromType ()
	{
		switch (_type) {
		case Tile.TileType.Earth:
			_spriteRenderer.sprite = _grassCard;
			break;
		case Tile.TileType.Water:
			_spriteRenderer.sprite = _waterCard;
			break;
		case Tile.TileType.Sand:
			_spriteRenderer.sprite = _sandCard;
			break;
		case Tile.TileType.Rock:
			_spriteRenderer.sprite = _rockCard;
			break;
		}
	}

	public void Hide ()
	{
		_spriteRenderer.enabled = false;
		_collider.enabled = false;
		_canDrag = false;
	}

	public void Disable ()
	{		
		_canDrag = false;
	}

	public void Enable () 
	{		
		_canDrag = true;
	}

	public void DisableColor() {
		DOTween.To (() => _rampAmount, (v) => _rampAmount = v, 0, 0.5f);
	}

	public void EnableColor() {
		DOTween.To (() => _rampAmount, (v) => _rampAmount = v, 1, 0.5f);
	}

	public void TweenIntoPosition (Vector3 offset, float delay)
	{		
		SetSpriteFromType ();

		_spriteRenderer.enabled = true;

		transform.DOMove (transform.position, .4f)
			.SetDelay (delay)
			.SetEase (Ease.OutCirc);
		transform.localPosition += offset;
	}

	public override void HandleFailedDrop (Collider2D col)
	{
		transform.DOMove (_startPosition, .4f).SetEase (Ease.OutCirc);
	}
}
