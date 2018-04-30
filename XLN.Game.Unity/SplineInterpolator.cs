using System;
using System.Collections.Generic;
using UnityEngine;

namespace XLN.Game.Unity
{
    

    public class SplineInterpolator : BaseBehavior
    {
        public enum EndPointsMode { AUTO, AUTOCLOSED, EXPLICIT }
        public enum WrapMode { ONCE, LOOP }
        public Action mOnEndCallback;

        private enum State
        {
            RESET,
            STOPPED,
            LOOP,
            ONCE
        }

        EndPointsMode mEndPointsMode = EndPointsMode.AUTO;

        internal class SplineNode
        {
            internal Vector3 Point;
            internal Quaternion Rot;
            internal float Time;
            //internal Vector2 EaseIO;

            internal SplineNode(Vector3 p, Quaternion q, float t) { Point = p; Rot = q; Time = t;  }
            internal SplineNode(SplineNode o) { Point = o.Point; Rot = o.Rot; Time = o.Time;  }
        }

        List<SplineNode> mNodes = new List<SplineNode>();
        State mState = State.RESET;
        bool mRotations;
        Rigidbody2D m_RigidBody2D;

        private float m_Tension = 0.1f;
        void Awake()
        {
            Reset();
        }

        void Start()
        {
            m_RigidBody2D = GetComponent<Rigidbody2D>();
        }

        public void StartInterpolation(Action endCallback, bool bRotations, WrapMode mode)
        {
          
            mState = mode == WrapMode.ONCE ? State.ONCE : State.LOOP;
            mRotations = bRotations;
            mOnEndCallback = endCallback;

            SetInput();
        }

        public void Reset()
        {
            mNodes.Clear();
            mState = State.RESET;
            mCurrentIdx = 1;
            mCurrentTime = 0;
            mRotations = false;
            mEndPointsMode = EndPointsMode.AUTO;
        }

        public void AddPoint(Vector3 pos, Quaternion quat, float timeInSeconds)
        {
          
            mNodes.Add(new SplineNode(pos, quat, timeInSeconds));
        }


        void SetInput()
        {
            if (mRotations)
            {
                for (int c = 1; c < mNodes.Count; c++)
                {
                    SplineNode node = mNodes[c];
                    SplineNode prevNode = mNodes[c - 1];

                    // Always interpolate using the shortest path -> Selective negation
                    if (Quaternion.Dot(node.Rot, prevNode.Rot) < 0)
                    {
                        node.Rot.x = -node.Rot.x;
                        node.Rot.y = -node.Rot.y;
                        node.Rot.z = -node.Rot.z;
                        node.Rot.w = -node.Rot.w;
                    }
                }
            }

            if (mEndPointsMode == EndPointsMode.AUTO)
            {
                mNodes.Insert(0, mNodes[0]);
                mNodes.Add(mNodes[mNodes.Count - 1]);
            }
          
        }

        void SetExplicitMode()
        {
            mEndPointsMode = EndPointsMode.EXPLICIT;
        }

        public void SetAutoCloseMode(float joiningPointTime)
        {

            mEndPointsMode = EndPointsMode.AUTOCLOSED;

            mNodes.Add(new SplineNode(mNodes[0] as SplineNode));
            mNodes[mNodes.Count - 1].Time = joiningPointTime;

            Vector3 vInitDir = (mNodes[1].Point - mNodes[0].Point).normalized;
            Vector3 vEndDir = (mNodes[mNodes.Count - 2].Point - mNodes[mNodes.Count - 1].Point).normalized;
            float firstLength = (mNodes[1].Point - mNodes[0].Point).magnitude;
            float lastLength = (mNodes[mNodes.Count - 2].Point - mNodes[mNodes.Count - 1].Point).magnitude;

            SplineNode firstNode = new SplineNode(mNodes[0] as SplineNode);
            firstNode.Point = mNodes[0].Point + vEndDir * firstLength;

            SplineNode lastNode = new SplineNode(mNodes[mNodes.Count - 1] as SplineNode);
            lastNode.Point = mNodes[0].Point + vInitDir * lastLength;

            mNodes.Insert(0, firstNode);
            mNodes.Add(lastNode);
        }

