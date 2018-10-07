using UnityEngine;
using System.Collections;

public class TongueController : MonoBehaviour {
	
	public float moveDistance = 9f;
	public float frequency = 2.5f;
	private float initZ;
	private bool isPush = true;
	private float longTongue = 14f;
	// Use this for initialization
	void Start () {
		initZ = transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void FixedUpdate () {
		if (isPush ) {
			if(EventsController.Instance.stretchOut == false){
				if (transform.position.z > initZ - moveDistance) {
					transform.position -= new Vector3(0f, 0f, moveDistance / (frequency * 0.4f) * Time.fixedDeltaTime );	
				}else{
					isPush = false;
				}
			}else{
				if(transform.position.z > initZ - longTongue){
					transform.position -= new Vector3(0f, 0f, longTongue / (frequency * 0.8f) * Time.fixedDeltaTime);
				}else{
					isPush = false;
					EventsController.Instance.stretchOut = true;
				}
			}
		} else {
			if(EventsController.Instance.stretchOut == false){
				if (transform.position.z < initZ) {
					transform.position += new Vector3(0f, 0f, moveDistance / (frequency * 0.4f) * Time.fixedDeltaTime );
				}else{
					isPush = true;
				}
			} else {
				if(transform.position.z < initZ){
					transform.position += new Vector3 (0f, 0f, longTongue / (frequency * 0.8f) * Time.fixedDeltaTime);
				}else{
					isPush = true;
					EventsController.Instance.stretchOut = false;
				}
			}
		}
	}
	public void OnCollisionEnter(Collision collision){
		collision.collider.rigidbody.AddForce(new Vector3(0f, 0f , -50f));
	}
	
}
