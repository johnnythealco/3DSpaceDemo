using System;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Enum describing which direction a move is operating in
	/// </summary>
	[Version(1)]
	public enum MoveMode
	{
		/// <summary>
		/// Move is being performed
		/// </summary>
		Forward,
		/// <summary>
		/// Move is being undone
		/// </summary>
		Backwards
	}


	/// <summary>
	/// A class that represents a change in state to the game.
	/// Moves can change their state forwards and backwards, so that they
	/// can undo changes that they have made and restore the former state
	/// </summary>
	[Version(1)]
	public abstract class GameMove<TPoint, TPieceSettings> : IEquatable<GameMove<TPoint, TPieceSettings>>
		where TPieceSettings : PieceSettings<TPoint, TPieceSettings>
	{
		#region Fields
		protected readonly IGameManager<TPoint, TPieceSettings> gameManager;
		#endregion


		#region Properties
		/// <summary>
		/// The direction this move is currently moving in
		/// </summary>
		public MoveMode Direction { get; protected set; }

		/// <summary>
		/// Gets whether the move has done executing.
		/// </summary>
		public virtual bool IsDone
		{
			get { return true; }
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Create this move to operate on a given state
		/// </summary>
		public GameMove(IGameManager<TPoint, TPieceSettings> gameManager)
		{
			this.gameManager = gameManager;
		}
		#endregion


		#region Methods
		/// <summary>
		/// Start executing this move in the forward direction. Do not call directly. Will be called by
		/// MoveManager. Instead commit this move via <see cref="IGameManager.CommitMove"/>
		/// </summary>
		public virtual void Start()
		{
			Start(MoveMode.Forward);
		}


		/// <summary>
		/// Start executing this move in the specific direction. Do not call directly. Will be called by
		/// MoveManager. Instead commit this move via <see cref="IGameManager.CommitMove"/>
		/// </summary>
		public virtual void Start(MoveMode mode)
		{
			Direction = mode;
		}


		/// <summary>
		/// Perform necessary updates per frame. Do not call directly. Will be called by
		/// MoveManager
		/// </summary>
		/// <remarks>
		/// Will not be called if IsDone returns true
		/// </remarks>
		public virtual void Update()
		{
		}


		/// <summary>
		/// Commits the state change to the game. Do not call directly. Will be called by
		/// MoveManager
		/// </summary>
		public abstract void OnMoveEnd();
		#endregion


		#region Equatable
		public abstract bool Equals(GameMove<TPoint, TPieceSettings> other);


		public override bool Equals(object obj)
		{
			var move = obj as GameMove<TPoint, TPieceSettings>;

			if (move != null) return Equals(move);

			return false;
		}


		public abstract override int GetHashCode();
		#endregion
	}


	/// <summary>
	/// Class that handles waiting for animation
	/// </summary>
	[Version(1)]
	public abstract class WaitForAnimationMove<TPoint, TPieceSettings> : GameMove<TPoint, TPieceSettings>
		where TPieceSettings : PieceSettings<TPoint, TPieceSettings>
	{
		#region Fields
		public IMoveAnimator animator;
		#endregion


		#region Properties
		public override bool IsDone
		{
			get
			{
				return animator == null ||
					animator.AnimationFinished;
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Constructs a new move that waits for a given animation to complete 
		/// before committing changes to the game state
		/// </summary>
		/// <param name="gameState">The game state to modify</param>
		/// <param name="animator">The <see cref="IMoveAnimator"/> to wait on</param>
		public WaitForAnimationMove(IGameManager<TPoint, TPieceSettings> gameManager, IMoveAnimator animator)
			: base(gameManager) 
		{
			this.animator = animator;
		}
		#endregion


		#region Methods
		public override void Update()
		{
			base.Update();
			if (animator != null) animator.Update();
		}


		public override void Start(MoveMode mode)
		{
			base.Start(mode);
			if (animator != null) animator.Start();
		}


		public override void OnMoveEnd()
		{
			if (animator != null) animator.Finished();
		}
		#endregion
	}
}
