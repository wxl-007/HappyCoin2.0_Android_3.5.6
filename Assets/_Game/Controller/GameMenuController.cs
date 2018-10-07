using UnityEngine;
using System.Collections;

public class GameMenuController : MonoBehaviour {
	private static GameMenuController instance;	
	public SpriteText Counter;
	private int time = 59;
	
	
	public static GameMenuController Instance {
		get 
		{
			if (instance == null)
			{
				instance = (GameMenuController)FindObjectOfType(typeof(GameMenuController));
			}
			if (!instance)
			{
				Debug.LogError("GameMenuController could not find himself!");
			}
			return instance;
		}
	}
	public SpriteText ShowCoins;
	// Use this for initialization
	void Start () {
        


		ShowCoinsInGM();
		InvokeRepeating( "Timepiece", 0f, 1f);
	}
	//show the coins in game menu
	public void ShowCoinsInGM(){
		ShowCoins.Text = "" + GlobalManager.CoinNum;
		
	}
	
	// Update is called once per frame
	void Update () {
		ShowCoinsInGM();
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
	public void LoadScence(){
		Application.LoadLevel("Menu");
	}
	public void Pause(){
		Time.timeScale = 0f;
	}
	public void Continue(){
		Time.timeScale = 1f;
	}
	public void Timepiece(){
		Counter.Text = time.ToString();
		if(time > 0){
			time--;
		}else{
			time = 59;
            if (time == 30 || time == 59) GameController.Instance.AddCoin(1); 
		}	
	}
}
