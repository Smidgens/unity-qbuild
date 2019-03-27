#if UNITY_EDITOR
namespace Smidgenomics.QuickBuild
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	public static partial class RectExtensions
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
#endif