using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor (typeof(PlatformSpecifics))]
public class PlatformSpecificsEditor : Editor {
	
	public Texture2D addTextureUp;
	public Texture2D addTextureDown;
	public Texture2D removeTextureUp;
	public Texture2D removeTextureDown;
	
	public Texture2D moveItemAboveUp;
	public Texture2D moveItemAboveDown;
	public Texture2D moveItemBelowUp;
	public Texture2D moveItemBelowDown;
	
	[SerializeField] GUIStyle addButtonStyle;
	[SerializeField] GUIStyle removeButtonStyle;
	[SerializeField] GUIStyle moveItemAboveStyle;
	[SerializeField] GUIStyle moveItemBelowStyle;
	[SerializeField] GUIStyle enumPopupStyle;
	
	delegate void DrawArray<T>(int index, ref T[] array, ref int itemToDelete, ref int moveItemIdx, ref int moveItemDirection);
	delegate T Constructor<T>();
	delegate T[] AddAll<T>();
	
	static readonly Platform[] defaultPlatforms = {Platform.iPhone, Platform.iPhoneRetina, Platform.iPad, Platform.Standalone, Platform.iPadRetina};
	static readonly AspectRatio[] defaultAspectRatios = {AspectRatio.Aspect4by3, AspectRatio.Aspect16by10, AspectRatio.Aspect16by9};
	
	PlatformSpecifics specifics;
	
	bool showRestrictPlatforms = false;
	bool showMaterials = false;
	bool showLocalScalePerPlatform = false;
	bool showLocalScalePerAspectRatio = false;
	bool showLocalPositionPerPlatform = false;
	bool showLocalPositionPerAspectRatio = false;
	bool showFonts = false;
	bool showTextMeshText = false;
	
	void OnEnable() {
		specifics = target as PlatformSpecifics;
		specifics.Init();
		
		//find texture references
		string path = null;
		int index;
		foreach(string assetPath in AssetDatabase.GetAllAssetPaths()) 
		{
			index = assetPath.IndexOf("MultiPlatformToolSuite");
			if(index >= 0)
			{
				path = assetPath.Substring(0, index);
				break;
			}
		}
		string editorPath = path + "MultiPlatformToolSuite" + Path.DirectorySeparatorChar + "Editor" + Path.DirectorySeparatorChar;
		path = editorPath + "Textures" + Path.DirectorySeparatorChar;
		
		addTextureUp = (Texture2D) AssetDatabase.LoadAssetAtPath(path + "editorAddButtonUp.tga", typeof(Texture2D));
		addTextureDown = (Texture2D) AssetDatabase.LoadAssetAtPath(path + "editorAddButtonDown.tga", typeof(Texture2D));
		removeTextureUp = (Texture2D) AssetDatabase.LoadAssetAtPath(path + "editorRemoveButtonUp.tga", typeof(Texture2D));
		removeTextureDown = (Texture2D) AssetDatabase.LoadAssetAtPath(path + "editorRemoveButtonDown.tga", typeof(Texture2D));
		moveItemAboveUp = (Texture2D) AssetDatabase.LoadAssetAtPath(path + "moveItemAboveUp.tga", typeof(Texture2D));
		moveItemAboveDown = (Texture2D) AssetDatabase.LoadAssetAtPath(path + "moveItemAboveDown.tga", typeof(Texture2D));
		moveItemBelowUp = (Texture2D) AssetDatabase.LoadAssetAtPath(path + "moveItemBelowUp.tga", typeof(Texture2D));
		moveItemBelowDown = (Texture2D) AssetDatabase.LoadAssetAtPath(path + "moveItemBelowDown.tga", typeof(Texture2D));
		
		addButtonStyle = new GUIStyle();
		addButtonStyle.normal.background = addTextureUp;
		addButtonStyle.active.background = addTextureDown;
		addButtonStyle.fixedWidth = addTextureUp.width;
		addButtonStyle.fixedHeight = addTextureUp.height;
		addButtonStyle.margin = new RectOffset(4,4,4,4);
		
		removeButtonStyle = new GUIStyle(addButtonStyle); //Inherit margin and fixedWidth/Height from addButtonStyle
		removeButtonStyle.normal.background = removeTextureUp;
		removeButtonStyle.active.background = removeTextureDown;
		
		moveItemAboveStyle = new GUIStyle(addButtonStyle); //Inherit margin and fixedWidth/Height from addButtonStyle
		moveItemAboveStyle.normal.background = moveItemAboveUp;
		moveItemAboveStyle.active.background = moveItemAboveDown;
		
		moveItemBelowStyle = new GUIStyle(addButtonStyle); //Inherit margin and fixedWidth/Height from addButtonStyle
		moveItemBelowStyle.normal.background = moveItemBelowUp;
		moveItemBelowStyle.active.background = moveItemBelowDown;
		
		showRestrictPlatforms = (specifics.restrictPlatform != null && specifics.restrictPlatform.Length > 0);
		showMaterials = (specifics.materialPerPlatform != null && specifics.materialPerPlatform.Length > 0);
		showLocalScalePerPlatform = (specifics.localScalePerPlatform != null && specifics.localScalePerPlatform.Length > 0);
		showLocalScalePerAspectRatio = (specifics.localScalePerAspectRatio != null && specifics.localScalePerAspectRatio.Length > 0);
		showLocalPositionPerPlatform = (specifics.localPositionPerPlatform != null && specifics.localPositionPerPlatform.Length > 0);
		showLocalPositionPerAspectRatio = (specifics.localPositionPerAspectRatio != null && specifics.localPositionPerAspectRatio.Length > 0);
		showFonts = (specifics.fontPerPlatform != null && specifics.fontPerPlatform.Length > 0);
		showTextMeshText = (specifics.textMeshTextPerPlatform != null && specifics.textMeshTextPerPlatform.Length > 0);
	}
	
