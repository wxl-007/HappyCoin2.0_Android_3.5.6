using UnityEngine;
using System.Collections;

public class UIBoss : MonoBehaviour {

	public int bossID;
	public bool isLock;
	public int bossPrice;
	// Use this for initialization
	void Start () {
		UIBtnChangePanel btn = gameObject.GetComponent<UIBtnChangePanel>();
		if (isLock){
			btn.AddValueChangedDelegate(UIUnlocked);
		}else{
			btn.AddValueChangedDelegate(SetCurrentBoss);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void UIUnlocked(IUIObject obj){
		MenuController.Instance.SetBoss(bossID, bossPrice);
		MenuController.Instance.ChangeTextBoss(bossPrice);
		MenuController.Instance.RequireCoinsBoss(bossPrice);
		MenuController.Instance.CurrentCoins();	
	}
	void SetCurrentBoss(IUIObject obj){
		MenuController.Instance.SetCurrentBoss(bossID);
        MenuController.Instance.LoadScence(bossID);
	}
}
