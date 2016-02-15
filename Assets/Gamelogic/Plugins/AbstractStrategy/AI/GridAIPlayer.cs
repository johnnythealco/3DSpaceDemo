using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// AI player for grid type game states, that uses behaviour trees
	/// </summary>
	[Version(1)]
	public class GridAIPlayer : Player<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		/// <summary>
		/// Our behaviour tree
		/// </summary>
		public BehaviourTree behaviourTree;

		/// <summary>
		/// Our blackboard
		/// </summary>
		private Blackboard blackboard;
		#endregion


		#region Constructor
		/// <summary>
		/// Create instance of a grid human player
		/// </summary>
		public GridAIPlayer(string id, BehaviourTree ai) : base(id) 
		{
			behaviourTree = ai;
			blackboard = new Blackboard();
		}
		#endregion


		#region Methods
		public override void OnTurnStart()
		{
			base.OnTurnStart();

			// Automatically progress turn state to playing state
			gameManager.TurnManager.AdvanceTurnState();
		}


		public override void OnTurnEnd()
		{
			base.OnTurnEnd();

			gameManager.TurnManager.AdvanceTurnState();
		}


		public override void Tick()
		{
			Behave();
		}


		/// <summary>
		/// AI player decision making process, defers to behaviour tree
		/// </summary>
		protected virtual void Behave()
		{
			behaviourTree.Behave(blackboard, this, gameManager);
		}


		public override void StartGame()
		{
			base.StartGame();
			blackboard.ResetForNewGame();
		}
		#endregion
	}
}
