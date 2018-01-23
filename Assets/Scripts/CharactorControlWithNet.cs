//-----------------------------------------------------------------------
// <author>T.Mitsuyasu</author>
//-----------------------------------------------------------------------
using System;
using Assets.Utility;
using SocketIO;
using UnityEngine;
using UnityStandardAssets._2D;

[RequireComponent(typeof(PlatformerCharacter2D))]
public class CharactorControlWithNet : MonoBehaviour
{
    // ネットワーク関係の固定変数
    private static readonly string POSITION_X = "x";
    private static readonly string POSITION_Y = "y";
    private static readonly string POSITION_Z = "z";
    private static readonly string ROTATE_X = "rx";
    private static readonly string ROTATE_Y = "ry";
    private static readonly string ROTATE_Z = "rz";
    private static readonly string IS_JUMP = "J";
    private static readonly string MOVE_DIR = "MD";
    private static readonly string IS_CLOUCH = "C";
    private static readonly string EMIT_TIME = "ET";
    private static readonly string OWNER_ID = "OI";
    private static SocketIOComponent socket = null;
    // その他コード
    private PlatformerCharacter2D myCharacter;
   
    private string ownerId;
    private bool isJump;
    private bool isClouch; // ネットから取得するため追加
    private int isPositionSameCount = 0;
    private bool isMoveDirectionLeft = false; // trueなら右向き　falseなら左向きに歩く

    private float last_data_owner_time; //最後に処理したデータをオーナーが送信した時刻

    public void Jump()
    {
        this.isJump = true;
    }

    private void Awake()
    {
        this.myCharacter = this.gameObject.GetComponentInChildren<PlatformerCharacter2D>();
    }

    private void Start()
    {
        if (socket == null)
        {
            try
            {
                GameObject go = GameObject.Find("SocketIO");
                socket = go.GetComponent<SocketIOComponent>();
                socket.On("CharactorUpdate", this.OnCharactorUpdate);
            }
            catch (Exception e)
            {
                Debug.Log("Cannot found server interface program!" + e.Message);
            }
        }
    }

    private void OnCharactorUpdate(SocketIOEvent e)
    {
        JSONObject json = e.data;
        if (json.GetField(OWNER_ID).str == socket.sid) //自分からの指定は更新しない
            return;
        if (json.GetField(OWNER_ID).str != this.ownerId) //キャラクターのオーナーからの指定でなければ更新しない
            return;
        float emit_time = json.GetField(EMIT_TIME).f;
        //通信の入れ違いを防ぐために、最後に受け取って処理したデータの オーナー側での送信時時刻と、今回のデータのオーナー側での送信時時刻を比べる
        if (this.last_data_owner_time >= emit_time)
        {
            return;
        }
        //以下、オブジェクトをパースして情報を取り出す。
        float position_x = json.GetField(POSITION_X).f;
        float position_y = json.GetField(POSITION_Y).f;
        float position_z = json.GetField(POSITION_Z).f;
        float rotate_x = json.GetField(ROTATE_X).f;
        float rotate_y = json.GetField(ROTATE_Y).f;
        float rotate_z = json.GetField(ROTATE_Z).f;
        bool direction = json.GetField(MOVE_DIR).b;
        bool is_jump = json.GetField(IS_JUMP).b;
        bool is_crouch = json.GetField(IS_CLOUCH).b;

        TaskSystem.getInstance().addTask(() =>
        {
            Debug.Log("呼び出しあり");
            //場所情報などを挿入していく
            this.myCharacter.transform.position = new Vector3(position_x, position_y, position_z);
            this.myCharacter.transform.rotation = Quaternion.Euler(rotate_x, rotate_y, rotate_z);
            this.isJump = is_jump;
            this.isMoveDirectionLeft = direction;
            this.isClouch = is_crouch;
            this.last_data_owner_time = emit_time;
        });
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        // Read the inputs.
        //bool crouch = Input.GetKey(KeyCode.LeftControl);
        // float h = CrossPlatformInputManager.GetAxis("Horizontal");
        // Pass all parameters to the character control script.
        float h = this.isMoveDirectionLeft ? -1.0f : 1.0f;

        this.myCharacter.Move(h, this.isClouch, this.isJump);
        this.isJump = false;
    }
}