using Gamelogic.Grids;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.Grids
{
	/// <summary>
	/// Move that adds a piece to the game board
	/// </summary>
	[Version(1)]
	public class AddPieceMove : WaitForAnimationMove<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		private readonly RectPoint position;
		private readonly IPieceProperties piece;
		#endregion


		#region Properties
		/// <summary>
		/// Gets the destination for this add move
		/// </summary>
		public RectPoint Position { get { return position; } }
		/// <summary>
		/// The piece we're operating on
		/// </summary>
		public IPieceProperties Piece { get { return piece; } }
		#endregion


		#region Constructor
		public AddPieceMove(IGameManager<RectPoint, GridGamePieceSettings> gameManager, RectPoint position, IPieceProperties piece, IMoveAnimator animator)
			: base(gameManager, animator)
		{
			this.position = position;
			this.piece = piece;
		}
		#endregion


		#region Methods
		public override void Start(MoveMode mode)
		{
			if (mode == MoveMode.Forward)
			{
				gameManager.CreateVisualPiece(piece);
			}
			animator = gameManager.GetAddAnimator(piece, mode, position);

			base.Start(mode);
		}


		public override void OnMoveEnd()
		{
			base.OnMoveEnd();

			// Commit move to state
			if (Direction == MoveMode.Forward)
			{
				gameManager.State.PlaceGamePiece(position, piece);
			}
			else
			{
				gameManager.State.RemoveGamePiece(position, piece);
			}
		}
		#endregion


		#region Equatable
		public override bool Equals(GameMove<RectPoint, GridGamePieceSettings> other)
		{
			if (other == null)
				return false;

			var apm = other as AddPieceMove;

			if (apm == null)
				return false;

			return apm.position == position &&
				// Does not compare uniqueID, since moves that add the same piece ID for the same
				// player are safe to consider equivalent even when the uniqueID differs
				apm.piece.playerID == piece.playerID &&
				apm.piece.pieceID == piece.pieceID;
		}


		public override int GetHashCode()
		{
			unchecked
			{
				int result = position.GetHashCode();
				result = (17 * result) ^ piece.playerID.GetHashCode();
				result = (17 * result) ^ piece.pieceID.GetHashCode();
				return result;
			}
		}
		#endregion
	}


	/// <summary>
	/// Move that removes a piece to the game board
	/// </summary>
	[Version(1)]
	public class RemovePieceMove : WaitForAnimationMove<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		private readonly RectPoint position;
		private readonly IPieceProperties piece;
		#endregion


		#region Properties
		/// <summary>
		/// Gets the source for this remove move
		/// </summary>
		public RectPoint Position { get { return position; } }
		/// <summary>
		/// The piece we're operating on
		/// </summary>
		public IPieceProperties Piece { get { return piece; } }
		#endregion


		#region Constructor
		public RemovePieceMove(IGameManager<RectPoint, GridGamePieceSettings> gameManager, RectPoint position, IPieceProperties piece, IMoveAnimator animator)
			: base(gameManager, animator)
		{
			this.position = position;
			this.piece = piece;
		}
		#endregion


		#region Methods
		public override void Start(MoveMode mode)
		{
			if (mode == MoveMode.Backwards)
			{
				gameManager.CreateVisualPiece(piece);
			}
			animator = gameManager.GetRemoveAnimator(piece, Direction, position); 

			base.Start(mode);
		}

		public override void OnMoveEnd()
		{
			base.OnMoveEnd();

			// Commit move to state
			if (Direction == MoveMode.Forward)
			{
				gameManager.State.RemoveGamePiece(position, piece);
			}
			else
			{
				gameManager.State.PlaceGamePiece(position, piece);
			}
		}
		#endregion


		#region Equatable
		public override bool Equals(GameMove<RectPoint, GridGamePieceSettings> other)
		{
			if (other == null)
				return false;

			var rpm = other as RemovePieceMove;

			if (rpm == null)
				return false;

			return rpm.position == position &&
				rpm.piece.Equals(piece);
		}


		public override int GetHashCode()
		{
			unchecked
			{
				int result = position.GetHashCode();
				result = (17 * result) ^ piece.GetHashCode();
				return result;
			}
		}
		#endregion
	}


	/// <summary>
	/// Move that moves a piece from the game board to another position
	/// </summary>
	[Version(1)]
	public class MovePieceMove : WaitForAnimationMove<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		private readonly RectPoint originalPosition;
		private readonly RectPoint destinationPosition;
		private readonly IPieceProperties piece;
		#endregion


		#region Properties
		/// <summary>
		/// Gets the destination for this move
		/// </summary>
		public RectPoint Destination { get { return destinationPosition; } }

		/// <summary>
		/// Gets the source for this move
		/// </summary>
		public RectPoint Source { get { return originalPosition; } }

		/// <summary>
		/// The piece we're operating on
		/// </summary>
		public IPieceProperties Piece { get { return piece; } }
		#endregion


		#region Constructor
		public MovePieceMove(IGameManager<RectPoint, GridGamePieceSettings> gameManager, RectPoint originalPosition, IPieceProperties piece, RectPoint destinationPosition, IMoveAnimator animator)
			: base(gameManager, animator)
		{
			this.originalPosition = originalPosition;
			this.destinationPosition = destinationPosition;
			this.piece = piece;
		}
		#endregion


		#region Methods
		public override void Start(MoveMode mode)
		{
			animator = gameManager.GetMoveAnimator(piece, mode, originalPosition, destinationPosition);

			base.Start(mode);
		}


		public override void OnMoveEnd()
		{
			base.OnMoveEnd();

			// Commit move to state
			if (Direction == MoveMode.Forward)
			{
				gameManager.State.MoveGamePiece(originalPosition, piece, destinationPosition);
			}
			else
			{
				gameManager.State.MoveGamePiece(destinationPosition, piece, originalPosition);
			}
		}
		#endregion


		#region Equatable
		public override bool Equals(GameMove<RectPoint, GridGamePieceSettings> other)
		{
			if (other == null)
				return false;

			var mpm = other as MovePieceMove;

			if (mpm == null)
				return false;

			return mpm.originalPosition == originalPosition &&
				mpm.destinationPosition == destinationPosition &&
				mpm.piece.Equals(piece);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = originalPosition.GetHashCode();
				result = (17 * result) ^ destinationPosition.GetHashCode();
				result = (17 * result) ^ piece.GetHashCode();
				return result;
			}
		}
		#endregion
	}


	/// <summary>
	/// Move for replacing a piece with another, as a single action. A combination of
	/// removing and adding pieces.
	/// </summary>
	[Version(1, 1)]
	public class ReplacePieceMove : WaitForAnimationMove<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		private readonly RectPoint position;
		private readonly IPieceProperties originalPiece;
		private readonly IPieceProperties replacementPiece;
		#endregion


		#region Properties
		/// <summary>
		/// Gets the tile this move operates on
		/// </summary>
		public RectPoint Position { get { return position; } }

		/// <summary>
		/// Gets the original piece
		/// </summary>
		public IPieceProperties Original { get { return originalPiece; } }

		/// <summary>
		/// The replacement piece
		/// </summary>
		public IPieceProperties Replacement { get { return replacementPiece; } }
		#endregion


		#region Constructor
		public ReplacePieceMove(IGameManager<RectPoint, GridGamePieceSettings> gameManager, RectPoint position,
			IPieceProperties originalPiece, IPieceProperties replacementPiece, IMoveAnimator animator)
			: base(gameManager, animator)
		{
			this.position = position;
			this.originalPiece = originalPiece;
			this.replacementPiece = replacementPiece;
		}
		#endregion


		#region Methods
		public override void Start(MoveMode mode)
		{
			if (mode == MoveMode.Forward)
			{
				gameManager.CreateVisualPiece(replacementPiece);
			}
			if (mode == MoveMode.Backwards)
			{
				gameManager.CreateVisualPiece(originalPiece);
			}

			animator = gameManager.GetReplaceAnimator(originalPiece, replacementPiece, mode, position);

			base.Start(mode);
		}


		public override void OnMoveEnd()
		{
			base.OnMoveEnd();

			// Commit move to state
			if (Direction == MoveMode.Forward)
			{
				gameManager.State.RemoveGamePiece(position, originalPiece);
				gameManager.State.PlaceGamePiece(position, replacementPiece);
			}
			else
			{
				gameManager.State.RemoveGamePiece(position, replacementPiece);
				gameManager.State.PlaceGamePiece(position, originalPiece);
			}
		}
		#endregion


		#region Equatable
		public override bool Equals(GameMove<RectPoint, GridGamePieceSettings> other)
		{
			if (other == null)
				return false;

			var rpm = other as ReplacePieceMove;

			if (rpm == null)
				return false;

			return rpm.position == position &&
				rpm.originalPiece.Equals(originalPiece) &&
				// Does not compare uniqueID, since moves that add the same piece ID for the same
				// player are safe to consider equivalent even when the uniqueID differs
				rpm.replacementPiece.pieceID == replacementPiece.pieceID &&
				rpm.replacementPiece.playerID == replacementPiece.playerID;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = position.GetHashCode();
				result = (17 * result) ^ originalPiece.GetHashCode();
				result = (17 * result) ^ replacementPiece.playerID.GetHashCode();
				result = (17 * result) ^ replacementPiece.pieceID.GetHashCode();
				return result;
			}
		}
		#endregion
	}


	/// <summary>
	/// Move for capturing a piece with another, as a single action. A combination of
	/// moving and adding pieces.
	/// </summary>
	[Version(1, 1)]
	public class CapturePieceMove : WaitForAnimationMove<RectPoint, GridGamePieceSettings>
	{
		#region Fields
		private readonly RectPoint originalPosition;
		private readonly RectPoint destinationPosition;
		private readonly RectPoint capturedPosition;
		private readonly IPieceProperties capturedPiece;
		private readonly IPieceProperties capturingPiece;
		#endregion


		#region Properties
		/// <summary>
		/// Gets the tile the capturing piece moves from
		/// </summary>
		public RectPoint OriginalPosition { get { return originalPosition; } }

		/// <summary>
		/// Gets the tile the capturing piece moves from
		/// </summary>
		public RectPoint DestinationPosition { get { return destinationPosition; } }

		/// <summary>
		/// Gets the tile of the captured piece
		/// </summary>
		public RectPoint CapturedPosition { get { return capturedPosition; } }

		/// <summary>
		/// Gets the original piece
		/// </summary>
		public IPieceProperties CapturedPiece { get { return capturedPiece; } }

		/// <summary>
		/// The replacement piece
		/// </summary>
		public IPieceProperties CapturingPiece { get { return capturingPiece; } }
		#endregion


		#region Constructor
		public CapturePieceMove(IGameManager<RectPoint, GridGamePieceSettings> gameManager, RectPoint originalPosition, RectPoint destinationPosition,
			RectPoint capturedPosition, IPieceProperties capturedPiece, IPieceProperties capturingPiece, 
			IMoveAnimator animator)

			: base(gameManager, animator)
		{
			this.originalPosition = originalPosition;
			this.destinationPosition = destinationPosition;
			this.capturedPosition = capturedPosition;
			this.capturedPiece = capturedPiece;
			this.capturingPiece = capturingPiece;
		}
		#endregion


		#region Methods
		public override void Start(MoveMode mode)
		{
			animator = gameManager.GetCaptureAnimator(capturingPiece, capturedPiece, mode, originalPosition, destinationPosition, capturedPosition);

			if (mode == MoveMode.Backwards)
			{
				gameManager.CreateVisualPiece(CapturedPiece);
			}

			base.Start(mode);
		}


		public override void OnMoveEnd()
		{
			base.OnMoveEnd();

			// Commit move to state
			if (Direction == MoveMode.Forward)
			{
				gameManager.State.RemoveGamePiece(capturedPosition, capturedPiece);
				gameManager.State.MoveGamePiece(originalPosition, capturingPiece, destinationPosition);
			}
			else
			{
				gameManager.State.MoveGamePiece(destinationPosition, capturingPiece, originalPosition);
				gameManager.State.PlaceGamePiece(capturedPosition, capturedPiece);
			}
		}
		#endregion


		#region Equatable
		public override bool Equals(GameMove<RectPoint, GridGamePieceSettings> other)
		{
			if (other == null)
				return false;

			var cpm = other as CapturePieceMove;

			if (cpm == null)
				return false;

			return cpm.originalPosition == originalPosition &&
				cpm.destinationPosition == destinationPosition &&
				cpm.capturedPosition == capturedPosition &&
				cpm.capturedPiece.Equals(capturedPiece) &&
				cpm.capturingPiece.Equals(capturingPiece);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = originalPosition.GetHashCode();
				result = (17 * result) ^ destinationPosition.GetHashCode();
				result = (17 * result) ^ capturedPosition.GetHashCode();
				result = (17 * result) ^ capturedPiece.GetHashCode();
				result = (17 * result) ^ capturingPiece.GetHashCode();
				return result;
			}
		}
		#endregion
	}
}
