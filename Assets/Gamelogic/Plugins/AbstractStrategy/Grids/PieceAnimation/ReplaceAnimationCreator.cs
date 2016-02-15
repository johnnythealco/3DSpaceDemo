using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Base class for logic that will create add animations for pieces
	/// </summary>
	[Version(1, 1)]
	public abstract class ReplaceAnimationCreator : GLMonoBehaviour
	{
		#region Methods
		/// <summary>
		/// Creates an animator component that'll replace a piece to the game board
		/// </summary>
		public abstract IMoveAnimator GetReplaceAnimator(IGamePiece replacedPiece, IGamePiece replacingPiece, Vector3 position);
		#endregion
	}
}
