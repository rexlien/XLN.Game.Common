//using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLN.Game.Common;
using System;
using System.Runtime.InteropServices;
//using Pathfinding;


/***

   This timer class aims to step simulation on objects update according to the time elapsed with the same order.
    

**/
namespace XLN.Game.Common
{

    [GuidAttribute("16B48531-31AA-4BCE-9F32-AE26089BDFAC")]
    public class TimerService : IService
    {
        public interface ITimeObserver
        {
            void OnFixedTimeUpdateAI(float accuTime, float deltaTime);
            void OnFixedTimeUpdateObject(float accuTime, float deltaTime);
            void OnResolveCollision(float accuTime, float deltaTime);
            void OnUpdateVelocity(float accuTime, float deltaTime);
            //int GetObjectID();
            //Bounds GetBound();
        }


        private List<ITimeObserver> m_Observer = new List<ITimeObserver>();
        private List<ITimeObserver> m_ObserverToRemove = new List<ITimeObserver>();

        private float m_AccumulatedTime = 0;
        private float m_RemainingAIUpdateTime = 0;
        private float m_AIUpdateInterval = 0.1f; //intervel to update ai and behavior
        private float m_RemainingObjectUpdateTime = 0;
        private float m_ObjectUpdateInterval = 0.03f; //interval to update object properties 33 FPS

        private float m_SimulationInterval = 0.01f; //minimum simulation step, must be less than update intervales


        private void UpdateObserverObject(float delta)
        {

        }

        private void WaitPathfindingWorks()
        {
            //if (AstarPath.active != null)
            //    AstarPath.active.FlushWorkItems(false, true);
        }

        private void UpdateAI(float accuTime, float deltaTime)
        {
            foreach (ITimeObserver observer in m_Observer)
            {
                observer.OnFixedTimeUpdateAI(accuTime, deltaTime);
            }
        }

        private void UpdateObject(float accuTime, float deltaTime)
        {
            foreach (ITimeObserver observer in m_Observer)
            {
                observer.OnUpdateVelocity(accuTime, deltaTime);

            }

            foreach (ITimeObserver observer in m_Observer)
            {
                observer.OnResolveCollision(accuTime, deltaTime);

            }

            foreach (ITimeObserver observer in m_Observer)
            {
                //if (AstarPath.active != null)
                //    AstarPath.active.UpdateGraphs(new AStarGraphPreUpdateBound(observer.GetBound()));
                WaitPathfindingWorks();


                //  observer.GetBound());
            }
            foreach (ITimeObserver observer in m_Observer)
            {
                observer.OnFixedTimeUpdateObject(accuTime, deltaTime);
            }

            foreach (ITimeObserver observer in m_Observer)
            {
                //if (AstarPath.active != null)
                //    AstarPath.active.UpdateGraphs(new AStarGraphPostUpdateBound(observer.GetBound()));
                WaitPathfindingWorks();
            }
        }

        public void SubscribeTimer(ITimeObserver obj)
        {
            m_Observer.Add(obj);
        }

        public void UnSubscribeTimer(ITimeObserver obj)
        {
            m_ObserverToRemove.Add(obj);
            //m_Observer.Remove(obj);
        }

        public override bool OnInit()
        {
            return true;
        }

        public override bool OnDestroy()
        {
            return true;
        }

        public override bool OnUpdate(float delta)
        {
            foreach (ITimeObserver observer in m_ObserverToRemove)
            {
                m_Observer.Remove(observer);
            }
            m_ObserverToRemove.Clear();
            return true;
        }

        public override bool OnPostUpdate(float delta)
        {

            float curAccuTime = m_AccumulatedTime;
            m_AccumulatedTime += delta;

            while (delta >= m_SimulationInterval)
            {
                m_RemainingAIUpdateTime += m_SimulationInterval;
                m_RemainingObjectUpdateTime += m_SimulationInterval;
                curAccuTime += m_SimulationInterval;

                if (m_RemainingAIUpdateTime >= m_AIUpdateInterval)
                {
                    UpdateAI(curAccuTime, m_AIUpdateInterval);
                    WaitPathfindingWorks();
                    m_RemainingAIUpdateTime -= m_AIUpdateInterval;
                }
                if (m_RemainingObjectUpdateTime >= m_ObjectUpdateInterval)
                {
                    UpdateObject(curAccuTime, m_ObjectUpdateInterval);
                    WaitPathfindingWorks();
                    m_RemainingObjectUpdateTime -= m_ObjectUpdateInterval;
                }

                delta -= m_SimulationInterval;
            }

            //add remaining deltaTime;
            m_RemainingAIUpdateTime += delta;
            m_RemainingObjectUpdateTime += delta;

            /*
            while (m_RemainingTimeToUpdate >= m_UpdateInterval)
            {
                UpdateObserver(m_UpdateInterval);
                m_RemainingTimeToUpdate -= m_UpdateInterval;
            }
            */
            return true;
        }
    }

}