using System;
using System.Linq;
using Gamelogic.AbstractStrategy.Editor;
using Gamelogic.Diagnostics;
using Gamelogic.Editor;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace Gamelogic.AbstractStrategy.AI.Editor
{
	/// <summary>
	/// A window for editing graphs.
	/// </summary>
	[Version(1)]
	public class GraphWindow : EditorWindow
	{
		#region Constants
		private readonly Vector2 LayoutRectSize = new Vector2(200, 100);
		private readonly Vector2 LayoutSpacing = new Vector2(10, 80);
		#endregion
		#region Public Fields
		public Dictionary<int, SerializedObject> serializedObjects = new Dictionary<int, SerializedObject>();
		#endregion

		#region Private Fields
		private BehaviourTree graph;
		private bool addLink;
		private BTNode rootWindow;

		private Vector2 mainScrollPosition = Vector2.zero;
		private Vector2 toolbarScrollPosition = Vector2.zero;

		private Type[] nodeTypes;
		#endregion
		#region Static Methods
		/// <summary>
		/// Brings up a save file dialog that allows the user to specify a location to 
		/// save a new colorgraph, makes a new colorgraph, and saves it to the specified 
		/// location.
		/// </summary>
		[MenuItem("Gamelogic/New Behaviour Tree")]
		public static void MakeNewBehaviourTree()
		{
			var graph = CreateInstance<BehaviourTree>();

			

			var path = EditorUtility.SaveFilePanel(
				"Create new Behaviour Tree",
				"Assets",
				"AI" + ".behaviourtree.asset",
				"asset");

			if (path != "")
			{
				path = "Assets" + path.Substring(Application.dataPath.Length);
				
				var root = graph.Reset();
				

				AssetDatabase.CreateAsset(graph, path);
				AssetDatabase.SaveAssets();

				AssetDatabase.AddObjectToAsset(root, graph);
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(root));

				EditorUtility.SetDirty(graph);
			}
		}
		#endregion

		public static void ShowEditor(BehaviourTree graph)
		{
			var editor = GetWindow<GraphWindow>("Graph");

			editor.graph = graph;
			editor.UpdateSerializableObjects();
			editor.addLink = false;
			editor.nodeTypes = typeof(BehaviourTree).Assembly.GetTypes().Where
				(t => t.IsSubclassOf(typeof(BTNode)) &&
					t.GetCustomAttributes(true).Any(a => a is GraphEditorAttribute)).ToArray();
		}

		private void ClearNullNodes()
		{
			graph.Nodes.RemoveAll(r => r == null);
		}

		public void OnGUI()
		{
			//ClearNullNodes();

			EditorGUILayout.BeginHorizontal();
			DrawToolbar();
		
			GLEditorGUI.VerticalLine();

			mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition);

			GUILayout.Label("", GUILayout.Width(4000), GUILayout.Height(1000));
			AddLink();
			DrawWindows();
			DrawCurves();
			HandleNodeDeleteButtons();
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndHorizontal();
		}

		private void HandleNodeDeleteButtons()
		{
			var e = Event.current;

			if (e.type != EventType.mouseDown) return;

			var mousePos = e.mousePosition;

			foreach (var window in graph.Nodes)
			{
				if (window.children != null)
				{
					foreach (var input in window.children)
					{
						var x = (input.rect.center.x + window.rect.center.x) / 2;
						var y = (input.rect.yMin + window.rect.yMax) / 2;

						var pos = new Vector2(x, y);

						//Debug.Log(pos + " " + (mousePos - pos).magnitude);

						if ((mousePos - pos).magnitude <= 10 &&
							input != graph.Root)
						{
							window.RemoveChild(input);
							EditorUtility.SetDirty(window);
							serializedObjects[window.id].ApplyModifiedProperties();

							e.Use();
							ApplySerializedProperties();
							Repaint();
							return;
						}
					}
				}
			}
		}

		private void DrawCurves()
		{
			if (graph != null && graph.Nodes != null)
			{
				foreach (var parent in graph.Nodes)
				{
					if (parent.children != null)
					{
						foreach (var child in parent.children)
						{
							EditorUtils.DrawNodeCurve(parent.rect, child.rect);
						}
					}
				}
			}
		}

		private void AddLink()
		{
			if (!addLink) return;
		
			var e = Event.current;
			var mousePos = e.mousePosition;

			EditorUtils.DrawNodeCurve(rootWindow.rect, mousePos);
			Repaint();

			if (e.type != EventType.mouseDown) return;

			var childWindow = graph.Nodes.FirstOrDefault(t => t.rect.Contains(mousePos));

			if (childWindow != null)
			{
				rootWindow.AddNodeChild(childWindow);
				DoLayout(); 
				serializedObjects[rootWindow.id].ApplyModifiedProperties();
				EditorUtility.SetDirty(rootWindow);
			}

			e.Use();

			addLink = false;
			rootWindow = null;
		}

		private void DrawToolbar()
		{
			toolbarScrollPosition = EditorGUILayout.BeginScrollView(toolbarScrollPosition, GUILayout.Width(120));
			EditorGUILayout.BeginVertical();

			if (GraphToolbarButton("Clear"))
			{
				var root = graph.Reset();

				AssetDatabase.AddObjectToAsset(root, graph);
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(root));

				EditorUtility.SetDirty(graph);
				
				var path = AssetDatabase.GetAssetOrScenePath(graph);

				AssetDatabase.ImportAsset(path);
				AssetDatabase.SaveAssets();
				EditorUtility.SetDirty(graph);

				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				UpdateSerializableObjects();
			}
