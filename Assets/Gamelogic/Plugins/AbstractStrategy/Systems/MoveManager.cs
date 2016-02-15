using System;
using System.Collections.Generic;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Management class for move states
	/// </summary>
	[Serializable]
	[Version(1)]
	public sealed class MoveManager<TPoint, TPieceSettings>
		where TPieceSettings : PieceSettings<TPoint, TPieceSettings>
	{
		private struct QueuedMove
		{
			public GameMove<TPoint, TPieceSettings> move;
			public MoveMode direction;
		}

		#region Fields
		private bool done;
		private GameMove<TPoint, TPieceSettings> currentMove;
		private readonly Queue<QueuedMove> moves;
		//private readonly IGameManager<TPoint, TPieceSettings> game;
		#endregion


		#region Events
		/// <summary>
		/// Event fired when after a move ends
		/// </summary>
		public event Action<GameMove<TPoint, TPieceSettings>> onMoveEnd;
		/// <summary>
		/// Event fired just before a move starts
		/// </summary>
		public event Action<GameMove<TPoint, TPieceSettings>> onMoveStart;
		/// <summary>
		/// Event fired after all queued moves have completed
		/// </summary>
		public event Action onAllMovesEnd;
		#endregion


		#region Properties
		/// <summary>
		/// History of all moves
		/// </summary>
		public List<GameMove<TPoint, TPieceSettings>> MoveHistory
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets whether all moves have been processed and we're waiting for more
		/// </summary>
		public bool Done
		{
			get { return done; }
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Initialize the move manager
		/// </summary>
		public MoveManager(IGameManager<TPoint, TPieceSettings> gameManager)
		{
			done = true;
			currentMove = null;
			//game = gameManager;

			moves = new Queue<QueuedMove>();
			MoveHistory = new List<GameMove<TPoint, TPieceSettings>>();
		}
		#endregion


		#region Methods
		/// <summary>
		/// Initialize this move manager. Do not call directly
		/// </summary>
		internal void StartGame()
		{
			MoveHistory.Clear();
			moves.Clear();
			done = true;
			currentMove = null;
		}


		/// <summary>
		/// Enqueue a move for processing
		/// </summary>
		public void ScheduleMove(GameMove<TPoint, TPieceSettings> move)
		{
			ScheduleMove(move, MoveMode.Forward);
		}

		/// <summary>
		/// Enqueue a move for processing
		/// </summary>
		public void ScheduleMove(GameMove<TPoint, TPieceSettings> move, MoveMode direction)
		{
			done = false;
			moves.Enqueue(new QueuedMove { move = move, direction = direction });
		}


		/// <summary>
		/// Schedule an undo of the last move
		/// </summary>
		public void ScheduleUndo()
		{
			if (MoveHistory.Count == 0)
			{
				throw new InvalidOperationException("Trying to undo a move but we haven't performed any yet.");
			}

			ScheduleMove(MoveHistory[MoveHistory.Count - 1], MoveMode.Backwards);
		}


		/// <summary>
		/// Called every turn by the game manager to updated the queued moves. Do not call directly
		/// </summary>
		internal void Tick()
		{
			if (!done)
			{
				if (currentMove == null)
				{
					GetNextMove();
				}

				if (currentMove != null)
				{
					if (!currentMove.IsDone)
					{
						currentMove.Update();
					}

					// Check if it's done now
					if (currentMove.IsDone)
					{
						// Tell move to commit its change
						currentMove.OnMoveEnd();

						if (currentMove.Direction == MoveMode.Forward)
						{
							// Add to move history
							MoveHistory.Add(currentMove);
						}
						else
						{
							MoveHistory.RemoveAt(MoveHistory.Count - 1);
						}

						// Fire event
						if (onMoveEnd != null)
						{
							onMoveEnd(currentMove);
						}

						if (!GetNextMove())
						{
							currentMove = null;
							done = true;

							if (onAllMovesEnd != null)
							{
								onAllMovesEnd();
							}
						}
					}
				}
			}
		}
		#endregion


		#region Helpers
		private bool GetNextMove()
		{
			if (moves.Count > 0)
			{
				var queuedMove = moves.Dequeue();

				currentMove = queuedMove.move;

				// Fire event
				if (onMoveStart != null)
				{
					onMoveStart(currentMove);
				}

				// Tell move it's starting
				currentMove.Start(queuedMove.direction);

				return true;
			}

			return false;
		}
		#endregion
	}
}
