using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Grids;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Rule for pieces that can be moved from their current position to any open space on the grid
	/// </summary>
	[Version(1)]
	[AddComponentMenu("Gamelogic/Strategy/Piece Rules/Move/Any Open Space")]
	public class OpenSpaceMoveRule : PieceRule<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		/// <summary>
		/// Whether or not we can only move a piece adjacent to some piece
		/// </summary>
		public bool adjacentOnly;
		/// <summary>
		/// The ID of the piece we can be adjacent to, if <see cref="adjacentOnly"/> is true
		/// Set to blank to not filter.
		/// </summary>
		public string adjacentPieceID;
		/// <summary>
		/// The ID of the player whose pieces we can be adjacent to, if <see cref="adjacentOnly"/> is true.
		/// Set to blank to not filter.
		/// </summary>
		public string adjacentPlayerID;
		#endregion


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


		public override IEnumerable<RectPoint> GetValidDestinationPositions(IGameState<RectPoint, GridGamePieceSettings> state, IPieceProperties piece, RectPoint initialPosition)
		{
			// Search grid for all unoccupied cells
			foreach (var point in state.GetAllPoints())
			{
				if (point == initialPosition) continue;

				var cell = state.GetPiecesAtPoint(point);
				if (cell.IsEmpty())
				{
					if (!adjacentOnly)
					{
						yield return point;
					}
					else
					{
						// Check adjacent squares
						bool filterPassed = false;

						foreach (var neighbour in state.GetNeighboursOfPoint(point))
						{
							if (state.Contains(neighbour))
							{
								filterPassed = state.TestPosition(neighbour, adjacentPieceID, adjacentPlayerID) != null;
								if (filterPassed)
								{
									break;
								}
							}
						}

						if (filterPassed)
						{
							yield return point;
						}
					}
				}
			}
		}


		public override IEnumerable<RectPoint> GetValidAddPositions(IGameState<RectPoint, GridGamePieceSettings> state)
		{
			return Enumerable.Empty<RectPoint>();
		}
	}
}
