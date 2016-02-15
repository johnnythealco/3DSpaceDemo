using System.Diagnostics;
using System.Linq;
using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Diagnostics;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.Examples
{
	public class ReversiVictoryRules : VictoryRules<RectPoint, GridGamePieceSettings>
	{
		protected override void CheckVictory()
		{
			var cells = ((GridState) game.State).gameGrid.Values;
			var boardFull = cells.All(c => c.Any());

			GLDebug.Log("CheckVictory");

			if (boardFull)
			{
				int player0Count = 0;
				int player1Count = 0;

				foreach (var cell in cells)
				{
					if (cell[0].playerID == Reversi.Player0ID)
					{
						player0Count++;
					}
					else
					{
						player1Count++;
					}
				}

				if (player0Count == player1Count)
				{
					game.GetPlayerByID(Reversi.Player0ID).SetVictoryState(VictoryType.Draw);
					game.GetPlayerByID(Reversi.Player1ID).SetVictoryState(VictoryType.Draw);
				}
				else
				{
					var winner = game.GetPlayerByID(player0Count > player1Count ? Reversi.Player0ID : Reversi.Player1ID);
			
					winner.SetVictoryState(VictoryType.Victory);
				}
			}
			else
			{
				bool canSomeoneMove = false;

				foreach (var player in game.Players)
				{
					var moves = AddReversiPieceRule.GetValidPositions(game.State, player);

					if (moves.Any())
					{
						canSomeoneMove = true;
						break;
					}
				}

				if (!canSomeoneMove)
				{
					game.GetPlayerByID(Reversi.Player0ID).SetVictoryState(VictoryType.Draw);
					game.GetPlayerByID(Reversi.Player1ID).SetVictoryState(VictoryType.Draw);
				}
			}
		}
	}
}