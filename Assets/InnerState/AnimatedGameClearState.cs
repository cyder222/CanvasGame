using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.innerState
{
    public class AnimatedGameClearState : AnimatedBaseState
    {
        [SerializeField] private PanelMenuScript panel;
        [SerializeField] private PhisicsObjectCreator creator;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void prepareBeginState()
        {
            BgmManager.Instance.Play("bgm_gameclear");
        }

        public override void beginState()
        {
            panel.PauseGame();
            toolCanvas.GetComponent<Canvas>().enabled = false;
            creator.enableCreator = false;
        }

        public override void endState()
        {
            panel.UnpauseGame();
        }

    }
}