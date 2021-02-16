using UnityEngine;

namespace ProductionPipeline
{
    public abstract class BasicSource : Source
    {
        /// <summary>
        /// Random generated color (at creation). Color assigned via inspector will be ignored.
        /// </summary>
        public Color Color { get { return _color; } private set { _color = value; } }
        [SerializeField]
        private Color _color;

        private TextMesh _3dName;

        public virtual void Initialize(Module creationModule)
        {
            CreationModule = creationModule;
            SetProperties();
            AdjustMeshPosition();
            SourceCreated();
        }

        private void SetProperties()
        {
            Id = GenerateId();
            Color = Random.ColorHSV();
            name = Id;
            MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
            mr.material.color = Color;
            
            // Assuming that Width is the size on X axis, Height is the size on Y axis, Width is the size on Z axis
            Vector3 meshBoundsSize = mr.bounds.size;
            Width  = meshBoundsSize.x;
            Height = meshBoundsSize.y;
            Depth  = meshBoundsSize.z;
            
            _3dName = Instantiate(Resources.Load<TextMesh>("Prefabs/Source3dName"));
            _3dName.transform.SetParent(transform);
            _3dName.transform.localPosition = new Vector3(0, Height + .1f, 0);
            _3dName.text = Id;
        }

        private string GenerateId()
        {
            return Utilities.RandomId(6);
        }

        /// <summary>
        /// This method moves the child transform of this BasicSource, in such a way that 
        /// the pivot point of the whole BasicSource looks on the bottom of the mesh.
        /// (Based on the assumption that the BasicSource has a child with the pivot centered on it)
        /// </summary>
        private void AdjustMeshPosition()
        {
            transform.GetChild(0).localPosition = new Vector3(0, Height / 2f, 0);
        }

    }

}
