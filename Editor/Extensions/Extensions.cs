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

		public static bool IsStandalone(this EBuildPlatform p)
		{
			return
			p != 0
			&& p != EBuildPlatform.WebGL;
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

		public static BuildTarget ToBuildTarget(this EBuildPlatform p)
		{
			switch (p)
			{
				case EBuildPlatform.Windows: return BuildTarget.StandaloneWindows;
				case EBuildPlatform.Windows64: return BuildTarget.StandaloneWindows64;
				case EBuildPlatform.Linux64: return BuildTarget.StandaloneLinux64;
				case EBuildPlatform.WebGL: return BuildTarget.WebGL;
			}
			return BuildTarget.NoTarget;
		}

		public static BuildOptions ToBuildOptions(this ECompressionMethod c)
		{
			switch (c)
			{
				case ECompressionMethod.LZ4: return BuildOptions.CompressWithLz4;
				case ECompressionMethod.LZ4HC: return BuildOptions.CompressWithLz4HC;
			}
			return BuildOptions.None;
		}

		public static BuildOptions ToBuildOptions(this EDevBuildOptions o)
		{
			BuildOptions opts = 0;
			if (o.HasFlag(EDevBuildOptions.Profiler))
			{
				opts |= BuildOptions.ConnectWithProfiler;
			}
			if (o.HasFlag(EDevBuildOptions.ScriptsOnly))
			{
				opts |= BuildOptions.BuildScriptsOnly;
			}
			if (o.HasFlag(EDevBuildOptions.Debugging))
			{
				opts |= BuildOptions.AllowDebugging;
			}
			return opts;
		}

		public static bool IsWindows(this EBuildPlatform p)
		{
			return p == EBuildPlatform.Windows || p == EBuildPlatform.Windows64;
		}

	}
}