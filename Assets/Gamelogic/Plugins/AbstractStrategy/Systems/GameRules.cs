using System;
using System.Collections.Generic;
using System.Linq;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Class to handle general game flow, moving between states, move logic and 
	/// validation, and victory states.
	/// Attach victory rules to this object to have them processed when the game progresses.
	/// </summary>
	[Version(1)]
	public abstract class GameRules<TPoint, TPieceSettings> : GLMonoBehaviour, IGameRules<TPoint, TPieceSettings>
		where TPieceSettings : PieceSettings<TPoint, TPieceSettings>
	{
		#region Fields
		/// <summary>
		/// Number of moves a player can make per turn. Set to 0 to keep rules from automatically
		/// progressing the turn.
		/// </summary>
		public int movesPerTurn = 1;
		/// <summary>
		/// Collection of pieces available to the game rules
		/// </summary>
		public TPieceSettings[] pieces;
		/// <summary>
		/// Whether to calculate moves for all players during all turns, instead of just moves for the 
		/// active player.
		/// </summary>
		/// <remarks>This can be useful to allow AIs to determine which moves will be valid for opponents, 
		/// but results in more costly computations every turn</remarks>
		public bool calculateMovesForAllPlayers;

		/// <summary>
		/// Counter for UIDs for pieces
		/// </summary>
		protected uint pieceCounter;
		/// <summary>
		/// List of victory rules attached to us
		/// </summary>
		protected List<VictoryRules<TPoint, TPieceSettings>> victoryRules;
		/// <summary>
		/// Stored reference to our game manager
		/// </summary>
		protected IGameManager<TPoint, TPieceSettings> game;

		/// <summary>
		/// All valid moves, indexed by player id
		/// </summary>
		private Dictionary<string, List<GameMove<TPoint, TPieceSettings>>> validMoves;
		#endregion


		#region Properties
		/// <summary>
		/// Gets the list of players who have achieved the victory state, in the order that they did.
		/// </summary>
		public List<Player<TPoint, TPieceSettings>> VictoriousPlayers { get; protected set; }
		/// <summary>
		/// Gets the number of moves that have been made this turn.
		/// </summary>
		public int MovesMadeThisTurn { get; protected set; }
		#endregion


		#region Methods
		/// <summary>
		/// Initialise this component, registering all required events. Do not call directly.
		/// </summary>
		public virtual void Initialise(IGameManager<TPoint, TPieceSettings> game)
		{
			this.game = game;

			// Register events
			game.TurnManager.onPlayerChange += OnPlayerChange;
			game.TurnManager.onTurnStateChange += OnTurnStateChange;
			game.TurnManager.onPlayerVictoryStateChange += OnPlayerVictoryChange;
			game.MoveManager.onAllMovesEnd += OnAllMovesEnded;
			game.MoveManager.onMoveEnd += OnMoveEnd;
			game.MoveManager.onMoveStart += OnMoveStart;
			game.State.OnPieceAdded += OnPieceAdded;
			game.State.OnPieceRemoved += OnPieceRemoved;
			game.State.OnPieceMoved += OnPieceMoved;

			// Find all victory rules attached to us
			victoryRules = new List<VictoryRules<TPoint, TPieceSettings>>(
				GetComponents<VictoryRules<TPoint, TPieceSettings>>());
			validMoves = new Dictionary<string, List<GameMove<TPoint, TPieceSettings>>>();

			foreach (var rule in victoryRules)
			{
				rule.Initialise(game);
			}
		}


		/// <summary>
		/// Gets the valid moves list for a given player for this turn
		/// </summary>
		/// <param name="player">The player to query</param>
		public virtual List<GameMove<TPoint, TPieceSettings>> GetValidMovesListForPlayer(Player<TPoint, TPieceSettings> player)
		{
			return GetValidMovesListForPlayer(player.PlayerID);
		}


		/// <summary>
		/// Gets the valid moves list for a given player for this turn
		/// </summary>
		/// <param name="playerId">The id of the player to query</param>
		public virtual List<GameMove<TPoint, TPieceSettings>> GetValidMovesListForPlayer(string playerId)
		{
			List<GameMove<TPoint, TPieceSettings>> moveList;
			if (!validMoves.TryGetValue(playerId, out moveList))
			{
				moveList = new List<GameMove<TPoint, TPieceSettings>>();
				validMoves.Add(playerId, moveList);
			}
			return moveList;
		}


		/// <summary>
		/// Creates a collection of all valid add moves that can be made given the current game state.
		/// </summary>
		protected virtual void UpdateValidMoves()
		{
			// Reset all moves
			foreach (var player in game.TurnManager.Players)
			{
				var moveList = GetValidMovesListForPlayer(player);
				moveList.Clear();
			}

			if (calculateMovesForAllPlayers)
			{
				foreach (var player in game.TurnManager.Players)
				{
					UpdateMovesForPlayer(GetValidMovesListForPlayer(player), player);
				}
			}
			else
			{
				var player = game.CurrentPlayer;
				UpdateMovesForPlayer(GetValidMovesListForPlayer(player), player);
			}
		}


		/// <summary>
		/// Updates the valid moves for a given player, were that player to have control of the game now
		/// </summary>
		/// <param name="movesList">The list into which to place the moves</param>
		/// <param name="player">The player whose moves we want to calculate</param>
		protected virtual void UpdateMovesForPlayer(List<GameMove<TPoint, TPieceSettings>> movesList, Player<TPoint, TPieceSettings> player)
		{
			var gameState = game.State;

			// Check for add moves
			foreach (var pieceSettings in pieces)
			{
				if (pieceSettings.owningPlayerID == player.PlayerID)
				{
					foreach (var position in pieceSettings.GetAddPositions(gameState))
					{
						movesList.Add(gameState.CreateAddPieceMove(CreatePieceProperties(pieceSettings, player), position));
					}
				}
			}

			// Check for existing pieces, and add their move, capture and removal options
			foreach (var point in gameState.GetAllPoints())
			{
				var gridPoint = gameState.GetPiecesAtPoint(point);

				// For each piece on this tile that belongs to the player
				foreach (var piece in gridPoint.Where(t => t.playerID == player.PlayerID))
				{
					var pieceSettings = GetPieceSettings(piece);
					// Moves
					foreach (var movePosition in pieceSettings.GetMovePositions(gameState, piece, point))
					{
						movesList.Add(gameState.CreateMovePieceMove(piece, point, movePosition));
					}

					// Remove?
					if (pieceSettings.CanRemove(gameState, piece, point))
					{
						movesList.Add(gameState.CreateRemovePieceMove(piece, point));
					}

					// Capture
					foreach (var capture in pieceSettings.GetCaptures(gameState, piece, point))
					{
						movesList.Add(gameState.CreateCapturePieceMove(piece, capture.piece, point, capture.position,
							gameState.FindPiece(capture.piece)));
					}
				}
			}
		}


		/// <summary>
		/// Validates a given move by comparing it to all known acceptable moves
		/// </summary>
		/// <param name="Move"></param>
		public virtual bool IsValidMove(GameMove<TPoint, TPieceSettings> Move)
		{
			return GetValidMovesListForPlayer(game.CurrentPlayer).Contains(Move);
		}


		/// <summary>
		/// Create the properties for a given source piece, owned by a given player
		/// </summary>
		public abstract IPieceProperties CreatePieceProperties(TPieceSettings sourcePiece, Player<TPoint, TPieceSettings> owner);


		/// <summary>
		/// Create the properties for a given source piece, owned by a given player
		/// </summary>
		public abstract IPieceProperties CreatePieceProperties(TPieceSettings sourcePiece, string ownerID);


		/// <summary>
		/// Gets the creation properties for a given piece
		/// </summary>
		public virtual TPieceSettings GetPieceSettings(IPieceProperties piece)
		{
			return GetPieceSettings(piece.pieceID, piece.playerID);
		}
		


		/// <summary>
		/// Gets the creation properties for a given piece
		/// </summary>
		public TPieceSettings GetPieceSettings(String pieceID, String playerID)
		{
			return pieces.First(t => t.pieceID == pieceID &&
				t.owningPlayerID == playerID);
		}
		#endregion


		#region Lines
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
		public virtual string HasLine(TPoint rootPoint, int lineLength)
		{
			TPoint outPoint;
			return HasLine(rootPoint, lineLength, null, out outPoint);
		}



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
		public abstract string HasLine(TPoint rootPoint, int lineLength, String pieceTypeID, out TPoint foundDirection);


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
		public virtual bool CanMakeLine(TPoint rootPoint, string playerID, int lineLength, out TPoint missingPosition)
		{
			return CanMakeLine(rootPoint, playerID, lineLength, null, out missingPosition);
		}


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
		public abstract bool CanMakeLine(TPoint rootPoint, string playerID, int lineLength, String pieceTypeID, out TPoint missingPosition);
		#endregion


		#region Game flow
		/// <summary>
		/// Initialise this component. Do not call directly. Instead use
		/// <see cref="IGameManager.StartGame"/>
		/// </summary>
		public virtual void StartGame()
		{
			pieceCounter = 0;
			validMoves.Clear();
			MovesMadeThisTurn = 0;

			if (VictoriousPlayers == null)
			{
				VictoriousPlayers = new List<Player<TPoint, TPieceSettings>>();
			}
			else
			{
				VictoriousPlayers.Clear();
			}
		}


		/// <summary>
		/// Called every frame by the game manager.  Do not call directly
		/// </summary>
		public virtual void Tick()
		{
		}


		/// <summary>
		/// Event fired when players change
		/// </summary>
		/// <param name="PreviousPlayer">The player whose turn just ended</param>
		/// <param name="CurrentPlayer">The player whose turn just began</param>
		protected virtual void OnPlayerChange(Player<TPoint, TPieceSettings> PreviousPlayer, Player<TPoint, TPieceSettings> CurrentPlayer)
		{
			if (!game.GameOver)
			{
				UpdateValidMoves();
			}
			else
			{
				foreach (var keyPair in validMoves)
				{
					keyPair.Value.Clear();
				}
			}
			MovesMadeThisTurn = 0;

			foreach (var rule in victoryRules)
			{
				if (rule.enabled)
					rule.OnPlayerChange();
			}
		}


		/// <summary>
		/// Event fired when turn states change.
		/// </summary>
		/// <param name="PreviousState">The former state.</param>
		/// <param name="CurrentState">The new current state.</param>
		protected virtual void OnTurnStateChange(PlayerTurnState PreviousState, PlayerTurnState CurrentState)
		{
			foreach (var rule in victoryRules)
			{
				if (rule.enabled)
					rule.OnTurnStateChange();
			}
		}


		/// <summary>
		/// Event fired just after a move ends
		/// </summary>
		/// <param name="EndedMove">The move that just ended</param>
		protected virtual void OnMoveEnd(GameMove<TPoint, TPieceSettings> EndedMove)
		{
		}


		/// <summary>
		/// Event fired just before a move starts
		/// </summary>
		/// <param name="StartedMove">The move that just started</param>
		protected virtual void OnMoveStart(GameMove<TPoint, TPieceSettings> StartedMove)
		{
		}


		/// <summary>
		/// Event fired when move queue is empty
		/// </summary>
		protected virtual void OnAllMovesEnded()
		{
			foreach (var rule in victoryRules)
			{
				if (rule.enabled)
					rule.OnAllMovesEnd();
			}

			MovesMadeThisTurn++;
			if (MovesMadeThisTurn >= movesPerTurn &&
				movesPerTurn > 0)
			{
				game.TurnManager.AdvanceTurnState();
			}
		}


		/// <summary>
		/// Called when a player's victory state changes
		/// </summary>
		protected virtual void OnPlayerVictoryChange(Player<TPoint, TPieceSettings> player, VictoryType newState)
		{
			if (newState == VictoryType.Victory)
			{
				VictoriousPlayers.Add(player);
			}


			foreach (var rule in victoryRules)
			{
				if (rule.enabled)
					rule.OnPlayerVictoryChange();
			}
		}


		/// <summary>
		/// Event fired just after a piece has moved
		/// </summary>
		/// <param name="originalPosition">The piece's initial position</param>
		/// <param name="destination">The destination position of the piece</param>
		/// <param name="piece">The piece that is moving</param>
		protected virtual void OnPieceMoved(TPoint originalPosition, TPoint destination, IPieceProperties piece)
		{
		}


		/// <summary>
		/// Event fired just after a piece is removed
		/// </summary>
		/// <param name="position">The position from which the piece is removed</param>
		/// <param name="piece">The piece that is being removed</param>
		protected virtual void OnPieceRemoved(TPoint position, IPieceProperties piece)
		{
		}


		/// <summary>
		/// Event fired just after a piece is added
		/// </summary>
		/// <param name="position">The position at which the piece will b e added</param>
		/// <param name="piece">The piece that was added</param>
		protected virtual void OnPieceAdded(TPoint position, IPieceProperties piece)
		{
		}
		#endregion
	}
}
