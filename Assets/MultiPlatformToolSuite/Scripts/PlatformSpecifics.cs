using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlatformSpecifics : MonoBehaviour {
	
	public Platform[] restrictPlatform;
	
	[System.Serializable]
	public class MaterialPerPlatform {
		public Platform platform;
		public Material mat;
		
		public MaterialPerPlatform (Platform _platform, Material _mat) {
			platform = _platform;
			mat = _mat;
		}
	}
	public MaterialPerPlatform[] materialPerPlatform;
	
	
	[System.Serializable]
	public class LocalScalePerPlatform {
		public Platform platform;
		public Vector3 localScale;
		
		public LocalScalePerPlatform (Platform _platform, Vector3 _localScale) {
			platform = _platform;
			localScale = _localScale;
		}
	}
	public LocalScalePerPlatform[] localScalePerPlatform;
	
	[System.Serializable]
	public class LocalScalePerAspectRatio {
		public AspectRatio aspectRatio;
		public Vector3 localScale;
		
		public LocalScalePerAspectRatio (AspectRatio _aspectRatio, Vector3 _localScale) {
			aspectRatio = _aspectRatio;
			localScale = _localScale;
		}
	}
	public LocalScalePerAspectRatio[] localScalePerAspectRatio;
	
	[System.Serializable]
	public class LocalPositionPerPlatform {
		public Platform platform;
		public Vector3 localPosition;
		
		public LocalPositionPerPlatform (Platform _platform, Vector3 _localPosition) {
			platform = _platform;
			localPosition = _localPosition;
		}
	}
	public LocalPositionPerPlatform[] localPositionPerPlatform;
	
	[System.Serializable]
	public class LocalPositionPerAspectRatio {
		public AspectRatio aspectRatio;
		public Vector3 localPosition;
		
		public LocalPositionPerAspectRatio (AspectRatio _aspectRatio, Vector3 _localPosition) {
			aspectRatio = _aspectRatio;
			localPosition = _localPosition;
		}
	}
	public LocalPositionPerAspectRatio[] localPositionPerAspectRatio;
	
	[System.Serializable]
	public class FontPerPlatform {
		public Platform platform;
		public Font font;
		public Material mat;
		
		public FontPerPlatform (Platform _platform, Font _font, Material _mat) {
			platform = _platform;
			font = _font;
			mat = _mat;
		}
	}
	public FontPerPlatform[] fontPerPlatform;
	
	[System.Serializable]
	public class TextMeshTextPerPlatform {
		public Platform platform;
		public string text;
		
		public TextMeshTextPerPlatform (Platform _platform, string _text) {
			platform = _platform;
			text = _text;
		}
	}
	public TextMeshTextPerPlatform[] textMeshTextPerPlatform;
	
	void Awake () {
		Init();
		ApplySpecifics(Platforms.platform);
	}
	
	public void Init() {
		if(restrictPlatform == null) restrictPlatform = new Platform[0];
		if(materialPerPlatform == null) materialPerPlatform = new MaterialPerPlatform[0];
		if(localScalePerPlatform == null) localScalePerPlatform = new LocalScalePerPlatform[0];
		if(localScalePerAspectRatio == null) localScalePerAspectRatio = new LocalScalePerAspectRatio[0];
		if(localPositionPerPlatform == null) localPositionPerPlatform = new LocalPositionPerPlatform[0];
		if(localPositionPerAspectRatio == null) localPositionPerAspectRatio = new LocalPositionPerAspectRatio[0];
		if(fontPerPlatform == null) fontPerPlatform = new FontPerPlatform[0];
		if(textMeshTextPerPlatform == null) textMeshTextPerPlatform = new TextMeshTextPerPlatform[0];
	}
	
	public void ApplySpecifics(Platform platform) {
		ApplySpecifics(platform, true);
	}
	
	public void ApplySpecifics(Platform platform, bool applyPlatformRestriction) {
		if(applyPlatformRestriction) {
			bool currentPlatformActive = ApplyRestrictPlatform(platform);
			if(!currentPlatformActive)
				return;
		}
		
		ApplyMaterial(platform);
		ApplyLocalScale(platform);
		ApplyLocalPosition(platform);
		ApplyFont(platform);
		ApplyTextMeshText(platform);
	}
	
	public bool ApplyRestrictPlatform(Platform platform) {
		if(restrictPlatform != null && restrictPlatform.Length > 0) {
			bool foundPlatform = false;
			for(int i=0; i<restrictPlatform.Length; i++) {
				if(platform == restrictPlatform[i]) {
					foundPlatform = true;
					break;
				}
			}
			if(!foundPlatform) {
				if(Application.isEditor)
					DestroyImmediate(gameObject, true);
				else
					Destroy(gameObject);
				
				return false;
			} else {
				return true;
			}
		}
		return true;
	}
	
	public void ApplyMaterial(Platform platform) {
		if(materialPerPlatform != null) {
			foreach(MaterialPerPlatform pair in materialPerPlatform) {
				if(platform == pair.platform) {
					renderer.sharedMaterial = pair.mat;
					break;
				}
			}
		}
	}
	
	public void ApplyLocalScale(Platform platform) {
		if(localScalePerPlatform != null) {
			foreach(LocalScalePerPlatform pair in localScalePerPlatform) {
				if(platform == pair.platform) {
					transform.localScale = pair.localScale;
					break;
				}
			}
		}
		
		if(Platforms.IsPlatformAspectBased(platform.ToString()) && localScalePerAspectRatio != null) {
			foreach(LocalScalePerAspectRatio pair in localScalePerAspectRatio) {
				if(AspectRatios.GetAspectRatio() == pair.aspectRatio) {
					transform.localScale = pair.localScale;
					break;
				}
			}
		}
	}
	
	public void ApplyLocalPosition(Platform platform) {
		if(localPositionPerPlatform != null) {
			foreach(LocalPositionPerPlatform pair in localPositionPerPlatform) {
				if(platform == pair.platform) {
					transform.localPosition = pair.localPosition;
					break;
				}
			}
		}
		
		if(Platforms.IsPlatformAspectBased(platform.ToString()) && localPositionPerAspectRatio != null) {
			foreach(LocalPositionPerAspectRatio pair in localPositionPerAspectRatio) {
				if(AspectRatios.GetAspectRatio() == pair.aspectRatio) {
					transform.localPosition = pair.localPosition;
					break;
				}
			}
		}
	}
	
	public void ApplyFont(Platform platform) {
		if(fontPerPlatform != null) {
			foreach(FontPerPlatform pair in fontPerPlatform) {
				if(platform == pair.platform) {
					this.GetComponent<TextMesh>().font = pair.font;
					renderer.sharedMaterial = pair.mat;
					break;
				}
			}
		}
	}
	
	public void ApplyTextMeshText(Platform platform) {
		if(textMeshTextPerPlatform != null) {
			foreach(TextMeshTextPerPlatform pair in textMeshTextPerPlatform) {
				if(platform == pair.platform) {
					this.GetComponent<TextMesh>().text = pair.text;
					break;
				}
			}
		}
	}
}
