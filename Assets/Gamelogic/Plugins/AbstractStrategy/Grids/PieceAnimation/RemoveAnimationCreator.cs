using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Base class for logic that will create remove animations for pieces
	/// </summary>
	[Version(1)]
	public abstract class RemoveAnimationCreator :GLMonoBehaviour
	{
		#region Methods
		/// <summary>
		/// Creates an animator component that'll remove a piece from the game board
		/// </summary>
		public abstract IMoveAnimator GetRemoveAnimator(IGamePiece gamePiece, Vector3 position);
		#endregion
	}
}
