using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Diagnostics;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// A sequence node ticks all of its children sequentially, until one of them returns
	/// <see cref="BTNodeState.Failure"/>, <see cref="BTNodeState.Running"/> or
	/// <see cref="BTNodeState.Error"/>. If all children return <see cref="BTNodeState.Success"/>,
	/// then it will return that too.
	/// </summary>
	[GraphEditor("Sequence")]
	[Version(1)]
	public class Sequence : CompositeNode
	{
		protected override BTNodeState DoLogic(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			foreach (var child in children)
			{
				var childResult = child.Behave(blackboard, agent, manager);

				if (childResult != BTNodeState.Success)
				{
					return childResult;
				}
			}

			return BTNodeState.Success;
		}
	}
}
