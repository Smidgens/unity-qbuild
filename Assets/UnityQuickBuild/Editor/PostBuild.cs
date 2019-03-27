#if UNITY_EDITOR
namespace Smidgenomics.QuickBuild
{
	using System;
	using UnityEngine;
	using UnityEditor.Callbacks;
	using UnityEditor;
	using System.IO;

	internal static class PostBuild
	{
		public delegate void BuildCallback(string builtPath);
		public static event BuildCallback onBuilt = null;

		[PostProcessBuildAttribute(1)]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
		{
			if(onBuilt != null){ onBuilt(pathToBuiltProject); }
			onBuilt = null;
		}
	}
}
#endif