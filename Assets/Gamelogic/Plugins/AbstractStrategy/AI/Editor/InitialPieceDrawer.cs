using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gamelogic.AbstractStrategy.Grids;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.AbstractStrategy.Editor
{
	/// <summary>
	/// Custom inspector widget for initial piece settings
	/// </summary>
	[CustomPropertyDrawer(typeof(InitialPiecePlacement))]
	public class InitialPieceDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, UnityEngine.GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			float cellWidth = position.width / 3;

			var indent = EditorGUI.indentLevel;

			Rect pieceRect = new Rect(position.x, position.y, cellWidth - 2, position.height);
			Rect playerRect = new Rect(pieceRect.xMax + 2, position.y, cellWidth - 2, position.height);
			Rect posRect = new Rect(playerRect.xMax + 2, position.y, cellWidth, position.height);

			EditorGUIUtility.labelWidth = 70;
			EditorGUI.PropertyField(pieceRect, property.FindPropertyRelative("pieceID"));
			EditorGUIUtility.labelWidth = 60;
			EditorGUI.indentLevel = 0;
			EditorGUI.PropertyField(playerRect, property.FindPropertyRelative("playerID"));
			EditorGUI.PropertyField(posRect, property.FindPropertyRelative("position"), GUIContent.none);

			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}
}
