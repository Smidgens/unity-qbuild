// smidgens @ github

namespace Smidgenomics.Unity.QBuild.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System.IO;
	using System.Linq;

	[CreateAssetMenu(menuName = Config.CreateAssetMenu.BUILD_PROFILE)]
	internal class BuildProfile : ScriptableObject
	{
		public const string BUILD_FOLDER = "Builds";
		public bool HasPlatform => _platform != 0;
		public bool IsWebGL => _platform == Platform.WebGL;
		public bool IsStandalone => _platform.IsStandalone();

		internal static class _PF
		{
			public const string
			PLATFORM = nameof(_platform),
			COMPRESSION = nameof(_compressionMethod),
			DEV_OPTIONS = nameof(_devOptions),
			DEV_BUILD = nameof(_developmentBuild),
			DEFINES = nameof(_extraScriptDefines),
			SCENES = nameof(_scenes);
		}

		[SerializeField] DevBuildOptions _devOptions = default;
		[SerializeField] private bool _developmentBuild = false;
		[SerializeField] private Platform _platform = default;
		[SerializeField] private CompressionMethod _compressionMethod = default;
		[SerializeField] private BuildScene[] _scenes = { };
		[SerializeField] private string[] _extraScriptDefines = { };

		private string[] GetIncludedScenePaths()
		{
			return _scenes
			.Where(x => x.asset && !x.skip)
			.Select(x => x.asset.GetAssetPath())
			.ToArray();
		}

		public void StartBuild()
		{
			var buildTarget = _platform.ToBuildTarget();

			if(buildTarget == BuildTarget.NoTarget) { return; }

			var scenePaths = GetIncludedScenePaths();

			if(scenePaths.Length == 0) { return; }

			var options = new BuildPlayerOptions();
			options.target = buildTarget;
			options.scenes = GetIncludedScenePaths();
			options.options = 0;
			options.options |= _compressionMethod.ToBuildOptions();
			options.locationPathName = GetOutputLocation();
			options.extraScriptingDefines = _extraScriptDefines;

			if (_developmentBuild)
			{
				options.options |= BuildOptions.Development;
				options.options |= _devOptions.ToBuildOptions();
			}

			BuildPipeline.BuildPlayer(options);
		}

		private string GetExeName()
		{
			if(_platform == Platform.WebGL)
			{
				return "index.html";
			}
			var n = name;
			if (_platform.IsWindows()) { return $"{n}.exe"; }
			return n;
		}
		private string GetOutputLocation()
		{
			var exe = GetExeName();
			var projectPath = Application.dataPath.Replace("/Assets", "");
			return Path.Combine(projectPath, BUILD_FOLDER, name, exe);
		}
	}
}
