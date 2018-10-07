using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {
	private static MenuController instance;	
		
	public static MenuController Instance {
		get 
		{
			if (instance == null)
			{
				instance = (MenuController)FindObjectOfType(typeof(MenuController));
			}
			if (!instance)
			{
				Debug.LogError("MenuController could not find himself!");
			}
			return instance;
		}
	}

	public UIButton[] BtnLocked;
    public UIButton[] BtnLockedBoss;
    public UIButton[] BtnProps;
    public UIButton[] BtnBosses;

	public SpriteText SpriteText;
	public SpriteText NeedCoins;
    [HideInInspector]
    public int tobeUnlockedProps = -1;
    [HideInInspector]
	public int tobeUnlockedBoss = -1;
       [HideInInspector]
	public int unlockedPropsPrice = -1;
       [HideInInspector]
	public int unlockedBossPrice = -1;
       [HideInInspector]
	public string unlockedBoss;
       [HideInInspector]
	public string propsNum;

	public UIPanelManager PanelManagerScript;

	public UIPanelManager panelManager;

    public tk2dSprite propsPic;
	
    /// <summary>
    /// 集中处理存储数据
    /// </summary>
	void Awake(){
        AndroidJNI.AttachCurrentThread();
       // GlobalManager.ShowAd();

        PlayerPrefs.GetString("First", "0");
		PlayerPrefs.GetString("first","0");

        PlayerPrefs.SetString("propLockNum", "1111111111");
        PlayerPrefs.SetString("BossLocked", "11111");

		GlobalManager.CoinNum = PlayerPrefs.GetInt( "coinNum", 0);
		GlobalManager.SaveAllToPlayerPrefs();
		PlayerPrefs.Save();
	}

	void Start () {
		CurrentCoins();
		ShowUnlockedPrefabs();
	}
    /// <summary>
    /// 跳转界面
    /// </summary>
	public void PanelChange(){
		if(PlayerPrefs.GetInt("coinNum") == 0 && PlayerPrefs.GetInt("first")== 0 ){
			panelManager.BringIn("PanelGift");	
		}else{
			panelManager.BringIn("PanelStart");
		}
	}
    /// <summary>
    /// 第一次游戏奖励
    /// </summary>
	public void ReceiveGift(){
		GlobalManager.CoinNum += 200; 
		PlayerPrefs.SetInt("coinNum", GlobalManager.CoinNum);
		CurrentCoins();
		PlayerPrefs.SetInt("first",1);
		Debug.Log(PlayerPrefs.GetInt("first"));
		GlobalManager.SaveAllToPlayerPrefs();
	}
	
	// Update is called once per frame
	void Update () {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                NativeDialogs.Instance.ShowMessageBox("提示", "真的要退出吗?", new string[] { "退出", "取消" }, (string button) => { QuitHint(button); });
            }

        }
	}

    void QuitHint(string code)
    {
        if (code.Equals("退出"))
        {
            Application.Quit();
        }
    }

	/// <summary>
	/// 存储解锁的道具
	/// </summary>
	/// <param name="propsID"></param>
	public void SaveUnlockedProps(int propsID){
		propsNum = PlayerPrefs.GetString("propLockNum", "1000000000");
		propsNum = propsNum.Remove(propsID, 1);
		propsNum = propsNum.Insert(propsID, "1");
		PlayerPrefs.SetString("propLockNum",propsNum);
		PlayerPrefs.Save();
	}
	/// <summary>
	/// 存储解锁的boss
	/// </summary>
	/// <param name="bossID"></param>
	public void SaveUnlockedBoss(int bossID){
		unlockedBoss = PlayerPrefs.GetString("BossLocked" , "10000");
		unlockedBoss = unlockedBoss.Remove(bossID, 1);
		unlockedBoss = unlockedBoss.Insert(bossID, "1");
		PlayerPrefs.SetString("BossLocked", unlockedBoss);
		PlayerPrefs.Save();
	}
	//Show the unlocked props;
	public void ShowUnlockedPrefabs(){
		ShowUnlockedProps();
		unlockedBoss = PlayerPrefs.GetString("BossLocked" , "10000");
		for(int n = 1 ; n < unlockedBoss.Length; n++){
			if(unlockedBoss[n] == '1'){
				BtnLockedBoss[n].Hide(true);

			}
		}
	}
	public void ShowUnlockedProps(){
		propsNum = PlayerPrefs.GetString("propLockNum", "1000000000");
		Debug.Log(propsNum);
		for(int n = 0; n < propsNum.Length; n++){
			if( propsNum[n] == '1'){
				BtnLocked[n].Hide(true);
			}else{
				BtnLocked[n].Hide(false);
			}
		}
		PlayerPrefs.Save();
	}
	
	public void ShowUnlockedBoss(){
		unlockedBoss = PlayerPrefs.GetString("BossLocked" , "10000");
		for(int n = 1; n < unlockedBoss.Length; n++){	
			if( unlockedBoss[n] == '1' ){
				BtnLockedBoss[n].Hide(true);
		//		BtnLockedBoss[n].hideAtStart = true;
			}else{
				BtnLockedBoss[n].Hide(false);
			}
		}
		PlayerPrefs.Save();
	}
	public void SetProps(int propsID, int propsPrice){
		unlockedPropsPrice = propsPrice;
		tobeUnlockedProps = propsID;
	}
	public void SetBoss(int bossID, int bossPrice){
		tobeUnlockedBoss = bossID;
		unlockedBossPrice = bossPrice;
	}
	
	public void DoUnlockedProps(){
		propsNum = PlayerPrefs.GetString("propLockNum", "1000000000");
		Debug.Log(propsNum);
		if(tobeUnlockedProps != -1){
			if(GlobalManager.CoinNum >= unlockedPropsPrice){
				CountPropsCoins(unlockedPropsPrice);
			    SaveUnlockedProps(tobeUnlockedProps);
			    ShowUnlockedProps();
			}else{
				PanelManagerScript.BringIn("PanelBuyCoin");
			}
		}
		unlockedBoss = PlayerPrefs.GetString("BossLocked", "10000");
		if(tobeUnlockedBoss != -1){
			if(GlobalManager.CoinNum >= unlockedBossPrice){
				CountBossCoins(unlockedBossPrice);
				SaveUnlockedBoss(tobeUnlockedBoss);
				ShowUnlockedBoss();
	//			Debug.Log(unlockedBoss);
			}else{
				PanelManagerScript.BringIn("PanelBuyCoin");
			}
		}
	}
	
