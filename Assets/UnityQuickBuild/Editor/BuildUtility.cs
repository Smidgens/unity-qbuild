#if UNITY_EDITOR
namespace Smidgenomics.QuickBuild
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using System.Linq;

	internal static class BuildUtility
	{

		public static string GetDisplayName(BuildTarget platform)
		{
			string name;
			if(_targetDisplayNames.TryGetValue(platform, out name))
			{
				return name;
			}
			return platform.ToString();
		}

		public static List<BuildTarget> GetSupportedTargets()
		{
			var values = Enum.GetValues(typeof(BuildTarget)).Cast<BuildTarget>();
			List<BuildTarget> supported = new List<BuildTarget>();
			foreach(var v in values)
			{
				if(IsSupported(v)) { supported.Add(v); }
			}
			return supported;
		}

		public static BuildTargetGroup[] GetSupportedTargets(BuildTarget[] targets)
		{
			List<BuildTargetGroup> supported = new List<BuildTargetGroup>();
			foreach(var t in targets)
			{
				var g = GetGroup(t);
				if(g != BuildTargetGroup.Unknown)
				{
					if(!supported.Contains(g)) { supported.Add(g); }
				}
			}
			return supported.ToArray();
		}

		public static Texture2D FindIcon(BuildTargetGroup target, bool small = false)
		{
			var name = "";
			switch(target)
			{
				case BuildTargetGroup.iOS: name = "iPhone"; break;
				default: name = target.ToString(); break;
			}
			var path = string.Format("BuildSettings.{0}{1}", name, small ? ".small" : "");
			return EditorGUIUtility.FindTexture(path);
		}

		public static Texture2D FindIcon(Platform target, bool small = false)
		{
			string name = null;
			
			switch(target)
			{
				case Platform.Windows:
				case Platform.Windows64:
				case Platform.Linux:
				case Platform.Linux64:
				case Platform.LinuxUniversal:
					name = "Standalone"; break;
				case Platform.WebGL:
					name = "WebGL"; break;
			}
			if(name != null)
			{
				var path = string.Format("BuildSettings.{0}{1}", name, small ? ".small" : "");
				return EditorGUIUtility.FindTexture(path);
			}
			return null;
		}

		public static Dictionary<BuildTarget, string> _targetDisplayNames = new Dictionary<BuildTarget, string>()
		{
			{ BuildTarget.StandaloneWindows, "Windows/x86" },
			{ BuildTarget.StandaloneWindows64, "Windows/x64" },
			{ BuildTarget.StandaloneLinux, "Linux/x86" },
			{ BuildTarget.StandaloneLinux64, "Linux/x64" },
			{ BuildTarget.StandaloneLinuxUniversal, "Linux/Universal" },
			{ BuildTarget.WebGL, "WebGL" }
		};

		private static Type _moduleManager = null;
		private static System.Reflection.MethodInfo _isPlatformSupportLoaded = null, _getTargetString = null;

		private static BuildTargetGroup GetGroup(BuildTarget target)
		{
			switch(target)
			{
				case BuildTarget.WebGL: return BuildTargetGroup.WebGL;
				case BuildTarget.iOS: return BuildTargetGroup.iOS;
				case BuildTarget.Android: return BuildTargetGroup.Android;
			}
			
			if(target.ToString().Contains("Standalone")) {  return BuildTargetGroup.Standalone; } 
			return BuildTargetGroup.Unknown;
		}

		// modified from source: https://answers.unity.com/questions/1324195/detect-if-build-target-is-installed.html
		// user: https://answers.unity.com/users/103152/brianruggieri.html
		private static bool IsSupported(BuildTarget target)
		{

			if(_isPlatformSupportLoaded == null)
			{
				_moduleManager = System.Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
				_isPlatformSupportLoaded = _moduleManager.GetMethod("IsPlatformSupportLoaded", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
				_getTargetString = _moduleManager.GetMethod("GetTargetStringFromBuildTarget", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			}
			return (bool)_isPlatformSupportLoaded.Invoke(null,new object[] {(string)_getTargetString.Invoke(null, new object[] {target})});
		}


	}
}
#endif