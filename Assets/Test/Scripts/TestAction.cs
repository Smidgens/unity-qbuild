using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="QuickBuild/Test Action")]
internal class TestAction : ScriptableObject
{
	public void LogString(string v) { Debug.Log(v); }
}
