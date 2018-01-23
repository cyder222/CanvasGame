using UnityEngine;
using System.Collections;
using CanvasGame.ObjectCreator;
using System;
public class CurrentTool : MonoBehaviour {

    void Awake()
    {
        //DontDestroyOnLoad(this);
    }
	// Use this for initialization
	void Start () {
        ObjectCreatorToolManager.getInstance().OnChange += OnChangeTool;
	}
	void OnChangeTool(object sender,EventArgs e)
    {
        Debug.Log("OnChange : CurrentTool");
        ObjectType current_type = ObjectCreatorToolManager.getInstance().getCurrentTool().getType();
        if (current_type == ObjectType.UP_VELOCITY)
            this.GetComponent<Animator>().SetTrigger("UpButton");
        else if (current_type == ObjectType.LINE_GROUND)
            this.GetComponent<Animator>().SetTrigger("LineGroundTool");
    }
	// Update is called once per frame
	void Update () {
	   
	}
}