/*	public UIButton[] BuyCoin;
	public void BtnBuyCoin(){
		for(int i = 0; i < 4; i++){
			for(int j = 0; j = i < 4; j++){
				BuyCoin[i] = PlayerPrefs.GetInt("coinNum") + (i+j+1)*100;
				PlayerPrefs.SetInt("coinNum" , BuyCoin[i]);
				PlayerPrefs.Save();
				CurrentCoins();
			}
		}	
	}
	*/
	public void DontUnlockedProps(){
		
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="propsPrice"></param>
	public void ChangeTextProps(int propsPrice){
		if(propsPrice > GlobalManager.CoinNum){
			SpriteText.Text = "You still need" ;
		}else{
		SpriteText.Text = "Are you sure unlock it with:";
		}
	}
	
	public void RequireCoinsProps(int propsPrice){
		if(propsPrice <= GlobalManager.CoinNum){
			NeedCoins.Text = "" + propsPrice;
		}else{
			NeedCoins.Text = "" + (propsPrice - GlobalManager.CoinNum);
		}
	}
	
	public SpriteText[] ShowCoins;
	public void CurrentCoins(){
		for(int i = 0; i < 2; i++){
			ShowCoins[i].Text ="" + GlobalManager.CoinNum;
			GlobalManager.SaveAllToPlayerPrefs();
		}
		Debug.Log(PlayerPrefs.GetInt("coinNum"));
	}
	
	public void CountPropsCoins(int propsPrice){
	//	coins = PlayerPrefs.GetInt("coinNum") - propsPrice;
		GlobalManager.CoinNum -= propsPrice;
	    PlayerPrefs.SetInt("coinNum" ,GlobalManager.CoinNum);
        PlayerPrefs.Save();
	    CurrentCoins();	
	}
	public void CountBossCoins(int bossPrice){
//		coins = PlayerPrefs.GetInt("coinNum") - bossPrice;
		GlobalManager.CoinNum -= bossPrice;
	    PlayerPrefs.SetInt("coinNum" ,GlobalManager.CoinNum);
        PlayerPrefs.Save();
	    CurrentCoins();	
	}
	
	public void ChangeTextBoss(int bossPrice){
		if(bossPrice > GlobalManager.CoinNum){
			SpriteText.Text = "If unlock this,you still need";
		}else{
			SpriteText.Text = "Are you sure unlock it with:";		
		}
	}
	public void RequireCoinsBoss(int bossPrice){
		if(bossPrice <= GlobalManager.CoinNum){
			NeedCoins.Text = "" + bossPrice; 	
		}else{
			NeedCoins.Text = "" + (bossPrice - GlobalManager.CoinNum);
		}
	}
	
/*	public void ShowUnlockedBoss(){
		unlockedBoss = PlayerPrefs.GetString("BossLocked" , "10000");
		for(int n = 0; n < 4; n++){
			if( unlockedBoss[n] == '1'){
				LockedBoss[n].Hide(true);
			}else{
				LockedBoss[n].Hide(false);
		//		Debug.Log(n);
		Debug.Log(unlockedBoss);		
			}
		}
		PlayerPrefs.Save();
	}*/
	//record the props which your choose 
	public void SetCurrentProps(int propsID){
		PlayerPrefs.SetInt("usingProp", propsID);
		PlayerPrefs.Save();
	}
	
	public void GoToPanel (string panelName) {
		PanelManagerScript.BringIn(panelName);
	}
	
	public void LoadScence(int ID){
		Application.LoadLevel("LoadingScene");
        PlayerPrefs.SetInt("Boss", ID +1);
	}

	public void SetCurrentBoss(int BossID){
		PlayerPrefs.SetInt("CurrentBoss", BossID);
		PlayerPrefs.Save();
		ShowPropTip();
	}
	
	
	public void ShowPropTip(){
		propsPic.spriteId = PlayerPrefs.GetInt("usingProp");
	//	propsPic.GetSpriteIdByName("daoju" + propsPic.spriteId);
		Debug.Log("propsPic.spriteId" +propsPic.spriteId);
	}

}
