using UnityEngine;
using System.Collections;
using Gamelogic.Grids;
using Gamelogic;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleManager : GridBehaviour<FlatHexPoint>
{
//	public TurnManager turn;
//	public Fleet PlayerFleet;
//	public Fleet EnemyFleet;
//

	public GameObject unitPrefab;

	public bool somethingSelected;
	public Unit unitSelected;
	private FlatHexPoint selectedPoint;

	public Dictionary<FlatHexPoint, float> validTargets;
	private Dictionary<FlatHexPoint, float> AvailableMoves;
	private FlatHexPoint selectedTarget;



	void OnAwake()
	{
		somethingSelected = false;


		validTargets = new  Dictionary<FlatHexPoint, float>(); 
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


				var point = Sector.Map[worldPosition]; 

				if (Sector.Grid.Contains (point))
				{

//					Sector.Grid[point].Color = Color.red;
					CameraController.camController.CentreOn (Sector.Map [point]);




				}
			}
		}
	}


	public void CreateUnitAtRandomPoint()
	{
		var point = Sector.Grid.SampleRandom<FlatHexPoint> (1);

		foreach(var p in point)
		{
			CreateUnit (p, unitPrefab);
		}


	}

private void CreateUnit (FlatHexPoint point, GameObject prefab)
	{
		//Create a new unit from the Prefab and Register it on the Grid
		GameObject newUnit = Instantiate (prefab, Sector.Map [point], Quaternion.identity) as GameObject;
		Unit unit = newUnit.GetComponent<Unit> ();
		Sector.Grid [point].unit = unit;

		Sector.Grid [point].contents = Cell.Contents.unit;	
		Sector.Grid [point].isAccessible = false;

		unit.position = Sector.Grid [point].name;



		CameraController.camController.CentreOn (Sector.Map [point]);

	}


}
