using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SocketIO;
using Assets.Utility;

namespace CanvasGame.ObjectCreator
{
    public enum ObjectType
    {
        LINE_GROUND, //通常の地面、
                     // HEAVY_CUBE_KINEMATIC, //動かない四角形の重り
        UP_VELOCITY, //上に加速度を与える
        //RIGHT_VELOCITY, //右に加速度を与える
        //LEFT_VELOCITY, //左に加速度を加える
        LINE_GROUND_RIGID, //重力に影響を受ける地面
        //HEAVY_RIGID,      //重力に影響を受ける重り
        GIVE_UP
    };

    /**
     * 作成するオブジェクトのあたり判定に着けるマテリアル
     **/
    public enum ObjectColliderMaterial
    {
        NONE, BOUNCE, HEAVY_FRICTION,
    };
    //
    // ゲームのプレイヤーが使うオブジェクト作成ツールの共通インターフェース
    // 
    public interface IObjectCreatorTool
    {
        ObjectType getType(); //オブジェクトのタイプを返す
        void OnControlDown(Vector3 down_position);//コントロールが押されたときの処理
        void OnControlOn(Vector3 control_position);//コントロールが押されているときの処理
        void OnControlUp(Vector3 up_position);//コントロールが上がった時の処理
        void OnControlDefault(Vector3 control_position);//通常状態の処理
    }
    public abstract class ObjectCreatorTool
    {
        protected IObjectCreatorInterface creator_interface;

        public ObjectCreatorTool(IObjectCreatorInterface creator_interface)
        {
            this.creator_interface = creator_interface;
        }
    }
    /**
     * <summary>
        オブジェクトツールのマネージャークラス
        ○ これを用いて、現在のツールを取得する。加えて、現在のツールを取得したりインスタンスを取得したりする。
        ○ シングルトンだが、引数もあるため、最初にInit関数を実行する
        ○ 利用するオブジェクトは、OnChangeイベントハンドラにデリゲートを追加して、変更を伝えてもらう。
                  利用例 : Tool変更時にアニメーショントリガを実行(アニメーションを実行するオブジェクト上で)
                                OnChange+=(sender,event)=>{ this.GetComponent<Animator>().setTrigger("changeTool");   }
        </summary>

     **/
    public class ObjectCreatorToolManager
    {
        private static ObjectCreatorToolManager __instance = null;
        public EventHandler OnChange;
        private IObjectCreatorTool current_tool;
        public Dictionary<ObjectType, IObjectCreatorTool> tools;
        private ObjectCreatorToolManager(IObjectCreatorInterface creator_interface,GameObject parent_object)
        {
            tools = new Dictionary<ObjectType, IObjectCreatorTool>();
            OnChange = (e,s) => { };
            tools.Add(ObjectType.LINE_GROUND, new GroundLineTool(creator_interface,parent_object));
            tools.Add(ObjectType.UP_VELOCITY, new UpArrowTools(creator_interface));
            tools.TryGetValue(ObjectType.LINE_GROUND, out this.current_tool);
        }
        public IObjectCreatorTool getCurrentTool()
        {
            return current_tool;
        }
        public static ObjectCreatorToolManager getInstance()
        {
            if (__instance == null)
                throw new Exception("Please initialize tool manager first");
            return __instance;
        }
        public static void Init(IObjectCreatorInterface creator_interface,GameObject parent_object)
        {
                __instance = new ObjectCreatorToolManager(creator_interface, parent_object);
        }
        public void changeTool(IObjectCreatorTool to)
        {
            current_tool = to;
            OnChange(this, EventArgs.Empty);
        }
        public void changeTool(ObjectType type)
        {
            tools.TryGetValue(type, out current_tool);
            OnChange(this, EventArgs.Empty);
        }
    }
    /**
     * 地面を作成するラインツール
     **/
    public class GroundLineTool:ObjectCreatorTool,IObjectCreatorTool{
        private Vector3 start_contoler_position;
        private Vector3 start_line_position;
        private Vector3 before_control_position;
        private Vector3 last_mouse_position;
        private bool is_capturing = false;
        private GameObject parent_object;
      
