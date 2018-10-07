using UnityEngine;
using UnityEditor;

public class EasyPlatform : EditorWindow {
	
	static Platform currentPlatform;
	
    [MenuItem("Window/MultiPlatform ToolKit/Easy Platform %#l", false, 3)] //Cmd+Shift+L
    static void Init () {
        EasyPlatform window = (EasyPlatform)EditorWindow.GetWindow(typeof(EasyPlatform));
		window.Show();
    }
	
	void OnEnable() {
		currentPlatform = (Platform) System.Enum.Parse(typeof(Platform), EditorPrefs.GetString(Platforms.editorPlatformOverrideKey, "Standalone"));
		minSize = new Vector2(160, 20);
		maxSize = new Vector2(300, 40);
	}
	
	void OnGUI() {
		currentPlatform = (Platform) EditorGUILayout.EnumPopup("Platform: ", (System.Enum) currentPlatform);
		if(GUI.changed)  {
			EditorPrefs.SetString(Platforms.editorPlatformOverrideKey, currentPlatform.ToString());
		}
	}
	
	//Do we want to remove the override when the editor window is closed?
//	void OnDisable() {
//		EditorPrefs.DeleteKey(PrefKeys.editorPlatformOverride);
//	}
}