	void GatherStyles() {
		if(enumPopupStyle == null) {
			enumPopupStyle = new GUIStyle(EditorStyles.popup);
			enumPopupStyle.fontStyle = FontStyle.Bold;
			enumPopupStyle.fontSize = 10;
			enumPopupStyle.fixedHeight = 18;
		}
	}

	public override void OnInspectorGUI() {
		//Styles that we derive from EditorStyles can't be initialized in OnEnable (causes null-ref)
		GatherStyles();
		
		GUILayout.Space(4f);
		
		//Draw restrict to platforms
		DrawSection<Platform>
		(ref showRestrictPlatforms,
		"Restrict to Platforms",
		ref specifics.restrictPlatform,
		DrawRestrictToPlatforms<Platform>,
		() => {return Platform.Standalone;},
		null);
		
		//Draw materials per platform
		DrawSection<PlatformSpecifics.MaterialPerPlatform>
		(ref showMaterials,
		"Materials",
		ref specifics.materialPerPlatform,
		DrawMaterials<PlatformSpecifics.MaterialPerPlatform>,
		() => {return new PlatformSpecifics.MaterialPerPlatform(Platform.Standalone, null);},
		() => {
				if(specifics.renderer == null) return null;
				PlatformSpecifics.MaterialPerPlatform[] mats = new PlatformSpecifics.MaterialPerPlatform[defaultPlatforms.Length];
				for(int i=0; i<mats.Length; i++)
					mats[i] = new PlatformSpecifics.MaterialPerPlatform(defaultPlatforms[i], specifics.renderer.sharedMaterial);
				return mats;
			});
		
		//Draw local scale per platform
		DrawSection<PlatformSpecifics.LocalScalePerPlatform>
		(ref showLocalScalePerPlatform,
		"Local Scale per Platform",
		ref specifics.localScalePerPlatform,
		DrawLocalScalePerPlatform<PlatformSpecifics.LocalScalePerPlatform>,
		() => {return new PlatformSpecifics.LocalScalePerPlatform(Platform.Standalone, Vector3.zero);},
		() => {
				PlatformSpecifics.LocalScalePerPlatform[] array = new PlatformSpecifics.LocalScalePerPlatform[defaultPlatforms.Length];
				for(int i=0; i<array.Length; i++)
					array[i] = new PlatformSpecifics.LocalScalePerPlatform(defaultPlatforms[i], specifics.transform.localScale);
				return array;
			});
		
		//Draw local scale per aspect ratio
		DrawSection<PlatformSpecifics.LocalScalePerAspectRatio>
		(ref showLocalScalePerAspectRatio,
		"Local Scale per Aspect Ratio",
		ref specifics.localScalePerAspectRatio,
		DrawLocalScalePerAspectRatio<PlatformSpecifics.LocalScalePerAspectRatio>,
		() => {return new PlatformSpecifics.LocalScalePerAspectRatio(AspectRatio.Aspect4by3, Vector3.zero);},
		() => {
				PlatformSpecifics.LocalScalePerAspectRatio[] array = new PlatformSpecifics.LocalScalePerAspectRatio[defaultAspectRatios.Length];
				for(int i=0; i<array.Length; i++)
					array[i] = new PlatformSpecifics.LocalScalePerAspectRatio(defaultAspectRatios[i], specifics.transform.localScale);
				return array;
			});
		
		//Draw local position per platform
		DrawSection<PlatformSpecifics.LocalPositionPerPlatform>
		(ref showLocalPositionPerPlatform,
		"Local Position per Platform",
		ref specifics.localPositionPerPlatform,
		DrawLocalPositionPerPlatform<PlatformSpecifics.LocalPositionPerPlatform>,
		() => {return new PlatformSpecifics.LocalPositionPerPlatform(Platform.Standalone, Vector3.zero);},
		() => {
				PlatformSpecifics.LocalPositionPerPlatform[] array = new PlatformSpecifics.LocalPositionPerPlatform[defaultPlatforms.Length];
				for(int i=0; i<array.Length; i++)
					array[i] = new PlatformSpecifics.LocalPositionPerPlatform(defaultPlatforms[i], specifics.transform.localPosition);
				return array;
			});
		
		//Draw local position per aspect ratio
		DrawSection<PlatformSpecifics.LocalPositionPerAspectRatio>
		(ref showLocalPositionPerAspectRatio,
		"Local Position per Aspect Ratio",
		ref specifics.localPositionPerAspectRatio,
		DrawLocalPositionPerAspectRatio<PlatformSpecifics.LocalPositionPerAspectRatio>,
		() => {return new PlatformSpecifics.LocalPositionPerAspectRatio(AspectRatio.Aspect4by3, Vector3.zero);},
		() => {
				PlatformSpecifics.LocalPositionPerAspectRatio[] array = new PlatformSpecifics.LocalPositionPerAspectRatio[defaultAspectRatios.Length];
				for(int i=0; i<array.Length; i++)
					array[i] = new PlatformSpecifics.LocalPositionPerAspectRatio(defaultAspectRatios[i], specifics.transform.localPosition);
				return array;
			});
		
		//Draw font per platform
		DrawSection<PlatformSpecifics.FontPerPlatform>
		(ref showFonts,
		"Fonts",
		ref specifics.fontPerPlatform,
		DrawFonts<PlatformSpecifics.FontPerPlatform>,
		() => {return new PlatformSpecifics.FontPerPlatform(Platform.Standalone, null, null);},
		() => {
				TextMesh textMesh = specifics.GetComponent<TextMesh>();
				if(textMesh == null || specifics.renderer == null) return null;
				PlatformSpecifics.FontPerPlatform[] array = new PlatformSpecifics.FontPerPlatform[defaultAspectRatios.Length];
				for(int i=0; i<array.Length; i++)
					array[i] = new PlatformSpecifics.FontPerPlatform(defaultPlatforms[i], textMesh.font, specifics.renderer.sharedMaterial);
				return array;
			});
		
		//Draw text mesh text per platform
		DrawSection<PlatformSpecifics.TextMeshTextPerPlatform>
		(ref showTextMeshText,
		"Text mesh text",
		ref specifics.textMeshTextPerPlatform,
		DrawTextMeshText<PlatformSpecifics.TextMeshTextPerPlatform>,
		() => {return new PlatformSpecifics.TextMeshTextPerPlatform(Platform.Standalone, string.Empty);},
		() => {
				TextMesh textMesh = specifics.GetComponent<TextMesh>();
				if(textMesh == null) return null;
				PlatformSpecifics.TextMeshTextPerPlatform[] array = new PlatformSpecifics.TextMeshTextPerPlatform[defaultAspectRatios.Length];
				for(int i=0; i<array.Length; i++)
					array[i] = new PlatformSpecifics.TextMeshTextPerPlatform(defaultPlatforms[i], textMesh.text);
				return array;
			});
	}
	
