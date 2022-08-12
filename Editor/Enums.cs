// smidgens @ github

namespace Smidgenomics.Unity.QBuild.Editor
{
	using System;
	using UnityEngine;

	[Flags]
	internal enum DevBuildOptions
	{
		None = 0,
		Profiler = 1,
		Debugging = 2,
		ScriptsOnly = 4,
		Everything = ~0,
	}

	internal enum Platform
	{
		None,
		Windows,
		[InspectorName("Windows (x64)")]
		Windows64,
		[InspectorName("Linux (x64)")]
		Linux64,
		WebGL,
	}

	internal enum CompressionMethod
	{
		None,
		LZ4,
		LZ4HC,
	}
}