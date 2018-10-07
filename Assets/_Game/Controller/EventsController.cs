using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventsController : MonoBehaviour {
	private static EventsController instance;	
		
	public static EventsController Instance {
		get 
		{
			if (instance == null)
			{
				instance = (EventsController)FindObjectOfType(typeof(EventsController));
			}
			if (!instance)
			{
				Debug.LogError("EventsController could not find himself!");
			}
			return instance;
		}
	}
	public GameObject zhuaziAni;
	public GameObject propAni;
	public GameObject CoinPrefab;
	public tk2dAnimatedSprite leftAni;
	public tk2dAnimatedSprite rightAni;
	private bool attackOccur;
	public Vector3 Edge;
//	public Vector3 rightEdge;
	public GameObject Border;
	private bool borderOccur;
	public GameObject barrier;
	private bool defenceOccur;
	public Vector3 leftBarrierPos;
	public Vector3 rightBarrierPos;
    public tk2dAnimatedSprite[] SpecialThs;
    public GameObject[] Special;
  //  private Vector3 initPos;
	
	// Use this for initialization
    void Start()
    {
        SpecialThs = new tk2dAnimatedSprite[2];
        GlobalManager.LoadPlayerPrefabs();
  //      initPos = taiziCollider.transform.position;
        
    }
    /*
          void OnGUI() {
              if (GUI.Button(new Rect(0,0,200,100),"AttackEvent"))
              {
                  //StartCoroutine(ShakeEvent());
                  StartCoroutine(AttackEvent());
                 // ClawEvent();
              }
          }
      
            void OnGUI (){
                GUILayout.BeginArea(new Rect(200,0,100,300));
                if(GUILayout.Button("Attack")){
                    StartCoroutine(AttackEvent());
                    DropCoinsEvent();
			
                }
                if(GUILayout.Button("Bomb")){
                    BombEvent();
                }
		
                if(GUILayout.Button("Jackpot")){
                    StartCoroutine(JackpotController.Instance.JackpotBegin());
                }
                if(GUILayout.Button("Border")){
                    BorderEvent();
                }
                if(GUILayout.Button("Tongue")){
                    TongueEvent();
                }
                if (GUILayout.Button("Shock")) {
                    TigerShock();
                }
                if (GUILayout.Button("Metor")) CoinsController.Instance.MeteorEvent();
                if (GUILayout.Button("Spider")) SpiderEvent();
                if (GUILayout.Button("Barrier")) DefenceEvent();
                if (GUILayout.Button("TestBtn")) TestCoinAnimation();
                if (GUILayout.Button("Lv Up")) JackpotController.Instance.Add50Exp();
                if(GUILayout.Button("shake")) ShakeEvent();
                GUILayout.EndArea();
                GUILayout.BeginArea(new Rect( 0f, 0f,100f,200));
                if (GUILayout.Button("DisableArea")) CoinsController.Instance.TestDisableEvent();
                if (GUILayout.Button("EnableArea")) CoinsController.Instance.TestEnableEvent();
                if (GUILayout.Button("NoArea things")) CoinsController.Instance.TestSpecialArea();
                if (GUILayout.Button("YesArea things")) CoinsController.Instance.TestSpecialNoArea();
                GUILayout.EndArea();  
            }
    
        */	
	public IEnumerator AttackEvent(){
		if(attackOccur == false){
			attackOccur = true;
			LoadProps();
			propAni.rigidbody.isKinematic = false;
            BossAniController();
            DropCoinsEvent();
        //    MonkeyAnimationController();
            zhuaziAni.animation.Play();
			propAni.animation.Play();
			propAni.rigidbody.velocity = Vector3.zero;
			propAni.rigidbody.AddForce(new Vector3 (800f, 1000f, 0));
			propAni.rigidbody.angularVelocity = new Vector3(0f, 0f, 60f);
           // this.DropCoinsEvent();
			yield return new WaitForSeconds(1f);
			SoundController.Instance.ThrowingSound();
			yield return new WaitForSeconds(1f);
			attackOccur = false;
            
		}
	}
	public Vector3[] CoinPos;
	void OnDrawGizmos(){
		Gizmos.color =Color.red;
		foreach (Vector3 pos in CoinPos){
			Gizmos.DrawWireSphere( pos, 1.0f);
		}
		
	}
	public void DropCoinsEvent(){
		int RangeCoins = Random.Range( 2,5);
        Debug.Log("===========Drop Coin" + RangeCoins);
		for(int i = 0; i <= RangeCoins; i++){
            int Coins = Random.Range(0, 4);
			//Vector3 pos = new Vector3(RangeX, RangeY, RangeZ);
			GameObject Coin = Instantiate(CoinPrefab, CoinPos[Coins], Quaternion.identity)as GameObject ;
			Coin.rigidbody.AddForce( 0f, -50f, -50f);
           
		}
		SoundController.Instance.PlayDropCoin();
	}
	public GameObject bigCoin;
	public void DropBigCoin(){
		leftAni.Play("Y");
		rightAni.Play("Y");
       //BigCoinShock();
        Invoke("BigCoinShock", 0f);
        //TigerShock();
		int RangeX = Random.Range(-12, 10);
	    int RangeY = Random.Range(35, 38);
	    int RangeZ = Random.Range(-19, -14);
		Vector3 randomPos = new Vector3(RangeX,RangeY,RangeZ);
		GameObject Coin_10 = Instantiate(bigCoin, randomPos, Quaternion.identity)as GameObject;
		Coin_10.rigidbody.AddTorque(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
		Coin_10.rigidbody.AddForce( 0f, -5f, -5f);

	}
  	
	public void BorderEvent(){
		leftAni.Play("Y");
		rightAni.Play("Y");
		if(GameObject.FindObjectOfType(typeof(BorderController)) == null ){
			SoundController.Instance.BorderSoundPlay();
//			Vector3 leftPos = leftEdge;
			Vector3 Pos = Edge;
			Instantiate(Border, Pos,Border.transform.rotation);
//			Instantiate(Border, rightPos,Border.transform.rotation);
		}
	}
		
	public void DefenceEvent(){
		leftAni.Play("B");	
		rightAni.Play("B");
		Instantiate(barrier, leftBarrierPos, Quaternion.identity);
		Instantiate(barrier, rightBarrierPos, Quaternion.identity);
	}
	public bool shakeOccur;
	public IEnumerator ShakeEvent(){
		shakeOccur = true;
		leftAni.Play("Z");
		rightAni.Play("Z");
		BuildShakeAni();
		yield return new WaitForSeconds(4f);
		shakeOccur = false;
        shakeAni.SetActiveRecursively(false);
        
	}

	public GameObject shakeAni;
	public void BuildShakeAni(){
        if (shakeOccur)
        {
            //Instantiate(shakeAni);
            shakeAni.SetActiveRecursively(true);
            shakeAni.GetComponent<tk2dAnimatedSprite>().playAutomatically = true;

        }

	}
	// Update is called once per frame
	void Update () {
		if(attackOccur == false){
				if(propAni.transform.position.x <= 180f	|| propAni.transform.position.y <= -24 || propAni.transform.position.y >= 25){
					propAni.rigidbody.isKinematic = true;
				}	
			}
        
    /*    
        if (BigShock == true)
        {
          
            taiziCollider.transform.position += new Vector3(0f, 50 * Time.deltaTime, -30 * Time.deltaTime);
            Debug.Log("=================shock============");
            BigShock = false;
        
        }
        else
        {
            taiziCollider.transform.position = initPos;
        }
      */
	}
//	public tk2dSprite propSprit;
	public void LoadProps(){
		propAni.GetComponent<tk2dSprite>().spriteId =PlayerPrefs.GetInt("usingProp");
	//	propSprit.spriteId = PlayerPrefs.GetInt("usingProp");
	//	propSprit.b
	}
/*	private int unlockProp;
	public void LoadProps(){
		string SpriteID;
		if(unlockPropsArr.Count >= 2){
			n = Random.Range(0, unlockPropsArr.Count);
			SpriteID = "daoju" + (unlockPropsArr[n]+1);
		}else{
			SpriteID = "daoju1";
		}
		propSprit.spriteId = propSprit.GetSpriteIdByName(SpriteID);
		propSprit.Build();
		Debug.Log(SpriteID);
	}
//	private int[] unlockPropsArr = new int[11];
	private List<int> unlockPropsArr = new List<int>();
	private int n;
	public string unlockPropsStr;
	public void SaveUnlockProps(){
//		xxx.Add(1);
//		xxx.Add(3);
//		xxx[Random.Range(0, xxx.Count)];
		unlockPropsStr = PlayerPrefs.GetString("propLockNum");
		for(int i = 0; i < unlockPropsStr.Length ; i++){
			if(unlockPropsStr[i] == '1'){
				unlockPropsArr.Add(i);
			}
		}
	}
	*/
	public GameObject bombController;
	private bool bombEventOccur;
	public void BombEvent(){
		leftAni.Play("C");
		rightAni.Play("C");
        for (int i = 1; i < 3; i++)
        {
            int RangeX = Random.Range(-10, 12);
            int RangeY = Random.Range(35, 45);
            int RangeZ = Random.Range(-36, -15);
            Vector3 dropPos = new Vector3(RangeX, RangeY, RangeZ);
            Instantiate(bombController, dropPos, Quaternion.identity);
        }
	}
//leftClaw and rightClaw push the coin to invalid area;
	public GameObject leftClaw;
	public GameObject rightClaw;
	public void ClawEvent(){
		if(GameObject.FindObjectOfType(typeof(ClawController)) == null && GameObject.FindObjectOfType(typeof(BorderController)) == null) {
			leftAni.Play("A");
			rightAni.Play("A");
			SoundController.Instance.LaughSoundPlay();
			Instantiate(leftClaw,new Vector3(12, 32, -10),Quaternion.identity);
			Instantiate(rightClaw ,new Vector3( -12f, 32f,-7.2f),Quaternion.identity);
		}else if(GameObject.FindObjectOfType(typeof(BorderController))!= null){
			int i = Random.Range( 0, 2);
			if( i == 1) BombEvent();
			if( i == 2) DefenceEvent();
		}
	}
	// stretch out the tongue longer than before to push coin away;
	public bool stretchOut = false;
	public void TongueEvent(){
		leftAni.Play("A");
		rightAni.Play("A");
		stretchOut = true;
		
	}
    public tk2dAnimatedSprite[] bomb;
    public GameObject[] BombSprite;
    public IEnumerator RabbitBomb() {
        for(int i = 0 ; i < 2;i++){
            iTween.ScaleTo(BombSprite[i],iTween.Hash("x",1f,"y",1f,"time",1f));
            iTween.MoveTo(BombSprite[i], iTween.Hash("y",30,"time",3f));
        }
        
        yield return new WaitForSeconds(2f);
        
        Destroy(BombSprite[0]);
        Destroy(BombSprite[1]);
        for (int i = 0; i < 3; i++)
        {
            BombEvent();
        }
    
    }
/*	public void BossAni(){
		switch(PlayerPrefs.GetInt("CurrentBoss")){
		case 1:
			//HenAnimationController();
			break;
		case 2:
			//CatAnimationController();
			break;
		case 3:
			//RabbitAnimationController();
			break;
		case 4:
			//TigerAnimationController();
			break;
		default:
			MonkeyAnimationController();
			break;
		}
	}
 * */
    public tk2dSpriteAnimation MonkeyAniDefault;
//    public tk2dSpriteAnimation MonkeyAni2;
	public tk2dSpriteAnimation MonkeyAni3;
    public tk2dSpriteAnimation MonkeyAni4;
    public tk2dSpriteAnimation MonkeyAni5;
    public GameObject bubbleL;
    public GameObject bubbleR;
    public IEnumerator BubbleEffect() {
        yield return new WaitForSeconds(1.2f);
        bubbleL.particleEmitter.emit = true;
        bubbleR.particleEmitter.emit = true;
        yield return new WaitForSeconds(4f);
        bubbleR.particleEmitter.emit = false;
        bubbleL.particleEmitter.emit = false;
    }
    public GameObject boneL;
    public GameObject boneR;
    public IEnumerator BoneEffect(){
        yield return new WaitForSeconds(5f);
        iTween.ScaleTo(boneR, iTween.Hash("x", 1f, "y", 1f, "time", 0.5f));
        iTween.ScaleTo(boneL, iTween.Hash("x", 1f, "y", 1f, "time", 0.5f));  
        iTween.MoveTo(boneL, iTween.Hash("y", -30, "time", 5.0f, "easetype", "linear"));
        iTween.MoveTo(boneR, iTween.Hash("y", -30, "time", 5.0f, "easetype", "linear"));
        iTween.RotateFrom(boneL, iTween.Hash("z", 360, "time", 2.0f, "easetype", "linear"));
        iTween.RotateFrom(boneR, iTween.Hash("z", 360, "time", 2.0f, "easetype", "linear"));
        yield return new WaitForSeconds(4f);
        boneL.transform.localPosition = Vector3.zero;
        boneR.transform.localPosition = Vector3.zero;
        boneL.transform.localScale = Vector3.zero;
        boneR.transform.localScale = Vector3.zero;
    }
	public void MonkeyAnimationController(){
//		Debug.Log("Current Props is" +PlayerPrefs.GetInt("usingProp") );
        Debug.Log(PlayerPrefs.GetInt("usingProp"));
		switch(PlayerPrefs.GetInt("usingProp")){
		case 1:
			leftAni.Play("Soap");
			rightAni.Play("Soap");
            StartCoroutine(BubbleEffect());
        //    BubbleEffect();
			break;
		case 2:
            iTween.ScaleTo(Special[1],iTween.Hash("x", 1f, "y", 1f, "time", 0.5f));
            iTween.ScaleTo(Special[0], iTween.Hash("x", 1f, "y", 1f, "time", 0.5f));
			leftAni.Play("Pencil");
			rightAni.Play("Pencil");
            SpecialThs[0].Play("Drip");
            SpecialThs[1].Play("Drip");
            iTween.ScaleTo(Special[1],iTween.Hash("x", 0f, "y", 0f, "delay", 4f));
            iTween.ScaleTo(Special[0], iTween.Hash("x", 0f, "y", 0f, "delay", 4f));
			break;
		case 3:
			leftAni.Play("Paper");
			rightAni.Play("Paper");
			break;
		case 4:
			leftAni.Play("Chicken", 1f);
			rightAni.Play("Chicken",1f);
            StartCoroutine(BoneEffect());
			break;
		case 5:
            leftAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAni3;
            rightAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAni3;
            leftAni.Play("Glue");
            rightAni.Play("Glue");
			break;
		case 6:
            leftAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAni5;
            rightAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAni5;
		    leftAni.Play("Phone");
			rightAni.Play("Phone");
			break;
		case 7:
            leftAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAni4;
            rightAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAni4;
 		    leftAni.Play("Shoes");
			rightAni.Play("Shoes");
			break;
		case 8:
            leftAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAni3;
            rightAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAni3;	
            leftAni.Play("Ruler");
			rightAni.Play("Ruler");
			break;
		case 9:
            leftAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAni4;
            rightAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAni4;
            leftAni.Play("Plunger");
			rightAni.Play("Plunger");
			break;
		default:
            rightAni.Play("Pillow");
			leftAni.Play("Pillow");
//			Debug.Log("LeftAnimation Change Collection");
			break;	
		}
        leftAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAniDefault;
        rightAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAniDefault;
    }
	public tk2dSpriteAnimation MonkeySpecial;
 /*   public tk2dSpriteCollectionData Hen;
    public tk2dSpriteCollectionData Rabbit;
    
    public tk2dSpriteCollectionData Tiger;
  * 
*/
    public tk2dSpriteAnimation CatSpecial;
    public GameObject Treasure;
    public void BossSpecialAni(){
        GameController.Instance.AddCoin(30);
        switch(PlayerPrefs.GetInt("CurrentBoss")){
          case 1:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = henSpecial;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = henSpecial;
                leftAni.Play("SpecialAni1");
                rightAni.Play("SpecialAni1");
                Instantiate(Treasure, new Vector3(1, 42, -10), Quaternion.identity);
                break;
            case 2:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = CatSpecial;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = CatSpecial;
                leftAni.Play("SpecialAni");
                rightAni.Play("SpecialAni");
                GameController.Instance.AddCoin(100);
                break;
            case 3:
               leftAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitSpecial ;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitSpecial ;
                leftAni.Play("SpecialAni");
                rightAni.Play("SpecialAni");
                Instantiate(Treasure,new Vector3( 1, 42, -10),Quaternion.identity);
                break;
            case 4:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = tigerSpecialAni ;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = tigerSpecialAni ;
                leftAni.Play("SpecialAni");
                rightAni.Play("SpecialAni");
                break;
             default:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeySpecial;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeySpecial;
                leftAni.Play("SpecialAni");
                rightAni.Play("SpecialAni");
                break;
        }
 //       leftAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAniDefault;
 //       rightAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAniDefault;

    }
    public tk2dSpriteAnimation henSpecial;
    public void DisappointmentAni() {
        Debug.Log("DisappointmentAni PlayerPrefs");
        switch (PlayerPrefs.GetInt("CurrentBoss")) {
                
            case 1:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = henSpecial;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = henSpecial;
                rightAni.Play("SpecialAni");
                leftAni.Play("SpecialAni");
                break;
            case 2:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = CatSpecial;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = CatSpecial;
                leftAni.Play("Disappointment");
                rightAni.Play("Disappointment");
                break;
            case 3:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitSpecial;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitSpecial;
                leftAni.Play("Disappointment");
                rightAni.Play("Disappointment");
                RabbitBomb();    
                break;
            case 4:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = tigerSpecialAni;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = tigerSpecialAni;
                leftAni.Play("Disappointment");
                rightAni.Play("Disappointment");
                InvokeRepeating("TigerShock", 2f,1f);
               // TigerShock();
                break;
            default:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAniDefault;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = MonkeyAniDefault;
                leftAni.Play("Disappointment");
                rightAni.Play("Disappointment");
                break;
        }
    
    }
//    public GameObject taiziCollider;
    public GameObject table;
    public GameObject Camera;

    public bool BigShock = false;
    public void BigCoinShock()
    {
        iTween.ShakePosition(table, new Vector3(0f, 1.5f, -1f), 0.8f);
    }
	
    public int shakeTime = 0;
    public GameObject TigerL;
    public GameObject TigerR;
    public void TigerShock()
    {
        if (shakeTime == 8 || shakeTime == 11) {
            CancelInvoke("TigerShock");
            rightAni.Pause();
            leftAni.Pause();
        }

        switch (shakeTime % 3) { 
            case 1:
                iTween.MoveTo(TigerL,iTween.Hash("Y",7,"time",1f));
                iTween.MoveTo(TigerR,iTween.Hash("Y", 7,"time",1f));
            iTween.RotateTo(table, iTween.Hash("z", 8f, "time", 0.2f, "easetype", "linear"));
//			iTween.RotateTo(taizi, iTween.Hash("z", -8f, "time", 1.5f, "speed", 6f));
            break;
            case 2:
                iTween.MoveTo(TigerL,iTween.Hash("Y",5,"time",1f));
                iTween.MoveTo(TigerR,iTween.Hash("Y", 5,"time",1f));
            iTween.RotateTo(table, iTween.Hash("z", 0f, "time", 0.2f, "speed", 50f, "easetype", "linear"));
//			iTween.RotateTo(taizi, iTween.Hash("z", 0f, "time", 1.5f, "speed", 6f));
            break;
            default:
                iTween.MoveTo(TigerL,iTween.Hash("Y",7,"time",1f));
                iTween.MoveTo(TigerR,iTween.Hash("Y", 7,"time",1f));
            iTween.RotateTo(table, iTween.Hash("z", -8f, "time", 0.2f,"easetype","linear"));
//			iTween.RotateTo(taizi, iTween.Hash("z", 8f, "time", 1.5f, "speed", 6f));
            break;
        }
        shakeTime++;
    }


    public GameObject spider;
    public void SpiderEvent() {
        Instantiate(spider);
    }
    public void TestCoinAnimation() {
        CoinsController.Instance.DropSpecialCoin();
    }
    public tk2dSpriteAnimation catAni1;
    public tk2dSpriteAnimation catAni2;
    public void CatAnimationController() {
        switch (PlayerPrefs.GetInt("usingProp"))
        { 
            case 1:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = catAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = catAni1;
                leftAni.Play("Soap"); 
                rightAni.Play("Soap");
                StartCoroutine(BubbleEffect());
                break;
            case 2:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = catAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = catAni1;
                leftAni.Play("Pencil");
                rightAni.Play("Pencil");
                break;
            case 3:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = catAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = catAni1;
                leftAni.Play("Paper");
                rightAni.Play("Paper");
                break;
            case 4: 
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = catAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = catAni1;
                leftAni.Play("Chicken");
                rightAni.Play("Chicken");
                break;
            case 5:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = catAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = catAni2;
                leftAni.Play("Glue");
                rightAni.Play("Glue");
                break;
            case 6:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = catAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = catAni2;
                leftAni.Play("Phone");
                rightAni.Play("Phone");
                break;
            case 7:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = catAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = catAni2;
                leftAni.Play("Shoes");
                rightAni.Play("Shoes");
                break;
            case 8:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = catAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = catAni2;
                leftAni.Play("Ruler");
                rightAni.Play("Ruler");
                break;
            case 9:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = catAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = catAni2;
                leftAni.Play("Plunger");
                rightAni.Play("Plunger");
                break;
            default:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = catAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = catAni1;
                leftAni.Play("Pillow");
                rightAni.Play("Pillow");
                break;
        }
    }
    public void BossAniController() {
        switch (PlayerPrefs.GetInt("CurrentBoss")) { 
            case 1:
                HenAnimationController();
               break;
            case 2:
                CatAnimationController();
                break;
            case 3:
                RabbitAniController();
                break;
            case 4:
                TigerAniController();
                break;
            default:
                MonkeyAnimationController();
                break;

        }
    }
    public tk2dSpriteAnimation henAni1;
    public tk2dSpriteAnimation henAni2;

    public void HenAnimationController()
    {
        switch (PlayerPrefs.GetInt("usingProp"))
        {
            case 1:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = henAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = henAni1;
                leftAni.Play("Soap");
                rightAni.Play("Soap");
                StartCoroutine(BubbleEffect());
                break;
            case 2:
                iTween.ScaleTo(Special[1],iTween.Hash("x", 1f, "y", 1f, "time", 0.5f));
                iTween.ScaleTo(Special[0], iTween.Hash("x", 1f, "y", 1f, "time", 0.5f));
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = henAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = henAni1;
                leftAni.Play("Pencil");
                rightAni.Play("Pencil");
                SpecialThs[0].Play("Drip");
                SpecialThs[1].Play("Drip");
                iTween.ScaleTo(Special[1],iTween.Hash("x", 0f, "y", 0f, "delay", 5f));
                iTween.ScaleTo(Special[0], iTween.Hash("x", 0f, "y", 0f, "delay", 5f));
                break;
            case 3:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = henAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = henAni1;
                leftAni.Play("Paper");
                rightAni.Play("Paper");
                break;
            case 4:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = henAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = henAni1;
                leftAni.Play("Chicken");
                rightAni.Play("Chicken");
                break;
            case 5:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = henAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = henAni2;
                leftAni.Play("Glue");
                rightAni.Play("Glue");
                break;
            case 6:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = henAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = henAni2;
                leftAni.Play("Phone");
                rightAni.Play("Phone");
                break;
            case 7:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = henAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = henAni2;
                leftAni.Play("Shoes");
                rightAni.Play("Shoes");
                break;
            case 8:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = henAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = henAni2;
                leftAni.Play("Ruler");
                rightAni.Play("Ruler");
                break;
            case 9:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = henAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = henAni2;
                leftAni.Play("Plunger");
                rightAni.Play("Plunger");
                break;
            default:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = henAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = henAni1;
                leftAni.Play("Pillow");
                rightAni.Play("Pillow");
                break;
        }
    }
    public tk2dSpriteAnimation rabbitAni1;
    public tk2dSpriteAnimation rabbitAni2;
    public tk2dSpriteAnimation rabbitSpecial;
    public void RabbitAniController(){
        switch (PlayerPrefs.GetInt("usingProp"))
        {
            case 1:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni1;
                leftAni.Play("Soap");
                rightAni.Play("Soap");
                StartCoroutine(BubbleEffect());
                break;
            case 2:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni1;
                leftAni.Play("Pencil");
                rightAni.Play("Pencil");
                break;
            case 3:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni1;
                leftAni.Play("Paper");
                rightAni.Play("Paper");
                break;
            case 4:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni1;
                leftAni.Play("Chicken");
                rightAni.Play("Chicken");
                break;
            case 5:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni2;
                leftAni.Play("Glue");
                rightAni.Play("Glue");
                break;
            case 6:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni2;
                leftAni.Play("Phone");
                rightAni.Play("Phone");
                break;
            case 7:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni2;
                leftAni.Play("Shoes");
                rightAni.Play("Shoes");
                break;
            case 8:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni2;
                leftAni.Play("Ruler");
                rightAni.Play("Ruler");
                break;
            case 9:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni2;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni2;
                leftAni.Play("Plunger");
                rightAni.Play("Plunger");
                break;
            default:
                leftAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni1;
                rightAni.GetComponent<tk2dAnimatedSprite>().anim = rabbitAni1;
                leftAni.Play("Pillow");
                rightAni.Play("Pillow");
                break;
        }
    }
    public tk2dSpriteAnimation tigerAni1;
    public tk2dSpriteAnimation tigerAni2;
    public tk2dSpriteAnimation tigerSpecialAni;
    public void TigerAniController() {
            switch (PlayerPrefs.GetInt("usingProp"))
            {
                case 1:
                    leftAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni1;
                    rightAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni1;
                    leftAni.Play("Soap");
                    rightAni.Play("Soap");
                    StartCoroutine(BubbleEffect());
                    break;
                case 2:
                    leftAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni1;
                    rightAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni1;
                    leftAni.Play("Pencil");
                    rightAni.Play("Pencil");
                    break;
                case 3:
                    leftAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni1;
                    rightAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni1;
                    leftAni.Play("Paper");
                    rightAni.Play("Paper");
                    break;
                case 4:
                    leftAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni1;
                    rightAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni1;
                    leftAni.Play("Chicken");
                    rightAni.Play("Chicken");
                    StartCoroutine(BoneEffect());
                    break;
                case 5:
                    leftAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni2;
                    rightAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni2;
                    leftAni.Play("Glue");
                    rightAni.Play("Glue");
                    break;
                case 6:
                    leftAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni2;
                    rightAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni2;
                    leftAni.Play("Phone");
                    rightAni.Play("Phone");
                    break;
                case 7:
                    leftAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni2;
                    rightAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni2;
                    leftAni.Play("Shoes");
                    rightAni.Play("Shoes");
                    break;
                case 8:
                    leftAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni2;
                    rightAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni2;
                    leftAni.Play("Ruler");
                    rightAni.Play("Ruler");
                    break;
                case 9:
                    leftAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni2;
                    rightAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni2;
                    leftAni.Play("Plunger");
                    rightAni.Play("Plunger");
                    break;
                default:
                    leftAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni1;
                    rightAni.GetComponent<tk2dAnimatedSprite>().anim = tigerAni1;
                    leftAni.Play("Pillow");
                    rightAni.Play("Pillow");
                    break;
            }
        
        }
    
}   