	void DrawRestrictToPlatforms<T>(int index, ref T[] array, ref int itemToDelete, ref int moveItemIdx, ref int moveItemDirection) {
		Platform[] restrictPlatform = array as Platform[];
		DrawPlatformEnum(index, ref itemToDelete, ref moveItemIdx, ref moveItemDirection, ref restrictPlatform[index]);
	}
	
	void DrawMaterials<T>(int index, ref T[] array, ref int itemToDelete, ref int moveItemIdx, ref int moveItemDirection) {
		PlatformSpecifics.MaterialPerPlatform[] materialsPerPlatform = array as PlatformSpecifics.MaterialPerPlatform[];
		DrawPlatformEnum(index, ref itemToDelete, ref moveItemIdx, ref moveItemDirection, ref materialsPerPlatform[index].platform);
		
		GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			materialsPerPlatform[index].mat = EditorGUILayout.ObjectField(materialsPerPlatform[index].mat, typeof(Material), false) as Material;
		
			if(GUILayout.Button("Get")) {
				if(specifics.renderer != null) {
					Undo.RegisterUndo(specifics, "get material");
					materialsPerPlatform[index].mat = specifics.renderer.sharedMaterial;
				} else {
					Debug.Log("There is no Renderer component on this game object.");
				}
			}
			if(GUILayout.Button("Set")) {
				if(specifics.renderer != null) {
					Undo.RegisterUndo(specifics.renderer, "set material");
					specifics.renderer.sharedMaterial = materialsPerPlatform[index].mat;
				} else {
					Debug.Log("There is no Renderer component on this game object.");
				}
			}
		GUILayout.EndHorizontal();
	}
	
