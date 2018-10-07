using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class NativeDialogs : MonoBehaviour {
#if UNITY_ANDROID
	AndroidJavaObject _pluginObject;
#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern int messageBox(string caption, string message, string[] buttons, int numButtons, string gameObject);
	
	[DllImport ("__Internal")]
	private static extern int loginPasswordMessageBox(string caption, string message, string[] buttons, int numButtons, string gameObject);
	
	[DllImport ("__Internal")]
	private static extern int promptMessageBox(string caption, string message, string[] buttons, int numButtons, string gameObject);
	
	[DllImport ("__Internal")]
	private static extern int securePromptMessageBox(string caption, string message, string[] buttons, int numButtons, string gameObject);
	
	[DllImport ("__Internal")]
	private static extern void progressDialog(string caption, string message);
	
	[DllImport ("__Internal")]
	private static extern void hideProgressDialog();
#endif
	
	
	[StructLayout (LayoutKind.Explicit)]
	public struct ActionContainer
	{
		[FieldOffset (0)]
		public Action<string> action1;
		[FieldOffset (0)]
		public Action<string, string> action2;
		[FieldOffset (0)]
		public Action<string, string, string> action3;		
	}
	
	
	static NativeDialogs _instance;
	Dictionary<int,ActionContainer> _actions = new Dictionary<int, ActionContainer>();	
	
	
	public static NativeDialogs Instance {
		get {
			return _instance;
		}
	}
	
	bool isMobileRuntime {
		get {
			return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
		}
	}
	
	void Awake() {		
		_instance = this;
		
		if (isMobileRuntime == false) {
			Debug.LogWarning("Due to platform specific NativeDialogs was designed to run only on iOS/Android device. Plugin function call has no effect on other platforms.");
			return;
		}
		
#if UNITY_ANDROID		
		_pluginObject = new AndroidJavaObject("ua.org.jeff.unity.nativedialogs.AndroidPlugin");
#endif
	}
	
	
	void MessageBoxButtonClicked(string message) {
		string[] elements = message.Split('\n');
		
		if(elements.Length == 0) {
			return;
		}
		
		if (elements[0] == "MessageBox") {
			int id = int.Parse(elements[1]);
			string button = elements[2];
			
			_actions[id].action1.Invoke(button);
			_actions.Remove(id);
			
		} else if (elements[0] == "LoginPasswordMessageBox") {
			int id = int.Parse(elements[1]);
			string login = elements[2];
			string password = elements[3];
			string button = elements[4];
			
			_actions[id].action3.Invoke(login, password, button);
			_actions.Remove(id);
			
		} else if (elements[0] == "PromptMessageBox") {
			int id = int.Parse(elements[1]);
			string prompt = elements[2];
			string button = elements[3];
			
			_actions[id].action2.Invoke(prompt, button);
			_actions.Remove(id);
			
		} else if (elements[0] == "SecurePromptMessageBox") {
			int id = int.Parse(elements[1]);
			string prompt = elements[2];
			string button = elements[3];
			
			_actions[id].action2.Invoke(prompt, button);
			_actions.Remove(id);
		}
	}

	int makeJNICall(string method, string caption, string message, string[] buttons)
	{
		if (isMobileRuntime == false) {
			return 0;
		}
		
#if UNITY_ANDROID
		AndroidJavaObject obj_ArrayList = new AndroidJavaObject("java.util.ArrayList");		
		
		Debug.Log("unity message: " + message);
		
		jvalue val = new jvalue();
		val.l = AndroidJNI.NewStringUTF(message);
		
		IntPtr method_Add = AndroidJNIHelper.GetMethodID(obj_ArrayList.GetRawClass(), "add", "(Ljava/lang/Object;)Z");
		foreach (string button in buttons) {
			AndroidJNI.CallBooleanMethod(obj_ArrayList.GetRawObject(), method_Add, AndroidJNIHelper.CreateJNIArgArray(new string[] {button}));			
		}						
		
		return _pluginObject.Call<int>(method, caption, message, obj_ArrayList, "NativeDialogs");
#else
		return 0;
#endif
	}
	
	
	
	/**
	 * Show alert dialog.
	 * caption - alert title
	 * message - alert message
	 * buttons - list of buttons
	 * 
	 * string parameter of onClickAction will be set to text of button clicked
	 */
	public void ShowMessageBox(string caption, string message, string[] buttons, Action<string> onClickAction) {
		if (isMobileRuntime == false) {
			return;
		}
		
		if (buttons.Length == 0 || buttons.Length > 3) {
			Debug.Log("Buttons count should be from 1 to 3");
			return;
		}
		
		int id = 0;
#if UNITY_ANDROID
		id = makeJNICall("messageBox", caption, message, buttons);
#elif UNITY_IPHONE
		id = messageBox(caption, message, buttons, buttons.Length, "NativeDialogs");
#endif
		ActionContainer container = new ActionContainer();
		container.action1 = onClickAction;
		_actions.Add(id, container);
		
	}
	
	/**
	 * Show login/password dialog.
	 * caption - dialog title
	 * message - dialog message
	 * buttons - list of buttons
	 * 
	 * first string parameter of onClickAction will be set to login entered
	 * second string parameter of onClickAction will be set to password entered
	 * third string parameter of onClickAction will be set to text of button clicked
	 */
	public void ShowLoginPasswordMessageBox(string caption, string message, string[] buttons, Action<string, string, string> onClickAction) {		
		if (isMobileRuntime == false) {
			return;
		}
		
		if (buttons.Length == 0 || buttons.Length > 3) {
			Debug.Log("Buttons count should be from 1 to 3");
			return;
		}
		
		int id = 0;
#if UNITY_ANDROID		
		id = makeJNICall("loginPasswordMessageBox", caption, message, buttons);		
#elif UNITY_IPHONE
		id = loginPasswordMessageBox(caption, message, buttons, buttons.Length, "NativeDialogs");
#endif
		ActionContainer container = new ActionContainer();
		container.action3 = onClickAction;
		_actions.Add(id, container);
	}
	
	
	/**
	 * Show prompt dialog.
	 * caption - dialog title
	 * message - dialog message
	 * buttons - list of buttons
	 * 
	 * first string parameter of onClickAction will be set to data entered
	 * second string parameter of onClickAction will be set to text of button clicked
	 */
	public void ShowPromptMessageBox(string caption, string message, string[] buttons, Action<string, string> onClickAction) {
		if (isMobileRuntime == false) {
			return;
		}
		
		if (buttons.Length == 0 || buttons.Length > 3) {
			Debug.Log("Buttons count should be from 1 to 3");
			return;
		}
		
		int id = 0;
#if UNITY_ANDROID
		id = makeJNICall("promptMessageBox", caption, message, buttons);
#elif UNITY_IPHONE
		id = promptMessageBox(caption, message, buttons, buttons.Length, "NativeDialogs");
#endif
		ActionContainer container = new ActionContainer();
		container.action2 = onClickAction;
		_actions.Add(id, container);
	}
	
	/**
	 * Show secure prompt dialog. User input is masked by asterisk character
	 * caption - dialog title
	 * message - dialog message
	 * buttons - list of buttons
	 * 
	 * first string parameter of onClickAction will be set to data entered
	 * second string parameter of onClickAction will be set to text of button clicked
	 */
	public void ShowSecurePromptMessageBox(string caption, string message, string[] buttons, Action<string, string> onClickAction) {
		if (isMobileRuntime == false) {
			return;
		}
		
		if (buttons.Length == 0 || buttons.Length > 3) {
			Debug.Log("Buttons count should be from 1 to 3");
			return;
		}
		
		int id = 0;
#if UNITY_ANDROID
		id = makeJNICall("securePromptMessageBox", caption, message, buttons);
#elif UNITY_IPHONE
		id = securePromptMessageBox(caption, message, buttons, buttons.Length, "NativeDialogs");
#endif
		ActionContainer container = new ActionContainer();
		container.action2 = onClickAction;
		_actions.Add(id, container);
	}
	
	
	/**
	 * Show window with spinning progress indicator. It will dismiss previously shown progress window if needed.
	 * caption - window title
	 * message - window message
	 */
	public void ShowProgressDialog(string caption, string message) {
		if (isMobileRuntime == false) {
			return;
		}
		
#if UNITY_ANDROID
		_pluginObject.Call("progressDialog", caption, message);
#elif UNITY_IPHONE
		progressDialog(caption, message);
#endif
	}
	
	/**
	 * Hide window with spinning progress indicator
	 */
	public void HideProgressDialog() {
		if (isMobileRuntime == false) {
			return;
		}
		
#if UNITY_ANDROID
		_pluginObject.Call("hideProgressDialog");
#elif UNITY_IPHONE
		hideProgressDialog();
#endif
	}

}
