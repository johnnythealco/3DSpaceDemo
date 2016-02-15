using UnityEngine;
using UnityEngine.UI;

namespace Gamelogic.AbstractStrategy
{
	/// <summary>
	/// Component that fades a game piece in or out. Game pieces must contain Sprite Renderer scripts to fade
	/// </summary>
	[Version(1)]
	public class FadeAnimator : IMoveAnimator
	{
		#region Fields
		/// <summary>
		/// The game piece we're moving
		/// </summary>
		private readonly IGamePiece gamePiece;
		/// <summary>
		/// The original position
		/// </summary>
		private readonly Color startColour;
		/// <summary>
		/// Our destination
		/// </summary>
		private readonly Color finalColour;
		/// <summary>
		/// Animation duration
		/// </summary>
		private readonly float duration;
		/// <summary>
		/// Cached tilecell
		/// </summary>
		private readonly IColorable colorable;
		/// <summary>
		/// Position to move to at the start of the animation
		/// </summary>
		private readonly Vector3 position;
		#endregion


		#region Properties
		/// <summary>
		/// Gets the progress for this animation in seconds
		/// </summary>
		public float Progress { get; protected set; }
		public bool AnimationFinished { get { return colorable == null ||
			Progress >= duration - Mathf.Epsilon; } }
		#endregion


		#region Constructor
		public FadeAnimator(IGamePiece gamePiece, Vector3 position, float startAlpha, float finalAlpha, float duration)
		{
			foreach (var c in ((MonoBehaviour)gamePiece).GetComponents(typeof(MonoBehaviour)))
			{
				Debug.Log(c.GetType());
			}

			var gamePieceBehaviour = ((MonoBehaviour) gamePiece);
            var sprite = gamePieceBehaviour.GetComponentInChildren<SpriteRenderer>();

			if (sprite != null)
			{
				colorable = new ColorableSprite(sprite);
			}
			else
			{
				var image = gamePieceBehaviour.GetComponentInChildren<Image>();

				if (image != null)
				{
					colorable = new ColorableImage(image);
				}
				else
				{
					Debug.LogError("Image is null");
				}
			}


			this.gamePiece = gamePiece;
			startColour = colorable.Color;
			startColour.a = startAlpha;
			finalColour = colorable.Color;
			finalColour.a = finalAlpha;
			this.duration = duration;
			this.position = position;
		}
		#endregion


		#region Animation methods
		public void Start()
		{
			Progress = 0;

			if (colorable != null)
			{
				colorable.Color = startColour;
			}

			gamePiece.MovePiece(position);
		}


		public void Update()
		{
			Progress = Mathf.Min(Progress + Time.deltaTime, duration);
			if (colorable != null)
			{
				colorable.Color  = Color.Lerp(startColour, finalColour, Progress / duration);
			}
		}

		public void Finished()
		{
			if (colorable != null)
			{
				colorable.Color = finalColour;
			}
		}
		#endregion
	}

	public interface IColorable
	{
		Color Color { get; set; }
	}

	public class ColorableSprite : IColorable
	{
		private readonly SpriteRenderer sprite;

		public ColorableSprite(SpriteRenderer sprite)
		{
			this.sprite = sprite;
		}

		public Color Color
		{
			get { return sprite.color; }
			set { sprite.color = value; }
		}
	}

	public class ColorableImage : IColorable
	{
		private readonly Image image;

		public ColorableImage(Image image)
		{
			this.image = image;
		}

		public Color Color
		{
			get { return image.color; }
			set { image.color = value; }
		}
	}
}
