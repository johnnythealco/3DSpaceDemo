using UnityEngine;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Component that animates a specific game piece between two positions linearly
	/// </summary>
	[Version(1)]
	public class LinearTranslationAnimator : IMoveAnimator
	{
		#region Fields
		/// <summary>
		/// The game piece we're moving
		/// </summary>
		private readonly IGamePiece gamePiece;
		/// <summary>
		/// The original position
		/// </summary>
		private readonly Vector3 startPosition;
		/// <summary>
		/// Our destination
		/// </summary>
		private readonly Vector3 finalPosition;
		/// <summary>
		/// Animation duration
		/// </summary>
		private readonly float duration;
		#endregion


		#region Properties
		/// <summary>
		/// Gets the progress for this animation in seconds
		/// </summary>
		public float Progress { get; protected set; }
		public bool AnimationFinished { get { return Progress >= duration - Mathf.Epsilon; } }
		#endregion


		#region Constructor
		public LinearTranslationAnimator(IGamePiece gamePiece, Vector3 currentPosition, Vector3 finalPosition, float duration)
		{
			this.gamePiece = gamePiece;
			this.finalPosition = finalPosition;
			this.startPosition = currentPosition;
			this.duration = duration;
		}
		#endregion


		#region Animation methods
		public void Start()
		{
			Progress = 0;
		}


		public void Update()
		{
			Progress = Mathf.Min(Progress + Time.deltaTime, duration);
			gamePiece.MovePiece(Vector3.Lerp(startPosition, finalPosition, Progress / duration));
		}

		public void Finished()
		{
			gamePiece.MovePiece(finalPosition);
		}
		#endregion
	}
}
