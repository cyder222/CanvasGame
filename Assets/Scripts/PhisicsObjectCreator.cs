using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Assets.Utility;
using CanvasGame.ObjectCreator;

/// <summary>
/// ゲームで使うアイテムを生成するスクリプト
/// </summary>
public class PhisicsObjectCreator : MonoBehaviour {

    private bool m_enableCreator;
    public bool enableCreator
    {
        get { return m_enableCreator; }
        set { m_enableCreator = value; }
    }
    public Material material;
    private bool is_capturing = false;
    Vector3 start_mouse_ppsition, before_mouse_position, last_mouse_position,start_line_position;
    public  Canvas tool_bar;
    private GameObject createPrimitive(ObjectType type)
    {
        
        if ((type == ObjectType.LINE_GROUND) || (type == ObjectType.LINE_GROUND_RIGID))
        {
            GameObject gObject = new GameObject();
            float box_width = 0.4f;
            float collider_upper_cut = 0.4f;
            Vector3 point1 = new Vector3(0.0f, -1 * box_width);
            Vector3 point2 = new Vector3(0.0f, box_width);
            Vector3 point3 = new Vector3(1.0f, box_width);
            Vector3 point4 = new Vector3(1.0f, -1 * box_width);

            Mesh mesh = creatingMesh(point1, point2, point3, point4);
            
            MeshFilter filter = gObject.AddComponent<MeshFilter>();
            MeshRenderer render = gObject.AddComponent<MeshRenderer>();
            filter.mesh = mesh;
            BoxCollider2D box_collider = gObject.gameObject.AddComponent<BoxCollider2D>();
            box_collider.transform.position = new Vector3(box_collider.transform.position.x, box_collider.transform.position.y - collider_upper_cut);
            box_collider.size = box_collider.size - new Vector2(0.1f, collider_upper_cut);
            render.material = material;
            if (type == ObjectType.LINE_GROUND_RIGID)
                gObject.gameObject.AddComponent<Rigidbody2D>();
            return gObject;
        }
        else if(type==ObjectType.UP_VELOCITY)
        {
            GameObject gObject = Instantiate(Resources.Load("Prefab/CreatingObjects/UpObject/UpVelocityObject"), Vector3.zero, Quaternion.identity) as GameObject;
     
            return gObject;
        }
        else
        {
            return null;
        }
    }
    void Awake()
    {
        ObjectCreatorToolManager.Init(new ObjectCreatorSinglePlay(), this.gameObject);
    }
    // Use this for initialization
    void Start() {
        /**
         *　使うオブジェクトをプールしておく。
         **/
        foreach (ObjectType object_type in Enum.GetValues(typeof(ObjectType)) )
        {
            Debug.Log("ObjectTYpe = "+object_type.ToString());
            ObjectPoolManager.getInstance().createNewObjectPool(object_type.ToString(), 100, () =>
            {
                GameObject gObject =  createPrimitive(object_type);
                if (gObject != null)
                {
                    gObject.name = object_type.ToString();
                }
                return gObject;
            });
        }
        this.m_enableCreator = true;
        
    }

    // Update is called once per frame
    void Update() {
        IObjectCreatorTool tool = ObjectCreatorToolManager.getInstance().getCurrentTool();
        
        Vector3 viewport = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        bool is_not_on_toolbar = (viewport.x > 0.05f);
        if (is_not_on_toolbar && m_enableCreator)
        {
            if (Input.GetMouseButtonDown(0))
            {
               tool.OnControlDown(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                tool.OnControlUp(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                tool.OnControlOn(Input.mousePosition);
            }
            else
            {
                tool.OnControlDefault(Input.mousePosition);
            }
        }
    }
    Mesh creatingMesh(Vector3 point1,Vector3 point2,Vector3 point3,Vector3 point4)
    {
        Mesh mesh = new Mesh();

        // 頂点の指定
        mesh.vertices = new Vector3[] {
        point1,
        point2,
        point3,
        point4,
        };
        // UV座標の指定
        mesh.uv = new Vector2[] {
        new Vector2(0, 0),
        new Vector2(0,1),
        new Vector2(1, 1),
        new Vector2(1 ,0),
        };
        // 頂点インデックスの指定
        mesh.triangles = new int[] {
        0, 1, 2,
        0, 2, 3,
        };

        mesh.RecalculateNormals();
        
        mesh.RecalculateBounds();
        return mesh;
      
    }


}
