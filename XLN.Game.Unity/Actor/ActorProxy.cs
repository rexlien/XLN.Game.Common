using System;
using UnityEngine;
using XLN.Game.Common;

namespace XLN.Game.Unity.Actor
{
    public class ActorProxy : BaseBehavior
    {
        public ActorProxy()
        {
            
        }

        void Start()
        {
           
            //Actor proxy gameobject must have parent
            m_RootGameObject = gameObject.transform.root.gameObject;
            Actor = m_RootGameObject.GetComponent<ActorComponent>().Actor;

        }


        private void OnTriggerEnter(UnityEngine.Collider other)
        {
            BaseActor collidee = null;
            ActorProxy proxy = other.gameObject.GetComponent<ActorProxy>();
            if(proxy)
            {
                collidee = proxy.Actor;
            }
            else
            {
                ActorComponent component = other.gameObject.transform.root.gameObject.GetComponent<ActorComponent>();
                collidee = component.Actor;
            }

            Actor.EnterIntercept(collidee);
        }

        private void OnTriggerExit(UnityEngine.Collider other)
        {
            BaseActor collidee = null;
            ActorProxy proxy = other.gameObject.GetComponent<ActorProxy>();
            if (proxy)
            {
                collidee = proxy.Actor;
            }
            else
            {
                ActorComponent component = other.gameObject.transform.root.gameObject.GetComponent<ActorComponent>();
                collidee = component.Actor;
            }

            Actor.LeaveIntercept(collidee);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            BaseActor collidee = null;
            ActorProxy proxy = collision.gameObject.GetComponent<ActorProxy>();
            if (proxy)
            {
                collidee = proxy.Actor;
            }
            else
            {
                ActorComponent component = collision.gameObject.transform.root.gameObject.GetComponent<ActorComponent>();
                if (component)
                {
                    collidee = component.Actor;
                }
            }
            if(collidee != null)
                Actor.EnterIntercept(collidee);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            BaseActor collidee = null;
            ActorProxy proxy = collision.gameObject.GetComponent<ActorProxy>();
            if (proxy)
            {
                collidee = proxy.Actor;
            }
            else
            {
                ActorComponent component = collision.gameObject.transform.root.gameObject.GetComponent<ActorComponent>();
                if (component)
                    collidee = component.Actor;
            }

            if(collidee != null)
                Actor.LeaveIntercept(collidee);
        }

        private GameObject m_RootGameObject;
    }
}
