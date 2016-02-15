using System;
using System.Linq;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Concrete class implementation for game rules on a grid
	/// </summary>
	[Version(1)]
	public class GridGameRules : GameRules<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		/// <summary>
		/// Configure directions in which lines can be made, for various piece-in-a-row conditions and tests.
		/// These should be relative direction vectors.
		/// </summary>
		public InspectableVectorPoint[] lineDirections;


		private RectPoint[] cachedLineDirections;
		#endregion


		#region Lines
		public override void Initialise(IGameManager<RectPoint, GridGamePieceSettings> game)
		{
			base.Initialise(game);
			
			cachedLineDirections = lineDirections.Select(t => t.GetRectPoint()).ToArray();
		}


		public override string HasLine(RectPoint rootPoint, int lineLength, String pieceTypeID, out RectPoint outPoint)
		{
			var state = game.State;
			outPoint = RectPoint.Zero;

			foreach (var rootPiece in state.GetPiecesAtPoint(rootPoint))
			{
				if (rootPiece != null &&
					(rootPiece.pieceID == pieceTypeID ||
					String.IsNullOrEmpty(pieceTypeID)))
				{
					// Check directions
					for (int dirIndex = 0; dirIndex < cachedLineDirections.Length; ++dirIndex)
					{
						bool fullLine = true;
						var currentPos = rootPoint;

						for (int i = 1; i < lineLength; ++i)
						{
							currentPos = GridAlgorithms.NextPointInLineUniform(currentPos, dirIndex, cachedLineDirections);
							if (state.Contains(currentPos))
							{
								fullLine = state.TestPosition(currentPos, pieceTypeID, rootPiece.playerID) != null;

								if (!fullLine)
								{
									break;
								}
							}
							else
							{
								fullLine = false;
								break;
							}
						}

						// A player has a line
						if (fullLine)
						{
							outPoint = cachedLineDirections[dirIndex];
							return rootPiece.playerID;
						}
					}
				}
			}
			return null;
		}


		public override bool CanMakeLine(RectPoint rootPoint, string playerID, int lineLength, string pieceTypeID, out RectPoint missingPosition)
		{
			return CanMakeLine(rootPoint, playerID, lineLength, pieceTypeID, true, out missingPosition);
		}


		/// <summary>
		/// Check whether, with one valid placement, a line can be made by a given player
		/// starting at a root point going through the given directions, of a specific piece ID.
		/// Also optionally confirms the validity of the move.
		/// </summary>
		/// <remarks>
		/// A line is a connected strip of pieces owned by the same player, of the same piece ID
		/// (when a piece ID is specified). When <paramref name="confirmValidity"/> is true, will
		/// also check for the validity of the move that would place a piece in the missing position.
		/// Note that if <see cref="GameRules{TPoint,TPieceSettings}.calculateMovesForAllPlayers"/> is set to false,
		/// only moves valid during the current turn will pass, so only pieces placed by
		/// the current player will be valid for this test.
		/// If you need to check for players other than the current player, make sure to set
		/// <see cref="GameRules{TPoint,TPieceSettings}.calculateMovesForAllPlayers"/> to true.
		/// </remarks>
		/// <param name="rootPoint">The point to start the line</param>
		/// <param name="playerID">The player ID whose lines we are checking for</param>
		/// <param name="lineLength">The required line length</param>
		/// <param name="pieceTypeID">The piece type ID to match. If null, will match any piece</param>
		/// <param name="missingPosition">The point that could be placed to create the line. Undefined if the return value is false</param>
		/// <param name="confirmValidity">Check whether a move is valid for this turn</param>
		/// <returns>True if only one placement is required to make a line</returns>
		public virtual bool CanMakeLine(RectPoint rootPoint, string playerID, int lineLength, string pieceTypeID, bool confirmValidity, out RectPoint missingPosition)
		{
			var state = game.State;
			missingPosition = RectPoint.Zero;

			var playerMoves = GetValidMovesListForPlayer(playerID);

			for (int dirIndex = 0; dirIndex < cachedLineDirections.Length; ++dirIndex)
			{
				int missingPieces = 0;
				var currentPos = rootPoint;

				for (int i = 0; i < lineLength; ++i)
				{
					if (i != 0) 
					{
						currentPos = GridAlgorithms.NextPointInLineUniform(currentPos, dirIndex, cachedLineDirections);
					}
					if (state.Contains(currentPos))
					{
						bool hasPiece = state.TestPosition(currentPos, pieceTypeID, playerID) != null;
						if (!hasPiece)
						{
							// Can we legally place a piece here?
							if (!confirmValidity || playerMoves.Any(t =>
							{
								AddPieceMove apm = t as AddPieceMove;
								return (apm != null &&
									apm.Position == currentPos &&
									apm.Piece.playerID == playerID &&
									(pieceTypeID == null ||
									apm.Piece.pieceID == pieceTypeID));
							}))
							{
								missingPosition = currentPos;
								missingPieces++;
							}
							else
							{
								missingPieces = int.MaxValue;
								break;
							}
						}
					}
					else
					{
						missingPieces = int.MaxValue;
						break;
					}
				}

				if (missingPieces == 1)
				{
					return true;
				}
			}

			return false;
		}
		#endregion


		#region Piece creation
		public override IPieceProperties CreatePieceProperties(GridGamePieceSettings sourcePiece, Player<RectPoint, GridGamePieceSettings> owner)
		{
			return CreatePieceProperties(sourcePiece, owner.PlayerID);
		}

		public override IPieceProperties CreatePieceProperties(GridGamePieceSettings sourcePiece, String ownerID)
		{
			return new GridGamePieceProperties(sourcePiece.pieceID, pieceCounter++, ownerID);
		}
		#endregion
	}
}
