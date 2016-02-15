using System;
using System.Collections.Generic;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Class containing a collection of grid utility algorithms
	/// </summary>
	public static class GridAlgorithms
	{

		public static IEnumerable<TPoint> LineIterator<TPoint>(this IGrid<TPoint> grid, TPoint start, TPoint dir) 
			where TPoint : IVectorPoint<TPoint>, IGridPoint<TPoint>
		{
			var currentPoint = start;

			while (grid.Contains(currentPoint))
			{
				yield return currentPoint;
				
				currentPoint = currentPoint.Translate(dir);
			}
		}

		public static IEnumerable<TPoint> SequenceIterator<TPoint>(this IGrid<TPoint> grid, TPoint start, Func<TPoint, TPoint> next)
			where TPoint : IVectorPoint<TPoint>, IGridPoint<TPoint>
		{
			var currentPoint = start;

			while (grid.Contains(currentPoint))
			{
				yield return currentPoint;

				currentPoint = next(currentPoint);
			}
		}

		

		public static TPoint NextPointInLineUniform<TPoint>(TPoint currentPoint, int directionIndex, IList<TPoint> directions)
			where TPoint : IVectorPoint<TPoint>
		{
			return NextPointInLineUniform(currentPoint, directionIndex, true, directions);
		}

		public static TPoint NextPointInLineUniform<TPoint>(TPoint currentPoint, int directionIndex, bool forwards,
			IList<TPoint> directions)
			where TPoint : IVectorPoint<TPoint>
		{
			if (forwards)
			{
				return currentPoint.Translate(directions[directionIndex]);
			}
			else
			{
				return currentPoint.Translate(directions[directionIndex].Negate());
			}
		}

#if GridsPro
		private static PointyTriPoint NextPointInLinePointyTri(PointyTriPoint currentPoint, int directionIndex)
		{
			return NextPointInLinePointyTri(currentPoint, directionIndex, true);
		}


		private static PointyTriPoint NextPointInLinePointyTri(PointyTriPoint currentPoint, int directionIndex, bool forwards)
		{
			switch (forwards ? directionIndex : (directionIndex + 3%6))
			{
				case 0:
					return currentPoint.I == 0
						? currentPoint.MoveBy(new PointyTriPoint(1, 0, 1))
						: currentPoint.MoveBy(new PointyTriPoint(0, 0, 1));
				case 1:
					return currentPoint.I == 0
						? currentPoint.MoveBy(new PointyTriPoint(0, 0, 1))
						: currentPoint.MoveBy(new PointyTriPoint(0, 1, 1));
				case 2:
					return currentPoint.I == 0
						? currentPoint.MoveBy(new PointyTriPoint(-1, 0, 1))
						: currentPoint.MoveBy(new PointyTriPoint(0, 1, 1));
				case 3:
					return currentPoint.I == 0
						? currentPoint.MoveBy(new PointyTriPoint(-1, 0, 1))
						: currentPoint.MoveBy(new PointyTriPoint(0, 0, 1));
				case 4:
					return currentPoint.I == 0
						? currentPoint.MoveBy(new PointyTriPoint(0, -1, 1))
						: currentPoint.MoveBy(new PointyTriPoint(0, 0, 1));
				case 5:
				default:
					return currentPoint.I == 0
						? currentPoint.MoveBy(new PointyTriPoint(0, -1, 1))
						: currentPoint.MoveBy(new PointyTriPoint(1, 0, 1));
			}
		}

		private static FlatTriPoint NextPointInLineFlatTri(FlatTriPoint currentPoint, int directionIndex, bool forwards)
		{
			switch (forwards ? directionIndex : (directionIndex + 3)%6)
			{
				case 0:
					return currentPoint.I == 0
						? currentPoint.MoveBy(new FlatTriPoint(0, 0, 1))
						: currentPoint.MoveBy(new FlatTriPoint(1, 0, 1));
				case 1:
					return currentPoint.I == 0
						? currentPoint.MoveBy(new FlatTriPoint(0, 0, 1))
						: currentPoint.MoveBy(new FlatTriPoint(0, 1, 1));
				case 2:
					return currentPoint.I == 0
						? currentPoint.MoveBy(new FlatTriPoint(-1, 0, 1))
						: currentPoint.MoveBy(new FlatTriPoint(0, 1, 1));
				case 3:
					return currentPoint.I == 0
						? currentPoint.MoveBy(new FlatTriPoint(-1, 0, 1))
						: currentPoint.MoveBy(new FlatTriPoint(0, 0, 1));
				case 4:
					return currentPoint.I == 0
						? currentPoint.MoveBy(new FlatTriPoint(0, -1, 1))
						: currentPoint.MoveBy(new FlatTriPoint(0, 0, 1));
				case 5:
				default:
					return currentPoint.I == 0
						? currentPoint.MoveBy(new FlatTriPoint(0, -1, 1))
						: currentPoint.MoveBy(new FlatTriPoint(1, 0, 1));
			}
		}
#endif
	}
}
