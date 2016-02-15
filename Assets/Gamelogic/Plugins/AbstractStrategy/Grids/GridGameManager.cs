using System;
using System.Collections.Generic;
using Gamelogic.Grids;
using UnityEngine;
using System.Linq;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Container object to hold information about initial pieces on the board
	/// </summary>
	[Version(1)]
	[Serializable]
	public struct InitialPiecePlacement
	{
		/// <summary>
		/// The id of the piece to place
		/// </summary>
		public string pieceID;
		/// <summary>
		/// The player ID to place
		/// </summary>
		public string playerID;
		/// <summary>
		/// The position of the piece
		/// </summary>
		public InspectableVectorPoint position;
	}


	/// <summary>
	/// Primary game management component.
	/// </summary>
	[Version(1)]
	[RequireComponent(typeof(GridGameRules))]
	public class GridGameManager : GridBehaviour<RectPoint>,
		IGameManager<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		private Dictionary<IPieceProperties, GridGamePiece> pieceMap;


		/// <summary>
		/// Component that will create move animations for pieces
		/// </summary>
		public MoveAnimationCreator moveAnimator;

		/// <summary>
		/// Component that will create remove animations for pieces
		/// </summary>
		public RemoveAnimationCreator removeAnimator;

		/// <summary>
		/// Component that will create add animations for pieces
		/// </summary>
		public AddAnimationCreator addAnimator;

		/// <summary>
		/// Component that will create remove animations for pieces
		/// </summary>
		[Version(1, 1)]
		public ReplaceAnimationCreator replaceAnimator;

		/// <summary>
		/// Component that will create add animations for pieces
		/// </summary>
		[Version(1, 1)]
		public CaptureAnimationCreator captureAnimator;

		/// <summary>
		/// List of initial piece positions on the grid
		/// </summary>
		public InitialPiecePlacement[] initialPieces;


		/// <summary>
		/// List of all pieces we've created
		/// </summary>
		[HideInInspector]
		[SerializeField]
		private List<GridGamePiece> allPieces;
		#endregion


		#region Properties
		/// <summary>
		/// Gets the move manager for the game
		/// </summary>
		public MoveManager<RectPoint, GridGamePieceSettings> MoveManager
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the turn manager for the game
		/// </summary>
		public TurnManager<RectPoint, GridGamePieceSettings> TurnManager
		{
			get;
			protected set;
		}

		/// <summary>
		/// Wraps <see cref="TurnManager.CurrentPlayer"/>
		/// </summary>
		public Player<RectPoint, GridGamePieceSettings> CurrentPlayer
		{
			get { return TurnManager.CurrentPlayer; }
		}


		/// <summary>
		/// Gets all of the players in the game. Wraps <see cref="TurnManager.Players"/>
		/// </summary>
		public List<Player<RectPoint, GridGamePieceSettings>> Players
		{
			get { return TurnManager.Players; }
		}


		/// <summary>
		/// Gets the winners of the game, if there are any.
		/// It is possible for a player to have won before the game is entirely over, if the
		/// configuration of the victory conditions allow it.
		/// </summary>
		[Version(1, 1)]
		public IEnumerable<Player<RectPoint, GridGamePieceSettings>> Winners
		{
			get
			{
				return TurnManager.Players.Where(t => t.VictoryState == VictoryType.Victory);
			}
		}


		/// <summary>
		/// Gets the losers of the game, if there are any.
		/// It is possible for a player to have lost before the game is entirely over, if the
		/// configuration of the victory conditions allow it.
		/// </summary>
		[Version(1, 1)]
		public IEnumerable<Player<RectPoint, GridGamePieceSettings>> Losers
		{
			get
			{
				return TurnManager.Players.Where(t => t.VictoryState == VictoryType.Defeat);
			}
		}

		/// <summary>
		/// Gets all players who have tied, if there are any
		/// It is possible for a player to have tied before the game is entirely over, if the
		/// configuration of the victory conditions allow it.
		/// </summary>
		[Version(1, 1)]
		public IEnumerable<Player<RectPoint, GridGamePieceSettings>> TiedPlayers
		{
			get
			{
				return TurnManager.Players.Where(t => t.VictoryState == VictoryType.Draw);
			}
		}

		/// <summary>
		/// Gets all players who are still left in the game.
		/// </summary>
		[Version(1, 1)]
		public IEnumerable<Player<RectPoint, GridGamePieceSettings>> RemainingPlayers
		{
			get
			{
				return TurnManager.Players.Where(t => t.VictoryState == VictoryType.None);
			}
		}


		/// <summary>
		/// Gets our current game state
		/// </summary>
		public IGameState<RectPoint, GridGamePieceSettings> State
		{
			get;
			protected set;
		}


		/// <summary>
		/// Game rules object
		/// </summary>
		public IGameRules<RectPoint, GridGamePieceSettings> Rules
		{
			get;
			protected set;
		}

		/// <summary>
		/// Grid of visual pieces on the game
		/// </summary>
		public RectGrid<List<GridGamePiece>> VisualGrid
		{
			get;
			protected set;
		}


		/// <summary>
		/// Gets our logical game grid. Wraps <see cref="GridState.gameGrid"/>
		/// </summary>
		public RectGrid<GridGameCell> GameGrid
		{
			get { return ((GridState)State).gameGrid; }
		}


		/// <summary>
		/// Gets whether the game is over. Returns true when all players have a victory state.
		/// </summary>
		public bool GameOver
		{
			get;
			protected set;
		}
		#endregion


		#region Events
		/// <summary>
		/// Event fired when the game ends
		/// </summary>
		public event Action OnGameOver;
		#endregion


		#region Methods
		public override void InitGrid()
		{
			VisualGrid = (RectGrid<List<GridGamePiece>>)Grid.CloneStructure<List<GridGamePiece>>();
			foreach (RectPoint point in VisualGrid)
			{
				VisualGrid[point] = new List<GridGamePiece>();
			}

			if (!Application.isPlaying)
			{
				PopulateInitialGrid(true);
			}
			else
			{
				// Existing pieces?
				if (allPieces != null)
				{
					foreach (var piece in allPieces)
					{
						if (piece != null && piece.gameObject != null)
						{
							Destroy(piece.gameObject);
						}
					}
					allPieces.Clear();
				}
			}
		}

		/// <summary>
		/// Create and initialize all our components
		/// </summary>
		protected virtual void Initialise(GameRules<RectPoint, GridGamePieceSettings> rules)
		{
			Rules = rules;

			pieceMap = new Dictionary<IPieceProperties, GridGamePiece>();

			TurnManager = new TurnManager<RectPoint, GridGamePieceSettings>(this);
			MoveManager = new MoveManager<RectPoint, GridGamePieceSettings>(this);
			State = new GridState((RectGrid<GridGameCell>)Grid.CloneStructure<GridGameCell>(), this);

			// Register state events
			State.OnPieceAdded += OnPieceAdded;
			State.OnPieceMoved += OnPieceMoved;
			State.OnPieceRemoved += OnPieceRemoved;
			TurnManager.onPlayerVictoryStateChange += OnPlayerVictoryStateChange;

			rules.Initialise(this);
		}


		/// <summary>
		/// Start a game
		/// </summary>
		public virtual void StartGame()
		{
			GameOver = false;

			if (allPieces != null)
			{
				foreach (var piece in allPieces)
				{
					if (piece != null && piece.gameObject != null)
					{
						Destroy(piece.gameObject);
					}
				}

				allPieces.Clear();
			}
			pieceMap.Clear();

			foreach (RectPoint point in VisualGrid)
			{
				if (VisualGrid[point] == null)
				{
					VisualGrid[point] = new List<GridGamePiece>();
				}
				else
				{
					VisualGrid[point].Clear();
				}
			}

			State.StartGame();
			Rules.StartGame();
			PopulateInitialGrid(false);

			MoveManager.StartGame();
			TurnManager.StartGame();
		}


		/// <summary>
		/// Advance the current turn. Wraps <seealso cref="TurnManager{TPoint,TPieceSettings}.AdvanceTurnState"/>
		/// </summary>
		public virtual void AdvanceTurn()
		{
			TurnManager.AdvanceTurnState();
		}


		/// <summary>
		/// Register a new player with the game. Wraps <seealso cref="TurnManager{TPoint,TPieceSettings}.RegisterPlayer"/>
		/// </summary>
		public virtual void RegisterPlayer(Player<RectPoint, GridGamePieceSettings> newPlayer)
		{
			TurnManager.RegisterPlayer(newPlayer);
		}


		/// <summary>
		/// Gets whether all players have some victory state achieved. Wraps <see cref="TurnManager{TPoint,TPieceSettings}.AllPlayersDone"/>
		/// </summary>
		/// <returns>True if all players have a victory state that is not <see cref="VictoryType.None"/></returns>
		public virtual bool AllPlayersDone()
		{
			return TurnManager.AllPlayersDone();
		}


		/// <summary>
		/// Gets a player object by their player ID
		/// </summary>
		/// <remarks>Wraps Gamelogic.Strategy.TurnManager.GetPlayerByID</remarks>
		public virtual Player<RectPoint, GridGamePieceSettings> GetPlayerByID(string playerID)
		{
			return TurnManager.GetPlayerByID(playerID);
		}


		/// <summary>
		/// Schedules a move to be applied to the game state. Will only commit valid moves.
		/// based on the game rules.
		/// </summary>
		/// <returns>
		/// False if the move was rejected. This will happen if it does not pass validation, or if the game
		/// is over
		/// </returns>
		/// <remarks>
		/// Any move that does not adhere to the current game rules as configured on the 
		/// pieces will be rejected and not scheduled for processing.
		/// </remarks>
		public virtual bool CommitMove(GameMove<RectPoint, GridGamePieceSettings> move)
		{
			return CommitMove(move, true);
		}


		/// <summary>
		/// Schedules a move to be applied to the game state, optionally performing validation on the move
		/// based on the game rules.
		/// </summary>
		/// <returns>
		/// False if the move was rejected. This will happen if it does not pass validation, or if the game
		/// is over
		/// </returns>
		/// <remarks>
		/// If <paramref name="validate"/> is true, any move that does not adhere to the current game rules
		/// as configured on the pieces will be rejected and not scheduled for processing.
		/// </remarks>
		[Version(1, 1)]
		public virtual bool CommitMove(GameMove<RectPoint, GridGamePieceSettings> move, bool validate)
		{
			// Validate
			if (!GameOver && (!validate || Rules.IsValidMove(move)))
			{
				MoveManager.ScheduleMove(move);
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// Create a visual piece on our visual grid
		/// </summary>
		public virtual void CreateVisualPiece(IPieceProperties properties)
		{
			GridGamePieceSettings settings = Rules.GetPieceSettings(properties.pieceID, properties.playerID);

			if (settings != null && settings.piecePrefab != null)
			{
				GameObject go = Instantiate(settings.piecePrefab, Vector3.zero, Quaternion.identity);
				go.transform.SetParent(transform, false);

				GridGamePiece gpg = go.GetComponent<GridGamePiece>();

				if (gpg == null)
				{
					throw new NotSupportedException("Game pieces should inherit from GridGamePiece");
				}

				if (allPieces == null) allPieces = new List<GridGamePiece>();
				allPieces.Add(gpg);
				pieceMap.Add(properties, gpg);
			}
			else
			{
				throw new InvalidOperationException("Tried to create a piece with an ID that wasn't set up in our rules");
			}
		}


		/// <summary>
		/// Gets the visual piece object for a given logical piece structure
		/// </summary>
		public virtual GridGamePiece GetExistingVisualPiece(IPieceProperties piece)
		{
			return pieceMap[piece];
		}
		#endregion


		#region Game Flow
		protected virtual void OnPieceRemoved(RectPoint originalPosition, IPieceProperties piece)
		{
			// Find visual piece
			var visualPiece = pieceMap[piece];
			VisualGrid[originalPosition].Remove(visualPiece);
			Destroy(visualPiece.gameObject);
			pieceMap.Remove(piece);
		}

		protected virtual void OnPieceMoved(RectPoint originalPosition, RectPoint destinationPosition, IPieceProperties piece)
		{
			var visualPiece = pieceMap[piece];
			VisualGrid[originalPosition].Remove(visualPiece);
			VisualGrid[destinationPosition].Add(visualPiece);

			// Position
			visualPiece.transform.position = Map[destinationPosition];
		}

		protected virtual void OnPieceAdded(RectPoint position, IPieceProperties piece)
		{
			var visualPiece = pieceMap[piece];
			VisualGrid[position].Add(visualPiece);

			// Position
			visualPiece.transform.position = Map[position];
		}


		protected virtual void OnPlayerVictoryStateChange(Player<RectPoint, GridGamePieceSettings> arg1, VictoryType arg2)
		{
			if (!GameOver && TurnManager.AllPlayersDone())
			{
				// Throw game over event
				GameOver = true;

				if (OnGameOver != null)
				{
					OnGameOver();
				}
			}
		}


		/// <summary>
		/// Create pieces for initial grid setup
		/// </summary>
		/// <param name="visualOnly">Create visual prefabs only</param>
		protected void PopulateInitialGrid(bool visualOnly)
		{
			if (Rules == null && !Application.isPlaying) OnEnable(); // Force enable to allow edit mode previews to work

			// Existing pieces?
			if (allPieces != null)
			{
				foreach (var piece in allPieces)
				{
					if (piece != null && piece.gameObject != null)
					{
						Destroy(piece.gameObject);
					}
				}
				allPieces.Clear();
			}

			foreach (var initialPiece in initialPieces)
			{
				var pieceSettings = Rules.GetPieceSettings(
					initialPiece.pieceID, initialPiece.playerID);
				var pieceProperties = Rules.CreatePieceProperties(pieceSettings, initialPiece.playerID);
				var pos = initialPiece.position.GetRectPoint();

				CreateVisualPiece(pieceProperties);
				if (!visualOnly)
				{
					State.PlaceGamePiece(pos, pieceProperties);
				}
				else
				{
					OnPieceAdded(pos, pieceProperties);
				}
			}
		}


		/// <summary>
		/// Gets an animator for moving pieces from one cell to another.
		/// </summary>
		public IMoveAnimator GetMoveAnimator(IPieceProperties piece, MoveMode direction, RectPoint source, RectPoint destination)
		{
			if (direction == MoveMode.Backwards)
			{
				return GetMoveAnimator(piece, MoveMode.Forward, destination, source);
			}

			if (moveAnimator != null)
			{
				return moveAnimator.GetMoveAnimator(pieceMap[piece], Map[source], Map[destination]);
			}

			return null;
		}


		/// <summary>
		/// Gets an animator for removing pieces from the board
		/// </summary>
		public IMoveAnimator GetRemoveAnimator(IPieceProperties piece, MoveMode direction, RectPoint position)
		{
			if (direction == MoveMode.Backwards)
			{
				return GetAddAnimator(piece, MoveMode.Forward, position);
			}

			if (removeAnimator != null)
			{
				return removeAnimator.GetRemoveAnimator(pieceMap[piece], Map[position]);
			}

			return null;
		}


		/// <summary>
		/// Gets an animator for adding pieces to the board
		/// </summary>
		public IMoveAnimator GetAddAnimator(IPieceProperties piece, MoveMode direction, RectPoint position)
		{
			if (direction == MoveMode.Backwards)
			{
				return GetAddAnimator(piece, MoveMode.Forward, position);
			}

			if (addAnimator != null)
			{
				return addAnimator.GetAddAnimator(pieceMap[piece], Map[position]);
			}

			return null;
		}


		[Version(1, 1)]
		public IMoveAnimator GetReplaceAnimator(IPieceProperties replacedPiece, IPieceProperties replacingPiece, MoveMode direction, RectPoint position)
		{
			if (direction == MoveMode.Backwards)
			{
				return GetReplaceAnimator(replacingPiece, replacedPiece, MoveMode.Forward, position);
			}

			if (replaceAnimator != null)
			{
				return replaceAnimator.GetReplaceAnimator(pieceMap[replacedPiece], pieceMap[replacingPiece], Map[position]);
			}

			return null;
		}


		[Version(1, 1)]
		public IMoveAnimator GetCaptureAnimator(IPieceProperties capturingPiece, IPieceProperties capturedPiece, MoveMode direction, RectPoint source, RectPoint destination, RectPoint capturedPosition)
		{
			if (captureAnimator != null)
			{
				return captureAnimator.GetCaptureAnimator(direction, pieceMap[capturedPiece], pieceMap[capturingPiece], Map[source],
					Map[destination], Map[capturedPosition]);
			}

			return null;
		}
		#endregion


		#region Unity methods
		protected virtual void OnEnable()
		{
			// Find rules object on this game manager
			var rules = GetComponent<GameRules<RectPoint, GridGamePieceSettings>>();

			if (rules == null)
			{
				Debug.LogError("Rules object should be placed together with the game manager");
			}

			Initialise(rules);
		}


		protected virtual void Update()
		{
			Rules.Tick();
			MoveManager.Tick();
			TurnManager.Tick();
		}
		#endregion
	}
}
