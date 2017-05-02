using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GameUndo;

namespace DragDrop {

	public interface IDropHandler {
		void HandleDrop (Draggable draggable);
		void HandleFailedDrop (Draggable draggable);
	}

	[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
	public class Draggable : MonoBehaviour {

		private bool _isDragging = false;
		private bool _isDropped = false;

		private Vector3 _draggablePivotOffset;

		[SerializeField] protected bool _canDrag = true;

		public LayerMask LayerToCheckDropAgainst;
		
		public Action OnDragDown;
		public Action OnDrag;
		public Action OnDrop;

		public void OnMouseDown() {
			if (!_canDrag)
				return;

			_isDragging = true;

			if(OnDragDown != null) {
				OnDragDown ();
			}			

			var mouseWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

			_draggablePivotOffset = transform.position - mouseWorldPoint;
		}
			

		public void OnMouseDrag() {
			if(OnDrag != null) {
				OnDrag ();
			}
		}

		public void OnMouseUp() {

			if (!_canDrag)
				return;
			
			_isDragging = false;
			_isDropped = true;
		}
				
		// Update is called once per frame
		public virtual void Update () {
			if (_isDragging) {
				var mouseWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

				transform.position = mouseWorldPoint + _draggablePivotOffset;
			}

			if (_isDropped) {
				Collider2D col = Physics2D.OverlapPoint(transform.position, LayerToCheckDropAgainst.value);

				if(col != null && col.gameObject != null) {			
					HandleDrop (col);

					if(OnDrop != null) {
						OnDrop ();
					}

				} else {
					HandleFailedDrop (col);
				}

					
				_isDropped = false;
			}
		}

		void HandleDrop (Collider2D col)
		{
			var dropHandler = (IDropHandler) col.gameObject.GetComponent(typeof(IDropHandler));

			if (dropHandler != null) {
				dropHandler.HandleDrop (this);
			} else {
				HandleFailedDrop (col);
			}
		}

		public virtual void HandleFailedDrop (Collider2D col) {
			
		}
	}
}
