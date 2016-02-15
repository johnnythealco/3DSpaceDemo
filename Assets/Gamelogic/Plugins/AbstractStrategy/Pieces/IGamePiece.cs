using UnityEngine;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Interface implemented by visual game piece objects
	/// </summary>
	[Version(1)]
	public interface IGamePiece
	{
		/// <summary>
		/// Accessor for game piece properties
		/// </summary>
		IPieceProperties PieceProperties { get; }

		/// <summary>
		/// Move this piece to a new position.
		/// </summary>
		/// <remarks>This may be an intermediary position during animation</remarks>
		/// <param name="newPosition">The new position</param>
		void MovePiece(Vector3 newPosition);
	}
}
