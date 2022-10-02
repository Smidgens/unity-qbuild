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


			EditorGUI.BeginProperty(pos, l, prop);

			var muteRect = pos.SliceLeft(25f);

			var asset = prop.FindPropertyRelative(nameof(BuildScene.asset));
			var skip = prop.FindPropertyRelative(nameof(BuildScene.skip));
			skip.boolValue = !EditorGUI.Toggle(muteRect, !skip.boolValue);
			EditorGUI.PropertyField(pos, asset, GUIContent.none);

			EditorGUI.EndProperty();

		}

	}
}