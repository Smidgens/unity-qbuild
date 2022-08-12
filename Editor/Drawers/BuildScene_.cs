// smidgens @ github

namespace Smidgenomics.Unity.QBuild.Editor
{
	using UnityEngine;
	using UnityEditor;
	using SP = UnityEditor.SerializedProperty;

	[CustomPropertyDrawer(typeof(BuildScene))]
	internal class BuildScene_ : PropertyDrawer
	{
		public override void OnGUI(Rect pos, SP prop, GUIContent l)
		{
			if(l != GUIContent.none && !fieldInfo.FieldType.IsArray)
			{
				EditorGUI.PrefixLabel(pos, l);
			}

			pos.height = EditorGUIUtility.singleLineHeight;

			using (new EditorGUI.PropertyScope(pos, l, prop))
			{
				var cols = pos.SliceHorizontalMixed(25f, 1f);
				var asset = prop.FindPropertyRelative(nameof(BuildScene.asset));
				var skip = prop.FindPropertyRelative(nameof(BuildScene.skip));
				skip.boolValue = !EditorGUI.Toggle(cols[0], !skip.boolValue);
				EditorGUI.PropertyField(cols[1], asset, GUIContent.none);
			}
		}

	}
}