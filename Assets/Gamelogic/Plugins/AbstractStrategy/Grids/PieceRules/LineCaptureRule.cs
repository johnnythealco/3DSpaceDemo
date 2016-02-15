using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Grids;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// How line moves can perform captures
	/// </summary>
	[Version(1, 1)]
	public enum LineCaptureType
	{
		/// <summary>
		/// We can capture the first piece along the line, if it matches our occupied filter
		/// </summary>
		FirstPiece,
		/// <summary>
		/// We can capture any piece that matches our occupied filter along the line
		/// </summary>
		AnyPiece
	}


	/// <summary>
	/// Rule for capturing pieces by moving into their space based on linear transformation from
	/// the piece's current position
	/// </summary>
	[Version(1, 1)]
	[AddComponentMenu("Gamelogic/Strategy/Piece Rules/Capture/Line Capture")]
	public class LineCaptureRule : PieceRule<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		/// <summary>
		/// Number of cells this piece can move
		/// </summary>
		public int moveLength;

		/// <summary>
		/// Collection of valid directions in which a piece may move from its current position
		/// </summary>
		public InspectableVectorPoint[] validDirections;

		/// <summary>
		/// How we can capture
		/// </summary>
		public LineCaptureType captureType;

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


		#region Properties
		private RectPoint[] _cachedDirs;
		public RectPoint[] CachedDirections
		{
			get
			{
				if (_cachedDirs == null)
				{
					_cachedDirs = validDirections.Select(t => t.GetRectPoint()).ToArray();
				}
				return _cachedDirs;
			}
		}
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
			for (int dirIndex = 0; dirIndex < CachedDirections.Length; ++dirIndex)
			{
				var currentPos = initialPosition;

				for (int i = 0; i < moveLength; ++i)
				{
					currentPos = GridAlgorithms.NextPointInLineUniform(currentPos, dirIndex, CachedDirections);
					if (state.Contains(currentPos))
					{
						var cell = state.GetPiecesAtPoint(currentPos);
						if (!cell.IsEmpty())
						{
							var target = state.TestPosition(currentPos, capturePieceID, capturePlayerID);
							if (target != null)
							{
								yield return target;
								if (captureType == LineCaptureType.FirstPiece)
									break;
							}
							else
							{
								// Piece that doesn't match our capture rules. This piece blocks movement
								if (captureType == LineCaptureType.FirstPiece)
									break;
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
		#endregion
	}
}
