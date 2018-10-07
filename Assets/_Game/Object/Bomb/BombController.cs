using UnityEngine;
using System.Collections;

public class BombController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(BurstEvent());
	}
	private GameObject[] coins ;
	public tk2dAnimatedSprite BurstSprite;
	public GameObject Bomb;
	public tk2dAnimatedSprite SparkSprite;
	private Vector3 center;
	public IEnumerator BurstEvent(){
		BurstSprite = BurstSprite.GetComponent<tk2dAnimatedSprite>();
		SparkSprite = SparkSprite.GetComponent<tk2dAnimatedSprite>();
		SparkSprite.Play("spark");
		gameObject.rigidbody.velocity = new  Vector3(0, -18f, 0);
		yield return new WaitForSeconds(1.6f);
//		BurstSprite.Play("burst");
//		yield return new WaitForSeconds(0.5f);
		Destroy(gameObject);
	}
	public void OnCollisionEnter(Collision collision){
		Burst();
		BurstSprite.Play("burst");
		Destroy(SparkSprite);
		Destroy(Bomb);
		SoundController.Instance.PlayBombExplosion();
		collider.enabled = false;
	}
	public void Burst(){
		center = gameObject.transform.position;
		coins =  GameObject.FindGameObjectsWithTag("_coin");
		foreach(GameObject _coin in coins){
			Vector3 distance = center -	_coin.transform.position;
			Vector3 burstForce = -10f * distance ;
			if(distance.magnitude <= 10 && distance.magnitude > 7){
				_coin.rigidbody.AddForce(0f, 300f, 0f);
            //    _coin.particleEmitter.emit = false;
				_coin.rigidbody.AddForce(1f * burstForce);
			}
			if(distance.magnitude <= 7 && distance.magnitude >4){
				_coin.rigidbody.AddForce(0f, 350f, 0f);
          //      _coin.particleEmitter.emit = false;
				_coin.rigidbody.AddForce(2f * burstForce);
			}
			if(distance.magnitude <= 4 && distance.magnitude >0){
				_coin.rigidbody.AddForce(0f, 250, 0f);
          //      _coin.particleEmitter.emit = false;
				_coin.rigidbody.AddForce(1.5f * burstForce);
			}
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
