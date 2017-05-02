using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedObject : MonoBehaviour {

	[SerializeField] private SpriteRenderer _spriteRenderer;

	[Header("Sprites")]
	[SerializeField] private Sprite _sprite1;
	[SerializeField] private Sprite _sprite2;
	[SerializeField] private Sprite _sprite3;

	public void ChangeToSpriteOne() {
		_spriteRenderer.sprite = _sprite1;
	}

	public void ChangeToSpriteTwo() {
		_spriteRenderer.sprite = _sprite2;
	}

	public void ChangeToSpriteThree() {
		_spriteRenderer.sprite = _sprite3;
	}
}
