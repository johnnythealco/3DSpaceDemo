using System;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Information about a piece. Each piece should have a unique combination of <see cref="pieceID"/> and <see cref="playerID"/>,
	/// and a universally unique <see cref="uniqueID"/>
	/// </summary>
	[Version(1)]
	public interface IPieceProperties : IEquatable<IPieceProperties>
	{
		/// <summary>
		/// Gets the piece type id for this piece This only has to be unique for every <see cref="playerID"/>. That is, 
		/// multiple players can share piece IDs.
		/// </summary>
		String pieceID { get; }
		/// <summary>
		/// Gets a unique ID for this piece.
		/// </summary>
		uint uniqueID { get; }
		/// <summary>
		/// The player ID of the player who owns the given piece
		/// </summary>
		String playerID { get; }
	}
}
