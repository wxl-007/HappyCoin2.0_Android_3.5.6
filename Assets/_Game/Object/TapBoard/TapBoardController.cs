using UnityEngine;
using System.Collections;

public class TapBoardController : MonoBehaviour {
	
	public GameObject CoinPrefab;
	
	private Ray ray;
	private RaycastHit hit;
    public tk2dSprite touchTip; 

	
	// Use this for initialization
	void Start () {
        Destroy(touchTip, 5f);
	}
	
	// Update is called once per frame
	void Update () {
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Input.GetMouseButtonDown(0)&& CoinsController.Instance.coinTrigger == true) {
			if (Physics.Raycast(ray, out hit, 100) && hit.transform.name == this.name) {
				if (GameController.Instance.AddCoin(-1)) {
				//	float y = Mathf.Clamp(hit.point.y,27.5f,32f);
					Vector3 pos = new Vector3(hit.point.x ,hit.point.y, hit.point.z -5);
					 Instantiate(CoinPrefab, pos, Quaternion.identity);
//					Coin.rigidbody.AddTorque(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
					CoinsController.Instance.SubtractCoin(-1);
					StartCoroutine(SoundController.Instance.CoinDropEffect());
					
				}
			}
			GameMenuController.Instance.ShowCoinsInGM();
		}
	}
}