        public GroundLineTool(IObjectCreatorInterface creator_interface,GameObject parent_object):base(creator_interface)
        {
            this.parent_object = parent_object;
        }
        public ObjectType getType()
        {
            return ObjectType.LINE_GROUND;
        }
        public void OnControlDown(Vector3 down_position)
        {
            is_capturing = true;
            start_contoler_position = down_position;
            start_contoler_position.z -= Camera.main.transform.position.z;
            start_line_position = Camera.main.ScreenToWorldPoint(start_contoler_position);
            before_control_position = down_position;
        }
        public void OnControlOn(Vector3 control_position)
        {
            if (is_capturing)
            {
                TaskSystem.getInstance().addTask(() =>
                {
                    LineRenderer renderer = parent_object.GetComponent<LineRenderer>();
                    renderer.enabled = true;

                    // 線の幅
                    renderer.SetWidth(0.1f, 0.1f);
                    // 頂点の数
                    renderer.SetVertexCount(2);


                    // 頂点を設定
                    last_mouse_position = control_position;
                    last_mouse_position.z -= Camera.main.transform.position.z;
                    //Vector3 line_start_pos = Camera.main.ScreenToWorldPoint(start_mouse_ppsition);
                    //Vector3 line_start_pos = Camera.main.ScreenToWorldPoint(start_mouse_ppsition);
                    Vector3 line_end_pos = Camera.main.ScreenToWorldPoint(last_mouse_position);
                    renderer.SetPosition(0, start_line_position);
                    renderer.SetPosition(1, line_end_pos);
                });
            }

        }
        public void OnControlUp(Vector3 up_position)
        {
            is_capturing = false;
            Vector3 line_start_pos = start_line_position;
            Vector3 line_end_pos = Camera.main.ScreenToWorldPoint(last_mouse_position);

            //もし、ドラッグの最初のポジションが終了点より大きい時反転
            //理由は、回転しすぎて、テクスチャの向きがおかしくなるため。
            if (line_start_pos.x > line_end_pos.x)
            {
                Vector3 tmp;
                tmp = line_end_pos;
                line_end_pos = line_start_pos;
                line_start_pos = tmp;
            }
            ///////////////////////////////////////////////////////////////////////////////////
            //オブジェクトの生成
            ///////////////////////////////////////////////////////////////////////////////////
            Vector3 line = line_end_pos - line_start_pos;
            float line_mag = line.magnitude;//線分の長さ xを増やす
            float line_angle = Mathf.Atan2(line_end_pos.y - line_start_pos.y, line_end_pos.x - line_start_pos.x);
            creator_interface.create(line_start_pos, new Vector3(0, 0, line_angle*Mathf.Rad2Deg), new Vector3(line_mag, 1, 1), this.getType());
             LineRenderer renderer = parent_object.GetComponent<LineRenderer>();
             renderer.enabled = false;
        }
        public void OnControlDefault(Vector3 control_position)
        {

        }
       
    }
    /**
     * ジャンプするオブジェクトを作成するツール
     **/
    public class UpArrowTools : ObjectCreatorTool, IObjectCreatorTool
    {
        private Vector3 start_contoler_position;
        public UpArrowTools(IObjectCreatorInterface creator_interface):base(creator_interface)
        {

        }
        public ObjectType getType()
        {
            return ObjectType.UP_VELOCITY;
        }
        public void OnControlDown(Vector3 down_position)
        {
            start_contoler_position = down_position;
            start_contoler_position.z -= Camera.main.transform.position.z;
            Vector3 start_line_position = Camera.main.ScreenToWorldPoint(start_contoler_position);
            this.creator_interface.create(start_line_position, Vector3.zero, new Vector3(1, 1, 1), this.getType());
        }
        public void OnControlOn(Vector3 control_position)
        {

        }
        public void OnControlUp(Vector3 up_position)
        {
           
        }
        public void OnControlDefault(Vector3 control_position)
        {

        }
        
    }
    public class ObjectCreatorFunctions
    {
        public static void ObjectCreator(Vector3 position, Quaternion rotate, Vector3 scale, ObjectType object_type, ObjectColliderMaterial material = ObjectColliderMaterial.NONE,Action<GameObject> callback=null)
        {
            Debug.Log("ObjectCreator" + position.x);
            TaskSystem.getInstance().addTask(
                () =>
                {
                    Debug.Log("task_system" + position.x);
                    GameObject game_object = ObjectPoolManager.getInstance().getPool(object_type.ToString()).getNewObject();
                    game_object.transform.position = position;
                    game_object.transform.rotation = rotate;
                    game_object.transform.localScale = scale;
                    
                    game_object.SetActive(true);
                    switch (material)
                    {
                        case ObjectColliderMaterial.NONE:
                            break;
                        default:
                            break;
                    }
                    if (callback != null)
                        callback(game_object);
                });
            
        }
    }

