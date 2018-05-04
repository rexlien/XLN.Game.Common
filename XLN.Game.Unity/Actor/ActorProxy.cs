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

		private void OnTransformParentChanged()
		{
            /*
            if (gameObject.transform.root != null)
            {
                m_RootGameObject = gameObject.transform.root.gameObject;
                var actorComp = m_RootGameObject.GetComponent<ActorComponent>();
                if (actorComp != null)
                {
                    Actor = m_RootGameObject.GetComponent<ActorComponent>().Actor;
                    return;
                }
            }

            m_RootGameObject = null;
            Actor = null;
            */
		}

		

		private void OnDisable()
		{
            //Actor = null;
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

        //private GameObject m_RootGameObject;
    }
}
