using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	private static GameController instance;	
		
	public static GameController Instance {
		get 
		{
			if (instance == null)
			{
				instance = (GameController)FindObjectOfType(typeof(GameController));
			}
			if (!instance)
			{
				Debug.LogError("GameController could not find himself!");
			}
			return instance;
		}
	}
	
//	private int coinNum; 

	
	// Use this for initialization
	void Start () {

      //      AndroidJNI.AttachCurrentThread();
        //GlobalManager.ShowAd();
		GlobalManager.CoinNum = PlayerPrefs.GetInt("coinNum");
        SoundController.Instance.PlayBGM();
	}
	
	public bool AddCoin (int delta) {
		if (GlobalManager.CoinNum + delta < 0) return false;
		GlobalManager.CoinNum += delta;
//		PlayerPrefs.SetInt("coinNum", GlobalManager.CoinNum);
//		PlayerPrefs.Save();
		GlobalManager.SaveAllToPlayerPrefs();
		return true;
	}
  
    
	// Update is called once per frame
	void Update () {
		
	}
	
}
