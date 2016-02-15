using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Object that creates crossfade animations for replacement moves
	/// </summary>
	[Version(1, 1)]
	public class DefaultReplaceAnimationCreator : ReplaceAnimationCreator
	{
		#region Fields
		public float duration = 0.5f;
		public float fadeInAlpha = 1;
		public float fadeOutAlpha = 0;
		public bool sequential;
		#endregion

		public override IMoveAnimator GetReplaceAnimator(IGamePiece replacedPiece, IGamePiece replacingPiece, Vector3 position)
		{
			var fadeInAnim = new FadeAnimator(replacingPiece, position, fadeOutAlpha, fadeInAlpha, duration);
			var fadeOutAnim = new FadeAnimator(replacedPiece, position, fadeInAlpha, fadeOutAlpha, duration);
			return new CompoundAnimator(
				new IMoveAnimator[] { fadeOutAnim, fadeInAnim }, sequential);
		}
	}
}
