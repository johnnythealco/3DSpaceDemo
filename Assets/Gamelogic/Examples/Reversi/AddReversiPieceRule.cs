using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Grids;
using Gamelogic.AbstractStrategy.Grids;

namespace Gamelogic.AbstractStrategy.Examples
{
	public class AddReversiPieceRule : PieceRule<RectPoint, GridGamePieceSettings>
	{
		public override IEnumerable<RectPoint> GetValidAddPositions(IGameState<RectPoint, GridGamePieceSettings> state)
		{
			var gridState = state as GridState;

			if (gridState == null)
			{
				throw new NotSupportedException("OpenSpaceMoveRule requires a GridState game state");
			}

			return GetValidPositions(state, gridState.gameManager.CurrentPlayer);
		}

		public static IEnumerable<RectPoint> GetValidPositions(IGameState<RectPoint, GridGamePieceSettings> state, Player<RectPoint, GridGamePieceSettings> player)
		{
			var gridState = state as GridState;

			if (gridState == null)
			{
				throw new NotSupportedException("OpenSpaceMoveRule requires a GridState game state");
			}

			var grid = gridState.gameGrid;
			var occupiedCellCount = grid.Count(p => grid[p].Count > 0);

			//Special rule for first 4 pieces automatically added:
			if (occupiedCellCount < 4)
			{
				return grid.WhereCell(c => c.Count == 0);
			}
			else
			{
				return grid.Where(p => IsValidAddPosition(grid, p, player.PlayerID)).ToArray();
			}
		}

		public override IEnumerable<RectPoint> GetValidDestinationPositions(IGameState<RectPoint, GridGamePieceSettings> state, IPieceProperties piece, RectPoint initialPosition)
		{
			return Enumerable.Empty<RectPoint>();
		}

		public override bool CanRemovePiece(IGameState<RectPoint, GridGamePieceSettings> state, IPieceProperties piece, RectPoint position)
		{
			return false;
		}

		public override RectPoint GetCapturePosition(IGameState<RectPoint, GridGamePieceSettings> state, IPieceProperties capturePiece, IPieceProperties capturedPiece, RectPoint initialPosition)
		{
			return default(RectPoint);
		}

		public override IEnumerable<IPieceProperties> GetValidCapturePieces(IGameState<RectPoint, GridGamePieceSettings> state, IPieceProperties piece, RectPoint initialPosition)
		{
			return Enumerable.Empty<IPieceProperties>();
		}

		public static bool IsValidAddPosition(RectGrid<GridGameCell> grid, RectPoint point, string playerID)
		{
			if (grid[point].Count > 0) return false; //there is already a piece

			foreach (var neighbor in grid.GetNeighbors(point)) //check neighboring cells
			{
				if (grid[neighbor].Count == 1)//it must ha have a piece, 
				{
					if (grid[neighbor][0].playerID != playerID)//an enemy piece
					{
						//check if line in direction of enemy piece can be formed containing only 
						//enemeny pieces except the last one, which should be current player piece

						var dir = neighbor - point;
						var currentPointInRay = neighbor;


						while (
							grid.Contains(currentPointInRay) && 
							grid[currentPointInRay].Count == 1 &&
							grid[currentPointInRay][0].playerID != playerID) //while we are in a line with enemy pieces
						{
							currentPointInRay += dir;
						}

						if (grid.Contains(currentPointInRay) && 
						    grid[currentPointInRay].Count > 0 &&
						    grid[currentPointInRay][0].playerID == playerID)//if line ends with current player piece
						{
							return true;
						}
					}
				}
			}

			return false;
		}
	}
}