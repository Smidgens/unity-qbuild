// smidgens @ github

namespace Smidgenomics.Unity.QBuild.Editor
{
	using System;
	using UnityEngine;
	using UnityEditor;
	using System.IO;
	using System.Collections.Generic;

	[CreateAssetMenu(menuName = Config.CreateAssetMenu.BUILD_PROFILE)]
	internal sealed class BuildProfile : ScriptableObject
	{
		public const string BUILD_FOLDER = "Builds";
		public bool HasPlatform => _platform != 0;
		public bool IsWebGL => _platform == EBuildPlatform.WebGL;
		public bool IsStandalone => _platform.IsStandalone();

		[SerializeField] internal string _executableName;
		[SerializeField] internal EDevBuildOptions _devOptions;
		[SerializeField] internal bool _developmentBuild;
		[SerializeField] internal EBuildPlatform _platform;
		[SerializeField] internal ECompressionMethod _compressionMethod;
		[SerializeField] internal BuildScene[] _scenes = Array.Empty<BuildScene>();
		[SerializeField] internal string[] _extraScriptDefines = Array.Empty<string>();

		private string[] GetIncludedScenePaths()
		{
			List<string> paths = new();
			foreach (var s in _scenes)
			{
				if (!s.asset || s.skip)
				{
					continue;
				}
				paths.Add(s.asset.GetAssetPath());
			}
			return paths.ToArray();
		}

		public void StartBuild()
		{
			var buildTarget = _platform.ToBuildTarget();

			if (buildTarget == BuildTarget.NoTarget)
			{
				return;
			}

			var scenePaths = GetIncludedScenePaths();

			if (scenePaths.Length == 0)
			{
				return;
			}

			var options = new BuildPlayerOptions
			{
				target = buildTarget,
				scenes = scenePaths,
				locationPathName = GetOutputLocation(),
				options = _compressionMethod.ToBuildOptions(),
				extraScriptingDefines = _extraScriptDefines
			};

			if (_developmentBuild)
			{
				options.options |= BuildOptions.Development;
				options.options |= _devOptions.ToBuildOptions();
			}

			BuildPipeline.BuildPlayer(options);
		}

		private string GetExeName()
		{
			if(_platform == EBuildPlatform.WebGL)
			{
				return "index.html";
			}
			var n = !string.IsNullOrEmpty(_executableName) ? _executableName : name;
			if (_platform.IsWindows())
			{
				return $"{n}.exe";
			}
			return n;
		}
		private string GetOutputLocation()
		{
			var exe = GetExeName();
			var folderName = !string.IsNullOrEmpty(_executableName) ? _executableName : name;
			var projectPath = Application.dataPath.Replace("/Assets", "");
			return Path.Combine(projectPath, BUILD_FOLDER, folderName, exe);
		}
	}
}


namespace Smidgenomics.Unity.QBuild.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;

	using ReorderableList = UnityEditorInternal.ReorderableList;

	[CustomEditor(typeof(BuildProfile))]
	internal sealed class BuildProfile_ : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();

			GUILayout.Space(5f);
			
			EditorGUILayout.PropertyField(_exeName);
			GUILayout.Space(5f);
			_scenes.DoLayoutList();

			using (new EditorGUILayout.VerticalScope(GUI.skin.box))
			{
				EditorGUILayout.PropertyField(_platform);

				if (_Target.IsStandalone)
				{
					using (new EditorGUI.IndentLevelScope())
					{
						EditorGUILayout.PropertyField(_compression);
					}
				}
			}

			if(!_Target.HasPlatform)
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
						fn?.Invoke(_Target);
					}
				}
			}
			serializedObject.ApplyModifiedProperties();
		}

		protected override bool ShouldHideOpenButton()
		{
			return true;
		}

		private ReorderableList _scenes;
		private ReorderableList _defines;

		private static readonly (string, Action<BuildProfile>)[] _ACTIONS =
		{
			default,
			("Build", StartBuild),
		};

		private SerializedProperty _exeName;
		private SerializedProperty _platform;
		private SerializedProperty _compression;
		private SerializedProperty _devBuild;
		private SerializedProperty _devOptions;

		private BuildProfile _Target => target as BuildProfile;

		private void OnEnable()
		{
			_exeName = serializedObject.FindProperty(nameof(BuildProfile._executableName));
			_platform = serializedObject.FindProperty(nameof(BuildProfile._platform));
			_compression = serializedObject.FindProperty(nameof(BuildProfile._compressionMethod));
			_devBuild = serializedObject.FindProperty(nameof(BuildProfile._developmentBuild));
			_devOptions = serializedObject.FindProperty(nameof(BuildProfile._devOptions));

			_scenes = GUIHelper.GetList(serializedObject.FindProperty(nameof(BuildProfile._scenes)));
			_defines = GUIHelper.GetList(serializedObject.FindProperty(nameof(BuildProfile._extraScriptDefines)));
		}

		private static void StartBuild(BuildProfile t) => EditorApplication.delayCall += t.StartBuild;

	}
}