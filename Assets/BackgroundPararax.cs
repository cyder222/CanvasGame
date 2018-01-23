using UnityEngine;
using System.Collections;

/// <summary>
/// バックグラウンドテクスチャをカメラの動きに合わせてずらすことで動きを作り出す。
/// </summary>
public class BackgroundPararax : MonoBehaviour {
    public float scroll_speed = 20.0f;
    public float div_v = 1.0f;
    private Vector3 oldcampos;
    private Vector3 oldpos;
    [SerializeField]
    Camera main_camera;//メインカメラ
    // Use this for initialization
    void Start()
    {
        // 初期位置を覚えておく
        oldpos = transform.position;
        oldcampos = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // カメラ座標と同じにして追従させる
        Camera camera = main_camera;
        transform.localPosition = new Vector3(camera.transform.position.x+oldpos.x, camera.transform.position.y+oldpos.y, oldpos.z);
        // テクスチャオフセットをずらす量を算出
        float u = (camera.transform.position.x - oldcampos.x) / div_v;
        this.GetComponent<Renderer>().material.mainTextureOffset += new Vector2(u*Time.deltaTime*scroll_speed, 0);
        oldcampos = camera.transform.position;
    }
}
