#if UNITY_EDITOR
namespace Smidgenomics.QuickBuild
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor.Callbacks;
	using UnityEditor.SceneManagement;
	using UnityEditor;
	using System.IO;
	using System.Linq;

	using ReorderableList = UnityEditorInternal.ReorderableList;


	[CreateAssetMenu(menuName="QuickBuild/Build Configuration")]
	internal class BuildConfiguration : ScriptableObject
	{
		public IEnumerable<SceneAsset> Scenes { get { foreach(var s in _scenes) { if(s.Asset && s.Include) { yield return s.Asset; } } } }

		public string FullBuildPath { get { return GetFullBuildPath(); } }
		public string BuildPath { get { return GetBuildPath(); } }
		public bool HasRelativeOutput { get { return !Path.IsPathRooted(GetBuildPath()); } }
		public string ExecutableName { get { return GetExecutableName(); } }
		public Platform Platform { get { return _platform; } }


		[SerializeField] private bool _overrideName = false;
		[SerializeField] private bool _overridePath = false;
		[SerializeField] private string _executableName = "Name";
		[SerializeField] private string _buildPath = "Builds";
		[SerializeField] private bool _developmentBuild = false;
		[SerializeField] private bool _autoconnectProfiler = false;
		[SerializeField] private bool _scriptDebugging = false;
		[SerializeField] private bool _scriptsOnlyBuild = false;
		
		[Serializable] private class StringEvent : UnityEngine.Events.UnityEvent<string>{}
		[SerializeField] private StringEvent _onPostBuild = null;

		[SerializeField] private Platform _platform = Platform.None;
		[SerializeField] private CompressionMethod _compressionMethod = CompressionMethod.Default;

		[SerializeField] private List<SceneAssetReference> _scenes = new List<SceneAssetReference>();

		public void StartBuild()
		{
			BuildPlayerOptions options = new BuildPlayerOptions();

			options.scenes = Scenes.Select(x => GetPath(x)).ToArray();

			options.options = 0;
			options.target = BuildTarget.StandaloneWindows64;
			options.locationPathName = FullBuildPath;

			if(_compressionMethod == CompressionMethod.LZ4) { options.options |= BuildOptions.CompressWithLz4; }
			else if(_compressionMethod == CompressionMethod.LZ4HC) { options.options |= BuildOptions.CompressWithLz4HC; }

			if(_developmentBuild) { options.options |= BuildOptions.Development; }

			if(_autoconnectProfiler) { options.options |= BuildOptions.ConnectWithProfiler; }
			if(_scriptsOnlyBuild) { options.options |= BuildOptions.BuildScriptsOnly; }
			if(_scriptDebugging) { options.options |= BuildOptions.AllowDebugging; }


			if(_platform == Platform.Windows || _platform == Platform.Windows64)
			{
				options.locationPathName += "/" + ExecutableName;
			}

			BuildTarget target;
			switch(_platform)
			{
				case Platform.Windows: target = BuildTarget.StandaloneWindows; break;
				case Platform.Windows64: target = BuildTarget.StandaloneWindows64; break;
				case Platform.Linux: target = BuildTarget.StandaloneLinux; break;
				case Platform.Linux64: target = BuildTarget.StandaloneLinux64; break;
				case Platform.LinuxUniversal: target = BuildTarget.StandaloneLinuxUniversal; break;
				case Platform.WebGL: target = BuildTarget.WebGL; break;
				default: target = BuildTarget.NoTarget; break;
			}

			options.target = target;

			PostBuild.onBuilt += s =>
			{
				_onPostBuild.Invoke(s.Remove(s.Length - ExecutableName.Length));
			};
			BuildPipeline.BuildPlayer(options);
		}

		public void AddOpenScenes()
		{
			for(int i = 0; i < EditorSceneManager.loadedSceneCount; i++)
			{
				var s = EditorSceneManager.GetSceneAt(i);

				if(!s.IsValid() || s.path.Length == 0) { continue; }

				var sceneAsset = (SceneAsset)AssetDatabase.LoadAssetAtPath(s.path, typeof(SceneAsset));

				if(!_scenes.Exists(x => x.Asset == sceneAsset))
				{
					Undo.RecordObject(this, "Adding scene to build configuration list.");
					_scenes.Add(new SceneAssetReference(sceneAsset));
				}
			}
		}

		private string GetExecutableName()
		{
			if(_platform == Platform.WebGL) { return "index.html"; }
			var n = _overrideName ? _executableName : Application.productName;
			if(_platform == Platform.Windows || _platform == Platform.Windows64) { n += ".exe"; }
			return n;
		}

		private static string GetPath(SceneAsset scene)
		{
			return AssetDatabase.GetAssetPath(scene);
		}

		private string GetFullBuildPath()
		{
			string p = _overridePath ? _buildPath : "Builds/" + name;
			if(!Path.IsPathRooted(p))
			{
				p = Path.Combine(Application.dataPath.Replace("/Assets", ""), p).Replace(@"\", "/");
			}

			return p;
		}

		private string GetBuildPath()
		{
			return _overridePath ? _buildPath : "Builds/" + name + "/";
		}
	}
}
#endif