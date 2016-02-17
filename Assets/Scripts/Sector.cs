using UnityEngine;
using System.Collections;
using Gamelogic;
using Gamelogic.Grids;


public class Sector : GLMonoBehaviour
{
	public Cell cellPrefab;
	public int size = 12;
	public Vector2 padding;


	public static FlatHexGrid<Cell> Grid{ get; set; }
	public static IMap3D<FlatHexPoint> Map{ get; set;}




	// Use this for initialization
	void Start () 
	{
		
		positionCollider ();

		BuilkdGrid ();



	}
	


	public void BuilkdGrid()
	{

		var spacing = cellPrefab.Dimensions;
		spacing.Scale (padding);

		Grid = FlatHexGrid<Cell>.Hexagon (size);
		Map = new FlatHexMap (spacing).AnchorCellMiddleCenter ().To3DXZ ();

		foreach(var point in Grid)
		{
			var cell = Instantiate (cellPrefab);
			Vector3 worldPoint = Map [point];
			cell.transform.parent = this.transform;
			cell.transform.localScale = Vector3.one;
			cell.transform.localPosition = worldPoint;

			cell.name = point.ToString ();
			Grid [point] = cell;

		}

	}

	public void positionCollider()
	{
		var gridDimensions = new Vector2 ((float)size * 2.1f, (float)size * 2.1f);

		gridDimensions.Scale ( cellPrefab.Dimensions);

		Vector3 coliderSize = new Vector3(gridDimensions.x, 0.5f, gridDimensions.y);

		this.GetComponent<BoxCollider> ().size = coliderSize;

	}
	



}
