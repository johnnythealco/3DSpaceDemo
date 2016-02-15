using System.Collections.Generic;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// Blackboard object, unique to each agent using a behaviour tree, that contains information
	/// for the tree's operation
	/// </summary>
	[Version(1)]
	public class Blackboard : Dictionary<string, object>
	{
		private Dictionary<int, NodeBlackboard> nodeBlackboards;
		/// <summary>
		/// Nodes open in last tick
		/// </summary>
		[Version(1, 1)]
		public List<BTNode> lastOpenNodes;
		/// <summary>
		/// Nodes open in current tick
		/// </summary>
		[Version(1, 1)]
		public List<BTNode> currentOpenNodes;

		public Blackboard()
		{
			nodeBlackboards = new Dictionary<int, NodeBlackboard>();
			lastOpenNodes = new List<BTNode>();
			currentOpenNodes = new List<BTNode>();
		}

		public NodeBlackboard GetNodeBlackboard(int nodeID)
		{
			NodeBlackboard nodeBB;
			if (!nodeBlackboards.TryGetValue(nodeID, out nodeBB))
			{
				nodeBB = new NodeBlackboard();
				nodeBlackboards.Add(nodeID, nodeBB);
			}

			return nodeBB;
		}

		public void ResetForNewGame()
		{
			nodeBlackboards.Clear();
			lastOpenNodes.Clear();
			currentOpenNodes.Clear();
			Clear();
		}
	}

	/// <summary>
	/// Node specific blackboard, for node info
	/// </summary>
	[Version(1)]
	public class NodeBlackboard : Dictionary<string, object>
	{
		/// <summary>
		/// Is this node open
		/// </summary>
		[Version(1, 1)]
		public bool isOpen;
	}
}
