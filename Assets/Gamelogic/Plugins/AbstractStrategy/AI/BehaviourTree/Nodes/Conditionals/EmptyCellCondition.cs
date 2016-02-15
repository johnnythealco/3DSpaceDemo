using System.Linq;
using System.Collections.Generic;
using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// A conditional node to test the contents of a cell or a collection of cells on the grid.
	/// The last tested node optionally saves its position into <see cref="TestCellCondition.pointVariable"/> in the blackboard 
	/// for other nodes to operate on
	/// </summary>
	[GraphEditor("Empty cell condition")]
	[Version(1)]
	public class EmptyCell : ConditionalNode
	{
		#region Fields
		/// <summary>
		/// The points to test. If this is empty, test all points on the grid
		/// </summary>
		public InspectableVectorPoint[] testPoints;

		/// <summary>
		/// Variable name in the blackboard into which we can save the last tested point,
		/// regardless of whether or not the test passes.
		/// </summary>
		public string pointVariable;
		#endregion


		protected override bool TestCondition(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			RectPoint lastPoint = RectPoint.Zero;

			var state = manager.State;

			foreach (var point in getAllTestPoints(manager))
			{
				var empty = state.GetPiecesAtPoint(point).IsEmpty();

				if (empty)
				{
					if (!string.IsNullOrEmpty(pointVariable))
					{
						blackboard[pointVariable] = point;
					}
					return true;
				}

				lastPoint = point;
			}

			if (!string.IsNullOrEmpty(pointVariable))
			{
				blackboard[pointVariable] = lastPoint;
			}
			return false;
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
