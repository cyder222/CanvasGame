using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using CanvasGame.ObjectCreator;
public class ToolBarContoller : MonoBehaviour {

    List<Button> m_toolBarButton;
	// Use this for initialization
	void Start () {
        m_toolBarButton = new List<Button>();   
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
    public void Init()
    {
        m_toolBarButton = new List<Button>();
    }
    public void addToolBarButton(GameObject button)
    {
        m_toolBarButton.Add(button.GetComponent<Button>());
    }
    public void enableAllButton()
    {
        foreach(Button button in m_toolBarButton)
        {
            button.enabled = true;
        }
    }
    public void disableAllButton()
    {
        foreach(Button button in m_toolBarButton)
        {
            button.enabled = false;
        }
    }
    public void OnClick(string tool_name)
    {
        Debug.Log("OnClick : ToolBarControler");
        ObjectType type = (ObjectType)Enum.Parse(typeof(ObjectType), tool_name);
        ObjectCreatorToolManager.getInstance().changeTool(type);
    }
}
