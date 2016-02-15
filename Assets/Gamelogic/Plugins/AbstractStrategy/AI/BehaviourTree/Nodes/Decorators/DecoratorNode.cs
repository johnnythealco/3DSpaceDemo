
namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// A decorator node has one child and modifies its return result in some fashion.
	/// </summary>
	public abstract class DecoratorNode : BTNode
	{
		public override int MaxChildren
		{
			get
			{
				return 1;
			}
		}
	}
}
