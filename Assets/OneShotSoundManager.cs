using UnityEngine;
using System.Collections;
/// <summary>
/// 効果音用のサウンドマネージャー
/// 1フレームの間に複数回、同じ効果音を鳴らす判定があったときでも次のフレーム時に一度だけ
/// 効果音がなるようにする。
/// </summary>
public class OneShotSoundManager : MonoBehaviour {

    [SerializeField]
    private AudioClip coinGet;
    private bool emitCoinGet;
    private Vector3 emitCoinPosition;
    [SerializeField]
    private AudioClip playerDead;
    private bool emitPlayerDead;
    private Vector3 emitPlayerDeadPosition;
    [SerializeField]
    private AudioClip playerJump;
    private bool emitPlayerJump;
    private Vector3 emitPlayerJumpPoisiotn;

    static OneShotSoundManager __instance;
    private AudioSource m_audioSource;
	// Use this for initialization
	void Start () {
        m_audioSource = gameObject.GetComponent<AudioSource>();
        __instance = this;
    }
	public static OneShotSoundManager getInstance()
    {
        return __instance;
    }
    public void PlayCoinGet(GameObject to)
    {
        this.emitCoinGet = true;
        this.emitCoinPosition = to.transform.position;
    }
    public void PlayJumpPlayer(GameObject to)
    {
        this.emitPlayerJump = true;
    }
    public void PlayPlayerDead(GameObject to)
    {
        this.emitPlayerDead = true;
    }
	// Update is called once per frame
	void Update () {
        if (emitCoinGet)
        {
            AudioSource.PlayClipAtPoint(coinGet, Camera.main.transform.position);
            emitCoinGet = false;
        }
        if(emitPlayerJump)
        {
            m_audioSource.PlayOneShot(playerJump);
            emitPlayerJump = false;
        }
        if(emitPlayerDead)
        {
            AudioSource.PlayClipAtPoint(playerDead, Camera.main.transform.position);
            emitPlayerDead = false;
        }

    }
}
