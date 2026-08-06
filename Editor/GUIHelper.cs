// smidgens @ github

namespace Smidgenomics.Unity.QBuild.Editor
{
	using System;
	using UnityEngine;
	using UnityEditor;
	using UnityEditorInternal;

	internal static class GUIHelper
	{
		public static void LayoutDivider(float m = 5f)
		{
			GUILayout.Space(m);
			var r = EditorGUILayout.GetControlRect(GUILayout.Height(1f));
			EditorGUI.DrawRect(r, _DIVIDER_COLOR);
			GUILayout.Space(m);
		}

		public static ReorderableList GetList(SerializedProperty p)
		{
			var l = new ReorderableList(p.serializedObject, p, true, true, true, true);
			l.drawElementCallback = (r, i, a, f) =>
			{
				r.height = EditorGUIUtility.singleLineHeight;
				var p = l.serializedProperty.GetArrayElementAtIndex(i);
				EditorGUI.PropertyField(r, p, GUIContent.none);
			};
			l.drawHeaderCallback = r => EditorGUI.LabelField(r, l.serializedProperty.displayName);
			return l;
		}


		private static readonly Color _DIVIDER_COLOR =
		EditorGUIUtility.isProSkin
		? Color.white * 0.4f
		: Color.black * 0.5f;
	}
}