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



	public bool somethingSelected;
	public Unit unitSelected;
	private FlatHexPoint selectedPoint;

	public Dictionary<FlatHexPoint, float> validTargets;
	private Dictionary<FlatHexPoint, float> AvailableMoves;
	private FlatHexPoint selectedTarget;

	private IGrid<SectorCell, FlatHexPoint> grid;

	void OnAwake()
	{
		somethingSelected = false;


		validTargets = new  Dictionary<FlatHexPoint, float>(); 
	}

	override public void InitGrid()
	{
		grid = Grid.CastValues<SectorCell, FlatHexPoint> ();

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


				var point = Map [worldPosition]; 

				if (grid.Contains (point))
				{

					grid [point].Color = Color.red;
					CameraController.camController.CentreOn (Map [point]);




				}
			}
		}
	}
}
