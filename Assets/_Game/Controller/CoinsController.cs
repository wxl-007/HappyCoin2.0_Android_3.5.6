using UnityEngine;
using System.Collections;

public class CoinsController : MonoBehaviour {
	private static CoinsController instance;		
	public static CoinsController Instance {
		get 
		{
			if (instance == null)
			{
				instance = (CoinsController)FindObjectOfType(typeof(CoinsController));
			}
			if (!instance)
			{
				Debug.LogError("CoinsController could not find himself!");
			}
			return instance;
		}
	}

	private Vector3 initPos;
	private int counts = 0;
	private float lastY;
	public GameObject defenceCoin;
	public GameObject attackCoin;
	public GameObject angleCoin;
    public GameObject[] specialCoin;
	private float timeInterval = 420f;
    private int RandomNum;
    public int SpecialNum;
    private GameObject[] coins;
	// Use this for initialization
	void Start () {
		InvokeRepeating( "DropSpecialCoin", 300f, timeInterval);
		InvokeRepeating( "CoinLimit", 1.2f, 1.2f);
        InvokeRepeating("SpecialThings", 300,300f);
		InvokeRepeating("DropCoinEach3Min",180,180);

	}

	// Update is called once per frame
	void FixedUpdate () {	
		counts++;
		if (counts % 10 == 0) {
			coins =  GameObject.FindGameObjectsWithTag("_coin");
		}
		if (coins != null) {
			foreach (GameObject _coin in coins) 
			if (_coin != null)	{
				if (_coin.transform.position.y > 30) _coin.rigidbody.AddForce(0f, -20f, -4f);
				else if (_coin.transform.position.y > 32) _coin.rigidbody.AddForce(0f, -30f, -4f);
				else if (_coin.transform.position.y > 33) _coin.rigidbody.AddForce(0f, -40f, -4f);
				if(_coin.transform.position.x >= -10f && _coin.transform.position.x < 0 ) _coin.rigidbody.AddForce( 1.5f, -2f, -1.5f);
				if(_coin.transform.position.x <= 10f && _coin.transform.position.x >= 0 ) _coin.rigidbody.AddForce( -1.5f, -2f, -1.5f);
			}
		}  
		if(EventsController.Instance.shakeOccur){
			if (coins == null) coins =  GameObject.FindGameObjectsWithTag("_coin");
			float y = Input.acceleration.y;
			if (y > -0.8f) y -= -0.8f;
			else y += -0.8f;
			y = Mathf.Clamp(y, -0.15f, 0.2f);
			if (y * lastY <= 0) {
				lastY = y;
				Vector3 acc = new Vector3(0, 0, -y);
				foreach (GameObject _coin in coins) 
				if (_coin != null)	{
					_coin.rigidbody.AddForce(acc * 1200f);
				}
			}	
		} 
	}
   

 	public void DropSpecialCoin (){
		Debug.Log("DropSpecialCoin");
		int RangeX = Random.Range(-12, 14);
		int RangeY = Random.Range(38, 45);
		int RangeZ = Random.Range(-40, -16);
		Vector3 pos = new Vector3(RangeX, RangeY, RangeZ);
		RandomNum = Random.Range(0, 1000);
        if (RandomNum <= GlobalManager.probilityNum[GlobalManager.levelNum])
        {
            int randomSpecialCoin = Random.Range(0, GlobalManager.levelNum);
            Instantiate(specialCoin[randomSpecialCoin], pos, Quaternion.identity);
            Debug.Log("SpecialCoin=" + randomSpecialCoin);
        }
        else {
            Debug.Log("=================Fuck up============");
        } 
	}


	public tk2dAnimatedSprite[] light;
	private int usefulCoin = 4;
	public bool coinTrigger;
 
	void CoinLimit(){
		if(usefulCoin <= 3)SubtractCoin(+1);
			
	}
	public bool SubtractCoin(int delta){
		if(usefulCoin + delta < 0) return false;
		usefulCoin += delta ;
		return true;
	}
	
	void Update(){
		LightControll();
		if(usefulCoin != 0){
			coinTrigger = true;
		}else{
			coinTrigger = false;
		}
	}
	public void LightControll(){
		for (int i= 1; i<= 4; i++){
			if (i <= usefulCoin) light[i-1].Play("light");
			else light[i-1].Play("dark");
		}
	}
    public void MeteorEvent() {
        for (int n = 0; n < 10; n++)
        {
            int i = 0;
            Vector3 Pos = new Vector3(Random.Range(-26f, 26f), 80.5f, Random.Range( -44f, -1.7f));
            GameObject meteor = (GameObject)Instantiate(specialCoin[i], Pos, Quaternion.identity);
			meteor.rigidbody.velocity = Vector3.down * 80f;
        }
    }
	
	public void DropCoinEach3Min(){
		Instantiate(specialCoin[GlobalManager.levelNum -1],new Vector3(1, 50,-10),Quaternion.identity);
		
		
	}
    public void DropCoin() {
        for (int i = 0; i < 2; i++) { 
            int RangeX = Random.Range(-5, 5);
            int RangeY = Random.Range(34, 37);
            int RangeZ = Random.Range(-16,-10);
            Vector3 pos = new Vector3(RangeX, RangeY, RangeZ);
            Instantiate(specialCoin[GlobalManager.levelNum - 1], pos, Quaternion.identity);
           // Instantiate(specialCoin[GlobalManager.levelNum], pos, Quaternion.identity);
        }
    }
    public GameObject[] specialThings;
    public void SpecialThings() {
        int randomRate = Random.Range(0,100);
        if (randomRate <= 20) {
          Instantiate(specialThings[PlayerPrefs.GetInt("CurrentBoss")], new Vector3(-1f, 40f, -10), Quaternion.identity);
        }
      //  GameObject Things = (GameObject)Instantiate(specialThings[PlayerPrefs.GetInt("CurrentBoss")], new Vector3(-1f, 40f, -10), Quaternion.identity);
    }
}