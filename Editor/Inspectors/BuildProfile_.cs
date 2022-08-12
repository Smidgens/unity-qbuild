// smidgens @ github

namespace Smidgenomics.Unity.QBuild.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;

	using ReorderableList = UnityEditorInternal.ReorderableList;

	[CustomEditor(typeof(BuildProfile))]
	internal class BuildProfile_ : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();

			GUILayout.Space(5f);

			_scenes.DoLayoutList();

			using (new EditorGUILayout.VerticalScope(GUI.skin.box))
			{
				EditorGUILayout.PropertyField(_platform);

				if (_target.IsStandalone)
				{
					using (new EditorGUI.IndentLevelScope())
					{
						EditorGUILayout.PropertyField(_compression);
					}
				}
			}

			if(!_target.HasPlatform)
			{
				serializedObject.ApplyModifiedProperties();
				return;
			}

			using (new EditorGUILayout.VerticalScope(GUI.skin.box))
			{
				EditorGUILayout.PropertyField(_devBuild);
				if (_devBuild.boolValue)
				{
					using (new EditorGUI.IndentLevelScope())
					{
						EditorGUILayout.PropertyField(_devOptions, new GUIContent("Options"));
					}
				}
			}

			_defines.DoLayoutList();

			GUIHelper.LayoutDivider();

			using (new EditorGUILayout.HorizontalScope())
			{
				foreach(var (l,fn) in _ACTIONS)
				{
					if(fn == null)
					{
						GUILayout.FlexibleSpace();
						continue;
					}
					if (GUILayout.Button(l))
					{
						fn?.Invoke(_target);
					}
				}
			}
			serializedObject.ApplyModifiedProperties();
		}

		private ReorderableList
		_scenes = null,
		_defines = null;

		private static (string, Action<BuildProfile>)[] _ACTIONS =
		{
			("", null),
			("Build", StartBuild),
		};

		private SerializedProperty
		_platform,
		_compression,
		_devBuild,
		_devOptions;

		private BuildProfile _target = default;

		private void OnEnable()
		{
			_target = target as BuildProfile;
			_platform = serializedObject.FindProperty(BuildProfile._PF.PLATFORM);
			_compression = serializedObject.FindProperty(BuildProfile._PF.COMPRESSION);
			_devBuild = serializedObject.FindProperty(BuildProfile._PF.DEV_BUILD);
			_devOptions = serializedObject.FindProperty(BuildProfile._PF.DEV_OPTIONS);

			_scenes = GUIHelper.GetList(serializedObject.FindProperty(BuildProfile._PF.SCENES));
			_defines = GUIHelper.GetList(serializedObject.FindProperty(BuildProfile._PF.DEFINES));
		}

		private static void StartBuild(BuildProfile t) => EditorApplication.delayCall += t.StartBuild;

	}
}