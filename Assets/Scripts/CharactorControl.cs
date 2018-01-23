using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets._2D;
using Assets.Network;
[RequireComponent(typeof(PlatformerCharacter2D))]
public class CharactorControl : MonoBehaviour
{
    private PlatformerCharacter2D m_Character;
    private CharactorInfoEmitter m_emitter;
    private bool m_Jump;
    public bool getJump()
    {
        return m_Jump;
    }

   
    private float before_position_x;
    private float before_position_y;
    private int m_PositionSameCount = 0;
    private bool m_Clouch;//ネットから取得するため追加
    private bool m_MoveDirectionLeft = false;//trueなら右向き　falseなら左向きに歩く
    public bool getMoveDirection()
    {
        return m_MoveDirectionLeft;
    }
    public bool getClouch()
    {
        return m_Clouch; 
    }
    private void Start()
    {
        this.m_emitter = new CharactorInfoEmitter(this);
        //m_emitter.StartEmit();
    }
    private void Awake()
    {
        m_Character = this.gameObject.GetComponentInChildren<PlatformerCharacter2D>();
    }


    private void Update()
    {
        //////////////////////////////////////////
        // ポジションが一定時間かわらなかったら、向きを変更(ポーズ時を除く)//
        //////////////////////////////////////////
        if (Time.timeScale != 0)
        {
            if ((this.before_position_x == this.m_Character.transform.position.x) && (this.before_position_y == this.m_Character.transform.position.y))
            {
                m_PositionSameCount++;
            }
            if (m_PositionSameCount > 3)
            {
                m_MoveDirectionLeft = !m_MoveDirectionLeft;
                m_PositionSameCount = 0;
            }
        }
        ////////////////////////////////////////////

        this.before_position_x = this.m_Character.transform.position.x;
        this.before_position_y = this.m_Character.transform.position.y;
    }
    public void Jump()
    {
        this.m_Jump = true;
    }


    private void FixedUpdate()
    {
        // Read the inputs.
        bool crouch = Input.GetKey(KeyCode.LeftControl);
        m_Clouch = crouch;
        // float h = CrossPlatformInputManager.GetAxis("Horizontal");
        // Pass all parameters to the character control script.
        float h = m_MoveDirectionLeft ? -1.0f : 1.0f;
        if(m_Jump)
        {
            OneShotSoundManager.getInstance().PlayJumpPlayer(this.gameObject);
        }
        m_Character.Move(h, crouch, m_Jump);
        m_Jump = false;
    }
}