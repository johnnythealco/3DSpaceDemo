using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Structure to hold information about a capture
	/// </summary>
	[Version(1, 1)]
	public struct CaptureInfo<TPoint>
	{
		public IPieceProperties piece;
		public TPoint position;
	}

	/// <summary>
	/// Class to contain settings for each pieces. Attach rules for each piece as additional components
	/// to this script.
	/// </summary>
	/// <seealso cref="PieceRule{TPoint,TPieceSettings}"/>
	[Version(1)]
	public abstract class PieceSettings<TPoint, TSelf> : GLMonoBehaviour
		where TSelf : PieceSettings<TPoint, TSelf>
	{
		#region Fields
		/// <summary>
		/// The player ID of the player who will own this piece
		/// </summary>
		public string owningPlayerID;
		/// <summary>
		/// The unique ID that identifies this piece. Piece IDs can be shared between different 
		/// player IDs, but combinatinos of <see cref="owningPlayerID"/> and this should be unique.
		/// </summary>
		public string pieceID;
		/// <summary>
		/// The physical piece prefab
		/// </summary>
		public GameObject piecePrefab;

		private List<PieceRule<TPoint, TSelf>> rules;
		#endregion


		#region Methods
		/// <summary>
		/// Gets the accumulated valid positions to add this piece to the board
		/// from all of our rules
		/// </summary>
		public virtual IEnumerable<TPoint> GetAddPositions(IGameState<TPoint, TSelf> state)
		{
			GetRules();

			foreach (var rule in rules)
			{
				if (rule.enabled)
				{
					foreach (var position in rule.GetValidAddPositions(state))
					{
						yield return position;
					}
				}
			}
		}


		/// <summary>
		/// Gets the accumulated valid move positions from all of our rules
		/// </summary>
		/// <param name="piece">The piece we're moving</param>
		/// <param name="initialPosition">The original starting position of <paramref name="piece"/></param>
		public virtual IEnumerable<TPoint> GetMovePositions(IGameState<TPoint, TSelf> state, IPieceProperties piece, TPoint initialPosition)
		{
			GetRules();

			foreach (var rule in rules)
			{
				if (rule.enabled)
				{
					foreach (var position in rule.GetValidDestinationPositions(state, piece, initialPosition))
					{
						yield return position;
					}
				}
			}
		}


		/// <summary>
		/// Gets the accumulated valid move positions from all of our rules
		/// </summary>
		/// <param name="piece">The piece we're moving</param>
		/// <param name="initialPosition">The original starting position of <paramref name="piece"/></param>
		[Version(1, 1)]
		public virtual IEnumerable<CaptureInfo<TPoint>> GetCaptures(IGameState<TPoint, TSelf> state, IPieceProperties piece, TPoint initialPosition)
		{
			GetRules();

			foreach (var rule in rules)
			{
				if (rule.enabled)
				{
					foreach (var target in rule.GetValidCapturePieces(state, piece, initialPosition))
					{
						yield return new CaptureInfo<TPoint>()
							{
								piece = target,
								position = rule.GetCapturePosition(state, piece, target, initialPosition)
							};
					}
				}
			}
		}


		/// <summary>
		/// Gets whether any of our rules permits a piece to be removed from a given position
		/// by its owning player
		/// </summary>
		/// <param name="piece">The piece to test</param>
		/// <param name="position">The position the piece is on</param>
		/// <returns>True if any rules permit piece removal</returns>
		public virtual bool CanRemove(IGameState<TPoint, TSelf> state, IPieceProperties piece, TPoint position)
		{
			GetRules();

			foreach (var rule in rules)
			{
				if (rule.enabled)
				{
					if (rule.CanRemovePiece(state, piece, position)) return true;
				}
			}

			return false;
		}


		protected virtual void GetRules()
		{
			if (rules == null)
			{
				// Find all the rules
				rules = new List<PieceRule<TPoint, TSelf>>();
				rules.AddRange(GetComponents<PieceRule<TPoint, TSelf>>());
			}
		}
		#endregion
	}
}
