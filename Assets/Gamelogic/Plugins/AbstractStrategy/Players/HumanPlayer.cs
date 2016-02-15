using Gamelogic.AbstractStrategy.Grids;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Class to represent a human player, who makes decisions on how to act. You will probably
	/// not use this class directly, and instead use the non-generic variant
	/// <see cref="GridHumanPlayer"/>
	/// </summary>
	[Version(1)]
	public class HumanPlayer<TGameState, TPoint, TPieceSettings> : Player<TPoint, TPieceSettings>
		where TPieceSettings : PieceSettings<TPoint, TPieceSettings>
	{
		#region Fields
		/// <summary>
		/// Set this to false to not have this class automatically advance from
		/// <see cref="PlayerTurnState.Starting"/> and
		/// <see cref="PlayerTurnState.Ending"/> states.
		/// </summary>
		public bool autoAdvanceTurn = true;
		#endregion


		#region Constructor
		/// <summary>
		/// Create instance of a human player
		/// </summary>
		public HumanPlayer(string id) : base(id) { }
		#endregion


		public override void OnTurnStart()
		{
			base.OnTurnStart();

			// Automatically progress turn state
			if (autoAdvanceTurn)
			{
				gameManager.TurnManager.AdvanceTurnState();
			}
		}


		public override void OnTurnEnd()
		{
			base.OnTurnEnd();

			if (autoAdvanceTurn)
			{
				gameManager.TurnManager.AdvanceTurnState();
			}
		}
	}
}
