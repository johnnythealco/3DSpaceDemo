namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// A composite node in the behaviour tree has multiple children and ticks them in 
	/// some order
	/// </summary>
	public abstract class CompositeNode : BTNode
	{
		public override int MaxChildren
		{
			get
			{
				return int.MaxValue;
			}
		}
	}
}