#if DEBUG
			if (GraphToolbarButton("Reassign Parents"))
			{
				ReassignParents();
			}
#endif

			if (GraphToolbarButton("DoLayout"))
			{
				DoLayout();
				var path = AssetDatabase.GetAssetOrScenePath(graph);

				AssetDatabase.ImportAsset(path);
				AssetDatabase.SaveAssets();
				EditorUtility.SetDirty(graph);

				UpdateSerializableObjects();
			}

			EditorGUILayout.Separator();
			if(nodeTypes != null)
			{
				foreach (var type in nodeTypes)
				{
					GraphEditorAttribute graphAtt = type.GetCustomAttributes(typeof(GraphEditorAttribute), true).Cast<GraphEditorAttribute>().First();
					if (graphAtt != null &&
						GraphToolbarButton(graphAtt.name))
					{
						AddNode(type);
					}
				}
			}
		
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
		}

		private bool GraphToolbarButton(string label)
		{
			return GUILayout.Button(label, EditorStyles.toolbarButton);
		}

		private void DrawWindows()
		{
			BeginWindows();

			if (graph != null && graph.Nodes != null)
			{
				int i = 0;

				foreach (var nodeWindow in graph.Nodes)
				{
					var nodeWindowCopy = nodeWindow;
					nodeWindow.rect = GUILayout.Window(i, nodeWindow.rect, n => DrawNode(n, nodeWindowCopy), nodeWindow.name);
					i++;
				}
			}

			EndWindows();
		}

		private void ApplySerializedProperties()
		{
			foreach (var serializedObject in serializedObjects.Values)
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		public void OnEnable()
		{
			if (graph != null)
			{
				UpdateSerializableObjects();
			}
		}

		public void UpdateSerializableObjects()
		{
			serializedObjects.Clear();

			foreach (var node in graph.Nodes)
			{
				serializedObjects[node.id] = new SerializedObject(node);
			}
		}

		public void AddNode(Type type)
		{
			var node = graph.AddNode(type, mainScrollPosition);

			AssetDatabase.AddObjectToAsset(node, graph);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(node));

			EditorUtility.SetDirty(graph);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			UpdateSerializableObjects();
		}

		public void RemoveNode(BTNode window)
		{
			graph.RemoveNode(window);

			var path = AssetDatabase.GetAssetOrScenePath(window);
			DestroyImmediate(window, true);

			AssetDatabase.ImportAsset(path);
			AssetDatabase.SaveAssets();
			EditorUtility.SetDirty(graph);

			UpdateSerializableObjects();
		}

		public void DrawNode(int id, BTNode window)
		{
			//foreach (var input in window.Inputs)
			//{
			//	EditorUtils.DrawNodeCurve(input.rect, window.rect);
			//}

			float oldWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 100;

			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

			if (!(window is RootNode) && GUILayout.Button("Remove", EditorStyles.toolbarButton))
			{
				RemoveNode(window);
				return;
			}

			if (!(window is RootNode) && GUILayout.Button("<", EditorStyles.toolbarButton))
			{
				graph.MoveNodeLeft(window);
				DoLayout();
				return;
			}

			if (!(window is RootNode) && GUILayout.Button(">", EditorStyles.toolbarButton))
			{
				graph.MoveNodeRight(window);
				DoLayout();
				return;
			}

			var serializedObject = serializedObjects[window.id];
			serializedObject.Update();

			// Can we have more children than we have
			if (window.children != null)
			{
				if (window.children.Count < window.MaxChildren)
				{
					if (GUILayout.Button("Add Link", EditorStyles.toolbarButton))
					{
						addLink = true;
						rootWindow = window;
					}
				}
			}

			GUILayout.FlexibleSpace();

			EditorGUILayout.EndHorizontal();

			var property = serializedObjects[window.id].GetIterator();
			bool isFirst = true;
		
			while (property.NextVisible(isFirst))
			{
				isFirst = false;

				if (property.name != "m_Script")
				{
					EditorGUILayout.PropertyField(property, true);
				}
			
			}

			//EditorGUILayout.InspectorTitlebar(true, outputProperty.serializedObject.targetObject);

			EditorGUIUtility.labelWidth = oldWidth;
			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
			}

			//GUI.DragWindow();
			GUI.DragWindow();
		}

