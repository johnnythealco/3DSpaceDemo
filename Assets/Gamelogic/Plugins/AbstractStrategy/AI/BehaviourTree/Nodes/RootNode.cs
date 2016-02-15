using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// Class for the root node of the behaviour tree. Has exactly one child and never has a parent.
	/// Cannot be removed from the tree, and doesn't perform any action except simply ticking its child repeatedly.
	/// </summary>
	[Version(1)]
	public sealed class RootNode : BTNode
	{
		public override int MaxChildren
		{
			get
			{
				return 1;
			}
		}

		protected override BTNodeState DoLogic(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			// Tick our one child, if we have him, and return his state
			if (children.Count > 0)
			{
				return children[0].Behave(blackboard, agent, manager);
			}

			return BTNodeState.Error;
		}
	}
}
