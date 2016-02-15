using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Base class for logic that will create add animations for pieces
	/// </summary>
	[Version(1, 1)]
	public abstract class CaptureAnimationCreator : GLMonoBehaviour
	{
		#region Methods
		/// <summary>
		/// Creates an animator component that'll replace a piece to the game board
		/// </summary>
		public abstract IMoveAnimator GetCaptureAnimator(MoveMode direction, IGamePiece capturedPiece, IGamePiece capturingPiece,
			Vector3 originalPosition, Vector3 destinationPosition, Vector3 capturedPosition);
		#endregion
	}
}
