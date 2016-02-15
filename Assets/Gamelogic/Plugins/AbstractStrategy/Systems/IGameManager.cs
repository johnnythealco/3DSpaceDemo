using System.Collections.Generic;
using Gamelogic.AbstractStrategy.Grids;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Base interface for game management component. Most likely you will use
	/// <see cref="GridGameManager"/> instead of implementing this interface.
	/// </summary>
	[Version(1)]
	public interface IGameManager<TPoint, TPieceSettings>
		where TPieceSettings : PieceSettings<TPoint, TPieceSettings>
	{
		#region Properties
		/// <summary>
		/// Gets the move manager for the game
		/// </summary>
		MoveManager<TPoint, TPieceSettings> MoveManager { get; }

		/// <summary>
		/// Gets the turn manager for the game
		/// </summary>
		TurnManager<TPoint, TPieceSettings> TurnManager { get; }

		/// <summary>
		/// Gets the player whose turn is currently running
		/// </summary>
		Player<TPoint, TPieceSettings> CurrentPlayer { get; }


		/// <summary>
		/// Gets our current game state
		/// </summary>
		IGameState<TPoint, TPieceSettings> State { get; }


		/// <summary>
		/// Game rules object
		/// </summary>
		IGameRules<TPoint, TPieceSettings> Rules { get; }


		/// <summary>
		/// Gets all of the players in the game
		/// </summary>
		[Version(1, 1)]
		List<Player<TPoint, TPieceSettings>> Players { get; }


		/// <summary>
		/// Gets whether the game is over or not
		/// </summary>
		bool GameOver { get; }
		#endregion


		#region Methods
		/// <summary>
		/// Start a game
		/// </summary>
		void StartGame();

		/// <summary>
		/// Commit a move
		/// </summary>
		/// <returns>True if the move was valid and successfully committed</returns>
		bool CommitMove(GameMove<TPoint, TPieceSettings> move);

		/// <summary>
		/// Commit a move
		/// </summary>
		/// <returns>True if the move was valid and successfully committed</returns>
		[Version(1, 1)]
		bool CommitMove(GameMove<TPoint, TPieceSettings> move, bool validate);

		/// <summary>
		/// Create a visual piece for a given logical piece
		/// </summary>
		void CreateVisualPiece(IPieceProperties properties);

		/// <summary>
		/// Advance the current turn.
		/// </summary>
		[Version(1, 1)]
		void AdvanceTurn();

		/// <summary>
		/// Gets a player object by their player ID
		/// </summary>
		[Version(1, 1)]
		Player<TPoint, TPieceSettings> GetPlayerByID(string playerID);

		/// <summary>
		/// Register a new player with the game
		/// </summary>
		[Version(1, 1)]
		void RegisterPlayer(Player<TPoint, TPieceSettings> newPlayer);

		/// <summary>
		/// Returns a component that will handle a move animation
		/// </summary>
		/// <returns>Null if no animators are configured.</returns>
		IMoveAnimator GetMoveAnimator(IPieceProperties piece, MoveMode direction, TPoint source, TPoint destination);

		/// <summary>
		/// Returns a component that will handle an add animation
		/// </summary>
		/// <returns>Null if no animators are configured.</returns>
		IMoveAnimator GetAddAnimator(IPieceProperties piece, MoveMode direction, TPoint position);

		/// <summary>
		/// Returns a component that will handle a remove animation
		/// </summary>
		/// <returns>Null if no animators are configured.</returns>
		IMoveAnimator GetRemoveAnimator(IPieceProperties piece, MoveMode direction, TPoint position);

		/// <summary>
		/// Returns a component that will handle a replacement animation
		/// </summary>
		/// <returns>Null if no animators are configured.</returns>
		IMoveAnimator GetReplaceAnimator(IPieceProperties replacedPiece, IPieceProperties replacingPiece, MoveMode direction, TPoint position);

		/// <summary>
		/// Returns a component that will handle a capture animation
		/// </summary>
		/// <returns>Null if no animators are configured.</returns>
		IMoveAnimator GetCaptureAnimator(IPieceProperties capturingPiece, IPieceProperties capturedPiece, MoveMode direction, TPoint source, TPoint destination, TPoint capturedPosition);
		#endregion
	}
}
