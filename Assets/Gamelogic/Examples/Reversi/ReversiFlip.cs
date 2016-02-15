using Gamelogic.AbstractStrategy.AI;
using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.Examples
{
	[GraphEditor("Reversi Flip")]
	public class ReversiFlip : BTNode
	{
		public string variablePosition;

		protected override BTNodeState DoLogic(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			var point = (RectPoint) blackboard[variablePosition];
			var gameManager = (GridGameManager) manager;
			var positionsToReplace = Reversi.GetPositionsToReplace(gameManager, point);
		
			foreach (var rectPoint in positionsToReplace)
			{
				var newMove2 = gameManager.State.CreateReplacePieceMove(gameManager.GameGrid[rectPoint][0],
					gameManager.Rules.CreatePieceProperties(gameManager.Rules.GetPieceSettings(Reversi.Player0ID, agent.PlayerID), agent),
					rectPoint);

				gameManager.CommitMove(newMove2, false);
			}

			return BTNodeState.Success;
		}
	}
}