using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Grid specific implementation of <see cref="HumanPlayer{TGameState,TPoint,TPieceSettings}"/>
	/// </summary>
	[Version(1)]
	public class GridHumanPlayer : HumanPlayer<GridState, RectPoint, GridGamePieceSettings>
	{
		#region Constructor
		/// <summary>
		/// Create instance of a grid human player
		/// </summary>
		public GridHumanPlayer(string id) : base(id) { }
		#endregion
	}
}
