using UnityEngine;
using System.Collections;

public class UIProps : MonoBehaviour {
	public int propsID;
	private int coins;
	public bool isLocked;
	public int propsPrice;
	
	// Use this for initialization
	void Start () {
//		coins = PlayerPrefs.GetInt("coinNum");
	    UIBtnChangePanel btn = gameObject.GetComponent<UIBtnChangePanel>();
		if (isLocked){
			btn.AddValueChangedDelegate(UIUnlocked);
		}else{
			btn.AddValueChangedDelegate(SetCurrentProps);
		}	
	}
	// Update is called once per frame
	void Update () {
	
	}
	void UIUnlocked(IUIObject obj){
		MenuController.Instance.SetProps(propsID,propsPrice);
		MenuController.Instance.ChangeTextProps(propsPrice);
		MenuController.Instance.RequireCoinsProps(propsPrice);
		MenuController.Instance.CurrentCoins();
	}
	void SetCurrentProps(IUIObject obj){
		MenuController.Instance.SetCurrentProps(propsID);
        
	}
}
