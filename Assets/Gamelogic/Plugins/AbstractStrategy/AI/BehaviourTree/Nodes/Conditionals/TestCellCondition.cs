using System.Collections.Generic;
using System.Linq;
using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// A conditional node to test the contents of a cell or a collection of cells on the grid.
	/// The last passed node optionally saves its position into <see cref="TestCellCondition.pointVariable"/> in the blackboard 
	/// for other nodes to operate on
	/// </summary>
	[GraphEditor("Test cell condition")]
	[Version(1)]
	public class TestCellCondition : ConditionalNode
	{
		#region Fields
		/// <summary>
		/// Set to true to have the condition only return true if all points match. Otherwise it
		/// succeeds if at least one point does
		/// </summary>
		public bool matchAll;
		/// <summary>
		/// The points to test. If this is empty, test all points on the grid
		/// </summary>
		public InspectableVectorPoint[] testPoints;

		/// <summary>
		/// The id of pieces to test for at the given point. Leave blank to match any piece
		/// </summary>
		public string pieceID;
		/// <summary>
		/// The id of the owning player to test for at the given point. Leave blank to match
		/// any player's pieces
		/// </summary>
		public string playerID;

		/// <summary>
		/// Variable name in the blackboard into which we can save the last tested point,
		/// regardless of whether or not the test passes.
		/// </summary>
		public string pointVariable;
		#endregion


		protected override bool TestCondition(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			RectPoint lastPoint = RectPoint.Zero;

			foreach (var point in getAllTestPoints(manager))
			{
				var pointMatched = manager.State.TestPosition(point, pieceID, playerID);

				if ((pointMatched != null) && !matchAll)
				{
					if (!string.IsNullOrEmpty(pointVariable))
					{
						blackboard[pointVariable] = point;
					}
					return true;
				}
				else if ((pointMatched == null) && matchAll)
				{
					if (!string.IsNullOrEmpty(pointVariable))
					{
						blackboard[pointVariable] = point;
					}
					return false;
				}

				lastPoint = point;
			}

			if (matchAll)
			{
				if (!string.IsNullOrEmpty(pointVariable))
				{
					blackboard[pointVariable] = lastPoint;
				}
				return true;
			}
			else
			{
				if (!string.IsNullOrEmpty(pointVariable))
				{
					blackboard[pointVariable] = lastPoint;
				}
				return false;
			}
		}

		private IEnumerable<RectPoint> getAllTestPoints(IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			if (testPoints.Length == 0)
			{
				return manager.State.GetAllPoints();
			}
			else
			{
				return testPoints.Select(t => t.GetRectPoint());
			}
		}
	}
}
