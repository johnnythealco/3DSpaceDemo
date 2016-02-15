using System;
using System.Collections.Generic;
using System.Linq;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Different states a player turn can be in
	/// </summary>
	[Version(1)]
	public enum PlayerTurnState
	{
		/// <summary>
		/// About to start a player turn
		/// </summary>
		Starting,
		/// <summary>
		/// Player turn is executing
		/// </summary>
		Running,
		/// <summary>
		/// About to end a player turn
		/// </summary>
		Ending
	}


	/// <summary>
	/// Class that manages turn states for the game.
	/// </summary>
	[Version(1)]
	public class TurnManager<TPoint, TPieceSettings>
		where TPieceSettings : PieceSettings<TPoint, TPieceSettings>
	{
		#region Fields
		/// <summary>
		/// Stored reference to game manager
		/// </summary>
		protected readonly IGameManager<TPoint, TPieceSettings> gameManager;
		#endregion


		#region Properties
		/// <summary>
		/// Gets the player who is currently taking their turn
		/// </summary>
		public Player<TPoint, TPieceSettings> CurrentPlayer
		{
			get;
			protected set;
		}


		/// <summary>
		/// Gets the list of players in turn order
		/// </summary>
		public List<Player<TPoint, TPieceSettings>> Players
		{
			get;
			protected set;
		}


		/// <summary>
		/// Gets the current turn state for the current player
		/// </summary>
		public PlayerTurnState CurrentTurnState
		{
			get;
			protected set;
		}
		#endregion


		#region Events
		/// <summary>
		/// Event fired when the current player changes. The first parameter is the previous player, and the second is the new player.
		/// </summary>
		public event Action<Player<TPoint, TPieceSettings>, Player<TPoint, TPieceSettings>> onPlayerChange;

		/// <summary>
		/// Event fired when the turn state changes. The first parameter is the former state, and the second is the new state.
		/// </summary>
		public event Action<PlayerTurnState, PlayerTurnState> onTurnStateChange;

		/// <summary>
		/// Event fired when any player's victory state changes.
		/// </summary>
		public event Action<Player<TPoint, TPieceSettings>, VictoryType> onPlayerVictoryStateChange;
		#endregion


		#region Constructor
		/// <summary>
		/// Initialise the turn manager
		/// </summary>
		public TurnManager(IGameManager<TPoint, TPieceSettings> gameManager)
		{
			this.gameManager = gameManager;
			Players = new List<Player<TPoint, TPieceSettings>>();
		}
		#endregion


		#region Methods
		/// <summary>
		/// Start the game. Do not call directly.
		/// </summary>
		internal void StartGame()
		{
			CurrentPlayer = null;

			if (Players.Count == 0)
			{
				throw new InvalidOperationException("Players need to be registered with the turn manager before the game is started.");
			}

			foreach (var player in Players)
			{
				player.StartGame();
			}

			// Temporarily set to end state so that it will correctly advance to the first player
			CurrentTurnState = PlayerTurnState.Ending;
			AdvanceTurnState();
		}


		/// <summary>
		/// Register a new player with the game
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if a player with that ID is already registered</exception>
		public virtual void RegisterPlayer(Player<TPoint, TPieceSettings> newPlayer)
		{
			// Verify that a player with this ID is not already registered
			if (Players.Exists(t => t.PlayerID == newPlayer.PlayerID))
			{
				throw new InvalidOperationException("A player with the ID " + newPlayer.PlayerID + " is already registered");
			}
			Players.Add(newPlayer);
			newPlayer.OnRegistered(gameManager);
			newPlayer.onVictoryStateChanged += OnPlayerVictory;
		}


		/// <summary>
		/// Gets a player by their ID
		/// </summary>
		/// <param name="playerID"></param>
		/// <returns></returns>
		public Player<TPoint, TPieceSettings> GetPlayerByID(string playerID)
		{
			return Players.First(t => t.PlayerID == playerID);
		}


		/// <summary>
		/// Gets the next player in the turn state whose game is not over
		/// </summary>
		public Player<TPoint, TPieceSettings> GetNextPlayer()
		{
			if (Players.Count == 0)
			{
				throw new InvalidOperationException("Players need to be registered with the turn manager before state is advanced.");
			}
			if (AllPlayersDone())
			{
				throw new InvalidOperationException("Cannot get the next player in the cycle if no players are left in the game.");
			}

			// Find the index of the current player in the turn structure, then return
			// the next one that has a None victory state
			int ind;

			if (CurrentPlayer != null)
			{
				ind = Players.IndexOf(CurrentPlayer);
				ind = (ind + 1) % Players.Count;
			}
			else
			{
				ind = 0;
			}

			while (Players[ind].VictoryState != VictoryType.None)
			{
				ind = (ind + 1) % Players.Count;
			}

			return Players[ind];
		}


		/// <summary>
		/// Advances the current turn state. Will swap control between players.
		/// </summary>
		public void AdvanceTurnState()
		{
			// Only advance if players haven't all achieved victory
			if (!AllPlayersDone() &&
				!gameManager.GameOver)
			{
				switch (CurrentTurnState)
				{
					case PlayerTurnState.Starting:
						ChangeTurnState(PlayerTurnState.Running);
						break;
					case PlayerTurnState.Running:
						ChangeTurnState(PlayerTurnState.Ending);
						break;
					case PlayerTurnState.Ending:
						ChangePlayer(GetNextPlayer());
						ChangeTurnState(PlayerTurnState.Starting);
						break;
				}
			}
		}


		/// <summary>
		/// Gets whether all players have some victory state achieved
		/// </summary>
		/// <returns>True if all players have a victory state that is not <see cref="VictoryType.None"/></returns>
		public bool AllPlayersDone()
		{
			return !Players.Any(t => t.VictoryState == VictoryType.None);
		}


		/// <summary>
		/// Tick current player if move queue is empty. Do not call directly
		/// </summary>
		internal virtual void Tick()
		{
			if (gameManager.MoveManager.Done &&
				CurrentTurnState == PlayerTurnState.Running)
			{
				CurrentPlayer.Tick();
			}
		}


		/// <summary>
		/// Change the currently active player
		/// </summary>
		protected void ChangePlayer(Player<TPoint, TPieceSettings> newPlayer)
		{
			var previousPlayer = CurrentPlayer;

			CurrentPlayer = newPlayer;
			if (onPlayerChange != null)
			{
				onPlayerChange(previousPlayer, CurrentPlayer);
			}
		}


		/// <summary>
		/// Cause a change in turn state. Will not affect current player
		/// </summary>
		protected void ChangeTurnState(PlayerTurnState newTurnState)
		{
			PlayerTurnState prevState = CurrentTurnState;

			CurrentTurnState = newTurnState;
			if (onTurnStateChange != null)
			{
				onTurnStateChange(prevState, CurrentTurnState);
			}

			switch (CurrentTurnState)
			{
				case PlayerTurnState.Starting:
					CurrentPlayer.OnTurnStart();
					break;
				case PlayerTurnState.Ending:
					CurrentPlayer.OnTurnEnd();
					break;
			}
		}


		/// <summary>
		/// Called when a player's state changes
		/// </summary>
		protected void OnPlayerVictory(Player<TPoint, TPieceSettings> player, VictoryType newState)
		{
			if (onPlayerVictoryStateChange != null)
			{
				onPlayerVictoryStateChange(player, newState);
			}
		}
		#endregion
	}
}
