using UnityEngine;
using System.Collections;
using Gamelogic;
using Gamelogic.Grids;


public class SectorGridBuilder : GLMonoBehaviour
{
	public SectorCell cellPrefab;
	public int size = 12;
	public Vector2 padding;


	private FlatHexGrid<SectorCell> grid;
	private IMap3D<FlatHexPoint> map;




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

		grid = FlatHexGrid<SectorCell>.Hexagon (size);
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
		var gridDimensions = new Vector2 ((float)size * 2.1f, (float)size * 2.1f);

		gridDimensions.Scale ( cellPrefab.Dimensions);

		Vector3 coliderSize = new Vector3(gridDimensions.x, 0.5f, gridDimensions.y);

		this.GetComponent<BoxCollider> ().size = coliderSize;

	}
	
	    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;

			if (Physics.Raycast (ray, out hit))
			{
				Vector3 worldPosition = this.transform.InverseTransformPoint (hit.point);


				var point = map [worldPosition]; 
 
				if (grid.Contains (point))
				{
                
					grid [point].Color = Color.red;
					CameraController.camController.CentreOn (map [point]);

		


				}
			}
        }
    }


}
