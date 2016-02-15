using UnityEditor;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.AI.Editor
{
	/// <summary>
	/// Editor for displaying a Graph in the inspector.
	/// </summary>
	[Version(1)]
	[CustomEditor(typeof(BehaviourTree))]
	public class GraphEditor : UnityEditor.Editor
	{
		override public void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (GUILayout.Button("Edit"))
			{
				GraphWindow.ShowEditor((BehaviourTree)target);
			}
		}
	}
}