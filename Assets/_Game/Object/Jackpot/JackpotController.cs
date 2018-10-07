using UnityEngine;
using System.Collections;

public class JackpotController : MonoBehaviour {
	private static JackpotController instance;	
		
	public static JackpotController Instance {
		get 
		{
			if (instance == null)
			{
				instance = (JackpotController)FindObjectOfType(typeof(JackpotController));
			}
			if (!instance)
			{
				Debug.LogError("JackpotController could not find himself!");
			}
			return instance;
		}
	}
    public SpriteText level;
    public GameObject Upgrade;
    public EventsController eventsController;
	public Vector3[] eventsPos;
	public Vector3 center;
    private int posNum;
	public bool JackpotOn = false;
	private float timeScale ;
	private int last_i;
	private int stackCoins = 0;
	private tk2dSprite selectSymbol;
    public GameObject expBarObj;
    private UIProgressBar ExpBar;
	// slecting by rotating in Jackpot;

    void Awake()
    {
        ExpBar = expBarObj.GetComponent<UIProgressBar>();
        ExpBar.Value = PlayerPrefs.GetFloat("Exp");
 //       LevelNum();
        level.Text = PlayerPrefs.GetInt("Level").ToString();
        GlobalManager.receiveCoinNum = PlayerPrefs.GetInt("ReceiveCoin");
        Debug.Log(GlobalManager.receiveCoinNum);
    }
	void Start(){
        PlayerPrefs.SetInt("First",1);
        
	}
	
	// wait for seconds 
	public IEnumerator JackpotBegin (){
		JackpotOn = true;
		while (JackpotOn) {
			float timeInterval = 0.05f;
			SoundController.Instance.JackpotSoundEffect();
			timeScale = Random.Range(2.5f , 6.4f);
			for (float time = 0 ;time < timeScale ; time += timeInterval){
				MoveNext();	
				yield return new WaitForSeconds(timeInterval);
		    }
            if (posNum == 3 || posNum == 7 || posNum == 10 || posNum == 13)
            {
				StartCoroutine(FaceEvent());
				SoundController.Instance.FaceSound();
			}
			if(posNum == 1|| posNum == 4|| posNum == 14|| posNum == 8){
				SoundController.Instance.PassSound();       
			}
			if(posNum == 2|| posNum == 6|| posNum ==11|| posNum == 15){
				StartCoroutine(HandEvent());
				SoundController.Instance.FightEffect();
			}
			if(posNum == 0|| posNum == 5|| posNum == 9|| posNum == 12){
				StartCoroutine(WingEvent());
				SoundController.Instance.TinyWingSound();
			}
			StartCoroutine(EventsEffect(posNum));
			yield return new WaitForSeconds(1f);
			if(stackCoins != 0){
				stackCoins--;
			} else JackpotOn = false;
			SoundController.Instance.JackpotSoundEffect();
		}
	}
	//transforming next position of event;
	public void MoveNext(){
		int eventsNum = eventsPos.Length;
			posNum++;
		if(posNum == eventsNum ) posNum = 0;	
		gameObject.transform.position = eventsPos[posNum];
	}
	// draw a circle for locating each events position
    void OnDrawGizmos (){
		Gizmos.color = Color.black;
	    foreach(Vector3 eventPos in eventsPos){	
			Gizmos.DrawWireSphere( eventPos , 1.0f);
		}
	}
	public IEnumerator HandEvent(){
		StartCoroutine(eventsController.AttackEvent());
		yield return new WaitForSeconds(2.5f);
		eventsController.DropCoinsEvent();
	}
	public IEnumerator WingEvent(){
        //eventsController.DropBigCoin();
		
		switch( Random.Range(1, 5))
		{
			
		case 1:
			eventsController.DropBigCoin();
			break;
		case 2:  
			eventsController.BorderEvent();
			break;
		case 3:
			StartCoroutine(eventsController.ShakeEvent());
			break;
		case 4:
			EventsController.Instance.TongueEvent();
			break;
		}
		
	yield return new WaitForSeconds(1f);
	}
	
	public IEnumerator FaceEvent(){
		int m = Random.Range(1, 4);
		if(m == 1){
			eventsController.DefenceEvent();
		}
		if(m == 2){
			eventsController.ClawEvent();
		}
	    if(m == 3 ){
			eventsController.BombEvent();
		}	
		yield return new WaitForSeconds(1f);
	}
	public void JackpotTrigger(){
		if(JackpotOn == true ){
			stackCoins++;
		}else{
			StartCoroutine(JackpotBegin());
		}
	}
	
	// Update is called once per frame
	void Update () {
        SetExpBarOriginal();
	}
	public IEnumerator EventsEffect(int i){
		selectSymbol = gameObject.GetComponent<tk2dSprite>();
		for(int n = 0; n < 2;n++ ){
			if(i == 3 || i == 7 || i == 10 || i == 13){
				selectSymbol.spriteId = selectSymbol.GetSpriteIdByName("3");
			}
			 if(i == 1|| i == 4|| i == 14|| i == 8){
				selectSymbol.spriteId = selectSymbol.GetSpriteIdByName("4");	        
			}
			if(i == 2|| i == 6|| i ==11|| i == 15){
				selectSymbol.spriteId = selectSymbol.GetSpriteIdByName("2");		
			}
			if(i == 0|| i == 5|| i == 9|| i == 12){
				selectSymbol.spriteId = selectSymbol.GetSpriteIdByName("1");	
			}
			yield return new WaitForSeconds(0.2f);
			selectSymbol.spriteId = selectSymbol.GetSpriteIdByName("quanquan");
			yield return new WaitForSeconds(0.2f);
		}
	} 
	
	public void UpdateExpBar() {
        ExpBar = expBarObj.GetComponent<UIProgressBar>();
        ExpBar.Value = (float)GlobalManager.receiveCoinNum / GlobalManager.maxCoin;  
        PlayerPrefs.SetFloat("Exp", ExpBar.Value);
        Debug.Log("Exp" +PlayerPrefs.GetFloat("Exp"));
        PlayerPrefs.Save();
    }
    public void SetExpBarOriginal() {
        if (ExpBar.Value == 1 && GlobalManager.receiveCoinNum >= GlobalManager.maxCoin)
        {
            GlobalManager.ExpBarControll();
            GlobalManager.levelNum++;
            ExpBar.Value = 0;
            GlobalManager.receiveCoinNum = 0;
            UpgradeEffect();
            LevelNum();
            EventsController.Instance.DropCoinsEvent();
            CoinsController.Instance.DropCoin();
            GlobalManager.SaveAllToPlayerPrefs();
        }
	}
	public void Add15Exp(){
        GlobalManager.receiveCoinNum += 15;
        UpdateExpBar();
	}
    public void Add50Exp() {
        //ExpBar.Value += 0.5f;
        GlobalManager.receiveCoinNum += 50;
        UpdateExpBar();
    }
	
	public void LevelNum(){
		level.Text = GlobalManager.levelNum.ToString();
        Debug.Log("====================== LevelNum"+GlobalManager.levelNum+"=====================");
	}
    public void TreasureBox() {
        GlobalManager.receiveCoinNum += 80;
        GameController.Instance.AddCoin(20);
    }
    
    public void UpgradeEffect() {
        iTween.ScaleTo( Upgrade, iTween.Hash("x",1f,"y",1f,"z",1f,"time",1f,"easetype","linear"));//new Vector3( 1f, 1f, 1f), 2f);
        iTween.ScaleTo(Upgrade, iTween.Hash("x",0f,"y",0f,"delay",3f,"easetype","linear","time",1f));
    }
}
