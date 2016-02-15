using System.Collections.Generic;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Rule object describing how a given type of piece behaves. Implement these and attach them
	/// to <see cref="PieceSettings{TPoint,TSelf}"/> objects to describe behaviours
	/// for specific pieces.
	/// 
	/// You can disable the behaviour with <see cref="UnityEngine.Behaviour.enabled"/> to temporarily disable a rule.
	/// </summary>
	[Version(1)]
	public abstract class PieceRule<TPoint, TPieceSettings> : GLMonoBehaviour
		where TPieceSettings : PieceSettings<TPoint, TPieceSettings>
	{
		#region Methods
		/// <summary>
		/// Returns all valid positions to which a new instance of this piece can be added
		/// </summary>
		/// <param name="state">Current game state to use to evaluate the rules</param>
		public abstract IEnumerable<TPoint> GetValidAddPositions(IGameState<TPoint, TPieceSettings> state);

		/// <summary>
		/// Returns all valid positions to which a given piece can be moved
		/// </summary>
		/// <param name="state">Current game state to use to evaluate the rules</param>
		/// <param name="piece">The piece to move</param>
		/// <param name="initialPosition">The starting position for the piece</param>
		public abstract IEnumerable<TPoint> GetValidDestinationPositions(IGameState<TPoint, TPieceSettings> state, IPieceProperties piece, TPoint initialPosition);

		/// <summary>
		/// Returns all valid pieces this piece can capture
		/// </summary>
		/// <param name="state">Current game state to use to evaluate the rules</param>
		/// <param name="initialPosition">The starting position for the piece</param>
		/// <param name="piece">The piece doing the capturing</param>
		[Version(1, 1)]
		public abstract IEnumerable<IPieceProperties> GetValidCapturePieces(IGameState<TPoint, TPieceSettings> state, IPieceProperties piece, TPoint initialPosition);

		/// <summary>
		/// Returns the destination position a piece must move to in order to capture the given piece
		/// </summary>
		/// <param name="state">Current game state to use to evaluate the rules</param>
		/// <param name="capturePiece">The piece doing the capturing</param>
		/// <param name="capturedPiece">The piece to capture</param>
		/// <param name="initialPosition">Starting location of <paramref name="capturePiece"/></param>
		/// <returns>Destination position for capturing piece</returns>
		[Version(1, 1)]
		public abstract TPoint GetCapturePosition(IGameState<TPoint, TPieceSettings> state, IPieceProperties capturePiece, IPieceProperties capturedPiece, TPoint initialPosition);

		/// <summary>
		/// Returns whether or not a piece's owner can remove a piece from a given point
		/// </summary>
		/// <param name="state">Current game state to use to evaluate the rules</param>
		/// <param name="piece">The piece being removed</param>
		/// <param name="position">The position to remove the piece from</param>
		public abstract bool CanRemovePiece(IGameState<TPoint, TPieceSettings> state, IPieceProperties piece, TPoint position);
		#endregion
	}
}
