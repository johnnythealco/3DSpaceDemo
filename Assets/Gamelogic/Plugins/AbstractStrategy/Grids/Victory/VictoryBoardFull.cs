using Gamelogic.Grids;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// A victory condition that ends the game when the board becomes full.
	/// </summary>
	[Version(1)]
	[AddComponentMenu("Gamelogic/Strategy/Victory Conditions/Full Board Condition")]
	[RequireComponent(typeof(GameRules<RectPoint, GridGamePieceSettings>))]
	public class VictoryBoardFull : VictoryRules<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		/// <summary>
		/// Set to true to apply the victory state to all players rather than just the current active player
		/// </summary>
		public bool allPlayers;
		#endregion


		protected override void CheckVictory()
		{
			bool allOccupied = true;
			foreach (var point in game.State.GetAllPoints())
			{
				if (game.State.GetPiecesAtPoint(point).IsEmpty())
				{
					allOccupied = false;
					break;
				}
			}

			if (allOccupied)
			{
				if (allPlayers)
				{
					// Set victory state for all players
					foreach (var player in game.TurnManager.Players)
					{
						player.SetVictoryState(victoryKind);
					}
				}
				else
				{
					// Set for current player only
					game.CurrentPlayer.SetVictoryState(victoryKind);
				}
			}
		}
	}
}
