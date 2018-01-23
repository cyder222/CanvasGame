using UnityEngine;
using System.Collections;

public class VelocityObjectScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter2D(Collider2D collision)
    {
        CharactorControl ch_control = collision.GetComponent<CharactorControl>();
        if (ch_control!=null)
        {
            //ch_control.Jump();
            Debug.Log("collision upper");
            Rigidbody2D body = collision.GetComponent<Rigidbody2D>();
            OneShotSoundManager.getInstance().PlayJumpPlayer(this.gameObject);
            body.velocity = new Vector2(body.velocity.x, 20.0f);
        }
    }
}
