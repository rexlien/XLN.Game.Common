using System;
using System.Runtime.InteropServices;
using Game.Common.Unity;
using UnityEngine;
using XLN.Game.Common;
using XLN.Game.Common.Actor;

namespace XLN.Game.Unity.Actor
{
    [GuidAttribute("658D9007-2A36-4836-A3E2-0901838D6CC8")]
    [BaseType(typeof(RenderComponent))]
    public class RenderComponent : BaseComponent
    {
        public RenderComponent(string prefabName)
        {
            m_PrefabName = prefabName;
        }

        private string m_PrefabName;
        public string PrefabName
        {
            get
            {
                return m_PrefabName;
            }

            set
            {
                m_PrefabName = value;
            }
        }

        public Vector3 Scale
        {
            get
            {
                return m_Scale;
            }

            set
            {
                m_Scale = value;
                if (m_RenderObject)
                {
                    m_RenderObject.transform.localScale = m_Scale;
                }
            }
        }

        private Vector3 m_Scale;

        public override void Init()
        {
            base.Init();
            m_GOPoolService = ServiceMgr.GetServiceMgr().GetService<GameObjectPoolService>();
            m_ResService = ServiceMgr.GetServiceMgr().GetService<ResourceService>();
            UnityActor unityActor = (UnityActor)Actor;
            m_GameObject = unityActor.GameObject;

            UpdateRenderObject();

        }

        public override void Destroy()
        {
            base.Destroy();
            m_Destroyed = true;
            if (m_RenderObject)
            {

                m_GOPoolService.Return(m_RenderObject);
            }
        }

        private async void UpdateRenderObject()
        {
            if (m_ResService == null)
                return;

            if (m_GOPoolService == null)
                return;

            m_Prototype = await m_ResService.GetAsync<GameObject>(new ResourcePath(ResourcePath.PathType.Resource, m_PrefabName));
            if (m_Prototype == null)
            {
                Debug.LogError("prefab: " + m_PrefabName + "load failed");
                return;
            }
            if (m_Destroyed)
            {
                return;
            }
            GameObjectPoolService.AquireResult res = m_GOPoolService.Acquire(m_Prototype);

            m_RenderObject = res.GameObject;
            m_RenderObject.transform.localScale = m_Scale;

            if (!res.Pooled)
                m_RenderObject.AddComponent<ActorProxy>();

            UnityActor unityActor = (UnityActor)(Actor);
            Utils.Attach(unityActor.GameObject, m_RenderObject);


        }



        protected GameObject m_RenderObject;
        protected GameObject m_Prototype;
        private GameObjectPoolService m_GOPoolService;
        ResourceService m_ResService;
        private bool m_Destroyed = false;


		private GameObject m_GameObject;

        public GameObject GameObject { get => m_GameObject; set => m_GameObject = value; }


        public Vector3 UnityPos
        {
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


    }
}
