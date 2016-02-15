using System.Collections.Generic;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// A cell in the logical game grid that can contain pieces
	/// </summary>
	[Version(1)]
	public class GridGameCell : List<IPieceProperties>
	{
		#region Methods
		/// <summary>
		/// Finds the index of the item in this cell, by its unique ID
		/// </summary>
		public int IndexOf(uint uniqueID)
		{
			return this.FindIndex(t => t.uniqueID == uniqueID);
		}
		#endregion
	}
}
