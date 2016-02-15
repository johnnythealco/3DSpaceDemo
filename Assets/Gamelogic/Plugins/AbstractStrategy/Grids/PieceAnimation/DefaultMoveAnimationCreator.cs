using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Object that creates linear move animations for objects when moving between tiles
	/// </summary>
	[Version(1)]
	public class DefaultMoveAnimationCreator : MoveAnimationCreator
	{
		#region Fields
		public float duration = 0.5f;
		#endregion

		public override IMoveAnimator GetMoveAnimator(IGamePiece gamePiece, Vector3 initialPosition, Vector3 destinationPosition)
		{
			return new LinearTranslationAnimator(gamePiece, initialPosition, destinationPosition, duration);
		}
	}
}
