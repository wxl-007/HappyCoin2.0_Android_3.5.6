using UnityEngine;
using System.Collections;

public class TapText : MonoBehaviour {
	private float initY;
	private float moveDis = 2f;
	private float frequency = 0.5f;
	private bool Jump = true;
	// Use this for initialization
	void Start () {
		StartCoroutine(TapFontEffect());
		initY = transform.position.y; 
	}
	
	// Update is called once per frame
	void Update () {
		if(Jump){
			if(transform.position.y < initY + moveDis){
				transform.position += new Vector3( 0f, moveDis/(frequency )*Time.fixedDeltaTime, 0f );
			}else{
				transform.position = new Vector3(transform.position.x, initY + moveDis , transform.position.z);
				Jump = false;
			}
		}else{
			if(transform.position.y > initY){
				transform.position -= new Vector3( 0f, (moveDis - 0.8f) /(frequency ) *Time.fixedDeltaTime, 0f);
			}else{
				transform.position = new Vector3(transform.position.x, initY, transform.position.z );
				Jump = true;
			}
		}
	}
	void FixedUpdate(){
		
	}

	public IEnumerator TapFontEffect(){
		yield return new WaitForSeconds(2f);
		Destroy(gameObject);
	}
}
