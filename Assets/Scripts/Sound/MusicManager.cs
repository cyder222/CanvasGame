using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

    public AudioClip[] m_LevelMusicArray;
    private static MusicManager __instance;
    // Use this for initialization
    private AudioSource m_audioSource;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

	void Start () {
        m_audioSource = this.GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public void changeMusic(int no)
    {
        m_audioSource.clip = m_LevelMusicArray[no];
    }
}
