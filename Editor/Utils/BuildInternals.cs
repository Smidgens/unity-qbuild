// smidgens @ github

namespace Smidgenomics.Unity.QBuild.Editor
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using System.Linq;
	using System.Reflection;

	internal static class BuildInternals
	{
		// peeking into unity's business
		public static class InternalName
		{
			public const string
			MODULE_MANAGER = "UnityEditor.Modules.ModuleManager,UnityEditor.dll",
			IS_PLATFORM_SUPPORTED = "IsPlatformSupportLoaded",
			GET_TARGET_STRING = "GetTargetStringFromBuildTarget";
		}

		public static bool IsSupported(BuildTarget target)
		{
			var ts = (string)_GetTargetString.Value.Invoke(null, new object[] { target });
			return (bool)_IsPlatformSupportLoaded.Value.Invoke(null, new object[] { ts });
		}

		public static BuildTarget[] GetAvailableBuildTargets()
		{
			return
			Enum.GetValues(typeof(BuildTarget)).Cast<BuildTarget>()
			.Where(x => IsSupported(x))
			.ToArray();
		}

		public static BuildTargetGroup[] GetSupportedTargets(BuildTarget[] targets)
		{
			var supported = new List<BuildTargetGroup>();
			foreach(var t in targets)
			{
				var g = t.ToTargetGroup();
				if(g != BuildTargetGroup.Unknown)
				{
					if(!supported.Contains(g)) { supported.Add(g); }
				}
			}
			return supported.ToArray();
		}

		private static Lazy<Type> _ModManager = new Lazy<Type>(() =>
		{
			return Type.GetType(InternalName.MODULE_MANAGER);
		});

		private static Lazy<MethodInfo>
		_IsPlatformSupportLoaded = GetModMethod(InternalName.IS_PLATFORM_SUPPORTED),
		_GetTargetString = GetModMethod(InternalName.GET_TARGET_STRING);

		private const BindingFlags _STATIC_INTERNAL =
		BindingFlags.Static | BindingFlags.NonPublic;

		private static Lazy<MethodInfo> GetModMethod(string name)
		{
			return new Lazy<MethodInfo>(() => _ModManager.Value.GetMethod(name, _STATIC_INTERNAL));
		}
	}
}