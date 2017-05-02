using UnityEngine;
using System.Collections;

[CreateAssetMenuAttribute(menuName="Units/UnitInfo")]
public class UnitInfo : ScriptableObject 
{
	public Sprite CardSprite;
	public UnitCard UnityType;
	public int Strength;
	public int count;
	public string UnitName;
	[TextArea]
	public string UnitDescription;
	public GameObject Prefab;
	public VisitorEnum Visitor;
	public Tile.TileType OnlyPlaceOn = Tile.TileType.None;

	public IVisitor CreateIVisitorFromEnum() {
		switch (Visitor) {
		case VisitorEnum.Cross_1:
			return new CrossTileVisitor ();
			break;
		case VisitorEnum.Type_Water:
			return new TypeVisitor (Tile.TileType.Water, 3);
			break;
		case VisitorEnum.Type_Grass:
			return new TypeVisitor (Tile.TileType.Earth, 3);
			break;
		case VisitorEnum.Type_Rock:
			return new TypeVisitor (Tile.TileType.Rock, 3);
			break;
		case VisitorEnum.Type_Water_2:
			return new TypeVisitor (Tile.TileType.Water, 2);
			break;
		case VisitorEnum.Type_Rock_1:
			return new TypeVisitor (Tile.TileType.Rock, 1);
			break;
		case VisitorEnum.Change_Grass:
			return new ChangeTerrainVisitor (Tile.TileType.Earth);
			break;
		case VisitorEnum.Change_Rock:
			return new ChangeTerrainVisitor (Tile.TileType.Rock);
			break;
		case VisitorEnum.Change_Water:
			return new ChangeTerrainVisitor (Tile.TileType.Water);
			break;
		case VisitorEnum.Change_Void:
			return new ChangeSingleTerrainVisitor (Tile.TileType.Void);
			break;
		}

		return null;
	}

}
