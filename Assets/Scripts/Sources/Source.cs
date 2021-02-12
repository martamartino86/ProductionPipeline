using System;
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

        // The dimensions are set differently for the type of sources:
        // basic sources set their dimensions from the mesh;
        // assembled sources set their dimensions from the basic sources they are composed of
        public float Width { get; protected set; }
        public float Height { get; protected set; }
        public float Depth { get; protected set; }

        private MeshRenderer[] _renderers;

        public event EventHandler EndOfConveyor;
        protected virtual void OnIsArrived()
        {
            EventHandler handler = EndOfConveyor;
            handler?.Invoke(this, null);
        }

        public abstract SourceType GetSourceType();

        private void Awake()
        {
            _renderers = GetComponentsInChildren<MeshRenderer>();
        }

        public void Move(Vector3[] conveyorWaypoints, float duration)
        {
            transform.DOPath(conveyorWaypoints, duration)
                .SetOptions(false, AxisConstraint.None, AxisConstraint.X | AxisConstraint.Z)
                .SetEase(Ease.Linear)
                .SetLookAt(0.05f)
                .OnComplete(() => OnIsArrived());
        }

        public void Show(bool v)
        {
            foreach (var r in _renderers)
            {
                r.enabled = v;
            }
        }

    }

}
