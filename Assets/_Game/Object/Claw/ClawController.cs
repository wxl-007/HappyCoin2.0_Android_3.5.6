using UnityEngine;
using System.Collections;

public class ClawController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(ClawMove());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	IEnumerator ClawMove(){
		yield return new WaitForSeconds(2.5f);
		Destroy(gameObject);
	}
}
