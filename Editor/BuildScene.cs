// smidgens @ github

namespace Smidgenomics.Unity.QBuild.Editor
{
	using System;
	using UnityEditor;

	[Serializable]
	internal struct BuildScene
	{
		public bool skip;
		public SceneAsset asset;
	}
}