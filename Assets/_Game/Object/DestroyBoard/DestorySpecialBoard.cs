using UnityEngine;
using System.Collections;

public class DestorySpecialBoard : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("_coin"))
        {
            if (other.name.StartsWith("Banana") || other.name.StartsWith("BigCoin") || other.name.StartsWith("ExpCoin_50") || other.name.StartsWith("Fish") || other.name.StartsWith("GoldenBar") || other.name.StartsWith("GoldenEgg") || other.name.StartsWith("MetorCoin") || other.name.StartsWith("PushHandCoin") || other.name.StartsWith("SliverEgg") || other.name.StartsWith("Treasure") || other.name.StartsWith("Carrot") || other.name.StartsWith("Steak"))
            {
                EventsController.Instance.DisappointmentAni();
            }

            Destroy(other.gameObject);

        }
    }
}
