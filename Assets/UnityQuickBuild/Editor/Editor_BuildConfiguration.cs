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


	[CustomEditor(typeof(BuildConfiguration))]
	internal class CI_BuildConfiguration : Editor
	{
		public override void OnInspectorGUI()
		{
			if(_cnBox == null)
			{
				var cnbox = new GUIStyle((GUIStyle)"CN Box");
				_cnBox = new GUIStyle(GUI.skin.box);
				_cnBox.normal.background = cnbox.normal.background;
				_cnBox.active.background = cnbox.normal.background;
				_cnBox.hover.background = cnbox.hover.background;
			}
			DrawBuildSettingsGUI();

			if(_includeList.Length != _assetList.serializedProperty.arraySize)
			{
				_includeList = new int[_assetList.serializedProperty.arraySize];
			}

			int includeCounter = 0;
			for(int i = 0; i < _includeList.Length; i++)
			{
				var p = _assetList.serializedProperty.GetArrayElementAtIndex(i);
				var asset = p.FindPropertyRelative("_asset");
				if(asset.objectReferenceValue && !p.FindPropertyRelative("_mute").boolValue)
				{
					_includeList[i] = includeCounter;
					includeCounter++;
				}
				else
				{
					_includeList[i] = -1;
				}
			}

			if(_script.Platform == Platform.None) { return; 	}

		
			// output directory
			IndentedToggleGUI("Custom Output Folder", _overridePath, () =>
			{
				EditorGUILayout.PropertyField(_buildPath, new GUIContent("Path"));
			});

			// executable name
			IndentedToggleGUI("Override Executable Name", _overrideName, () =>
			{
				EditorGUILayout.PropertyField(_executableName, new GUIContent("Name"));
			});

			// scene list
			VerticalGUI(() =>
			{
				_assetList.DoLayoutList();
				var r = GUILayoutUtility.GetLastRect();
				r.width = 100f;
				if(GUI.Button(r, "Add Open Scenes", EditorStyles.miniButton)) { AddOpenScenes(); }
			}, _cnBox);

			VerticalGUI(() => EditorGUILayout.PropertyField(_onPostBuild), _cnBox);




			serializedObject.ApplyModifiedProperties();


			VerticalGUI(() =>
			{
				EditorGUILayout.LabelField("Scenes", EditorStyles.boldLabel);
				IndentedGUI(() =>
				{
					int activeScenes = 0;
					for(int i = 0; i < _includeList.Length; i++)
					{
						if(i >= _assetList.serializedProperty.arraySize) { break; }
						if(_includeList[i] < 0) { continue; }
						var p = _assetList.serializedProperty.GetArrayElementAtIndex(i);
						EditorGUILayout.LabelField("➤ " + _includeList[i] + " | " + ((SceneAsset)p.FindPropertyRelative("_asset").objectReferenceValue).name);
						activeScenes++;
						
					}
					if(activeScenes == 0) { EditorGUILayout.LabelField("No Scenes in Build!"); }
				});

				EditorGUILayout.LabelField("Path", EditorStyles.boldLabel);
				IndentedGUI(() =>
				{
					EditorGUILayout.LabelField(_script.HasRelativeOutput ? "/" + _script.BuildPath : _script.FullBuildPath, EditorStyles.wordWrappedMiniLabel);
				});

				EditorGUILayout.LabelField("Executable", EditorStyles.boldLabel);
				IndentedGUI(() =>
				{
					EditorGUILayout.LabelField(_script.ExecutableName, EditorStyles.wordWrappedMiniLabel);
				});

			}, GUI.skin.box);

			HorizontalGUI(() =>
			{
				if(GUILayout.Button("Start Build"))
				{
					EditorApplication.delayCall += _script.StartBuild;
				}
				GUILayout.FlexibleSpace();
			});
		}

		private GUIStyle _cnBox = null;

		private ReorderableList _assetList = null;
		private int[] _includeList = {};
		private SerializedProperty _overrideName = null;
		private SerializedProperty _overridePath = null;
		private SerializedProperty _executableName = null;
		private SerializedProperty _buildPath = null;
		private SerializedProperty _developmentBuild = null;
		private SerializedProperty _autoconnectProfiler = null;
		private SerializedProperty _scriptDebugging = null;
		private SerializedProperty _scriptsOnlyBuild = null;
		private SerializedProperty _compressionMethod = null;
		private SerializedProperty _onPostBuild = null;

		private BuildConfiguration _script = null;
		private List<BuildTarget> _supportedBuildTargets = null;
		
		private struct MenuOption
		{
			public Platform platform;
			public string displayName;
			public MenuOption(Platform p, string dn) { platform = p; displayName = dn; }
		}

		private List<MenuOption> _platformMenuOptions = new List<MenuOption>();


		private void OnEnable()
		{
			_supportedBuildTargets = BuildUtility.GetSupportedTargets();

			_platformMenuOptions.Add(new MenuOption(Platform.None, "None"));
			if(_supportedBuildTargets.Contains(BuildTarget.StandaloneWindows))
			{
				_platformMenuOptions.Add(new MenuOption(Platform.Windows, "Windows/x86"));
				_platformMenuOptions.Add(new MenuOption(Platform.Windows64, "Windows/x64"));
			}

			if(_supportedBuildTargets.Contains(BuildTarget.StandaloneLinux))
			{
				_platformMenuOptions.Add(new MenuOption(Platform.Linux, "Linux/x86"));
				_platformMenuOptions.Add(new MenuOption(Platform.Linux64, "Linux/x64"));
				_platformMenuOptions.Add(new MenuOption(Platform.LinuxUniversal, "Linux/Universal"));
			}

			if(_supportedBuildTargets.Contains(BuildTarget.WebGL))
			{
				_platformMenuOptions.Add(new MenuOption(Platform.WebGL, "WebGL"));
			}			

			_script = (BuildConfiguration)target;
			var sceneProp = serializedObject.FindProperty("_scenes");
			_onPostBuild = serializedObject.FindProperty("_onPostBuild");
			_compressionMethod = serializedObject.FindProperty("_compressionMethod");
			_overrideName = serializedObject.FindProperty("_overrideName");
			_executableName = serializedObject.FindProperty("_executableName");
			_overridePath = serializedObject.FindProperty("_overridePath");
			_buildPath = serializedObject.FindProperty("_buildPath");
			_developmentBuild = serializedObject.FindProperty("_developmentBuild");
			_autoconnectProfiler = serializedObject.FindProperty("_autoconnectProfiler");
			_scriptDebugging = serializedObject.FindProperty("_scriptDebugging");
			_scriptsOnlyBuild = serializedObject.FindProperty("_scriptsOnlyBuild");

			_assetList = new ReorderableList(serializedObject, sceneProp, true, true, true, true);
			_assetList.drawElementCallback = DrawElement;
			_assetList.drawHeaderCallback = DrawHeader;
			// _assetList.elementHeight = EditorGUIUtility.singleLineHeight * 1.5f;
			Undo.undoRedoPerformed -= OnUndo;
			Undo.undoRedoPerformed += OnUndo;
		}

		private void OnDisable()
		{
			Undo.undoRedoPerformed -= OnUndo;
		}

		private void OnUndo()
		{
			serializedObject.Update();
			Repaint();
		}

		private void DrawPlatformDropdown()
		{
			var platform = serializedObject.FindProperty("_platform");

			HorizontalGUI(() =>
			{
				string dn = "";
				var dd = _platformMenuOptions.FirstOrDefault(x => (int)x.platform == platform.enumValueIndex);

				if(dd.displayName == null) { dn = "None"; }
				else { dn = dd.displayName; }

				EditorGUILayout.PrefixLabel("Platform");

				if(GUILayout.Button(dn, EditorStyles.popup))
				{
					GenericMenu m = new GenericMenu();
					foreach(var mo in _platformMenuOptions)
					{
						int currentPlatform = (int)mo.platform;
						m.AddItem(new GUIContent(mo.displayName), currentPlatform == platform.enumValueIndex, () =>
						{
							platform.enumValueIndex = currentPlatform;
							serializedObject.ApplyModifiedProperties();
							Repaint();
						});
					}
					m.ShowAsContext();
				}
			});

		}

		private void DrawBuildSettingsGUI()
		{
			

			VerticalGUI(() =>
			{
				HorizontalGUI(() =>
				{
					var headerRect = EditorGUILayout.GetControlRect(GUILayout.Width(40f), GUILayout.Height(40f));
					DrawIconHeader(headerRect, _script.Platform);
					VerticalGUI(() =>
					{
						EditorGUILayout.LabelField(_script.Platform.ToString(), EditorStyles.largeLabel);
						var hr = EditorGUILayout.GetControlRect(GUILayout.Height(1f));
						// EditorGUI.DrawRect(hr, Color.black * 0.5f);
						EditorGUI.DrawRect(hr, GUI.color * 0.5f);
						DrawPlatformDropdown();

					});
				});
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_compressionMethod);


				


			}, GUI.skin.box);
			// }, _cnBox);
			

			if(_script.Platform == Platform.None) { return; }

			IndentedToggleGUI("Development Build", _developmentBuild, () =>
			{
				if(_script.Platform == Platform.WebGL)
				{
					EditorGUILayout.HelpBox("Note that WebGL builds are much larger than release builds and should not be published.", MessageType.None);
				}
				// EditorGUILayout.PropertyField(_usePrebuiltEngine);
				EditorGUILayout.PropertyField(_autoconnectProfiler);
				EditorGUILayout.PropertyField(_scriptDebugging);
				EditorGUILayout.PropertyField(_scriptsOnlyBuild);
			});

			
		}

		private void DrawIconHeader(Rect r, Platform platform)
		{
			var t = BuildUtility.FindIcon(platform);
			float s = Mathf.Min(r.width, r.height);
			var imgr = new Rect(0, 0, s, s);
			imgr.center = r.center;
			imgr.x = r.x;
			GUI.Box(imgr, GUIContent.none, EditorStyles.helpBox);
			if(t != null)
			{
				GUI.DrawTexture(imgr.Resize(-10f, -10f), t, ScaleMode.ScaleToFit);
			}
			else
			{
				GUI.DrawTexture(imgr.Resize(-20f, -20f), EditorGUIUtility.FindTexture( "console.warnicon" ), ScaleMode.ScaleToFit);
			}
		}

		private static void VerticalGUI(System.Action onGUI, GUIStyle s = null)
		{
			if(s != null) { EditorGUILayout.BeginVertical(s); } else { EditorGUILayout.BeginVertical(); }
			onGUI();
			EditorGUILayout.EndVertical();
		}

		private static void HorizontalGUI(System.Action onGUI, GUIStyle s = null)
		{
			if(s != null) { EditorGUILayout.BeginHorizontal(s); } else { EditorGUILayout.BeginHorizontal(); }
			onGUI();
			EditorGUILayout.EndHorizontal();
		}

		private static void IndentedToggleGUI(string label, SerializedProperty toggleProperty, System.Action onGUI, int indent = 1)
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);
			toggleProperty.boolValue = EditorGUILayout.ToggleLeft(label, toggleProperty.boolValue);
			if(toggleProperty.boolValue) { IndentedGUI(onGUI, indent); }
			EditorGUILayout.EndVertical();
		}

		private static void IndentedGUI(System.Action onGUI, int indent = 1)
		{
			EditorGUI.indentLevel += indent;
			onGUI();
			EditorGUI.indentLevel -= indent;
		}

		

		private void AddOpenScenes()
		{
			_script.AddOpenScenes();
			_assetList.serializedProperty.serializedObject.Update();
			Repaint();
		}

		private void DrawHeader(Rect r)
		{
			EditorGUI.LabelField(r.SetHeight(EditorGUIUtility.singleLineHeight), _assetList.serializedProperty.displayName);
		}

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			rect = rect.SetHeight(EditorGUIUtility.singleLineHeight);
			rect.y -= 1.5f;

			Rect[] cols = rect.SliceHorizontalMixed(20f, 30f, 0.4f, 0.6f);
		
			var p = _assetList.serializedProperty.GetArrayElementAtIndex(index);
			var asset = p.FindPropertyRelative("_asset");
			var mute = p.FindPropertyRelative("_mute");

			EditorGUI.LabelField(cols[0], new GUIContent(_includeList[index] > -1 ? _includeList[index].ToString() : ""));
			mute.boolValue = !EditorGUI.Toggle(cols[1], !mute.boolValue);

			EditorGUI.PrefixLabel(cols[2], new GUIContent(asset.objectReferenceValue ? ((SceneAsset)asset.objectReferenceValue).name : ""));
			asset.objectReferenceValue = EditorGUI.ObjectField(cols[3], GUIContent.none, asset.objectReferenceValue, typeof(SceneAsset), true);

		}
		
		private void TooltipLabel(Rect r, string label)
		{
			EditorGUI.LabelField(r,  new GUIContent(label, label));
		}
	}
}
#endif