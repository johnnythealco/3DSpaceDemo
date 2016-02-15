using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Base class for logic that will create move animations for pieces
	/// </summary>
	[Version(1)]
	public abstract class MoveAnimationCreator : GLMonoBehaviour
	{
		#region Methods
		/// <summary>
		/// Creates an animator component that'll move a game piece to a position on the grid
		/// </summary>
		public abstract IMoveAnimator GetMoveAnimator(IGamePiece gamePiece, Vector3 initialPosition, Vector3 destinationPosition);
		#endregion
	}
}
