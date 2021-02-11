using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace ProductionPipeline
{
    public abstract class Source : MonoBehaviour
    {
        public enum SourceType 
        {
            Base,
            Body,
            Detail,
            BaseBase,
            BaseBaseBody,
            BaseBaseBodyDetail
        }

        [SerializeField]
        protected SourceType _sourceType;

        protected float _width, _height, _depth;

        public event EventHandler EndOfConveyor;
        protected virtual void OnIsArrived()
        {
            EventHandler handler = EndOfConveyor;
            handler?.Invoke(this, null);
        }

        public abstract SourceType GetSourceType();

        public float GetWidth()
        {
            return _width;
        }
        public float GetHeight()
        {
            return _height;
        }
        public float GetDepth()
        {
            return _depth;
        }

        public void Move(Vector3[] conveyorWaypoints, float duration)
        {
            transform.DOPath(conveyorWaypoints, duration)
                .SetEase(Ease.Linear)
                .SetLookAt(0.05f)
                .OnComplete(() => OnIsArrived());
        }

        //public void Move(Vector3 fromPosition, Vector3 toPosition, float velocity)
        //{
        //    StartCoroutine(MoveCoroutine(fromPosition, toPosition, velocity));
        //}

        //// TODO: PER ADESSO IL PARAMETRO VELOCITY E' DEL TUTTO INUTILE.
        //// LE SOURCES DOVRANNO MUOVERSI SU SPLINE... PER ORA FACCIAMO CHE ARRIVANO A DESTINAZIONE IN QUALCHE MODO.
        //IEnumerator MoveCoroutine(Vector3 fromPosition, Vector3 toPosition, float velocity)
        //{
        //    while (transform.position != toPosition)
        //    {
        //        transform.position = Vector3.MoveTowards(transform.position, toPosition, velocity * Time.deltaTime);
        //        yield return new WaitForEndOfFrame();
        //    }
        //    OnIsArrived();
        //    yield return null;
        //}
    }

}
