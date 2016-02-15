using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// A conditional node is a leaf node that checks for some game state condition and returns
	/// <see cref="BTNodeState.Success"/> if it is met, or <see cref="BTNodeState.Failure"/> if not
	/// </summary>
	[Version(1)]
	public abstract class ConditionalNode : BTNode
	{
		public override int MaxChildren
		{
			get
			{
				return 0;
			}
		}


		/// <summary>
		/// Implemented logic method that calls <see cref="TestCondition"/> and returns the appropriate state
		/// </summary>
		protected override BTNodeState DoLogic(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			if (TestCondition(blackboard, agent, manager))
			{
				return BTNodeState.Success;
			}
			else
			{
				return BTNodeState.Failure;
			}
		}


		/// <summary>
		/// Implement this in derived classes to test appropriate conditions
		/// </summary>
		protected abstract bool TestCondition(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager);
	}
}