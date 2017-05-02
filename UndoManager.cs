using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameUndo {
	public interface IUndoableCommand  {
		void Do();
		void Undo();
	}

	public class UndoManager : MonoBehaviour {
	
		public Stack<IUndoableCommand> commandsDone = new Stack<IUndoableCommand>();

		#region static methods

		private static UndoManager _undoManager;

		public static UndoManager Instance {
			get{
				string name = "UndoManager (Dynamic-Singleton)";

				if (_undoManager == null) {

					if (GameObject.Find (name) != null) {
						_undoManager = GameObject.Find (name).GetComponent<UndoManager>();
					} else {
						var obj = new GameObject ();
						obj.name = name;
						_undoManager = obj.AddComponent<UndoManager> ();
					}
				} 

				return _undoManager;
			}
		}

		#endregion

		public void ExecuteCommand(IUndoableCommand command) {
			commandsDone.Push (command);
			command.Do ();
		}

		public void Undo() {
			if(commandsDone.Count > 0) {

				var command = commandsDone.Pop ();

				command.Undo ();

			}
		}
	}
}