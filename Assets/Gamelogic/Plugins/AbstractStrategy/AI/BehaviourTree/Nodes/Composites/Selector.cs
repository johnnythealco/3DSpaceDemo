using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// A selector node ticks all of its children sequentially, until one of them returns
	/// <see cref="BTNodeState.Success"/>, <see cref="BTNodeState.Running"/> or
	/// <see cref="BTNodeState.Error"/>. If all children return <see cref="BTNodeState.Failure"/>,
	/// then it will return that too.
	/// </summary>
	[GraphEditor("Selector")]
	[Version(1)]
	public class Selector : CompositeNode
	{
		protected override BTNodeState DoLogic(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			foreach (var child in children)
			{
				var childResult = child.Behave(blackboard, agent, manager);
				if (childResult != BTNodeState.Failure)
				{
					return childResult;
				}
			}

			return BTNodeState.Failure;
		}
	}
}
