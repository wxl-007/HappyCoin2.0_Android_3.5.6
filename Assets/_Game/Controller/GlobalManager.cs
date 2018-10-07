using UnityEngine;
using System.Collections;

public class GlobalManager {
    public static AndroidJavaClass _Ajc; 
	public static bool isSoundOn = true;
	public static int CoinNum = 0;
	public static int levelNum = 0;
	public static int maxCoin = 10;
	public static int receiveCoinNum =0;
    public static float experienceValue;
    public static int[]  probilityNum = {
         500, 333, 250, 167, 143, 125, 111, 100, 91, 90, 77, 100, 100, 63, 100, 77, 77, 77, 58, 250, 58,  250, 53, 52, 52,50,91,91,91,  125};
	

	public static void ToggleSound(){
		if(isSoundOn){
			isSoundOn = false;
		}else{
			isSoundOn = true;
		}
	}

    public static void ShowAd()
    {
       /* _Ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = _Ajc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("setupAds");
        * */
    }


	public static void SaveAllToPlayerPrefs(){
		PlayerPrefs.SetInt("isSoundOn", isSoundOn ? 1 : 0);
		PlayerPrefs.SetInt("coinNum", CoinNum);
		PlayerPrefs.SetInt("Level", levelNum);
        PlayerPrefs.SetInt("MaxCoin", maxCoin);
        PlayerPrefs.SetInt("ReceiveCoin",receiveCoinNum);
		PlayerPrefs.Save();
	}
	public static void LoadPlayerPrefabs(){
		GlobalManager.isSoundOn = (PlayerPrefs.GetInt("isSoundOn",1)==1);
		GlobalManager.CoinNum = PlayerPrefs.GetInt("coinNum" , 0);
        GlobalManager.levelNum = PlayerPrefs.GetInt("Level");
        GlobalManager.experienceValue = PlayerPrefs.GetFloat("Exp");
//       GlobalManager.expNum = PlayerPrefs.GetString("ExpText","0.0");
        GlobalManager.maxCoin = PlayerPrefs.GetInt("MaxCoin");
        receiveCoinNum = PlayerPrefs.GetInt("ReceiveCoin");
        CoinNum = PlayerPrefs.GetInt("coinNum", CoinNum);

        
	}
    public static int unlockCoin;
	public static void ExpBarControll(){
//		if(receiveCoinNum >= maxCoin){
			//levelNum++;
            unlockCoin = levelNum;
			maxCoin += 20;
			receiveCoinNum =0 ;
			SaveAllToPlayerPrefs();
//		}
	}
}
