using System;
using System.Collections.Generic;
using Gamelogic.AbstractStrategy.Grids;
using UnityEngine;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.AI
{
	/// <summary>
	/// An attribute used to decorate behaviour tree nodes. Apply this to custom behaviours
	/// to have them appear in the behaviour tree editor list.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class GraphEditorAttribute : Attribute
	{
		public string name;

		public GraphEditorAttribute(string name)
		{
			this.name = name;
		}
	}


	/// <summary>
	/// Possible return states from a node
	/// </summary>
	[Serializable]
	public enum BTNodeState
	{
		/// <summary>
		/// Node executed successfully
		/// </summary>
		Success,
		/// <summary>
		/// Node execution failed
		/// </summary>
		Failure,
		/// <summary>
		/// Node is running, and committed a move to the turn manager. Running nodes are kept open
		/// and do not close until they are either preempted by a priority node or subsequently return
		/// a different state
		/// </summary>
		Running,
		/// <summary>
		/// Node encountered some internal error
		/// </summary>
		Error
	}

	/// <summary>
	/// A class for representing an interval using a start point, and a width.
	/// It is essentially a 1D rectangle.
	/// </summary>
	[Serializable]
	public class Interval
	{
		/// <summary>
		/// The starting point of the interval.
		/// </summary>
		public float x;

		/// <summary>
		/// The width of the interval.
		/// </summary>
		public float width;
	}


	/// <summary>
	/// The base class for all nodes in a <see cref="BehaviourTree"/>.
	/// </summary>
	[Version(1)]
	[Abstract]
	public class BTNode: ScriptableObject
	{
		#region Fields
		// TODO: Make this private and provide a readonly property
		/// <exclude />
		[HideInInspector]
		public int id;

		/// <summary>
		/// The rectangle this node occupies when displayed visually.
		/// </summary>
		[HideInInspector]
		public Rect rect = new Rect(50, 50, 100, 0);

		/// <summary>
		/// The rectangle which encloses this node and all its children.
		/// </summary>
		[HideInInspector]
		[SerializeField]
		public Interval layoutHorizontalInterval;

		//private int layoutX = 0;
		//private int layoutY = 0;

		/// <summary>
		/// A list of nodes that are children of this node.
		/// </summary>
		[HideInInspector]
		public List<BTNode> children;

		/// <summary>
		/// The parent node of this node.
		/// </summary>
		[HideInInspector] 
		[SerializeField]
		private BTNode parent; 
		#endregion

		#region Properties
		/// <summary>
		/// Maximum number of children.
		/// </summary>
		public virtual int MaxChildren { get { return 0; } }

		public int ChildIndex
		{
			get { return parent == null ? 0 : parent.children.IndexOf(this); }
		}

		public BTNode Parent
		{
			get { return parent; }
#if DEBUG
			set { parent = value; } //TODO: Remove!
#endif
		}

		public BTNode LeftSibling
		{
			get { return parent.children[ChildIndex - 1]; }
		}

		public BTNode RightSibling
		{
			get { return parent.children[ChildIndex + 1]; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Adds a child to this node
		/// </summary>
		public virtual void AddNodeChild(BTNode child)
		{
			if (child.parent != null)
			{
				child.parent.RemoveChild(child);
			}

			if (children == null) children = new List<BTNode>();
			children.Add(child);
			child.parent = this;
		}


		public virtual void RemoveChild(BTNode child)
		{
			child.parent = null;
			children.Remove(child);
		}

		public void MoveLeft()
		{
			if (parent != null)
			{
				parent.MoveChildLeft(this);
			}
		}

		public void MoveRight()
		{
			if (parent != null)
			{
				parent.MoveChildRight(this);
			}
		}

		/// <summary>
		/// Moves the child left in it's parents list of children.
		/// </summary>
		/// <param name="child">The child to move.</param>
		public void MoveChildLeft(BTNode child)
		{
			int childIndex = child.ChildIndex;

			if (childIndex > 0 && childIndex < children.Count)
			{
				var tmp = children[childIndex];
				children[childIndex] = children[childIndex - 1];
				children[childIndex - 1] = tmp;
			}
		}

		/// <summary>
		/// Moves the child right in its list of children.
		/// </summary>
		/// <param name="child">The child to move.</param>
		public void MoveChildRight(BTNode child)
		{
			int childIndex = child.ChildIndex;

			if (childIndex >= 0 && childIndex < children.Count - 1)
			{
				var tmp = children[childIndex];
				children[childIndex] = children[childIndex + 1];
				children[childIndex + 1] = tmp;
			}
		}
		#endregion

		#region Message Handlers
		public void OnEnable()
		{
			hideFlags = HideFlags.HideInHierarchy;
		}
		#endregion


		#region BT Methods
		protected NodeBlackboard GetNodeBlackboard(Blackboard blackboard)
		{
			return blackboard.GetNodeBlackboard(id);
		}

		/// <summary>
		/// Ticks a BTNode, allowing it to perform all of its logic. Composite and decorator nodes
		/// call this on their children
		/// </summary>
		/// <returns>The resultant state of the node</returns>
		internal BTNodeState Behave(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			Enter(blackboard, agent, manager);

			var nodeBlackboard = GetNodeBlackboard(blackboard);
			if (!nodeBlackboard.isOpen)
			{
				Open(blackboard, agent, manager);
			}

			var result = DoLogic(blackboard, agent, manager);

			if (result != BTNodeState.Running)
			{
				Close(blackboard, agent, manager, false);
			}

			Exit(blackboard, agent, manager);

			return result;
		}


		/// <summary>
		/// Visits a node. Do not call directly
		/// </summary>
		internal void Enter(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			blackboard.currentOpenNodes.Add(this);

			OnEnter(blackboard, agent, manager);
		}


		/// <summary>
		/// Opens a node. Do not call directly.
		/// </summary>
		[Version(1, 1)]
		internal void Open(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			GetNodeBlackboard(blackboard).isOpen = true;

			OnOpen(blackboard, agent, manager);
		}


		/// <summary>
		/// Close a node. Do not call directly
		/// </summary>
		[Version(1, 1)]
		internal void Close(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager, bool forced)
		{
			if (!forced)
			{
				var openNodes = blackboard.currentOpenNodes;
				openNodes.RemoveAt(openNodes.Count - 1);
			}
			GetNodeBlackboard(blackboard).isOpen = false;

			OnClosed(blackboard, agent, manager);
		}


		/// <summary>
		/// Exits a node. Do not call directly
		/// </summary>
		[Version(1, 1)]
		internal void Exit(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			OnExit(blackboard, agent, manager);
		}


		/// <summary>
		/// Called when a node is visited. Implement in derived classes if you need logic to happen
		/// every time a node is visited, before any other logic
		/// </summary>
		[Version(1, 1)]
		protected virtual void OnEnter(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager) { }


		/// <summary>
		/// Called when a node is opened. Implement in derived classes if you need logic to happen
		/// only when a node is ticked while closed.
		/// </summary>
		[Version(1, 1)]
		protected virtual void OnOpen(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager) { }


		/// <summary>
		/// Called when a node is closed. Implement in derived classes if you need logic to happen
		/// when a node is closed.
		/// </summary>
		[Version(1, 1)]
		protected virtual void OnClosed(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager) { }


		/// <summary>
		/// Called when a node is left. Implement in derived classes if you need logic to happen
		/// every time a node is exited, afterany other logic
		/// </summary>
		[Version(1, 1)]
		protected virtual void OnExit(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager) { }


		/// <summary>
		/// Performs actual node logic. Implement this method in derived classes
		/// </summary>
		/// <returns>The resultant state of the node</returns>
		protected virtual BTNodeState DoLogic(Blackboard blackboard, GridAIPlayer agent, IGameManager<RectPoint, GridGamePieceSettings> manager)
		{
			throw new NotImplementedException("Derived BTNode classes must implement Process, and should not call base. Called from: " + GetType() );
		}
		#endregion
	}
}