// smidgens @ github


namespace Smidgenomics.Unity.QBuild.Editor
{
	using UnityEditor;

	internal static class Unity_
	{
		public static string GetAssetPath(this SceneAsset scene)
		{
			return AssetDatabase.GetAssetPath(scene);
		}
	}
}

namespace Smidgenomics.Unity.QBuild.Editor
{
	using UnityEditor;

	internal static class Enum_
	{
		public static BuildTargetGroup ToTargetGroup(this BuildTarget target)
		{
			switch (target)
			{
				case BuildTarget.WebGL: return BuildTargetGroup.WebGL;
				case BuildTarget.iOS: return BuildTargetGroup.iOS;
				case BuildTarget.Android: return BuildTargetGroup.Android;
			}
			if (target.ToString().Contains("Standalone")) { return BuildTargetGroup.Standalone; }
			return BuildTargetGroup.Unknown;
		}

		public static bool IsStandalone(this Platform p)
		{
			return
			p != 0
			&& p != Platform.WebGL;
		}

		public static string GetDisplayName(this BuildTarget b)
		{
			switch (b)
			{
				case BuildTarget.StandaloneWindows: return "Windows";
				case BuildTarget.StandaloneWindows64: return "Windows (x64)";
				case BuildTarget.StandaloneLinux64: return "Linux (x64)";
			}
			return b.ToString();
		}

		public static BuildTarget ToBuildTarget(this Platform p)
		{
			switch (p)
			{
				case Platform.Windows: return BuildTarget.StandaloneWindows;
				case Platform.Windows64: return BuildTarget.StandaloneWindows64;
				case Platform.Linux64: return BuildTarget.StandaloneLinux64;
				case Platform.WebGL: return BuildTarget.WebGL;
			}
			return BuildTarget.NoTarget;
		}

		public static BuildOptions ToBuildOptions(this CompressionMethod c)
		{
			switch (c)
			{
				case CompressionMethod.LZ4: return BuildOptions.CompressWithLz4;
				case CompressionMethod.LZ4HC: return BuildOptions.CompressWithLz4HC;
			}
			return BuildOptions.None;
		}

		public static BuildOptions ToBuildOptions(this DevBuildOptions o)
		{
			BuildOptions opts = 0;
			if (o.HasFlag(DevBuildOptions.Profiler))
			{
				opts |= BuildOptions.ConnectWithProfiler;
			}
			if (o.HasFlag(DevBuildOptions.ScriptsOnly))
			{
				opts |= BuildOptions.BuildScriptsOnly;
			}
			if (o.HasFlag(DevBuildOptions.Debugging))
			{
				opts |= BuildOptions.AllowDebugging;
			}
			return opts;
		}

		public static bool IsWindows(this Platform p)
		{
			return p == Platform.Windows || p == Platform.Windows64;
		}

	}
}