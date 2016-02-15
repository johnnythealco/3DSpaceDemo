using System.Linq;
using Gamelogic;
using Gamelogic.AbstractStrategy;
using Gamelogic.AbstractStrategy.AI;
using Gamelogic.AbstractStrategy.Examples;
using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Grids;

[GraphEditor("Find best position")]
public class ChooseBestLegalPositionNode :BTNode
{
	private int[][] ranking =
	{
		new[] {1, 8, 2, 7, 3, 3, 7, 2, 8, 1},
		new[] {8, 2, 7, 3, 6, 6, 3, 7, 2, 8},
		new[] {2, 7, 3, 6, 4, 4, 6, 3, 7, 2},
		new[] {7, 3, 6, 4, 5, 5, 4, 6, 3, 7},
		new[] {3, 6, 4, 5, 0, 0, 5, 4, 6, 3},
		new[] {3, 6, 4, 5, 0, 0, 5, 4, 6, 3},
		new[] {3, 6, 4, 5, 0, 0, 5, 4, 6, 3},
		new[] {7, 3, 6, 4, 5, 5, 4, 6, 3, 7},
		new[] {2, 7, 3, 6, 4, 4, 6, 3, 7, 2},
		new[] {8, 2, 7, 3, 6, 6, 3, 7, 2, 8},
		new[] {1, 8, 2, 7, 3, 3, 7, 2, 8, 1},
	};

	public string variable;

	protected override BTNodeState DoLogic(
		Blackboard blackboard, 
		GridAIPlayer agent, 
		IGameManager<RectPoint, GridGamePieceSettings> manager)
	{

		var validpositions = AddReversiPieceRule.GetValidPositions(manager.State, agent);

		if (validpositions.Any())
		{
			var bestMove = validpositions.MinBy(
				p => ranking[p.X][p.Y]
				);

			blackboard[variable] = bestMove;

			return BTNodeState.Success;
		}

		return BTNodeState.Failure;
	}
}
