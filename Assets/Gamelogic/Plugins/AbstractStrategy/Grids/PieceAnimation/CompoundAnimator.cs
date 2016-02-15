using System.Collections.Generic;
using Gamelogic.Grids;
using UnityEngine;
using System;
using System.Linq;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Compound animation component that contains other sub animations and updates them either in order
	/// or consecutively
	/// </summary>
	[Version(1, 1)]
	public class CompoundAnimator : IMoveAnimator
	{
		#region Fields
		/// <summary>
		/// All sub animators
		/// </summary>
		private readonly IMoveAnimator[] subAnimators;

		/// <summary>
		/// Whether to update animators sequentially
		/// </summary>
		private bool sequential;

		/// <summary>
		/// Current animator being updated if sequential
		/// </summary>
		private int progress;

		/// <summary>
		/// Completion flag
		/// </summary>
		private bool done;
		#endregion


		#region Properties
		public bool AnimationFinished { get { return done; } }
		#endregion


		#region Constructor
		public CompoundAnimator(IEnumerable<IMoveAnimator> animators, bool sequential)
		{
			subAnimators = animators.ToArray();
			this.sequential = sequential;
			done = false;
		}
		#endregion


		#region Animation methods
		public void Start()
		{
			done = false;
			progress = 0;

			foreach (var animator in subAnimators)
			{
				animator.Start();
			}
		}


		public void Update()
		{
			if (!sequential)
			{
				done = true;
				foreach (var animator in subAnimators)
				{
					if (!animator.AnimationFinished)
					{
						animator.Update();
					}

					// Check again, because update might have changed this state
					if (!animator.AnimationFinished)
					{
						done = false;
					}
				}
			}
			else
			{
				if (progress < subAnimators.Length)
				{
					var animator = subAnimators[progress];

					if (!animator.AnimationFinished)
					{
						animator.Update();
					}

					if (animator.AnimationFinished)
					{
						progress++;
					}
				}
				else
				{
					done = true;
				}
			}
		}

		public void Finished()
		{
			foreach (var animator in subAnimators)
			{
				animator.Finished();
			}
		}
		#endregion
	}
}