        float mCurrentTime;
        int mCurrentIdx = 1;

        public float Tension { get => m_Tension; set => m_Tension = value; }

        public void Update()
        {
            if (mState == State.RESET || mState == State.STOPPED || mNodes.Count < 4)
                return;

            mCurrentTime += Time.deltaTime;

            // We advance to next point in the path
            if (mCurrentTime >= mNodes[mCurrentIdx + 1].Time)
            {
                if (mCurrentIdx < mNodes.Count - 3)
                {
                    mCurrentIdx++;
                }
                else
                {
                    if (mState != State.LOOP)
                    {
                        mState = State.STOPPED;

                        // We stop right in the end point
                        transform.position = mNodes[mNodes.Count - 2].Point;

                        if (mRotations)
                            transform.rotation = mNodes[mNodes.Count - 2].Rot;

                        // We call back to inform that we are ended
                        if (mOnEndCallback != null)
                            mOnEndCallback();
                    }
                    else
                    {
                        mCurrentIdx = 1;
                        mCurrentTime = 0;
                    }
                }
            }

            if (mState != State.STOPPED)
            {
                // Calculates the t param between 0 and 1
                float param = (mCurrentTime - mNodes[mCurrentIdx].Time) / (mNodes[mCurrentIdx + 1].Time - mNodes[mCurrentIdx].Time);

                // Smooth the param
                //param = MathUtil.Ease(param, mNodes[mCurrentIdx].EaseIO.x, mNodes[mCurrentIdx].EaseIO.y);
                if (m_RigidBody2D != null)
                    m_RigidBody2D.MovePosition(GetHermiteInternal(mCurrentIdx, param));
                //transform.position = GetHermiteInternal(mCurrentIdx, param);

                if (mRotations)
                {
                    transform.rotation = GetSquad(mCurrentIdx, param);
                }
            }
        }

        Quaternion GetSquad(int idxFirstPoint, float t)
        {
            Quaternion Q0 = mNodes[idxFirstPoint - 1].Rot;
            Quaternion Q1 = mNodes[idxFirstPoint].Rot;
            Quaternion Q2 = mNodes[idxFirstPoint + 1].Rot;
            Quaternion Q3 = mNodes[idxFirstPoint + 2].Rot;

            Quaternion T1 = MathUtil.GetSquadIntermediate(Q0, Q1, Q2);
            Quaternion T2 = MathUtil.GetSquadIntermediate(Q1, Q2, Q3);

            return MathUtil.GetQuatSquad(t, Q1, Q2, T1, T2);
        }



        public Vector3 GetHermiteInternal(int idxFirstPoint, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            Vector3 P0 = mNodes[idxFirstPoint - 1].Point;
            Vector3 P1 = mNodes[idxFirstPoint].Point;
            Vector3 P2 = mNodes[idxFirstPoint + 1].Point;
            Vector3 P3 = mNodes[idxFirstPoint + 2].Point;

            Vector3 T1 = Tension * (P2 - P0);
            Vector3 T2 = Tension * (P3 - P1);

            float Blend1 = 2 * t3 - 3 * t2 + 1;
            float Blend2 = -2 * t3 + 3 * t2;
            float Blend3 = t3 - 2 * t2 + t;
            float Blend4 = t3 - t2;

            return Blend1 * P1 + Blend2 * P2 + Blend3 * T1 + Blend4 * T2;
        }


        public Vector3 GetHermiteAtTime(float timeParam)
        {
            if (timeParam >= mNodes[mNodes.Count - 2].Time)
                return mNodes[mNodes.Count - 2].Point;

            int c;
            for (c = 1; c < mNodes.Count - 2; c++)
            {
                if (mNodes[c].Time > timeParam)
                    break;
            }

            int idx = c - 1;
            float param = (timeParam - mNodes[idx].Time) / (mNodes[idx + 1].Time - mNodes[idx].Time);
            //param = MathUtil.Ease(param, mNodes[idx].EaseIO.x, mNodes[idx].EaseIO.y);

            return GetHermiteInternal(idx, param);
        }
    }
}
