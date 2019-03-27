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


	[Serializable]
	internal struct SceneAssetReference
	{
		public SceneAsset Asset { get { return _asset; } }
		public bool Include { get { return !_mute; }  }
		[SerializeField] private bool _mute;
		[SerializeField] private SceneAsset _asset;
		public SceneAssetReference(SceneAsset asset, bool mute = false) { _asset = asset; _mute = mute; }
	}
}
#endif