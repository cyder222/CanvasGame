using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.innerState
{

    public class AnimatedBaseState : MonoBehaviour, IAnimatedGameState
    {
        [SerializeField]
        Animator[] m_animatores;
        [SerializeField]
        protected GameObject toolCanvas;
        private List<Animator> m_listAnimatores;
        void Awake()
        {
            InitializeList();
        }
        protected void InitializeList()
        {
            if (m_animatores.Length > 0)
                m_listAnimatores = new List<Animator>(m_animatores);
            else
                m_listAnimatores = new List<Animator>();
        }
        // Use this for initialization
        void Start()
        {

        }
        // Update is called once per frame
        void Update()
        {

        }
        public virtual void prepareEndState()
        {

        }
        public virtual void prepareBeginState()
        {

        }
        public virtual void endState()
        {

        }
        public virtual void beginState()
        {

        }
        public List<Animator> getAnimatorList()
        {
            return this.m_listAnimatores;
        }
        public virtual AnimatedStateType getStateType()
        {
            return AnimatedStateType.BASE;
        }

    }
}