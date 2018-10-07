using UnityEngine;
using System.Collections;

public class BorderController : MonoBehaviour {
	private bool isElevate = true;
	public float moveDistance = 18f;
	public float borderX;
	private float initY;
	public float frequency = 0.8f;
	public float timeInterval = 30f;
	private float delayTime; 
	
	// Use this for initialization
	void Start () {
	 initY = transform.position.y;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(isElevate){
			if(transform.position.y < initY + moveDistance ){
				transform.position += new Vector3(0f, moveDistance /(frequency * 5f) * Time.fixedDeltaTime, 0f);
//				SoundController.Instance.BorderSoundPlay();
			}else{
				transform.position = new Vector3(transform.position.x , initY + moveDistance, transform.position.z);	
			    delayTime += Time.fixedDeltaTime; 
				if(delayTime > timeInterval){ 
					isElevate = false;
				}
			}	
		}else{
			if(transform.position.y > initY){
				transform.position -= new Vector3(0, moveDistance /(frequency * 5f) * Time.fixedDeltaTime, 0f );
			    delayTime = Time.fixedDeltaTime;
//				SoundController.Instance.BorderSoundPlay();
			}else{
				transform.position = new  Vector3(transform.position.x, initY, transform.position.z);
				Destroy(gameObject);
			}
		}
	}
}