#if DEBUG
		public void ReassignParents()
		{
			var processQueue = new Queue<BTNode>();
			processQueue.Enqueue(graph.Root);

			while (processQueue.Any())
			{
				var currentNode = processQueue.Dequeue();

				if(currentNode.children != null)
				{
					foreach (var child in currentNode.children)
					{
						child.Parent = currentNode;
						processQueue.Enqueue(child);
					}
				}
			}
		}
#endif

		public void OnDisable()
		{
			ApplySerializedProperties();
			AssetDatabase.SaveAssets();
		}

		#region Layout methods

		private void DoLayout()
		{
			CalculateLayoutParameters();

			var processQueue = new Queue<BTNode>();
			processQueue.Enqueue(graph.Root);

			var depthQueue = new Queue<int>();
			depthQueue.Enqueue(0);

			int previousDepth = 0;

			int x = 0;

			while (processQueue.Any())
			{
				var currentNode = processQueue.Dequeue();
				int currentDepth = depthQueue.Dequeue();

				if (currentDepth != previousDepth)
				{
					x = 0;
					previousDepth = currentDepth;
				}
				else
				{
					x++;
				}
				
				float nodeX = currentNode.layoutHorizontalInterval.x + (currentNode.layoutHorizontalInterval.width - LayoutRectSize.x)/2;
				
				currentNode.rect = new Rect(
					nodeX,
					LayoutSpacing.y + currentDepth*(LayoutRectSize.y + LayoutSpacing.y),
					LayoutRectSize.x,
					LayoutRectSize.y);

				GLDebug.Assert(currentNode.children != null, "children of node is null " + currentNode);

				foreach (var child in currentNode.children)
				{
					processQueue.Enqueue(child);
					depthQueue.Enqueue(currentDepth+1);
				}
			}
		}

		public void CalculateLayoutParameters()
		{
			CalculateLayoutParameters(graph.Root);
		}

		public void CalculateLayoutParameters(BTNode node)
		{
			if (node == null)
			{
				GLDebug.LogError("node is null");
			}

			if (node.children == null || node.children.Count == 0)
			{
				node.layoutHorizontalInterval.width = LayoutRectSize.x + LayoutSpacing.x;

				int childIndex = node.ChildIndex;

				if (childIndex <= 0)
				{
					node.layoutHorizontalInterval.x = node.Parent != null ? node.Parent.layoutHorizontalInterval.x : LayoutSpacing.x;
				}
				else
				{

					var leftSibling = node.LeftSibling;
					node.layoutHorizontalInterval.x = 
						leftSibling.layoutHorizontalInterval.x
						+ leftSibling.layoutHorizontalInterval.width;
				}
			}
			else
			{
				int childIndex = node.ChildIndex;

				if (childIndex == 0)
				{
					node.layoutHorizontalInterval.x = node.Parent != null ? node.Parent.layoutHorizontalInterval.x : LayoutSpacing.x;
				}
				else
				{
					var leftSibling = node.LeftSibling;
					node.layoutHorizontalInterval.x =
						leftSibling.layoutHorizontalInterval.x
						+ leftSibling.layoutHorizontalInterval.width;
				}

				float sumOfChildrenWidths = 0;

				foreach (var child in node.children)
				{
					CalculateLayoutParameters(child);

					sumOfChildrenWidths += child.layoutHorizontalInterval.width;
				}

				node.layoutHorizontalInterval.width = sumOfChildrenWidths;
			}
		}
		#endregion
	}
}