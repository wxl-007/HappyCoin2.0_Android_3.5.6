using UnityEngine;
using System.Collections;

public class GoalBoardController : MonoBehaviour
{
	
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "_coin")
        {
            GameController.Instance.AddCoin(1);
            GlobalManager.receiveCoinNum++;
			//Debug.Log(GlobalManager.receiveCoinNum);
            StartCoroutine(SoundController.Instance.CoinDropEffect());
			if(other.name.StartsWith("AttackCoin")){
                StartCoroutine(JackpotController.Instance.HandEvent());
            }
            else if(other.name.StartsWith("AnlgeCoin")){
                StartCoroutine(JackpotController.Instance.WingEvent());
            }
            else if(other.name.StartsWith("DefenceCoin")){
                StartCoroutine(JackpotController.Instance.FaceEvent());
            }else if(other.name.StartsWith("Coin_5")){
                GameController.Instance.AddCoin(4);
                GlobalManager.receiveCoinNum += 5;
            }else if(other.name.StartsWith("Coin_2")){
                GameController.Instance.AddCoin(2);
            }else if(other.name.StartsWith("Coin_10")){
                GameController.Instance.AddCoin(9);
                GlobalManager.receiveCoinNum += 20;
            }else if(other.name.StartsWith("ExpCoin_15")){
				JackpotController.Instance.Add15Exp();
			}else if(other.name.StartsWith("ExpCoin_50")){
				JackpotController.Instance.Add50Exp();
			}else if(other.name.StartsWith("BigCoin")){
                GameController.Instance.AddCoin(20);
                GlobalManager.receiveCoinNum += 20;
			}else if(other.name.StartsWith("TroubleCoin")){
                EventsController.Instance.ClawEvent();
			}else if(other.name.StartsWith("BombCoin")){
                EventsController.Instance.BombEvent();
			}else if(other.name.StartsWith("MeteorCoin")){
                CoinsController.Instance.MeteorEvent();
			}else if(other.name.StartsWith("SpiderCoin")){
                EventsController.Instance.SpiderEvent();
			}else if(other.name.StartsWith("BorderCoin")){
                EventsController.Instance.BorderEvent();
			}else if(other.name.StartsWith("BarrierCoin")){
                EventsController.Instance.DefenceEvent();
			}else if(other.name.StartsWith("Banana")){
                GameController.Instance.AddCoin(20);
                GlobalManager.receiveCoinNum += 20;
                EventsController.Instance.BossSpecialAni();
			}else if(other.name.StartsWith("Fish")){
                GameController.Instance.AddCoin(100);
                GlobalManager.receiveCoinNum += 100;
                EventsController.Instance.BossSpecialAni();
			}else if(other.name.StartsWith("Carrot")){

                GameController.Instance.AddCoin(20);
                GlobalManager.receiveCoinNum += 20;
                EventsController.Instance.BossSpecialAni();
			}else if(other.name.StartsWith("Steak")){
                for (int i = 0; i < 3; i++) {
                    CoinsController.Instance.MeteorEvent();
                }
                    
                GameController.Instance.AddCoin(30);
                GlobalManager.receiveCoinNum += 30;
                EventsController.Instance.BossSpecialAni();
            }else if (other.name.StartsWith("Treasure"))
            {
                EventsController.Instance.BossSpecialAni();
                JackpotController.Instance.TreasureBox();
            }else if (other.name.StartsWith("GoldenEgg"))
            {
                EventsController.Instance.BossSpecialAni();
                GameController.Instance.AddCoin(30);
                GlobalManager.receiveCoinNum += 30;
            }
            else if (other.name.StartsWith("PushHandCoin")) {
                EventsController.Instance.TongueEvent();
            }else if(other.name.StartsWith("GoldenBar")){
                GameController.Instance.AddCoin(30);
                GlobalManager.receiveCoinNum += 30;
            }
            else if (other.name.StartsWith("SliverEgg")) {
                GameController.Instance.AddCoin(15);
                GlobalManager.receiveCoinNum += 15;
            }
            else if (other.name.StartsWith("BorderCoin")) { 
            
            }

			JackpotController.Instance.UpdateExpBar();
        }
    }
}
