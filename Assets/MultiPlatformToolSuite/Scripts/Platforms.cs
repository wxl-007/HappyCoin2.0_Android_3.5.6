using UnityEngine;
using System.Collections;

//If you want to add new platforms to this enumeration, either put them at the end or give it a unique number
public enum Platform {iPhone=1, WebPlayer=2, iPad=3, iPhoneRetina=4, Standalone=5, Android=6, FlashPlayer=7, NaCl=8, iPadRetina=9};

public class Platforms : MonoBehaviour {

	const float ratioTolerance = 0.03f; //Tolerance of calculated ratio to exact ratio
	
	static bool platformCalculated = false;
	static Platform calculatedPlatform;
	
	public const string editorPlatformOverrideKey = "editorPlatformOverride";
	
	public static Platform platform {
		get {
#if UNITY_EDITOR
			//If in editor and platformOverride is set, return override
			string platformString = UnityEditor.EditorPrefs.GetString(editorPlatformOverrideKey);
			platformCalculated = true;
			if(platformString == Platform.iPhone.ToString()) {
				calculatedPlatform = Platform.iPhone;
			} 
			else if(platformString == Platform.iPhoneRetina.ToString()) {
				calculatedPlatform = Platform.iPhoneRetina;
			} 
			else if(platformString == Platform.Android.ToString()) {
				calculatedPlatform = Platform.Android;
			} 
			else if(platformString == Platform.FlashPlayer.ToString()) {
				calculatedPlatform = Platform.FlashPlayer;
			} else if(platformString == Platform.NaCl.ToString()) {
				calculatedPlatform = Platform.NaCl;	
			} 
			else if(platformString == Platform.iPad.ToString()) {
				calculatedPlatform = Platform.iPad;
			}
			else if(platformString == Platform.iPadRetina.ToString()) {
				calculatedPlatform = Platform.iPadRetina;
			}
			else if(platformString == Platform.WebPlayer.ToString()) {
				calculatedPlatform = Platform.WebPlayer;
			} 
			else {
				calculatedPlatform = Platform.Standalone;
			}
#endif
			if(!platformCalculated) { //If platform wasn't calculated before, calculate now
				if(Application.platform == RuntimePlatform.IPhonePlayer) {
					#if UNITY_IPHONE
					int screenWidth = (int) Screen.width;
					if(screenWidth == 480 || screenWidth == 320) {
						calculatedPlatform = Platform.iPhone;
					} else if(screenWidth == 960 || screenWidth == 640) {
						calculatedPlatform = Platform.iPhoneRetina;
					} else if(screenWidth == 1024 || screenWidth == 768) {
						calculatedPlatform = Platform.iPad;
					} else if(screenWidth == 2048 || screenWidth == 1536) {
						calculatedPlatform = Platform.iPadRetina;
					} else {
						calculatedPlatform = Platform.iPhone; //Default to iPhone
					}
					#endif
				} else if(Application.platform == RuntimePlatform.Android) { 
					calculatedPlatform = Platform.Android; //exact screen size will be calculated per-Aspect Ratio
				} 
#if UNITY_3_5
				else if(Application.platform == RuntimePlatform.FlashPlayer) { 
					calculatedPlatform = Platform.FlashPlayer; //exact screen size will be calculated per-Aspect Ratio
				} else if(Application.platform == RuntimePlatform.NaCl) { 
					calculatedPlatform = Platform.NaCl; //exact screen size will be calculated per-Aspect Ratio
				}
#endif
				else if(Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer) {
					calculatedPlatform = Platform.WebPlayer; //exact screen size will be calculated per-Aspect Ratio
				} else {
					calculatedPlatform = Platform.Standalone; //exact screen size will be calculated per-Aspect Ratio
				}
				platformCalculated = true;
				
			}
			return calculatedPlatform;
		}
	}
	
	public static bool IsPlatformAspectBased(string plat) {
		return plat == Platform.Standalone.ToString() 
			|| plat == Platform.Android.ToString() 
			|| plat == Platform.FlashPlayer.ToString() 
			|| plat == Platform.NaCl.ToString();
	}
	
	public static bool IsiOS {
		get {
			return (Platforms.platform == Platform.iPad || Platforms.platform == Platform.iPhone || Platforms.platform == Platform.iPadRetina || Platforms.platform == Platform.iPhoneRetina); 
		}
	}
}
