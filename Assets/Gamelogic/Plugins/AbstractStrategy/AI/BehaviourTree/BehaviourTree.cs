using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.AbstractStrategy.Grids;
using Gamelogic.Grids;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// A class the represents a computational graph. Each node in this graph
	/// takes some inputs, and calculates outputs, that can in turn be fed as 
	/// inputs into other nodes.
	/// 
	/// All nodes produces outputs as lists. When a node has multiple inputs, 
	/// these are all combined into a single list for the node to operate on.
	/// </summary>
	[Version(1)]
	public class BehaviourTree : ScriptableObject
	{
		#region Private Fields
		//[HideInInspector]
		[SerializeField]
		private int idCounter;

		//[HideInInspector]
		[SerializeField] 
// ReSharper disable once FieldCanBeMadeReadOnly.Local
// Cannot be readonly because it is serialized.
		private List<BTNode> nodes = new List<BTNode>();

		//[HideInInspector]
		[SerializeField] 
		private RootNode root;
		#endregion

		#region Events
		public event Action<BTNode> OnNodeAdded;
		#endregion

		#region Properties
		/// <summary>
		/// Returns all the nodes in this graph.
		/// </summary>
		public List<BTNode> Nodes
		{
			get { return nodes; }
		}

		/// <summary>
		/// Returns the root node of this tree
		/// </summary>
		public RootNode Root
		{
			get { return root; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Creates and adds a new unlinked node to the graph.
		/// </summary>
		/// <param name="nodeType">The type of node to add to the tree.</param>
		/// <param name="initialPosition">The initial position the node will be displayed
		///		in the visual representation.</param>
		/// <returns>The newly created node.</returns>
		public BTNode AddNode(Type nodeType, Vector2 initialPosition)
		{
			if (!nodeType.IsSubclassOf(typeof(BTNode)))
			{
				throw new InvalidOperationException("Nodes added to the behaviour tree must derive from BTNode");
			}
			var node = (BTNode)CreateInstance(nodeType);

			node.children = new List<BTNode>();

			node.id = idCounter;

			var attributes = nodeType.GetCustomAttributes(typeof(GraphEditorAttribute), true).Cast<GraphEditorAttribute>();
			if (attributes.Any())
			{
				GraphEditorAttribute graphAtt = attributes.First();
				node.name = "(" + idCounter + ") " + graphAtt.name;
			}
			else
			{
				node.name = "(" + idCounter + ") " + nodeType;
			}
			node.rect = new Rect(initialPosition.x, initialPosition.y, 0, 0);

			nodes.Add(node);
			idCounter++;

			if (OnNodeAdded != null)
			{
				OnNodeAdded(node);
			}
		
			return node;
		}

		/// <summary>
		/// Unlinks this node from other nodes, destroys it, and removes it from the graph.
		/// </summary>
		/// <param name="node">The node to remove.</param>
		public void RemoveNode(BTNode node)
		{
			if (node == root) return; // Can't destroy root node

			nodes.Remove(node);

			foreach (var node1 in nodes)
			{
				node1.RemoveChild(node);
			}
		}

		/// <summary>
		/// Moves the given node left among its siblings.
		/// </summary>
		/// <param name="node">The node to move.</param>
		public void MoveNodeLeft(BTNode node)
		{
			node.MoveLeft();
		}

		/// <summary>
		/// Moves the given node right among its siblings.
		/// </summary>
		/// <param name="node">The node to move.</param>
		public void MoveNodeRight(BTNode node)
		{
			node.MoveRight();
		}

		/// <summary>
		/// Called when this object is reset
		/// </summary>
		public BTNode Reset()
		{
			foreach (var node in nodes)
			{
				DestroyImmediate(node, true);
			}

			nodes.Clear();

			root = (RootNode)AddNode(typeof(RootNode), Vector2.zero);
			root.name = "Behaviour Tree";

			return root;
		}

		/// <summary>
		/// Traverse the graph
		/// </summary>
		public BTNodeState Behave(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			var result = root.Behave(blackboard, agent, manager);

			// Close open nodes from last tick
			int start = 0;
			var previousOpenNodes = blackboard.lastOpenNodes;
			var openNodes = blackboard.currentOpenNodes;

			for (int i = 0; i < Mathf.Min(previousOpenNodes.Count, openNodes.Count); ++i)
			{
				start = i + 1;
				if (openNodes[i] != previousOpenNodes[i])
				{
					break;
				}
			}

			for (int i = previousOpenNodes.Count - 1; i >= start; --i)
			{
				previousOpenNodes[i].Close(blackboard, agent, manager, true);
			}

			// Reset lists on blackboard
			blackboard.lastOpenNodes.Clear();
			blackboard.lastOpenNodes.AddRange(openNodes);
			blackboard.currentOpenNodes.Clear();

			return result;
		}
		#endregion
	}
}