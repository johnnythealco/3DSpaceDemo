using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Object that creates linear fade in animation for added pieces
	/// </summary>
	[Version(1)]
	public class DefaultAddAnimationCreator : AddAnimationCreator
	{
		#region Fields
		public float duration = 0.5f;
		public float startAlpha = 0;
		public float endAlpha = 1;
		#endregion

		public override IMoveAnimator GetAddAnimator(IGamePiece gamePiece, Vector3 position)
		{
			return new FadeAnimator(gamePiece, position, startAlpha, endAlpha, duration);
		}
	}
}
