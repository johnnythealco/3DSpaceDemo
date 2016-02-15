using System;
using System.Collections.Generic;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Base class for object that represents the game state. Knows about all pieces added to itself
	/// </summary>
	[Version(1)]
	public interface IGameState<TPoint, TPieceSettings> : IEnumerable<IPieceProperties>
		where TPieceSettings : PieceSettings<TPoint, TPieceSettings>
	{
		/// <summary>
		/// Call to reset our state at the start of a game
		/// </summary>
		void StartGame();


		/// <summary>
		/// Event fired when a piece is added to the game
		/// </summary>
		event Action<TPoint, IPieceProperties> OnPieceAdded;

		/// <summary>
		/// Event fired when a piece is removed from the game
		/// </summary>
		event Action<TPoint, IPieceProperties> OnPieceRemoved;

		/// <summary>
		/// Event fired when a piece is moved from one position to another
		/// </summary>
		event Action<TPoint, TPoint, IPieceProperties> OnPieceMoved;


		/// <summary>
		/// Move a piece from a given position to a destination
		/// </summary>
		/// <param name="initialPosition">The initial position for the piece</param>
		/// <param name="piece">The piece to move</param>
		/// <param name="destination">The destination position for the piece</param>
		/// <exception cref="InvalidOperationException"><paramref name="initialPosition"/> is empty or <paramref name="destination"/> is not empty</exception>
		void MoveGamePiece(TPoint initialPosition, IPieceProperties piece, TPoint destination);

		/// <summary>
		/// Remove a piece at the given position
		/// </summary>
		/// <param name="piece">The piece to remove</param>
		/// <param name="position">The position from which a piece will be removed</param>
		/// <exception cref="InvalidOperationException"><paramref name="position"/> is empty</exception>
		void RemoveGamePiece(TPoint position, IPieceProperties piece);

		/// <summary>
		/// Place a game piece on the board
		/// </summary>
		/// <param name="piece">The piece to add</param>
		/// <param name="position">The position on the grid to add the piece to</param>
		/// <exception cref="ArgumentNullException"><paramref name="piece"/> is null</exception>
		/// <exception cref="InvalidOperationException"><paramref name="position"/> is non-empty, or <paramref name="piece"/> is already on the board.</exception>
		void PlaceGamePiece(TPoint position, IPieceProperties piece);


		/// <summary>
		/// Create a move to add a piece to the game state
		/// </summary>
		/// <param name="piece">The piece to place</param>
		/// <param name="position">The position to add the piece</param>
		GameMove<TPoint, TPieceSettings> CreateAddPieceMove(IPieceProperties piece, TPoint position);


		/// <summary>
		/// Create a move to remove a piece from the game state
		/// </summary>
		/// <param name="piece">The piece to remove</param>
		/// <param name="position">The point from which to remove the piece</param>
		GameMove<TPoint, TPieceSettings> CreateRemovePieceMove(IPieceProperties piece, TPoint position);

		/// <summary>
		/// Create a move to capture a piece by moving another
		/// </summary>
		/// <param name="capturingPiece">The piece doing the capturing</param>
		/// <param name="capturedPiece">The piece to capture</param>
		/// <param name="capturePosition">Position of piece to capture</param>
		/// <param name="sourcePosition">Position to move <paramref name="capturingPiece"/> from</param>
		/// <param name="destinationPosition">Position to move <paramref name="capuringPiece"/> to</param>
		[Version(1, 1)]
		GameMove<TPoint, TPieceSettings> CreateCapturePieceMove(IPieceProperties capturingPiece, IPieceProperties capturedPiece,
			TPoint sourcePosition, TPoint destinationPosition, TPoint capturePosition);

		/// <summary>
		/// Create a move to remove a piece from a cell and add another in its place
		/// </summary>
		/// <param name="removedPiece">The piece to remove</param>
		/// <param name="addedPiece">The piece to add</param>
		/// <param name="position">The point from which to remove the piece</param>
		[Version(1, 1)]
		GameMove<TPoint, TPieceSettings> CreateReplacePieceMove(IPieceProperties removedPiece, IPieceProperties addedPiece, TPoint position);

		/// <summary>
		/// Create a move to move a piece around in the game state
		/// </summary>
		/// <param name="piece">The piece to move</param>
		/// <param name="sourcePosition">The point from which to remove the piece</param>
		/// <param name="destinationPosition">The point to add the piece</param>
		GameMove<TPoint, TPieceSettings> CreateMovePieceMove(IPieceProperties piece, TPoint sourcePosition, TPoint destinationPosition);


		/// <summary>
		/// Tests a position for a piece of any kind
		/// </summary>
		/// <param name="position">The position on the board to test</param>
		/// <returns>The first piece at the position, or null if it is empty</returns>
		IPieceProperties TestPosition(TPoint position);


		/// <summary>
		/// Tests a position for a piece of a specific ID
		/// </summary>
		/// <param name="position">The sposition on the board to test</param>
		/// <param name="pieceID">The piece ID to look for. If null, returns true for any piece</param>
		/// <returns>A piece with the id matching <paramref name="pieceID"/>, or null if none exist</returns>
		IPieceProperties TestPosition(TPoint position, string pieceID);


		/// <summary>
		/// Tests a position for a piece of a specific ID owned by a specific player
		/// </summary>
		/// <param name="position">The sposition on the board to test</param>
		/// <param name="pieceID">The piece ID to look for. If null, returns true for any piece</param>
		/// <param name="playerID">The plaer ID to look for. If null, returns true for any piece</param>
		/// <returns>TA piece with the id matching <paramref name="pieceID"/>
		/// and owned by a player matching <paramref name="playerID"/>, or null of none exist</returns>
		IPieceProperties TestPosition(TPoint position, string pieceID, string playerID);


		/// <summary>
		/// Returns a collection of all cells in this state
		/// </summary>
		IEnumerable<TPoint> GetAllPoints();


		/// <summary>
		/// Returns a collection of all pieces at a given cell
		/// </summary>
		ICollection<IPieceProperties> GetPiecesAtPoint(TPoint cell);


		/// <summary>
		/// Returns whether a position exists in this game state
		/// </summary>
		[Version(1, 1)]
		bool Contains(TPoint position);


		/// <summary>
		/// Returns all neighbours of a given point. They may or may not be part of the game state
		/// </summary>
		[Version(1, 1)]
		IEnumerable<TPoint> GetNeighboursOfPoint(TPoint point);


		/// <summary>
		/// Returns the location of a piece.
		/// </summary>
		/// <param name="piece">The piece to locate</param>
		/// <exception cref="InvalidOperationException">When the piece is not in the game state</exception>
		[Version(1, 1)]
		TPoint FindPiece(IPieceProperties piece);
	}
}
