using UnityEngine;
using System;
using XLN.Game.Common;
using XLN.Game.Common.Actor;
using XLN.Game.Unity.Extension;
using XLN.Game.Common.Utils;

namespace XLN.Game.Unity.Actor
{
    public class UnityActor : BaseActor
    {
        public UnityActor()
            :base()
        {
            m_GameObject = new GameObject();
            m_TransformComponent = AddComponent<TransformComponent>();
            m_GameObject.name = GetType().ToString() + this.ID.ToString();
        }

        public UnityActor(GameObject gameObject)
            :base()
        {
            m_GameObject = gameObject;
            BaseBehavior[] baseBehaviors = m_GameObject.GetComponents<BaseBehavior>();
            foreach(BaseBehavior behavior in baseBehaviors)
            {
                behavior.Actor = this;
            }
            m_TransformComponent = AddComponent<TransformComponent>();
        }

       
        public override void OnDestroy()
        {
            base.OnDestroy();
            UnityEngine.Object.Destroy(m_GameObject);
        }


        public override void OnCreated()
        {
            base.OnCreated();
            ActorComponent behavior = AddComponent<ActorComponent>();
          

        }

        protected override T _CreateComponent<T>(params object[] param)
        {
            
            if(typeof(T).IsSubclassOf(typeof(BaseBehavior)))
            {
                T behavior = (T)(object)m_GameObject.AddComponent(typeof(T));//AddComponent<T>();
                return behavior;
            }
            else
            {
                return base._CreateComponent<T>(param);
            }

        }

        protected override IComponent _CreateComponent(string componentType, params object[] param)
        {
            Type t = ClassUtils.GetType(componentType);
            if(t.IsSubclassOf(typeof(BaseBehavior)))
            {
                IComponent behavior = (IComponent)m_GameObject.AddComponent(t);
                return behavior; 
            }
                      
            return ClassUtils.CreateInstance<IComponent>(componentType, param);
        }

        public virtual T AddMonoComponent<T>() where T : Component
        {
            
            return m_GameObject.AddComponent<T>();
        }

        public override T GetComponent<T>()
        {
            if (typeof(IComponent).IsAssignableFrom(typeof(T)))
            {
                return base.GetComponent<T>();
            }

            return m_GameObject.GetComponent<T>();

        }

        //private UnityEngine.Vector3 m_UnityPos;
        //private UnityEngine.Quaternion m_UnityRot;
        //private UnityEngine.Vector3 m_UnityScale;

        protected GameObject m_GameObject;// = new GameObject();

        public GameObject GameObject { get => m_GameObject; set => m_GameObject = value; }
        public Vector3 UnityPos { 
            get 
            {
                return m_GameObject.transform.position;
            }
            set
            {
                m_GameObject.transform.position = value;
            }
        }
        public Quaternion UnityRot
        {
            get
            {
                return m_GameObject.transform.rotation;
            }

            set
            {
                m_GameObject.transform.rotation = value;
            }
        }
        public Vector3 UnityScale 
        {
            get => m_GameObject.transform.localScale;
            set => m_GameObject.transform.localScale = value; 
        }


        TransformComponent m_TransformComponent;
    }
}
