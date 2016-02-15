using Gamelogic.Grids;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// A victory condition for when a given player places multiple pieces in a row
	/// </summary>
	[Version(1)]
	[AddComponentMenu("Gamelogic/Strategy/Victory Conditions/Pieces in a Row Condition")]
	[RequireComponent(typeof(GameRules<RectPoint, GridGamePieceSettings>))]
	public class VictoryInARow : VictoryRules<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		/// <summary>
		/// The number of pieces that need to be placed in a row
		/// </summary>
		public int numberOfPieces;
		/// <summary>
		/// The <see cref="IPieceProperties.pieceID"/> of 
		/// the piece that needs to be placed in a row for victory.
		/// </summary>
		/// <remarks>If blank, will match any piece ID.</remarks>
		public string pieceTypeID;
		#endregion


		#region Methods
		/// <summary>
		/// Checks for this victory condition
		/// </summary>
		protected override void CheckVictory()
		{
			// Loop through the grid, looking down and right for lines of pieces owned
			// by the same player, with the given piece ID.

			var state = game.State;

			foreach (var point in state.GetAllPoints())
			{
				RectPoint lineDir;
				string winningPlayer = game.Rules.HasLine(point, numberOfPieces, pieceTypeID, out lineDir);

				if (winningPlayer != null)
				{
					var player = game.TurnManager.GetPlayerByID(winningPlayer);
					player.SetVictoryState(victoryKind);
				}
			}
		}
		#endregion
	}
}