    /// <summary>
    /// オブジェクトを作る関数インターフェース
    /// このインターフェースを切り替えて、ネットプレイ、シングルプレイ、また、対戦プレイなどの際のオブジェクト作成処理を切り替えていく
    /// </summary>
    public interface IObjectCreatorInterface
    {
        void create(Vector3 position, Vector3 rotate, Vector3 scale, ObjectType object_type, ObjectColliderMaterial material = ObjectColliderMaterial.NONE);
    }

   /// <summary>
   /// シングルプレイヤー用のオブジェクト
   /// </summary>
    public class ObjectCreatorSinglePlay : IObjectCreatorInterface
    {
      public void create(Vector3 position, Vector3 rotate, Vector3 scale, ObjectType object_type, ObjectColliderMaterial material = ObjectColliderMaterial.NONE)
      {
            ObjectCreatorFunctions.ObjectCreator(position, Quaternion.AngleAxis(rotate.z,new Vector3(0,0,1)), scale, object_type);   
      }

    }
    /// <summary>
    /// ネットワークプレイ用のオブジェクト
    /// </summary>
    public class ObjectCreatorNetworkPlay : IObjectCreatorInterface
    {
        private static SocketIOComponent socket = null;
        
        readonly static string POSITION_X = "x";
        readonly static string POSITION_Y = "y";
        readonly static string POSITION_Z = "z";
        readonly static string ROTATE_ANGLE = "ang";
        readonly static string SCALE_X = "sx";
        readonly static string SCALE_Y = "sy";
        readonly static string SCALE_Z = "sz";
        readonly static string TYPE_OBJECT = "TO";
        readonly static string NETWORK_ID = "NI";
        Dictionary<string,GameObject> object_network_ids;
        public ObjectCreatorNetworkPlay()
        {
            if (socket == null)
            {
                try {
                    GameObject go = GameObject.Find("SocketIO");
                    socket = go.GetComponent<SocketIOComponent>();
                    socket.On("newobject", OnNewObject);
                }catch(Exception e)
                {
                    Debug.Log("Cannot found server interface program!");
                    
                }
                object_network_ids = new Dictionary<string, GameObject>();
            }
        }
        public void OnNewObject(SocketIOEvent e)
        {

            JSONObject obj = e.data;
            Debug.Log(obj.ToString());
            ObjectType type = (ObjectType)Enum.Parse(typeof(ObjectType), obj.GetField(TYPE_OBJECT).str);
            float position_x = obj.GetField(POSITION_X).f;
            //Debug.Log(position_x);
            float position_y = obj.GetField(POSITION_Y).f;
            float position_z = obj.GetField(POSITION_Z).f;
            float rotate_angle = obj.GetField(ROTATE_ANGLE).f;
            float scale_x = obj.GetField(SCALE_X).f;
            float scale_y = obj.GetField(SCALE_Y).f;
            float scale_z = obj.GetField(SCALE_Z).f;
            string network_id = obj.GetField(NETWORK_ID).str;
            
            //object_network_ids.Add(network_id,game)
            ObjectCreatorFunctions.ObjectCreator(new Vector3(position_x, position_y, position_z), Quaternion.AngleAxis(rotate_angle, new Vector3(0, 0, 1))
                , new Vector3(scale_x, scale_y, scale_z), type,ObjectColliderMaterial.NONE,
                (game_object)=> {
                    object_network_ids.Add(network_id, game_object);
                });
           
           
        }
        public void create(Vector3 position, Vector3 rotate, Vector3 scale, ObjectType object_type, ObjectColliderMaterial material = ObjectColliderMaterial.NONE)
        {
            JSONObject jsonObject = new JSONObject(JSONObject.Type.OBJECT);
            jsonObject.AddField(POSITION_X, position.x);
            jsonObject.AddField(POSITION_Y, position.y);
            jsonObject.AddField(POSITION_Z, position.z);
            jsonObject.AddField(ROTATE_ANGLE, rotate.z);
            jsonObject.AddField(SCALE_X, scale.x);
            jsonObject.AddField(SCALE_Y, scale.y);
            jsonObject.AddField(SCALE_Z, scale.z);
            jsonObject.AddField(TYPE_OBJECT, object_type.ToString());
           
            socket.Emit("newobject", jsonObject);   
        }
    }
};