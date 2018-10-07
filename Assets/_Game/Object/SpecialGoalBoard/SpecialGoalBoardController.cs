	using UnityEngine;
using System.Collections;

public class SpecialGoalBoardController : MonoBehaviour {
//jackpot(LaoHuJi) event  trigger 
	private bool Move = true;
	private float fruquency = 6f;
	public float distance = 20f;
	private float initX;
//	private float interval =0.5f;
//	public tk2dAnimatedSprite arrowAni;
	private int toWaitforSceond = 0;
	
	public void OnTriggerEnter(Collider other){
		if (other.tag == "_coin"){
			JackpotController.Instance.JackpotTrigger();
			toWaitforSceond = 1;
			
		}
//		Debug.Log(toWaitforSceond);
	}
		
	void Start(){
		initX = transform.position.x;
	}
	void FixedUpdate(){
		if(toWaitforSceond == 1) StartCoroutine(ArrowSpriteEvent());
		if(toWaitforSceond == 0) StartCoroutine(Movement());
	}
	public IEnumerator ArrowSpriteEvent(){
//		arrowAni.Play("ArrowColor");
		yield return new WaitForSeconds(0.1f);			
		toWaitforSceond = 0;
	}
	public IEnumerator Movement(){
		if(Move && toWaitforSceond == 0){
			if(transform.position.x < initX + distance){
				transform.position += new Vector3(( distance) / fruquency * Time.fixedDeltaTime, 0f, 0f);
			}else{
				transform.position = new Vector3(initX + distance, transform.position.y, transform.position.z);
				Move = false;
			}
		}else if(Move == false && toWaitforSceond == 0){
			if(transform.position.x > initX){
				transform.position -= new Vector3(distance / fruquency * Time.fixedDeltaTime, 0f, 0f);
			}else{
				transform.position = new Vector3(initX, transform.position.y, transform.position.z);
				yield return new WaitForSeconds(0f);
				Move = true;
			}
		}
	}
}
