using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IAPController : MonoBehaviour {
	const string IAP_PROD_Coins100 = "com.tsi.HappyCoin.Coins100";
	const string IAP_PROD_Coins200 = "com.tsi.HappyCoin.Coins200";
	const string IAP_PROD_Coins450 = "com.tsi.HappyCoin.Coins450";
	const string IAP_PROD_Coins1000 = "com.tsi.HappyCoin.Coins1000";
	
	
	const string PANEL_CONNECT_NAME = "PanelBuyConnecting";
	const string PANEL_SUCCEED_NAME = "PanelBuySucceed";
	const string PANEL_FAILED_NAME = "PanelBuyFailed";
	public UIPanelManager panelManager;
	
//	public SpriteText addCoins;
//	private List<StoreKitProduct> _products;
	
	// Use this for initialization
	void Start () {
		string[] productIdentifiers = new string[] {IAP_PROD_Coins100, IAP_PROD_Coins200, IAP_PROD_Coins450, IAP_PROD_Coins1000};
//		StoreKitBinding.requestProductData(productIdentifiers);
			
//		StoreKitManager.productListReceived += allProducts =>{
//			Debug.Log("received total _products:" + allProducts.Count);
//			_products = allProducts;
//		};
		
	}
	
	public bool CheckProductData(string productIdentifier){
		#if UNITY_IPHONE
		foreach(StoreKitProduct prod in _products){
			if(prod.productIdentifier == productIdentifier)
				return true;
		}
		#endif
		return false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
#if UNITY_IPHONE
	public void DoBuyCoins100(){
		ShowConnectingiTunesPanel();
		if(CheckProductData(IAP_PROD_Coins100)){
			StoreKitBinding.purchaseProduct(IAP_PROD_Coins100,1);
		}else{
			PurchaseFailed();
		}
		
	}
	public void DoBuyCoins200(){
		ShowConnectingiTunesPanel();
		if(CheckProductData(IAP_PROD_Coins200)){
			StoreKitBinding.purchaseProduct(IAP_PROD_Coins200,1);
		}else{
			PurchaseFailed();
		}
	}
	public void DoBuyCoins450(){
		ShowConnectingiTunesPanel();
		if(CheckProductData(IAP_PROD_Coins450)){
			StoreKitBinding.purchaseProduct(IAP_PROD_Coins450,1);
		}else{
			PurchaseFailed();
		}
	}
	public void DoBuyCoins1000(){
		ShowConnectingiTunesPanel();
		if(CheckProductData(IAP_PROD_Coins1000)){
			StoreKitBinding.purchaseProduct(IAP_PROD_Coins1000,1);
		}else{
			PurchaseFailed();
		}
	}
#endif
	public void PurchaseSucceed(string productIdentifier){
		// quantity always = 1
		switch (productIdentifier){
			case IAP_PROD_Coins100: 
					// Increase 100 Coins for User
					GlobalManager.CoinNum += 100;
//					addCoins.Text = 100.ToString();
					break;
			case IAP_PROD_Coins200: 
					// Increase 200 Coins for User
					GlobalManager.CoinNum += 200;
//					addCoins.Text = 200.ToString();
					break;
			case IAP_PROD_Coins450: 
					// Increase 450 Coins for User
					GlobalManager.CoinNum += 450;
//					addCoins.Text = 450.ToString();

					break;
			case IAP_PROD_Coins1000: 
					// Increase 1000 Coins for User
					GlobalManager.CoinNum += 1000;
//					addCoins.Text = 1000.ToString();
					break;
		}
		GlobalManager.SaveAllToPlayerPrefs();
		Debug.Log("GlobalManager.CoinNum:" + GlobalManager.CoinNum);
		// Display the new coins number
		MenuController.Instance.CurrentCoins();
		
		// Back to the buy coins panel
		panelManager.BringIn(PANEL_SUCCEED_NAME);
		
	}
	public void PurchaseFailed(){
		// Back to the buy coins panel
		panelManager.BringIn(PANEL_FAILED_NAME);
	}
	
	public void DismissPanel(){
		panelManager.Dismiss();
	}
	
	private void ShowConnectingiTunesPanel(){
		// if (Application.platform == RuntimePlatform.IPhonePlayer){
			panelManager.BringIn(PANEL_CONNECT_NAME);
		//}
	}
}
