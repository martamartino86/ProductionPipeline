using UnityEngine;

namespace ProductionPipeline
{
    public abstract class BasicSource : Source
    {
        [SerializeField]
        public string Id { get; private set; }
        [SerializeField]
        protected Color Color { get; private set; }

        public virtual void Initialize()
        {
            Id = GenerateId();
            Color = Random.ColorHSV();
            SetProperties();
            AdjustMeshPosition();
        }

        private void SetProperties()
        {
            gameObject.name = Id;
            MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
            // Assuming that Width is the size on X axis, Height is the size on Y axis, Width is the size on Z axis
            Vector3 meshBoundsSize = mr.bounds.size;
            Width  = meshBoundsSize.x;
            Height = meshBoundsSize.y;
            Depth  = meshBoundsSize.z;
            mr.material.color = Color;
        }

        private string GenerateId()
        {
            string s = Utilities.RandomId(6);
            return s;
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
