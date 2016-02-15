using Gamelogic.Grids;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// A victory condition that ends the game when a player has no moves
	/// </summary>
	[Version(1, 1)]
	[AddComponentMenu("Gamelogic/Strategy/Victory Conditions/No Moves Condition")]
	[RequireComponent(typeof(GameRules<RectPoint, GridGamePieceSettings>))]
	public class VictoryNoMoves : VictoryRules<RectPoint, GridGamePieceSettings>
	{
		protected override void CheckVictory()
		{
			var currentPlayer = game.CurrentPlayer;
			var moves = game.Rules.GetValidMovesListForPlayer(currentPlayer);

			if (moves.IsEmpty())
			{
				currentPlayer.SetVictoryState(victoryKind);
			}
		}
	}
}
