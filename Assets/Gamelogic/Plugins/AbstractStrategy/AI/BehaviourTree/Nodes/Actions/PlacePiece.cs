using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Diagnostics;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// Place a piece at a position. Will return <see cref="BTNodeState.Running"/> if the move is valid
	/// or <see cref="BTNodeState.Failure"/> if it is not
	/// </summary>
	[GraphEditor("Place piece")]
	[Version(1)]
	public class PlacePiece : BTNode
	{
		public InspectableVectorPoint absolutePosition;
		public string variablePosition;
		public string pieceID;

		public override int MaxChildren
		{
			get
			{
				return 0;
			}
		}

		protected override BTNodeState DoLogic(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			RectPoint destination;
			if (!string.IsNullOrEmpty(variablePosition))
			{
				destination = (RectPoint)blackboard[variablePosition];
			}
			else
			{
				destination = absolutePosition.GetRectPoint();
			}

			var rules = manager.Rules;
			//var state = manager.State;
			var piece = rules.CreatePieceProperties(rules.GetPieceSettings(pieceID, agent.PlayerID), agent);
			var move = manager.State.CreateAddPieceMove(piece, destination);

			if (manager.CommitMove(move))
			{
				//GLDebug.Log("Running" + destination);
				return BTNodeState.Success;
			}
			else
			{
				GLDebug.Log("Failure");
				return BTNodeState.Failure;
			}
		}
	}
}
