using UnityEngine;
using System.Collections;

public class LoadingController : MonoBehaviour {
	void Start () {
		Invoke("LoadScene", 1.5f);
 //       AndroidJNI.AttachCurrentThread();
 //       GlobalManager.ShowAd();
	}
	
	void LoadScene () {
		Application.LoadLevel(PlayerPrefs.GetInt("Boss"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}


