using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Base class for logic that will create add animations for pieces
	/// </summary>
	[Version(1)]
	public abstract class AddAnimationCreator : GLMonoBehaviour
	{
		#region Methods
		/// <summary>
		/// Creates an animator component that'll add a piece to the game board
		/// </summary>
		public abstract IMoveAnimator GetAddAnimator(IGamePiece gamePiece, Vector3 position);
		#endregion
	}
}
