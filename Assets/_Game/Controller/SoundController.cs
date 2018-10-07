using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(AudioListener))]

public class SoundController : MonoBehaviour {
	public AudioClip[] dropCoinSound;
	private static SoundController instance;
//	private List<GameObject> players = new List<GameObject>();
	public AudioClip bombExplosion;
	public AudioClip[] throwSound;
	public static SoundController Instance {
		get 
		{
			if (instance == null)
			{
				instance = (SoundController)FindObjectOfType(typeof(SoundController));
			}
			if (!instance)
			{
				Debug.LogError("SoundController could not find himself!");
			}
			return instance;
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	public void PlayDropCoin(){
		int n = Random.Range(0, 4);
		AudioClip CoinSound = dropCoinSound[n];
		audio.PlayOneShot(CoinSound);
//		audio.Play(dropCoinSound[Random.Range(0,4)]);	
	}
	public void PlayBombExplosion(){
		audio.PlayOneShot(bombExplosion);
	}
	public void ThrowingSound(){
		int n = Random.Range(0,3);
		audio.PlayOneShot(throwSound[n]);
	}
	// Update is called once per frame
	void Update () {
	
	}
	public AudioClip borderSound;
	public void BorderSoundPlay(){
		audio.PlayOneShot(borderSound);
	}
	public AudioClip[] laughSound;
	public void LaughSoundPlay(){
		int n = Random.Range(0,2);
		audio.PlayOneShot(laughSound[n]);
	}
	public AudioClip passSoundEffect;
	public void PassSound(){
		audio.PlayOneShot(passSoundEffect);
	}
	public AudioClip bossBeAttacked;
	public void BeAttacked(){
		audio.PlayOneShot(bossBeAttacked);
	}
	public AudioClip wingSound;
	public void TinyWingSound(){
		audio.PlayOneShot(wingSound);
	}
	public AudioClip faceSound;
	public void FaceSound(){
		audio.PlayOneShot(faceSound);
	}
	public AudioClip jackpotSound;
	public void JackpotSoundEffect(){
		if(JackpotController.Instance.JackpotOn == true ){
			audio.PlayOneShot(jackpotSound);
			
		} if(JackpotController.Instance.JackpotOn == false ){
			//audio.clip = jackpotSound;
			audio.Stop();
//			Debug.Log("pause");
		}
	}
	public AudioClip fightSound;
	public void FightEffect(){
		audio.PlayOneShot(fightSound);
	}
	public AudioClip[] coinDrop;
	public IEnumerator CoinDropEffect(){
		yield return new WaitForSeconds(0.3f);
		int n = Random.Range( 0, 3);
		audio.PlayOneShot(coinDrop[n]);
	}
    public AudioClip BGM;
    public GameObject playerPrefab;

    public void PlayBGM()
    {
        GameObject BGMPlayer = Instantiate(playerPrefab) as GameObject;
        BGMPlayer.transform.parent = transform;
        BGMPlayer.transform.localPosition = new Vector3(0f, 0f, 0f);
        BGMPlayer.audio.clip = BGM;
        BGMPlayer.audio.volume = 0.6f;
        BGMPlayer.audio.loop = true;
        BGMPlayer.audio.Play();
    }
}
