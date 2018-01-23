using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SocketIO;
using System.Threading;
using Assets.Utility;
using UnityStandardAssets._2D;
namespace Assets.Network
{

    class CharactorInfoEmitter
    {

        //以下ネットワーク通信用の処理系
        readonly static string POSITION_X = "x";
        readonly static string POSITION_Y = "y";
        readonly static string POSITION_Z = "z";
        readonly static string ROTATE_X = "rx";
        readonly static string ROTATE_Y = "ry";
        readonly static string ROTATE_Z = "rz";
        readonly static string SCALE_X = "sx";
        readonly static string SCALE_Y = "sy";
        readonly static string SCALE_Z = "sz";
        readonly static string IS_JUMP = "J";
        readonly static string MOVE_DIR = "MD";
        readonly static string IS_CLOUCH = "C";
        readonly static string EMIT_TIME = "ET";
        readonly static string OWNER_ID = "OI";

        private static SocketIOComponent socket = null;
        
        private CharactorControl m_CharacterControl;
        private bool start;
        private bool emit_wait;
        private Thread emit_thread;
        private Mutex emit_mutex;
        private JSONObject jsonObject = new JSONObject(JSONObject.Type.OBJECT);
        public CharactorInfoEmitter(CharactorControl emit_control)
        {
            m_CharacterControl = emit_control;
            emit_mutex = new Mutex();
            start = false;
            emit_wait = false;
            if (socket == null)
            {
                try
                {
                    GameObject go = GameObject.Find("SocketIO");
                    socket = go.GetComponent<SocketIOComponent>();
                }
                catch (Exception e)
                {
                    Debug.Log("Cannot found server interface program!"+e.Message);

                }
            }
           
            
            emit_thread = new Thread(new ThreadStart(()=> {
                lock(emit_mutex)
                {
                    while (true)
                    {
                        while (!start)
                        {
                            Monitor.Wait(emit_mutex);
                        }
                        
                        TaskSystem.getInstance().addTask(() =>
                        {
                            EmitData(m_CharacterControl.transform, m_CharacterControl.getJump(), m_CharacterControl.getMoveDirection(),
                                 m_CharacterControl.getClouch(), UnityEngine.Time.realtimeSinceStartup);
                        });
                        emit_wait = true;
                        while(emit_wait)
                        {
                            Monitor.Wait(emit_mutex);
                            
                        }
                    }
                }
            }));
            emit_thread.Start();
        }
        public void StartEmit()
        {
            lock(emit_mutex)
            {
                start = true;
                Monitor.Pulse(emit_mutex);
            }
        }
        public void StopEmit()
        {
            this.start = false;
        }
        public void EmitData(Transform transform,bool is_jump,bool move_left,bool is_clouch,float emit_time)
        {
           
            Vector3 position = transform.position;
            Quaternion rotate = transform.rotation;
            Vector3 scale = transform.localScale;
           
            jsonObject.Clear();
            jsonObject.AddField(POSITION_X, position.x);
            jsonObject.AddField(POSITION_Y, position.y);
            jsonObject.AddField(POSITION_Z, position.z);
            jsonObject.AddField(ROTATE_X, rotate.x);
            jsonObject.AddField(ROTATE_Y, rotate.y);
            jsonObject.AddField(ROTATE_Z, rotate.z);
            jsonObject.AddField(SCALE_X, scale.x);
            jsonObject.AddField(SCALE_Y, scale.y);
            jsonObject.AddField(SCALE_Z, scale.z);
            jsonObject.AddField(MOVE_DIR,move_left);
            jsonObject.AddField(IS_JUMP, is_jump);
            jsonObject.AddField(IS_CLOUCH, is_clouch);
            jsonObject.AddField(EMIT_TIME, emit_time);
            jsonObject.AddField(OWNER_ID, socket.sid);

            socket.Emit("CharactorUpdate", jsonObject);
            lock(emit_mutex)
            {
                emit_wait = false;
                Monitor.Pulse(emit_mutex);
            }
        }
        public void OnSendComplete(SocketIOEvent e)
        {

        }

    }
}
