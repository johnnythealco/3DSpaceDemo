using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// A conditional node to test whether a given player can complete a line with
	/// only one placement
	/// </summary>
	[GraphEditor("Can Make Line Condition")]
	[Version(1)]
	public class CanMakeLine : ConditionalNode
	{
		/// <summary>
		/// The required length of a line
		/// </summary>
		public int lineLength;
		/// <summary>
		/// The player to test for
		/// </summary>
		public string playerID;
		/// <summary>
		/// The piece ID to test for. Will match any piece if null
		/// </summary>
		public string pieceID;
		/// <summary>
		/// The variable name in the blackboard into which we will save the 
		/// </summary>
		public string variableName;


		protected override bool TestCondition(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			var state = manager.State;
			var rules = manager.Rules;

			foreach (var point in state.GetAllPoints())
			{
				RectPoint missingPoint;
				if (rules.CanMakeLine(point, playerID, lineLength, out missingPoint))
				{
					if (variableName != null)
					{
						blackboard[variableName] = missingPoint;
					}
					return true;
				}
			}

			return false;
		}
	}
}
