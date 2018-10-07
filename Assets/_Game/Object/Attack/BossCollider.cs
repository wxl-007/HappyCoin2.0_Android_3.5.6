using UnityEngine;
using System.Collections;

public class BossCollider : MonoBehaviour {
	
//	private float initX;
	private bool bossShake;
	public float moveDistance =2f;
	private bool shakeStart;
	private Vector3 initPosition;
	public tk2dAnimatedSprite leftSprite;
//	private float leftX;
	private Vector3 leftPosition;
	// Use this for initialization
	void Start () {
		initPosition = gameObject.transform.parent.position;
//		initX = gameObject.transform.parent.position.x;
//		leftX = leftSprite.transform.position.x;
		leftPosition = leftSprite.transform.position;
	}
	public void OnCollisionEnter(Collision collision){
		bossShake = true;
		StartCoroutine(ShakeTime());
		SoundController.Instance.BeAttacked();
		
//		collision.collider.rigidbody.AddForce(new Vector3(0f, 0f , -70f));
		
	}
	public IEnumerator ShakeTime(){
		if(bossShake == true ){
			for(int i = 2; i <= 10; i+=2){
				transform.parent.position -= new Vector3(moveDistance * (1 - i * 0.1f), 0f, 0f);
				leftSprite.transform.position -= new Vector3(moveDistance * (1 - i * 0.1f), 0f, 0f);
				yield return new WaitForSeconds(0.02f);
				transform.parent.position += new Vector3(moveDistance * (1 - i * 0.1f), 0f , 0f);
				leftSprite.transform.position += new Vector3(moveDistance *	 (1 - i * 0.1f), 0f, 0f);
				yield return new WaitForSeconds(0.02f);
			}	
		}
		yield return new WaitForSeconds(1f);
//		animation.Stop();
//		leftSprite.animation.Stop();
		gameObject.transform.parent.position = initPosition;
		leftSprite.transform.position = leftPosition;
		bossShake = false;
	}
	// Update is called once per frame
	void Update () {
		
	}
}
