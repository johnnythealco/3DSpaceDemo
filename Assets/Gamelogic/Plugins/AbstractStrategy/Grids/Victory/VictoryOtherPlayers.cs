using Gamelogic.Grids;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Victory state that changes a player's victory state based on the victory state
	/// of other players. Often used to make players lose when other players win.
	/// </summary>
	[Version(1)]
	[AddComponentMenu("Gamelogic/Strategy/Victory Conditions/Other Players Condition")]
	public class VictoryOtherPlayers : VictoryRules<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		public VictoryType targetVictoryType;
		#endregion


		#region Methods
		protected override void CheckVictory()
		{
			bool playerHasMetVictory = false;

			// Look for player who has met the condition
			foreach (var player in game.TurnManager.Players)
			{
				if (player.VictoryState == targetVictoryType)
				{
					playerHasMetVictory = true;
					break;
				}
			}

			if (playerHasMetVictory)
			{
				foreach (var player in game.TurnManager.Players)
				{
					player.SetVictoryState(victoryKind);
				}
			}
		}
		#endregion
	}
}
