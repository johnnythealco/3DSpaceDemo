using System;
using System.Collections.Generic;
using System.Linq;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Rules concerning placement for a given piece
	/// </summary>
	[Flags]
	[Version(1)]
	public enum PiecePlacementRules
	{
		/// <summary>
		/// Piece cannot be placed on the board by a player
		/// </summary>
		None = 0x0,
		/// <summary>
		/// Any free space on the board
		/// </summary>
		OpenSpace = 0x1,
		/// <summary>
		/// Any occupied space on the board
		/// </summary>
		OccupiedSpace = 0x2,
		/// <summary>
		/// Any space on the board occupied by pieces owned by the same player
		/// </summary>
		OccupiedFriendlySpace = 0x4,
		/// <summary>
		/// Any space on the baord occupied by pieces owned by a different player
		/// </summary>
		OccupiedEnemySpace = 0x8,
		/// <summary>
		/// Any free space adjacent to another piece
		/// </summary>
		AdjacentOpenSpace = 0x10,
		/// <summary>
		/// Any free space adjacent to another piece owned by the same player
		/// </summary>
		AdjacentFriendlySpace = 0x20,
		/// <summary>
		/// Any free space adjacent to another piece owned by a different player
		/// </summary>
		AdjacentEnemySpace = 0x40,
	}


	/// <summary>
	/// Rules concerning movement for a given piece
	/// </summary>
	[Version(1)]
	public enum PieceMovementRules
	{
		/// <summary>
		/// Piece cannot move once placed
		/// </summary>
		None,
	}

	/// <summary>
	/// Different states a game can be in
	/// </summary>
	[Version(1)]
	public enum GameState
	{
		/// <summary>
		/// The game has not yet started
		/// </summary>
		NotStarted,
		/// <summary>
		/// The game has started
		/// </summary>
		Started,
		/// <summary>
		/// The game is over
		/// </summary>
		Ended,
	}


	/// <summary>
	/// Interface for object which provides game rules
	/// </summary>
	[Version(1, 1)]
	public interface IGameRules<TPoint, TPieceSettings>
		where TPieceSettings : PieceSettings<TPoint, TPieceSettings>
	{
		/// <summary>
		/// Gets the number of moves that have been made this turn.
		/// </summary>
		int MovesMadeThisTurn { get; }


		/// <summary>
		/// Gets the valid moves list for a given player for this turn
		/// </summary>
		/// <param name="player">The player to query</param>
		List<GameMove<TPoint, TPieceSettings>> GetValidMovesListForPlayer(Player<TPoint, TPieceSettings> player);


			/// <summary>
		/// Gets the valid moves list for a given player for this turn
		/// </summary>
		/// <param name="playerId">The id of the player to query</param>
		List<GameMove<TPoint, TPieceSettings>> GetValidMovesListForPlayer(string playerId);


			/// <summary>
		/// Validates a given move by comparing it to all known acceptable moves
		/// </summary>
		/// <param name="Move"></param>
		bool IsValidMove(GameMove<TPoint, TPieceSettings> Move);


		/// <summary>
		/// Create the properties for a given source piece, owned by a given player
		/// </summary>
		IPieceProperties CreatePieceProperties(TPieceSettings sourcePiece, Player<TPoint, TPieceSettings> owner);


		/// <summary>
		/// Create the properties for a given source piece, owned by a given player
		/// </summary>
		IPieceProperties CreatePieceProperties(TPieceSettings sourcePiece, string ownerID);


		/// <summary>
		/// Gets the creation properties for a given piece
		/// </summary>
		TPieceSettings GetPieceSettings(IPieceProperties piece);


		/// <summary>
		/// Gets the creation properties for a given piece
		/// </summary>
		TPieceSettings GetPieceSettings(String pieceID, String playerID);


		/// <summary>
		/// Check whether a line can be made from a given root point going through 
		/// the given directions, of a specific piece ID
		/// </summary>
		/// <remarks>
		/// A line is a connected strip of pieces owned by the same player, of the same piece ID
		/// (when a piece ID is specified)
		/// </remarks>
		/// <param name="rootPoint">The point to start the line</param>
		/// <param name="lineLength">The required line length</param>
		/// <param name="directions">The directions the line can be made from the root point</param>
		/// <returns>The player ID of the player who owns the line, or null if none was found</returns>
		string HasLine(TPoint rootPoint, int lineLength);


		/// <summary>
		/// Check whether a line can be made from a given root point going through 
		/// the given directions, of a specific piece ID.
		/// </summary>
		/// <remarks>
		/// A line is a connected strip of pieces owned by the same player, of the same piece ID
		/// (when a piece ID is specified)
		/// </remarks>
		/// <param name="rootPoint">The point to start the line</param>
		/// <param name="lineLength">The required line length</param>
		/// <param name="pieceTypeID">The piece type ID to match. If null, will match any piece</param>
		/// <param name="directions">The directions the line can be made from the root point</param>
		/// <param name="foundDirection">The direction in which a line was found. Undefined if the return value is null</param>
		/// <returns>The player ID of the player who owns the line, or null if none was found</returns>
		string HasLine(TPoint rootPoint, int lineLength, String pieceTypeID, out TPoint foundDirection);


		/// <summary>
		/// Check whether, with one valid placement, a line can be made by a given player
		/// starting at a root point going through the given directions
		/// </summary>
		/// <remarks>
		/// A line is a connected strip of pieces owned by the same player, of the same piece ID
		/// (when a piece ID is specified)
		/// </remarks>
		/// <param name="rootPoint">The point to start the line</param>
		/// <param name="playerID">The player ID whose lines we are checking for</param>
		/// <param name="lineLength">The required line length</param>
		/// <param name="directions">The directions the line can be made from the root point</param>
		/// <param name="missingPosition">The point that could be placed to create the line. Undefined if the return value is false</param>
		/// <returns>True if only one placement is required to make a line</returns>
		bool CanMakeLine(TPoint rootPoint, string playerID, int lineLength, out TPoint missingPosition);


		/// <summary>
		/// Check whether, with one valid placement, a line can be made by a given player
		/// starting at a root point going through the given directions, of a specific piece ID.
		/// </summary>
		/// <remarks>
		/// A line is a connected strip of pieces owned by the same player, of the same piece ID
		/// (when a piece ID is specified)
		/// </remarks>
		/// <param name="rootPoint">The point to start the line</param>
		/// <param name="playerID">The player ID whose lines we are checking for</param>
		/// <param name="lineLength">The required line length</param>
		/// <param name="pieceTypeID">The piece type ID to match. If null, will match any piece</param>
		/// <param name="directions">The directions the line can be made from the root point</param>
		/// <param name="missingPosition">The point that could be placed to create the line. Undefined if the return value is false</param>
		/// <returns>True if only one placement is required to make a line</returns>
		bool CanMakeLine(TPoint rootPoint, string playerID, int lineLength, String pieceTypeID, out TPoint missingPosition);


		/// <summary>
		/// Initialise this component. Do not call directly. Instead use
		/// <see cref="IGameManager.StartGame"/>
		/// </summary>
		void StartGame();


		/// <summary>
		/// Initialise this component, registering all required events. Do not call directly.
		/// </summary>
		void Initialise(IGameManager<TPoint, TPieceSettings> game);


		/// <summary>
		/// Called every frame by the game manager.  Do not call directly
		/// </summary>
		void Tick();
	}
}
