using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// The inverter node ticks its child and negates the child's return result.
	/// </summary>
	/// <remarks><see cref="BTNodeState.Sucess"/> will become <see cref="BTNodeState.Failure"/>, 
	/// and <see cref="BTNodeState.Failure"/> will become <see cref="BTNodeState.Sucess"/>.
	/// The inverter will not change <see cref="BTNodeState.Running"/> or <see cref="BTNodeState.Error"/>
	/// results.</remarks>
	[GraphEditor("Inverter")]
	[Version(1)]
	public class Inverter : DecoratorNode
	{
		protected override BTNodeState DoLogic(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			// Tick our one child, if we have him, and return his state
			if (children.Count > 0)
			{
				var result = children[0].Behave(blackboard, agent, manager);
				switch (result)
				{
					case BTNodeState.Failure:
						return BTNodeState.Success;
					case BTNodeState.Success:
						return BTNodeState.Failure;
					default:
						return result;
				}
			}
			return BTNodeState.Error;
		}
	}
}
