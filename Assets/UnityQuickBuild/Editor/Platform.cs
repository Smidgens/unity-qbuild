#if UNITY_EDITOR
namespace Smidgenomics.QuickBuild
{
	internal enum Platform
	{
		None,
		Windows, Windows64,
		Linux, Linux64, LinuxUniversal,
		WebGL
	}
}
#endif