	void DrawLocalScalePerPlatform<T>(int index, ref T[] array, ref int itemToDelete, ref int moveItemIdx, ref int moveItemDirection) {
		PlatformSpecifics.LocalScalePerPlatform[] localScalePerPlatform = array as PlatformSpecifics.LocalScalePerPlatform[];
		DrawPlatformEnum(index, ref itemToDelete, ref moveItemIdx, ref moveItemDirection, ref localScalePerPlatform[index].platform);
		
		GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			localScalePerPlatform[index].localScale = EditorGUILayout.Vector3Field("Local scale", localScalePerPlatform[index].localScale);
			
			GUILayout.BeginVertical();
			GUILayout.Space(18f);
			GUILayout.BeginHorizontal();
		
			if(GUILayout.Button("Get")) {
				Undo.RegisterUndo(specifics, "get local scale");
				localScalePerPlatform[index].localScale = specifics.transform.localScale;
			}
			if(GUILayout.Button("Set")) {
				Undo.RegisterUndo(specifics.transform, "set local scale");
				specifics.transform.localScale = localScalePerPlatform[index].localScale;
			}
		
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
	
	void DrawLocalScalePerAspectRatio<T>(int index, ref T[] array, ref int itemToDelete, ref int moveItemIdx, ref int moveItemDirection) {
		PlatformSpecifics.LocalScalePerAspectRatio[] localScalePerAspectRatio = array as PlatformSpecifics.LocalScalePerAspectRatio[];
		DrawAspectRatiosEnum(index, ref itemToDelete, ref moveItemIdx, ref moveItemDirection, ref localScalePerAspectRatio[index].aspectRatio);
		
		GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			localScalePerAspectRatio[index].localScale = EditorGUILayout.Vector3Field("Local scale", localScalePerAspectRatio[index].localScale);
		
			GUILayout.BeginVertical();
			GUILayout.Space(18f);
			GUILayout.BeginHorizontal();
		
			if(GUILayout.Button("Get")) {
				Undo.RegisterUndo(specifics, "get local scale");
				localScalePerAspectRatio[index].localScale = specifics.transform.localScale;
			}
			if(GUILayout.Button("Set")) {
				Undo.RegisterUndo(specifics.transform, "set local scale");
				specifics.transform.localScale = localScalePerAspectRatio[index].localScale;
			}
		
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
	
	void DrawLocalPositionPerPlatform<T>(int index, ref T[] array, ref int itemToDelete, ref int moveItemIdx, ref int moveItemDirection) {
		PlatformSpecifics.LocalPositionPerPlatform[] localPositionPerPlatform = array as PlatformSpecifics.LocalPositionPerPlatform[];
		DrawPlatformEnum(index, ref itemToDelete, ref moveItemIdx, ref moveItemDirection, ref localPositionPerPlatform[index].platform);
		
		GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			localPositionPerPlatform[index].localPosition = EditorGUILayout.Vector3Field("Local position", localPositionPerPlatform[index].localPosition);
		
			GUILayout.BeginVertical();
			GUILayout.Space(18f);
			GUILayout.BeginHorizontal();
		
			if(GUILayout.Button("Get")) {
				Undo.RegisterUndo(specifics, "get local position");
				localPositionPerPlatform[index].localPosition = specifics.transform.localPosition;
			}
			if(GUILayout.Button("Set")) {
				Undo.RegisterUndo(specifics.transform, "set local position");
				specifics.transform.localPosition = localPositionPerPlatform[index].localPosition;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
	
	void DrawLocalPositionPerAspectRatio<T>(int index, ref T[] array, ref int itemToDelete, ref int moveItemIdx, ref int moveItemDirection) {
		PlatformSpecifics.LocalPositionPerAspectRatio[] localPositionPerAspectRatio = array as PlatformSpecifics.LocalPositionPerAspectRatio[];
		DrawAspectRatiosEnum(index, ref itemToDelete, ref moveItemIdx, ref moveItemDirection, ref localPositionPerAspectRatio[index].aspectRatio);
		
		GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			localPositionPerAspectRatio[index].localPosition = EditorGUILayout.Vector3Field("Local position", localPositionPerAspectRatio[index].localPosition);
		
			GUILayout.BeginVertical();
			GUILayout.Space(18f);
			GUILayout.BeginHorizontal();
		
			if(GUILayout.Button("Get")) {
				Undo.RegisterUndo(specifics, "get local position");
				localPositionPerAspectRatio[index].localPosition = specifics.transform.localPosition;
			}
			if(GUILayout.Button("Set")) {
				Undo.RegisterUndo(specifics.transform, "set local position");
				specifics.transform.localPosition = localPositionPerAspectRatio[index].localPosition;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
	
	void DrawFonts<T>(int index, ref T[] array, ref int itemToDelete, ref int moveItemIdx, ref int moveItemDirection) {
		PlatformSpecifics.FontPerPlatform[] fontsPerPlatform = array as PlatformSpecifics.FontPerPlatform[];
		DrawPlatformEnum(index, ref itemToDelete, ref moveItemIdx, ref moveItemDirection, ref fontsPerPlatform[index].platform);
		
		GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			fontsPerPlatform[index].font = EditorGUILayout.ObjectField(fontsPerPlatform[index].font, typeof(Font), false) as Font;
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			fontsPerPlatform[index].mat = EditorGUILayout.ObjectField(fontsPerPlatform[index].mat, typeof(Material), false) as Material;
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			if(GUILayout.Button("Get")) {
				TextMesh textMesh = specifics.GetComponent<TextMesh>();
				if(textMesh != null && specifics.renderer != null) {
					Undo.RegisterUndo(specifics, "get font and material");
					fontsPerPlatform[index].font = textMesh.font;
					fontsPerPlatform[index].mat = specifics.renderer.sharedMaterial;
				} else {
					Debug.Log("There is no TextMesh or Renderer component on this game object.");
				}
			}
			if(GUILayout.Button("Set")) {
				TextMesh textMesh = specifics.GetComponent<TextMesh>();
				if(textMesh != null && specifics.renderer != null) {
					Undo.RegisterUndo(textMesh, "set font");
					Undo.RegisterUndo(specifics.renderer, "set material");
					textMesh.font = fontsPerPlatform[index].font;
					specifics.renderer.sharedMaterial = fontsPerPlatform[index].mat;
				} else {
					Debug.Log("There is no TextMesh or Renderer component on this game object.");
				}
			}
		GUILayout.EndHorizontal();
	}
	
	void DrawTextMeshText<T>(int index, ref T[] array, ref int itemToDelete, ref int moveItemIdx, ref int moveItemDirection) {
		PlatformSpecifics.TextMeshTextPerPlatform[] textMeshTextPerPlatform = array as PlatformSpecifics.TextMeshTextPerPlatform[];
		DrawPlatformEnum(index, ref itemToDelete, ref moveItemIdx, ref moveItemDirection, ref textMeshTextPerPlatform[index].platform);
		
		GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			textMeshTextPerPlatform[index].text = EditorGUILayout.TextField(textMeshTextPerPlatform[index].text);
		
			if(GUILayout.Button("Get")) {
				TextMesh textMesh = specifics.GetComponent<TextMesh>();
				if(textMesh != null) {
					Undo.RegisterUndo(specifics, "get TextMesh text");
					textMeshTextPerPlatform[index].text = textMesh.text;
				} else {
					Debug.Log("There is no TextMesh component on this game object.");
				}
			}
			if(GUILayout.Button("Set")) {
				TextMesh textMesh = specifics.GetComponent<TextMesh>();
				if(textMesh != null) {
					Undo.RegisterUndo(textMesh, "set TextMesh text");
					textMesh.text = textMeshTextPerPlatform[index].text;
				} else {
					Debug.Log("There is no TextMesh component on this game object.");
				}
			}
		GUILayout.EndHorizontal();
	}
	
	void DrawSection<T>(ref bool show, string header, ref T[] array, DrawArray<T> drawArray, Constructor<T> construct, AddAll<T> addAll) {
		show = EditorGUILayout.Foldout(show, header);
		
		if(!show) return;
		
		int itemToDelete = -1;
		int moveItemIdx = -1;
		int moveItemDirection = 0;
		for(int i=0; i < array.Length; i++) {
			drawArray(i, ref array, ref itemToDelete, ref moveItemIdx, ref moveItemDirection);
			GUILayout.Space(5f);
		}
		
		if(itemToDelete != -1) {
			Undo.RegisterUndo(specifics, "delete array item");
			List<T> list = new List<T>(array as IEnumerable<T>);
			list.RemoveAt(itemToDelete);
			array = list.ToArray();
			EditorUtility.SetDirty(specifics);
		}
		
		if(moveItemIdx != -1) {
			int newIdx = moveItemIdx + moveItemDirection;
			if(newIdx > -1 && newIdx < array.Length) {
				Undo.RegisterUndo(specifics, "move array item");
				List<T> list = new List<T>(array as IEnumerable<T>);
				T item = list[moveItemIdx];
				list.RemoveAt(moveItemIdx);
				list.Insert(newIdx, item);
				array = list.ToArray();
				EditorUtility.SetDirty(specifics);
			}
		}
		
		GUILayout.BeginHorizontal();
			GUILayout.Space(18f);
			if(GUILayout.Button(string.Empty, addButtonStyle)) {
				Undo.RegisterUndo(specifics, "add array item");
				List<T> list = new List<T>(array as IEnumerable<T>);
				list.Add(construct());
				array = list.ToArray();
				EditorUtility.SetDirty(specifics);
			}
			if(array.Length == 0 && addAll != null) {
				GUILayout.Space(5f);
				if(GUILayout.Button("Add Default Entries", GUILayout.Width(120f))) {
					Undo.RegisterUndo(specifics, "add default entries");
					T[] newArray = addAll();
					array = (newArray == null) ? array : newArray; //Only assign if new array isn't null 
				}
			}
		GUILayout.EndHorizontal();
	}
	
	void DrawMoveButtons(int index, ref int moveItemIdx, ref int moveItemDirection) {
		if(GUILayout.Button(string.Empty, moveItemAboveStyle)) {
			moveItemIdx = index;
			moveItemDirection = -1;
		}
		if(GUILayout.Button(string.Empty, moveItemBelowStyle)) {
			moveItemIdx = index;
			moveItemDirection = 1;
		}
	}
	
	void DrawRemoveButton(int index, ref int itemToDelete) {
		if(GUILayout.Button(string.Empty, removeButtonStyle))
			itemToDelete = index;
	}
	
	void DrawAspectRatiosEnum(int index, ref int itemToDelete, ref int moveItemIdx, ref int moveItemDirection, ref AspectRatio _enum) {
		GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			_enum = (AspectRatio) EditorGUILayout.EnumPopup((System.Enum)_enum, enumPopupStyle, GUILayout.Width(100f));
			EditorGUILayout.Space();
			DrawMoveButtons(index, ref moveItemIdx, ref moveItemDirection);
			EditorGUILayout.Space();
			DrawRemoveButton(index, ref itemToDelete);
		GUILayout.EndHorizontal();
	}
	
	void DrawPlatformEnum(int index, ref int itemToDelete, ref int moveItemIdx, ref int moveItemDirection, ref Platform _enum) {
		GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			_enum = (Platform) EditorGUILayout.EnumPopup((System.Enum)_enum, enumPopupStyle, GUILayout.Width(100f));
			EditorGUILayout.Space();
			DrawMoveButtons(index, ref moveItemIdx, ref moveItemDirection);
			EditorGUILayout.Space();
			DrawRemoveButton(index, ref itemToDelete);
		GUILayout.EndHorizontal();
	}
	
	void OnDisable() {
		specifics = null;
	}
}
