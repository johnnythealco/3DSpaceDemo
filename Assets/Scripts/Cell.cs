using UnityEngine;
using System.Collections;
using Gamelogic.Grids;
using Gamelogic;


public class Cell : SpriteCell
{
	public Unit unit;
	public bool isAccessible = true;
	public float Cost = 1;
	public Color[] colors; 
	public Contents contents;






	public enum Contents{empty, unit}

}
