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

        /// <summary>
        /// ID of the source.
        /// </summary>
        public string Id { get { return _id; } protected set { _id = value; } }
        [SerializeField]
        private string _id;

        // The dimensions are set differently for the type of sources.
        // Basic sources set their dimensions from the mesh.
        // Instead, assembled sources always set their dimensions from the basic sources they are composed of.
        public float Width { get; protected set; }
        public float Height { get; protected set; }
        public float Depth { get; protected set; }

        private MeshRenderer[] _renderers;
        public float CreationTime { get; private set; }

        public Module CreationModule { get; protected set; }

        protected Module _currentModuleParent { get; set; }

        /// <summary>
        /// This event is raised when the source reaches the end of the conveyor.
        /// </summary>
        public event EventHandler EndOfConveyor;
        protected virtual void OnIsArrived()
        {
            EventHandler handler = EndOfConveyor;
            handler?.Invoke(this, null);
        }

        /// <summary>
        /// Type of source (Base, Body, Detail, BaseBase, BaseBaseBody, BaseBaseBodyDetail)
        /// </summary>
        public abstract SourceType Type { get; protected set; }
        
        private void OnEnable()
        {
            PipelineManager.Instance.SimulationPaused += Instance_SimulationPaused;
        }

        private void OnDisable()
        {
            if (PipelineManager.Instance != null)
                PipelineManager.Instance.SimulationPaused -= Instance_SimulationPaused;
        }

        private void Instance_SimulationPaused(object sender, PipelineManager.SimulationEventArgs e)
        {
            if (e.IsPaused)
            {
                DOTween.PauseAll();
            }
            else
            {
                DOTween.PlayAll();
            }
        }

        private void Awake()
        {
            _renderers = GetComponentsInChildren<MeshRenderer>();
        }

        /// <summary>
        /// Set the current parent information and 
        /// </summary>
        /// <param name="parentModule">Parent module</param>
        /// <param name="newSource">Is this a new source?</param>
        /// <param name="resetPosition">Reset position after changing parent?</param>
        /// <param name="resetRotation">Reset rotation after changing parent?</param>
        public void SetCurrentParent(Module parentModule, bool newSource = false, bool resetPosition = true, bool resetRotation = false)
        {
            transform.SetParent(parentModule.transform);
            if (resetPosition)
            {
                transform.localPosition = Vector3.zero;
            }
            if (resetRotation)
            {
                transform.localRotation = Quaternion.identity;
            }
            _currentModuleParent = parentModule;
            if (!newSource)
            {
                PipelineManager.Instance.ModifySource(Id, GetStats());
            }
        }

        /// <summary>
        /// Moves the source on a path.
        /// </summary>
        /// <param name="conveyorWaypoints">Path (must be specified in editor)</param>
        /// <param name="duration">Duration of the movement</param>
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

        protected void SourceCreated()
        {
            CreationTime = Time.time;
            PipelineManager.Instance.AddSource(this);
        }

        private void OnDestroy()
        {
            if (PipelineManager.Instance != null)
                PipelineManager.Instance.RemoveSource(Id);
        }

        /// <summary>
        /// Handle the consequences of the source being assembled into another bigger source.
        /// </summary>
        public void Assembled()
        {
            PipelineManager.Instance.RemoveSource(Id);
        }

        public virtual string GetStats()
        {
            TimeSpan ctime = TimeSpan.FromSeconds(CreationTime);
            string stats = "<b>Id: " + Id + "</b>" +
                "\nSource created by " + CreationModule.ModuleName + " [" + ctime.ToString("hh':'mm':'ss") + "]";
            stats += "\nWidth: " + Width.ToString() + " Height: " + Height.ToString() + " Depth: " + Depth.ToString() +
                "\nThis source is on module " + _currentModuleParent;
            return stats;
        }
    }

}
