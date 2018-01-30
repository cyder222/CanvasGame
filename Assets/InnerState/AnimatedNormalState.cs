using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.innerState
{
    public class AnimatedNormalState : AnimatedBaseState
    {
        [SerializeField]
        Pausable pausable_object;
        [SerializeField]
        private PhisicsObjectCreator creator;


        void Awake()
        {
            InitializeList();
        }
        // Use this for initialization
        void Start()
        {

        }
        // Update is called once per frame
        void Update()
        {

        }
        public override void prepareEndState()
        {
            Debug.Log("prepareEndState");
        }
        public override void prepareBeginState()
        {
            Initializer.getInstance().InitializeAll();
            pausable_object.OnPause();
            ScoreModel.getInstance().StopTimer();
            this.toolCanvas.GetComponent<Canvas>().enabled = false;
            BgmManager.Instance.Play("bo-ken");
        }
        public override void endState()
        {
			Application.CaptureScreenshot(GlobalConstantValue.ScreenShotImageName);
        }
        public override void beginState()
        {
            pausable_object.OnResume();
            ScoreModel.getInstance().StartTimer();
            creator.enableCreator = true;
            this.toolCanvas.GetComponent<Canvas>().enabled = true;
        }
        public override AnimatedStateType getStateType()
        {
            return AnimatedStateType.NORMAL;
        }
    }
}