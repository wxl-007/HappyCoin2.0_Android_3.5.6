using UnityEngine;
using System.Collections;

public class BarrierController : MonoBehaviour {
	
    private bool isElevate = true;
	public float moveDistance = 8f;
	private float initY;
	public float frequency = 0.5f;
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
				transform.position += new Vector3(0f, moveDistance /(frequency * 1f) * Time.fixedDeltaTime, 0f);
			}else{
				transform.position = new Vector3(transform.position.x , initY + moveDistance, transform.position.z);	
			    delayTime += Time.fixedDeltaTime; 
				if(delayTime > timeInterval){ 
					isElevate = false;
				}
			}	
		}else{
			if(transform.position.y > initY){
				transform.position -= new Vector3(0, moveDistance /(frequency * 1f) * Time.fixedDeltaTime, 0f );
			    delayTime = Time.fixedDeltaTime;
			}else{
				transform.position = new  Vector3(transform.position.x, initY, transform.position.z);
				Destroy(gameObject);
			}
		}
	}
}