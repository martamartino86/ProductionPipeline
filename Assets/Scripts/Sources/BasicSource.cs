using UnityEngine;

namespace ProductionPipeline
{
    public abstract class BasicSource : Source
    {
        [SerializeField]
        protected string _id;
        [SerializeField]
        protected Color _color;

        // Set dimensions from Editor.
        // The scale of the object will be set based on these values.
        public float Width, Height, Depth;

        public virtual void Initialize()
        {
            _id = generateId();
            _color = Random.ColorHSV();
            setProperties();
        }


        private void setProperties()
        {
            gameObject.name = _id;
            _width = Width; _height = Height; _depth = Depth;
            transform.localScale = new Vector3(Width, Height, Depth);
            GetComponent<MeshRenderer>().material.color = _color;
        }

        private string generateId()
        {
            string s = ProductionUtilities.RandomId(6);
            return s;
        }

        public string GetId()
        {
            return _id;
        }

        public Color GetColor()
        {
            return _color;
        }

    }

}
