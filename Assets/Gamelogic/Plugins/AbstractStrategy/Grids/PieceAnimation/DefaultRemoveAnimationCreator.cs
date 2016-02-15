using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Object that creates linear fade animnation for removed pieces
	/// </summary>
	[Version(1)]
	public class DefaultRemoveAnimationCreator : RemoveAnimationCreator
	{
		#region Fields
		public float duration = 0.5f;
		public float startAlpha = 1;
		public float endAlpha = 0;
		#endregion

		public override IMoveAnimator GetRemoveAnimator(IGamePiece gamePiece, Vector3 position)
		{
			return new FadeAnimator(gamePiece, position, startAlpha, endAlpha, duration);
		}
	}
}
