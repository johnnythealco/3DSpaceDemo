using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Object that creates a linear translation and fade animation for capture moves
	/// </summary>
	[Version(1, 1)]
	public class DefaultCaptureAnimationCreator : CaptureAnimationCreator
	{
		#region Fields
		public float duration = 0.5f;
		public float fadeInAlpha = 1;
		public float fadeOutAlpha = 0;
		public bool sequential;
		#endregion


		public override IMoveAnimator GetCaptureAnimator(MoveMode direction, IGamePiece capturedPiece, IGamePiece capturingPiece, Vector3 originalPosition, Vector3 destinationPosition, Vector3 capturedPosition)
		{
			if (direction == MoveMode.Forward)
			{
				var fadeAnim = new FadeAnimator(capturedPiece, capturedPosition, fadeInAlpha, fadeOutAlpha, duration);
				var translateAnim = new LinearTranslationAnimator(capturingPiece, originalPosition, destinationPosition, duration);

				return new CompoundAnimator(
					new IMoveAnimator[] { translateAnim, fadeAnim }, sequential);
			}
			else
			{
				var fadeAnim = new FadeAnimator(capturedPiece, capturedPosition, fadeOutAlpha, fadeInAlpha, duration);
				var translateAnim = new LinearTranslationAnimator(capturingPiece, destinationPosition, originalPosition, duration);

				return new CompoundAnimator(
					new IMoveAnimator[] { fadeAnim, translateAnim }, sequential);
			}
		}
	}
}
