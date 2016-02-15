using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Grids;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Rule for capturing pieces by hopping over them. The tile behind the target must be open.
	/// </summary>
	[Version(1, 1)]
	[AddComponentMenu("Gamelogic/Strategy/Piece Rules/Capture/Hop Capture")]
	public class HopCaptureRule : PieceRule<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		/// <summary>
		/// Collection of valid directions to target pieces.
		/// </summary>
		public InspectableVectorPoint[] validDirections;

		/// <summary>
		/// The ID of the piece occupying a cell we're allowed to move to, if <see cref="moveType"/> is <see cref="RelativeMoveType.Occupied"/>
		/// Always used to test captures. Set to blank to not filter.
		/// </summary>
		public string capturePieceID;
		/// <summary>
		/// The ID of the player whose pieces occupy the cell we're trying to move to, if <see cref="moveType"/> is <see cref="RelativeMoveType.Occupied"/>
		/// Always used to test captures. Set to blank to not filter.
		/// </summary>
		public string capturePlayerID;
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
			var targetPos = state.FindPiece(capturedPiece);

			// Reflect relative position to us around targetPiece
			return (targetPos - initialPosition) + targetPos;
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
							// CHeck behind that piece
							var behindPosition = translatedPosition.Translate(relativeDir.GetRectPoint());
							if (state.Contains(behindPosition) &&
								state.GetPiecesAtPoint(behindPosition).IsEmpty())
							{
								yield return target;
							}
						}
					}
				}
			}
		}


		public override IEnumerable<RectPoint> GetValidDestinationPositions(IGameState<RectPoint, GridGamePieceSettings> state, IPieceProperties piece, RectPoint initialPosition)
		{
			return Enumerable.Empty<RectPoint>();
		}
	}
}
