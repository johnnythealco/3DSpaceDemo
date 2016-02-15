namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Interface for any class that handles animation of a piece during a move
	/// </summary>
	[Version(1)]
	public interface IMoveAnimator
	{
		/// <summary>
		/// Gets whether this animation has completed
		/// </summary>
		bool AnimationFinished { get; }


		/// <summary>
		/// Start the animation
		/// </summary>
		void Start();


		/// <summary>
		/// Perform animation updates
		/// </summary>
		void Update();


		/// <summary>
		/// Position in final location
		/// </summary>
		void Finished();
	}
}
