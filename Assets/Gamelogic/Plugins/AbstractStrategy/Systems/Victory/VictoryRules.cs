using System;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// The types of victory states that can be achieved
	/// </summary>
	[Version(1)]
	public enum VictoryType
	{
		/// <summary>
		/// The game is still in progress
		/// </summary>
		None,
		/// <summary>
		/// A player has achieved victory
		/// </summary>
		Victory,
		/// <summary>
		/// A player has lost
		/// </summary>
		Defeat,
		/// <summary>
		/// The game ended in a draw
		/// </summary>
		Draw,
	}


	/// <summary>
	/// Selects which points in the game loop victory will be checked
	/// </summary>
	[Flags]
	[Version(1)]
	public enum VictoryCheckTime
	{
		/// <summary>
		/// Check for victory when the turn state changes.
		/// </summary>
		/// <seealso cref="PlayerTurnState"/>
		TurnStateChange = 0x1,
		/// <summary>
		/// Check for victory when the current player changes
		/// </summary>
		PlayerChange = 0x2,
		/// <summary>
		/// Check for victory when all queued moves have completed
		/// </summary>
		AllMovesEnd = 0x4,
		/// <summary>
		/// Check for victory when a player's victory state changes
		/// </summary>
		PlayerVictoryStateChange = 0x8,
	}


	/// <summary>
	/// A component that represents a game victory rule.
	/// 
	/// You can disable the behaviour with <see cref="UnityEngine.Behaviour.enabled"/> to temporarily disable a victory rule.
	/// </summary>
	[Version(1)]
	public abstract class VictoryRules<TPoint, TPieceSettings> : GLMonoBehaviour
		where TPieceSettings : PieceSettings<TPoint, TPieceSettings>
	{
		#region Fields
		/// <summary>
		/// The victory kind set by this rule being met
		/// </summary>
		public VictoryType victoryKind = VictoryType.Victory;

		/// <summary>
		/// Set points where these rules will check for victory
		/// </summary>
		[InspectorFlags]
		public VictoryCheckTime victoryCheckPeriods;

		/// <summary>
		/// Stored reference to our game manager
		/// </summary>
		protected IGameManager<TPoint, TPieceSettings> game;
		#endregion


		#region Methods
		/// <summary>
		/// Checks the current game state for victory
		/// </summary>
		protected abstract void CheckVictory();


		/// <summary>
		/// Perform initialization for this rule. Do not call directly
		/// </summary>
		/// <param name="game"></param>
		internal virtual void Initialise(IGameManager<TPoint, TPieceSettings> game)
		{
			this.game = game;
		}


		/// <summary>
		/// Called when players changed. Do not call directly
		/// </summary>
		internal virtual void OnPlayerChange()
		{
			if ((victoryCheckPeriods & VictoryCheckTime.PlayerChange) != 0)
			{
				CheckVictory();
			}
		}


		/// <summary>
		/// Called when turn state changes. Do not call directly
		/// </summary>
		internal virtual void OnTurnStateChange()
		{
			if ((victoryCheckPeriods & VictoryCheckTime.TurnStateChange) != 0)
			{
				CheckVictory();
			}
		}


		/// <summary>
		/// Called when the move manager queue becomes empty after processing moves. Do not call directly
		/// </summary>
		internal virtual void OnAllMovesEnd()
		{
			if ((victoryCheckPeriods & VictoryCheckTime.AllMovesEnd) != 0)
			{
				CheckVictory();
			}
		}


		/// <summary>
		/// Called when players changed. Do not call directly
		/// </summary>
		internal virtual void OnPlayerVictoryChange()
		{
			if ((victoryCheckPeriods & VictoryCheckTime.PlayerVictoryStateChange) != 0)
			{
				CheckVictory();
			}
		}
		#endregion
	}
}
