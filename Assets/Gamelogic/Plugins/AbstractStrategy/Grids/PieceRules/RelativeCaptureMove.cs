using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Grids;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Rule for capturing pieces by moving into their space based on relative transformation from
	/// the piece's current position
	/// </summary>
	[Version(1, 1)]
	[AddComponentMenu("Gamelogic/Strategy/Piece Rules/Capture/Relative Capture")]
	public class RelativeCaptureMove : PieceRule<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		/// <summary>
		/// Collection of valid relative points in which a piece may move from its current position
		/// </summary>
		public InspectableVectorPoint[] validDirections;

		/// <summary>
		/// The ID of the piece occupying a cell we're allowed to capture. Set to blank to not filter.
		/// </summary>
		public string capturePieceID;

		/// <summary>
		/// The ID of the player whose pieces occupy the cell we're trying to capture
		/// Always used to test captures. Set to blank to not filter.
		/// </summary>
		public string capturePlayerID;
		#endregion


		#region Methods
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
			return state.FindPiece(capturedPiece);
		}


		public override IEnumerable<IPieceProperties> GetValidCapturePieces(IGameState<RectPoint, GridGamePieceSettings> state, IPieceProperties piece, RectPoint initialPosition)
		{
			foreach (var relativeDir in validDirections)
			{
				var translatedPosition = initialPosition.Translate(relativeDir.GetRectPoint());

				if (state.Contains(translatedPosition))
				{
					var cell = state.GetPiecesAtPoint(translatedPosition);
					if (!cell.IsEmpty())
					{
						var target = state.TestPosition(translatedPosition, capturePieceID, capturePlayerID);
						if (target != null)
						{
							yield return target;
						}
					}
				}
			}
		}


		public override IEnumerable<RectPoint> GetValidDestinationPositions(IGameState<RectPoint, GridGamePieceSettings> state, IPieceProperties piece, RectPoint initialPosition)
		{
			return Enumerable.Empty<RectPoint>();
		}
		#endregion
	}
}
