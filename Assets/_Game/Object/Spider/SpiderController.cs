using UnityEngine;
using System.Collections;

public class SpiderController : MonoBehaviour {
    private Ray ray;
    private RaycastHit hit;
    public GameObject spider;
    private Vector3[] point1Pos;
    private bool onceTap = false;
    private GameObject touchObj;
//    private bool catchObj = false;
	// Use this for initialization
	void Start () {
        onceTap = true;
	}
    /*   public void OnCollisionEnter(Collision collision) {
          touchObj = collision.gameObject; 
        
              IsInvoking("Invoke");

             // collision.gameObject.transform.parent = spider.transform;
        
      }
    */
    public void OnCollisionStay(Collision collision) {
             Debug.Log("===========pow==========");
             Debug.Log("========"+ collision.gameObject.tag);
             touchObj = collision.gameObject;
             Invoke();
            
         }
    public void Invoke() {
       if (touchObj.CompareTag("_coin")) {
          // touchObj.transform.position = spider.transform.position;
           iTween.MoveAdd(touchObj,spider.transform.position,3f);
           touchObj.rigidbody.useGravity = false;
           Debug.Log("================ catch it ===========");
       }
   }

    // Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && onceTap == true )
        {
            onceTap = false;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 200))
            {
                 point1Pos = new Vector3[4];
                 point1Pos[0] = new Vector3(hit.point.x,60f,hit.point.z);
                 point1Pos[1] = new Vector3(hit.point.x, 30f, hit.point.z);
                 point1Pos[2] = new Vector3(hit.point.x, 60f, hit.point.z);
                 point1Pos[3] = new Vector3(0f, 58f, -50f);
                 iTween.MoveTo(spider, iTween.Hash("path", point1Pos, "time", 5f,"easetype","linear", "movetopath", true));     
            }   
        }
        if (spider.transform.position == new Vector3(0f, 58f, -50f))
        {
            //touchObj.transform.parent = null;
            Debug.Log("--------------cancel?-------");
            touchObj.rigidbody.useGravity = true;
            CancelInvoke("Invoke");
        }
	}
}
