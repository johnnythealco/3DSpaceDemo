using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// Place a piece at a position. Will return <see cref="BTNodeState.Running"/> if the move is valid
	/// or <see cref="BTNodeState.Failure"/> if it is not
	/// </summary>
	[GraphEditor("Place piece randomly")]
	[Version(1)]
	public class PlacePieceRandomly : BTNode
	{
		public string pieceID;
		public RectGrid<ICell> grid;

		public override int MaxChildren
		{
			get
			{
				return 0;
			}
		}

		protected override BTNodeState DoLogic(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			//Get all points in the grid
			var points = manager.State.GetAllPoints();
		
			//Choose a random one
            var destination = points.RandomItem();

			var rules = manager.Rules;

			//Make a new piece to place
			var piece = rules.CreatePieceProperties(rules.GetPieceSettings(pieceID, agent.PlayerID), agent);

			//Create the move that will add the piece
			var move = manager.State.CreateAddPieceMove(piece, destination);

			//Commit the move to the move manager
			if (manager.CommitMove(move))
			{
				return BTNodeState.Running;
			}
			else
			{
				return BTNodeState.Failure;
			}
		}
	}
}