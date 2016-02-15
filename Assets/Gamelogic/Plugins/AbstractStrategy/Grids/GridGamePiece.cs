using System;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Properties for grid game pieces
	/// </summary>
	[Version(1)]
	public class GridGamePieceProperties : IPieceProperties
	{
		public String pieceID { get; private set; }
		public uint uniqueID { get; private set;  }
		public String playerID { get; private set; }

		public GridGamePieceProperties(String pieceID, uint uniqueID, String playerID)
		{
			this.pieceID = pieceID;
			this.uniqueID = uniqueID;
			this.playerID = playerID;
		}


		#region Equatable
		public bool Equals(IPieceProperties other)
		{
			if (other == null)
				return false;

			GridGamePieceProperties piece = other as GridGamePieceProperties;
			if (piece == null)
				return false;

			return other.pieceID == pieceID &&
				other.uniqueID == uniqueID &&
				other.playerID == playerID;
		}


		public override bool Equals(object obj)
		{
			var piece = obj as GridGamePieceProperties;

			if (piece != null) return Equals(piece);

			return false;
		}


		public override int GetHashCode()
		{
			unchecked
			{
				int result = pieceID.GetHashCode();
				result = (17 * result) ^ uniqueID.GetHashCode();
				result = (23 * result) ^ playerID.GetHashCode();
				return result;
			}
		}
		#endregion
	}


	/// <summary>
	/// Concrete Grid-based implementation of a game piece
	/// </summary>
	[Version(1)]
	public class GridGamePiece : GLMonoBehaviour, IGamePiece
	{
		#region Properties
		public IPieceProperties PieceProperties { get; protected set; }
		#endregion


		#region Methods
		public virtual void MovePiece(Vector3 newPosition)
		{
			transform.position = newPosition;
		}
		#endregion
	}
}
