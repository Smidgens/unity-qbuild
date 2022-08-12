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

namespace Smidgenomics.Unity.QBuild.Editor
{
	using UnityEngine;

	internal static class Rect_
	{
		public static Rect Resize(this Rect r, float xOffset, float yOffset)
		{
			Rect rr = new Rect(r.x, r.y, r.width + xOffset, r.height + yOffset);
			rr.center = r.center;
			return rr;
		}

		public static Rect[] SliceLeft(this Rect r, float leftWidth)
		{
			return new Rect[]
			{
				new Rect(r.x, r.y, leftWidth, r.height),
				new Rect(r.x + leftWidth, r.y, r.width - leftWidth, r.height)
			};
		}

		public static Rect[] SliceRight(this Rect r, float rightWidth)
		{
			return new Rect[]
			{
				new Rect(r.x, r.y, r.width - rightWidth, r.height),
				new Rect(r.x + r.width - rightWidth, r.y, rightWidth, r.height)
			};
		}

		public static Rect TrimHeight(this Rect r, float trim)
		{
			Vector2 center = r.center;
			r.height -= trim;
			r.center = center;
			return r;
		}

		public static Rect SetHeight(this Rect r, float height)
		{
			Vector2 center = r.center;
			r.height = height;
			r.center = center;
			return r;
		}

		public static Rect[] SliceLeftProportional(this Rect r, float leftProportion)
		{
			var leftWidth = r.width * leftProportion;
			return new Rect[]
			{
				new Rect(r.x, r.y, leftWidth, r.height),
				new Rect(r.x + leftWidth, r.y, r.width - leftWidth, r.height)
			};
		}

		public static Rect[] SliceHorizontalMixed(this Rect r, params float[] widths)
		{
			return r.SliceHorizontalMixedPadded(0f, widths);
		}

		public static Rect[] SliceHorizontalMixedPadded(this Rect r, float padding, params float[] widths)
		{
			float absoluteWidth = padding * (widths.Length - 1);
			for(int i = 0; i < widths.Length; i++)
			{
				if(widths[i] <= 1f){ continue; }
				absoluteWidth += widths[i];
			}
			float remainder = r.width - absoluteWidth;
			if(remainder < 0) { remainder = 0f; }

			Rect[] rects = new Rect[widths.Length];
			float offset = 0f;
			for(int i = 0; i < widths.Length; i++)
			{
				rects[i] = new Rect(r.x + offset, r.y, widths[i] <= 1f ? widths[i] * remainder : widths[i], r.height);
				offset += rects[i].width + padding;
			}
			return rects;
		}

		public static Rect[] SliceHorizontal(this Rect r, int n)
		{
			Rect[] rects = new Rect[n];
			r.width = r.width / n;

			for(int i = 0; i < rects.Length; i++)
			{
				rects[i] = r;
				r.x += r.width;
			}
			return rects;
		}
	}
}