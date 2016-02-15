using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Game state object contained in a grid
	/// </summary>
	[Version(1)]
	public class GridState : IGameState<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		public readonly RectGrid<GridGameCell> gameGrid;
		public readonly GridGameManager gameManager;
		#endregion


		#region Events
		/// <summary>
		/// Event fired when a piece is added to the grid. The first parameter is the destination, and the second is the piece.
		/// </summary>
		public event Action<RectPoint, IPieceProperties> OnPieceAdded;

		/// <summary>
		/// Event fired when a piece is removed from the grid. The first parameter is the position, and the second is the piece.
		/// </summary>
		public event Action<RectPoint, IPieceProperties> OnPieceRemoved;

		/// <summary>
		/// Event fired when a piece is moved around on the grid.. The first parameter is the source, the second the destination, and the third is the piece.
		/// </summary>
		public event Action<RectPoint, RectPoint, IPieceProperties> OnPieceMoved;
		#endregion


		#region Constructor
		/// <summary>
		/// Initialize this grid state with a given grid
		/// </summary>
		public GridState(RectGrid<GridGameCell> gameGrid, GridGameManager gameManager)
		{
			this.gameGrid = gameGrid;
			this.gameManager = gameManager;

			// Fill the grid with lists if they're empty
			foreach (var point in gameGrid)
			{
				if (gameGrid[point] == null)
				{
					gameGrid[point] = new GridGameCell();
				}
			}
		}
		#endregion


		#region Methods
		/// <summary>
		/// Initialise and reset the state for a new game. Do not call directly. Instead use
		/// <see cref="IGameManager.StartGame"/>
		/// </summary>
		public void StartGame()
		{
			// Clear all cells
			foreach (var point in gameGrid)
			{
				gameGrid[point].Clear();
			}
		}


		/// <summary>
		/// Place a game piece on the board. You should probably not call this directly, and instead
		/// commit a move through <see cref="IGameManager.CommitMove"/>
		/// </summary>
		/// <param name="piece">The piece to add</param>
		/// <param name="position">The position on the grid to add the piece to</param>
		/// <exception cref="ArgumentNullException"><paramref name="piece"/> is null</exception>
		/// <exception cref="InvalidOperationException"><paramref name="position"/> is non-empty, or <paramref name="piece"/> is already on the board.</exception>
		public virtual void PlaceGamePiece(RectPoint position, IPieceProperties piece)
		{
			PlaceGamePiece(position, piece, true);
		}


		protected virtual void PlaceGamePiece(RectPoint position, IPieceProperties piece, bool fireEvent)
		{
			piece.ThrowIfNull("piece");

			gameGrid[position].Add(piece);

			if (fireEvent && OnPieceAdded != null)
			{
				OnPieceAdded(position, piece);
			}
		}


		/// <summary>
		/// Remove a piece at the given position. You should probably not call this directly, and instead
		/// commit a move through <see cref="IGameManager.CommitMove"/>
		/// </summary>
		/// <param name="piece">The piece to remove</param>
		/// <param name="position">The position from which a piece will be removed</param>
		/// <exception cref="InvalidOperationException"><paramref name="position"/> is empty</exception>
		public virtual void RemoveGamePiece(RectPoint position, IPieceProperties piece)
		{
			RemoveGamePiece(position, piece, true);
		}


		protected virtual void RemoveGamePiece(RectPoint position, IPieceProperties piece, bool fireEvent)
		{
			int ind = gameGrid[position].IndexOf(piece);
			if (ind < 0)
				throw new InvalidOperationException("Trying to remove a piece from a position but that position doesn't have a piece of that type");

			gameGrid[position].RemoveAt(ind);

			if (fireEvent && OnPieceRemoved != null)
			{
				OnPieceRemoved(position, piece);
			}
		}


		/// <summary>
		/// Move a piece from a given position to a destination. You should probably not call this directly, and instead
		/// commit a move through <see cref="IGameManager.CommitMove"/>
		/// </summary>
		/// <param name="initialPosition">The initial position for the piece</param>
		/// <param name="piece">The piece to move</param>
		/// <param name="destination">The destination position for the piece</param>
		/// <exception cref="InvalidOperationException"><paramref name="initialPosition"/> is empty or <paramref name="destination"/> is not empty</exception>
		public virtual void MoveGamePiece(RectPoint initialPosition, IPieceProperties piece, RectPoint destination)
		{
			int ind = gameGrid[initialPosition].IndexOf(piece);
			if (ind < 0)
				throw new InvalidOperationException("Trying to move a piece from a position but that position doesn't have a piece of that type");

			RemoveGamePiece(initialPosition, piece, false);
			PlaceGamePiece(destination, piece, false);

			if (OnPieceMoved != null)
			{
				OnPieceMoved(initialPosition, destination, piece);
			}
		}


		/// <summary>
		/// Tests a position for a piece of any kind
		/// </summary>
		/// <param name="position">The position on the board to test</param>
		/// <returns>True if the position is occupied</returns>
		public virtual IPieceProperties TestPosition(RectPoint position)
		{
			return TestPosition(position, null, null);
		}


		/// <summary>
		/// Tests a position for a piece of a specific ID
		/// </summary>
		/// <param name="position">The sposition on the board to test</param>
		/// <param name="pieceID">The piece ID to look for. If null, returns true for any piece</param>
		/// <returns>True if the position is occupied by a piece with the id matching <paramref name="pieceID"/></returns>
		public virtual IPieceProperties TestPosition(RectPoint position, string pieceID)
		{
			return TestPosition(position, pieceID, null);
		}


		/// <summary>
		/// Tests a position for a piece of a specific ID owned by a specific player
		/// </summary>
		/// <param name="position">The sposition on the board to test</param>
		/// <param name="pieceID">The piece ID to look for. If null, returns true for any piece</param>
		/// <param name="playerID">The plaer ID to look for. If null, returns true for any piece</param>
		/// <returns>True if the position is occupied by a piece with the id matching <paramref name="pieceID"/>
		/// and owned by a player matching <paramref name="playerID"/></returns>
		public virtual IPieceProperties TestPosition(RectPoint position, string pieceID, string playerID)
		{
			var cell = gameGrid[position];
			if (!cell.IsEmpty())
			{
				foreach (var piece in cell)
				{
					if ((string.IsNullOrEmpty(playerID) || (piece.playerID == playerID)) &&
						(string.IsNullOrEmpty(pieceID) || (piece.pieceID == pieceID)))
					{
						return piece;
					}
				}
			}

			// Empty
			return null;
		}


		public IEnumerable<RectPoint> GetAllPoints()
		{
			return gameGrid;
		}


		public ICollection<IPieceProperties> GetPiecesAtPoint(RectPoint cell)
		{
			return gameGrid[cell];
		}


		[Version(1, 1)]
		public RectPoint FindPiece(IPieceProperties piece)
		{
			return gameGrid.First(t => gameGrid[t].Contains(piece));
		}


		[Version(1, 1)]
		public IEnumerable<RectPoint> GetNeighboursOfPoint(RectPoint point)
		{
			return gameGrid.GetAllNeighbors(point);
		}


		[Version(1, 1)]
		public bool Contains(RectPoint position)
		{
			return gameGrid.Contains(position);
		}
		#endregion


		#region Move generation
		/// <summary>
		/// Create a move to place a piece on the grid
		/// </summary>
		/// <param name="piece">The piece to place</param>
		/// <param name="position">The point on the grid to add the piece</param>
		public GameMove<RectPoint, GridGamePieceSettings> CreateAddPieceMove(IPieceProperties piece, RectPoint position)
		{
			return new AddPieceMove(gameManager, position, piece, null);
		}

		/// <summary>
		/// Create a move to remove a piece from the grid
		/// </summary>
		/// <param name="piece">The piece to remove</param>
		/// <param name="position">The point on the grid from which to remove the piece</param>
		public GameMove<RectPoint, GridGamePieceSettings> CreateRemovePieceMove(IPieceProperties piece, RectPoint position)
		{
			return new RemovePieceMove(gameManager, position, piece, null);
		}

		/// <summary>
		/// Create a move to move a piece on the grid
		/// </summary>
		/// <param name="piece">The piece to move</param>
		/// <param name="sourcePosition">The point on the grid from which to remove the piece</param>
		/// <param name="destinationPosition">The point on the grid to add the piece</param>
		public GameMove<RectPoint, GridGamePieceSettings> CreateMovePieceMove(IPieceProperties piece, RectPoint sourcePosition, RectPoint destinationPosition)
		{
			return new MovePieceMove(gameManager, sourcePosition, piece, destinationPosition, null);
		}


		[Version(1, 1)]
		public GameMove<RectPoint, GridGamePieceSettings> CreateCapturePieceMove(IPieceProperties capturingPiece, IPieceProperties capturedPiece, RectPoint sourcePosition, RectPoint destinationPosition, RectPoint capturePosition)
		{
			return new CapturePieceMove(gameManager, sourcePosition, destinationPosition, capturePosition, capturedPiece, capturingPiece, null);
		}


		[Version(1, 1)]
		public GameMove<RectPoint, GridGamePieceSettings> CreateReplacePieceMove(IPieceProperties removedPiece, IPieceProperties addedPiece, RectPoint position)
		{
			return new ReplacePieceMove(gameManager, position, removedPiece, addedPiece, null);
		}
		#endregion


		#region Enumerable
		public IEnumerator<IPieceProperties> GetEnumerator()
		{
			// Select all pieces on the grid
			return gameGrid.SelectValuesAt(gameGrid.Where(t => gameGrid[t].Any())).SelectMany<GridGameCell, IPieceProperties>(t => t).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}
