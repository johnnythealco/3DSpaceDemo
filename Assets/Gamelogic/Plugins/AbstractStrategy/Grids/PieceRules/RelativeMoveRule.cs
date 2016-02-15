using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Grids;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// The movement modes
	/// </summary>
	[Version(1)]
	public enum RelativeMoveType
	{
		/// <summary>
		/// Can move to any cell
		/// </summary>
		Any,
		/// <summary>
		/// Can only move to empty cells
		/// </summary>
		Empty,
		/// <summary>
		/// Can only move to occupied cells
		/// </summary>
		Occupied
	}

	/// <summary>
	/// Rule for pieces that have movement options that picks destinations entirely based on some
	/// relative transformation from their current position.
	/// </summary>
	[Version(1)]
	[AddComponentMenu("Gamelogic/Strategy/Piece Rules/Move/Relative Move")]
	public class RelativeMoveRule : PieceRule<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		/// <summary>
		/// Collection of valid relative points in which a piece may move from its current position
		/// </summary>
		public InspectableVectorPoint[] validDirections;

		/// <summary>
		/// Whether we can only move to open spaces
		/// </summary>
		public RelativeMoveType moveType;

		/// <summary>
		/// The ID of the piece occupying a cell we're allowed to move to, if <see cref="moveType"/> is <see cref="RelativeMoveType.Occupied"/>
		/// Set to blank to not filter.
		/// </summary>
		public string occupiedPieceID;
		/// <summary>
		/// The ID of the player whose pieces occupy the cell we're trying to move to, if <see cref="moveType"/> is <see cref="RelativeMoveType.Occupied"/>
		/// Set to blank to not filter.
		/// </summary>
		public string occupiedPlayerID;
		#endregion


		public override bool CanRemovePiece(IGameState<RectPoint, GridGamePieceSettings> state, IPieceProperties piece, RectPoint position)
		{
			return false;
		}


		public override IEnumerable<RectPoint> GetValidAddPositions(IGameState<RectPoint, GridGamePieceSettings> state)
		{
			return Enumerable.Empty<RectPoint>();
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
			foreach (var relativeDir in validDirections)
			{
				var translatedPosition = initialPosition.Translate(relativeDir.GetRectPoint());

				if (state.Contains(translatedPosition))
				{
					var cell = state.GetPiecesAtPoint(translatedPosition);

					switch (moveType)
					{
						case RelativeMoveType.Any:
							yield return translatedPosition;
							break;
						case RelativeMoveType.Empty:
							if (cell.IsEmpty())
							{
								yield return translatedPosition;
							}
							break;
						case RelativeMoveType.Occupied:
							if (state.TestPosition(translatedPosition, occupiedPieceID, occupiedPlayerID) != null)
							{
								yield return translatedPosition;
							}
							break;
					}
				}
			}
		}
	}
}
