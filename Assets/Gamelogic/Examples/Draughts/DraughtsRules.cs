using System.Collections.Generic;
using System.Linq;
using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.Examples
{
	/// <summary>
	/// Extension to ordinary rules for draughts specific rules.
	/// </summary>
	/// <remarks>
	/// Implements British draughts rules:
	///  - Obligation to capture
	///  - Pieces can only capture forwards
	///  - No flying kings
	/// </remarks>
	public class DraughtsRules : GridGameRules
	{
		// Implement obligation to capture
		protected override void UpdateMovesForPlayer(List<GameMove<RectPoint, GridGamePieceSettings>> movesList, Player<RectPoint, GridGamePieceSettings> player)
		{
			base.UpdateMovesForPlayer(movesList, player);

			// Check moves list for captures. If there are any, remove all other moves
			if (movesList.Any(t => t is CapturePieceMove))
			{
				movesList.RemoveAllBut(t => t is CapturePieceMove);
			}
		}

		protected override void OnMoveEnd(GameMove<RectPoint, GridGamePieceSettings> EndedMove)
		{
			base.OnMoveEnd(EndedMove);

			// If the piece that just did a capture has more capture options, then the turn does not end
			var moveList = GetValidMovesListForPlayer(game.TurnManager.CurrentPlayer);

			// Clear the list as a signal to OnAllMovesEnded
			moveList.Clear();

			// Any new capture moves to add?
			var captureMove = EndedMove as CapturePieceMove;
			if (captureMove != null && captureMove.Direction == MoveMode.Forward)
			{
				var pieceSettings = GetPieceSettings(captureMove.CapturingPiece);
				var gameState = game.State;

				foreach (var capture in pieceSettings.GetCaptures(gameState, captureMove.CapturingPiece, captureMove.DestinationPosition))
				{
					moveList.Add(gameState.CreateCapturePieceMove(captureMove.CapturingPiece, capture.piece, captureMove.DestinationPosition, capture.position,
						gameState.FindPiece(capture.piece)));
				}
			}

			// Crowing pieces
			// Red pieces crown at row 7, black at row 0
			IPieceProperties crownPiece = null;
			RectPoint crownPos = RectPoint.Zero;

			if (captureMove != null && captureMove.Direction == MoveMode.Forward)
			{
				if (captureMove.CapturingPiece.playerID == "red" &&
					captureMove.DestinationPosition.Y == 7)
				{
					crownPiece = captureMove.CapturingPiece;
					crownPos = captureMove.DestinationPosition;
				}
				else if (captureMove.CapturingPiece.playerID == "black" &&
					captureMove.DestinationPosition.Y == 0)
				{
					crownPiece = captureMove.CapturingPiece;
					crownPos = captureMove.DestinationPosition;
				}
			}
			var moveMove = EndedMove as MovePieceMove;
			if (moveMove != null && moveMove.Direction == MoveMode.Forward)
			{
				if (moveMove.Piece.playerID == "red" &&
					moveMove.Destination.Y == 7)
				{
					crownPiece = moveMove.Piece;
					crownPos = moveMove.Destination;
				}
				else if (moveMove.Piece.playerID == "black" &&
					moveMove.Destination.Y == 0)
				{
					crownPiece = moveMove.Piece;
					crownPos = moveMove.Destination;
				}
			}

			if (crownPiece != null)
			{
				var state = game.State;
				var crownMove = state.CreateReplacePieceMove(crownPiece,
					CreatePieceProperties(GetPieceSettings("king", crownPiece.playerID), crownPiece.playerID),
					crownPos);

				game.CommitMove(crownMove, false);
			}
		}

		protected override void OnAllMovesEnded()
		{
			base.OnAllMovesEnded();

			// If the moves list is empty, then we can advance the game
			// We take this beaviour away from the base by setting movesPerTurn to 0
			// so that it does not automatically advance after every move
			if (GetValidMovesListForPlayer(game.TurnManager.CurrentPlayer).IsEmpty())
			{
				game.TurnManager.AdvanceTurnState();
			}
		}
	}
}
