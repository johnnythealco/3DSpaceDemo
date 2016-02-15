using System;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Player base class
	/// </summary>
	[Version(1)]
	public abstract class Player<TPoint, TPieceSettings>
		where TPieceSettings : PieceSettings<TPoint, TPieceSettings>
	{
		#region Fields
		private readonly string _id;


		/// <summary>
		/// Held reference to turn manager
		/// </summary>
		protected IGameManager<TPoint, TPieceSettings> gameManager;
		#endregion


		#region Properties
		/// <summary>
		/// Gets the unique identifier for this player
		/// </summary>
		public string PlayerID { get { return _id; } }


		/// <summary>
		/// Gets our current victory state
		/// </summary>
		public VictoryType VictoryState { get; protected set; }
		#endregion


		#region Events
		/// <summary>
		/// Event fired when our victory state changes
		/// </summary>
		public event Action<Player<TPoint, TPieceSettings>, VictoryType> onVictoryStateChanged;
		#endregion


		#region Constructor
		/// <summary>
		/// Create instance of a player
		/// </summary>
		public Player(string id)
		{
			_id = id;
			VictoryState = VictoryType.None;
		}
		#endregion


		#region Methods
		/// <summary>
		/// Called when this player is registered as part of a game. Do not call directly. Instead, 
		/// register a player through the Game manager.
		/// </summary>
		/// <param name="turnManager"></param>
		internal virtual void OnRegistered(IGameManager<TPoint, TPieceSettings> gameManager)
		{
			this.gameManager = gameManager;
		}


		/// <summary>
		/// Reset our state. Do not call directly.
		/// </summary>
		public virtual void StartGame()
		{
			VictoryState = VictoryType.None;
		}


		/// <summary>
		/// Set our victory state.
		/// </summary>
		/// <param name="newVictoryState"></param>
		internal void SetVictoryState(VictoryType newVictoryState)
		{
			// Default case only changes victory state if it's set to None
			if (VictoryState == VictoryType.None)
			{
				VictoryState = newVictoryState;
				if (onVictoryStateChanged != null)
				{
					onVictoryStateChanged(this, newVictoryState);
				}
			}
		}


		/// <summary>
		/// Called by the turn manager any time this player is active, while the
		/// move queue is empty and turn state is <see cref="PlayerTurnState.Running"/>.
		/// Do not call directly
		/// </summary>
		public virtual void Tick()
		{
		}


		/// <summary>
		/// Called when the turn manager state becomes <see cref="PlayerTurnState.Starting"/>.
		/// Do not call directly
		/// </summary>
		public virtual void OnTurnStart() { }


		/// <summary>
		/// Called when the turn manager state becomes <see cref="PlayerTurnState.Ending"/>
		/// Do not call directly
		/// </summary>
		public virtual void OnTurnEnd() { }
		#endregion
	}
}
