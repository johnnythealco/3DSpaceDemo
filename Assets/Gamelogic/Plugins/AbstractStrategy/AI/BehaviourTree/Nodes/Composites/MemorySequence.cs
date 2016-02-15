using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// A sequence node ticks all of its children sequentially, until one of them returns
	/// <see cref="BTNodeState.Failure"/>, <see cref="BTNodeState.Running"/> or
	/// <see cref="BTNodeState.Error"/>. If all children return <see cref="BTNodeState.Success"/>,
	/// then it will return that too. Additionally, as long as this node remains open,
	/// whenever a child returns <see cref="BTNodeState.Running"/>, on subsequent ticks execution
	/// passes directly to that child instead of ticking previous children again.
	/// </summary>
	[GraphEditor("Memory Sequence")]
	[Version(1, 1)]
	public class MemorySequence : CompositeNode
	{
		protected override void OnOpen(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			base.OnOpen(blackboard, agent, manager);

			var nodeBlackboard = GetNodeBlackboard(blackboard);
			nodeBlackboard["runningNodeIndex"] = 0;
		}

		protected override BTNodeState DoLogic(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			var nodeBlackboard = GetNodeBlackboard(blackboard);
			int index = (int)nodeBlackboard["runningNodeIndex"];

			for (int i = index; i < children.Count; ++i)
			{
				var child = children[i];
				var childResult = child.Behave(blackboard, agent, manager);
				if (childResult != BTNodeState.Success)
				{
					if (childResult == BTNodeState.Running)
					{
						nodeBlackboard["runningNodeIndex"] = i;
					}
					return childResult;
				}
			}

			return BTNodeState.Success;
		}
	}
}
