using UnityEngine;
using System.Collections;

public class DestroyBoardController : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
        if (other.tag == "_coin") {
            Destroy(other.gameObject);
            
		}
    }
}
