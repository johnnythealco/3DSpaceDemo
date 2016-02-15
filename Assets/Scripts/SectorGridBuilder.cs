using UnityEngine;
using System.Collections;
using Gamelogic;
using Gamelogic.Grids;
using Gamelogic.Grids.Examples;

public class SectorGridBuilder : GLMonoBehaviour
{
	public SectorCell cellPrefab;
	public int width = 12;
	public int height = 8;
	public Vector2 padding;

	private FlatHexGrid<SectorCell> grid;
	private IMap3D<FlatHexPoint> map;




	// Use this for initialization
	void Start () 
	{
		
		positionCollider ();

		BuilkdGrid ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void BuilkdGrid()
	{

		var spacing = cellPrefab.Dimensions;
		spacing.Scale (padding);

		grid = FlatHexGrid<SectorCell>.FatRectangle (width, height);
		map = new FlatHexMap (spacing).AnchorCellMiddleCenter ().To3DXZ ();

		foreach(var point in grid)
		{
			var cell = Instantiate (cellPrefab);
			Vector3 worldPoint = map [point];
			cell.transform.parent = this.transform;
			cell.transform.localScale = Vector3.one;
			cell.transform.localPosition = worldPoint;

			cell.name = point.ToString ();
			grid [point] = cell;

		}

	}

	public void positionCollider()
	{
		var gridDimensions = new Vector2 (width, (height + Mathi.Div(width, 2)));
		gridDimensions.Scale ( cellPrefab.Dimensions);

		Vector3 coliderSize = new Vector3(gridDimensions.x, 0.5f, gridDimensions.y);
		Vector3 coliderCentre = new Vector3  ((float)width - cellPrefab.Dimensions.x , 0 , (float)height - cellPrefab.Dimensions.y );

		this.GetComponent<BoxCollider> ().size = coliderSize;
		this.GetComponent<BoxCollider> ().center = coliderCentre;
	}
}
