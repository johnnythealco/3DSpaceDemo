using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Grids;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Rule for pieces that have movement options that cause them to move some number of spaces
	/// in given directions, optionally blocked by other pieces.
	/// </summary>
	[Version(1, 1)]
	[AddComponentMenu("Gamelogic/Strategy/Piece Rules/Move/Line Move")]
	public class LineMoveRule : PieceRule<RectPoint, GridGamePieceSettings>
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
			return default(RectPoint);
		}


		public override IEnumerable<IPieceProperties> GetValidCapturePieces(IGameState<RectPoint, GridGamePieceSettings> state, IPieceProperties piece, RectPoint initialPosition)
		{
			return Enumerable.Empty<IPieceProperties>();
		}


		public override IEnumerable<RectPoint> GetValidDestinationPositions(IGameState<RectPoint, GridGamePieceSettings> state, IPieceProperties piece, RectPoint initialPosition)
		{
			for (int dirIndex = 0; dirIndex < CachedDirections.Length; ++dirIndex)
			{
				var currentPos = initialPosition;
				bool lineBlocked = false;

				for (int i = 0; i < moveLength; ++i)
				{
					if (lineBlocked) break;

					currentPos = GridAlgorithms.NextPointInLineUniform(currentPos, dirIndex, CachedDirections);
					if (state.Contains(currentPos))
					{
						var cell = state.GetPiecesAtPoint(currentPos);
						switch (moveType)
						{
							case RelativeMoveType.Any:
								yield return currentPos;
								break;
							case RelativeMoveType.Empty:
								if (cell.IsEmpty())
								{
									yield return currentPos;
								}
								else
								{
									lineBlocked = true;
								}
								break;
							case RelativeMoveType.Occupied:
								if (state.TestPosition(currentPos, occupiedPieceID, occupiedPlayerID) != null)
								{
									yield return currentPos;
								}
								else
								{
									lineBlocked = true;
								}
								break;
						}
					}
				}
			}
		}
		#endregion
	}
}
