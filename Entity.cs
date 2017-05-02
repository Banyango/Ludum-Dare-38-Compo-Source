using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour {

	[SerializeField] private Tile _tileOccupying;

	[Header("Attributes")]
	[SerializeField] private int _strength;

	public Tile TileOccupying {
		get { return _tileOccupying; }
	}

	public virtual void OnPlace(Tile tileBeingPlacedOn, Player playerPlacing) 
	{
		_tileOccupying = tileBeingPlacedOn;

		AddScore (tileBeingPlacedOn);
	}

	public virtual void AddScore(Tile tileBeingPlacedOn) {

	}

	public virtual void OnRemove() 
	{
		_tileOccupying = null;
	}
}

public interface IVisitable {
	void Visit(IVisitor visitor);
}

public interface IVisitor {
	void Accept (Tile tile);
	int AmountToAdd();
}

public enum VisitorEnum {
	None,
	Cross_1,
	Type_Grass,
	Type_Rock,
	Type_Water,
	Change_Grass,
	Change_Water,
	Change_Rock,
	Change_Void,
	Type_Water_2,
	Type_Rock_1,
}

public class ChangeTerrainVisitor : IVisitor {

	public Tile.TileType newTerrainType;

	private Tile.TileType _startingType;

	private List<Tile> visited = new List<Tile> ();

	public ChangeTerrainVisitor (Tile.TileType newTerrainType)
	{
		this.newTerrainType = newTerrainType;
	}

	public virtual void Accept (Tile tile)
	{
		
		if (visited.Contains (tile)) {
			return;
		}

		visited.Add (tile);

		if (tile.SouthWest != null && tile.TerrainType == tile.SouthWest.TerrainType) {
			tile.SouthWest.Visit (this);
		}

		if (tile.SouthEast != null && tile.TerrainType == tile.SouthEast.TerrainType) {
			tile.SouthEast.Visit (this);
		}

		if (tile.NorthEast != null && tile.TerrainType == tile.NorthEast.TerrainType) {
			tile.NorthEast.Visit (this);
		}

		if (tile.NorthWest != null && tile.TerrainType == tile.NorthWest.TerrainType) {
			tile.NorthWest.Visit (this);
		}
	
		if (tile.North != null && tile.TerrainType == tile.North.TerrainType) {
			tile.North.Visit (this);
		}

		if (tile.South != null && tile.TerrainType == tile.South.TerrainType) {
			tile.South.Visit (this);
		}
			
		DoOnVisit (tile);

	}

	public void DoOnVisit(Tile tile) {
		tile.TerrainType = newTerrainType;

		if (tile.Unit != null) {
			if (tile.Unit.Visitor is TypeVisitor) {
				if (((TypeVisitor)tile.Unit.Visitor).TerrainType != newTerrainType) {
					tile.Unit.Player.AddScore (-((TypeVisitor)tile.Unit.Visitor).AmountToAdd ());
					tile.AmountAdded = tile.AmountToAdd + tile.Unit.Info.Strength;
				} else {
					tile.Unit.Player.AddScore (((TypeVisitor)tile.Unit.Visitor).AmountToAdd ());
				}
			}

		}

		tile.Show ();
	}

	public int AmountToAdd() {
		return -1;
	}
}

public class ChangeSingleTerrainVisitor : IVisitor {

	public Tile.TileType newTerrainType;

	public ChangeSingleTerrainVisitor (Tile.TileType newTerrainType)
	{
		this.newTerrainType = newTerrainType;
	}

	public virtual void Accept (Tile tile)
	{
		DoOnVisit (tile);
	}

	public void DoOnVisit(Tile tile) {
		tile.TerrainType = newTerrainType;

		tile.Show ();

		tile.AmountToAdd = 0;
	}

	public int AmountToAdd() {
		return 0;
	}
}


public class CrossTileVisitor : IVisitor {

	private Player _player;
	private int _amountToAdd;

	public virtual void Accept (Tile tile)
	{
		_player = tile.Unit.Player;

		DoOnVisit (tile);

		if (tile.SouthWest != null) {
			DoOnVisit (tile.SouthWest);
		}

		if (tile.SouthEast != null) {
			DoOnVisit (tile.SouthEast);
		}

		if (tile.NorthEast != null) {
			DoOnVisit (tile.NorthEast);
		}

		if (tile.NorthWest != null) {
			DoOnVisit (tile.NorthWest);
		}
	}

	public void DoOnVisit(Tile tile) {
		if (tile.Unit != null && tile.Unit.Player == _player) {
			_amountToAdd++;
		}
	}

	public int AmountToAdd() {
		return _amountToAdd;
	}
}

public class TypeVisitor : IVisitor {

	private Tile.TileType _type;
	private int _amountToAdd;
	private int _amountAdded;

	public Tile.TileType TerrainType {
		get { return _type; }
	}

	public TypeVisitor(Tile.TileType type, int amount) {
		this._type = type;
		this._amountToAdd = amount;
	}

	public virtual void Accept (Tile tile)
	{		
		DoOnVisit (tile);
	}

	public void DoOnVisit(Tile tile) {
		if (tile.TerrainType == _type) {
			_amountAdded += _amountToAdd;
		}
	}
		
	public int AmountToAdd() {
		return _amountAdded;
	}
}
