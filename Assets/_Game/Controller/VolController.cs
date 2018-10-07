using UnityEngine;
using System.Collections;

public class VolController : MonoBehaviour {
	public UIButton btnOn,btnOff;
	// Use this for initialization
	void Start () {
		Refresh();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void Refresh(){
		btnOn.Hide(!GlobalManager.isSoundOn);
		btnOff.Hide(GlobalManager.isSoundOn);
	}
	void Toggle(){
		GlobalManager.ToggleSound();
		AudioListener.volume = GlobalManager.isSoundOn? 1f:0f;
		Refresh();
	}
}
