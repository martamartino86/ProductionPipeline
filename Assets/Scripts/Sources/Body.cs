using UnityEngine;

namespace ProductionPipeline
{
    public class Body : BasicSource
    {
        public enum y_Type
        {
            A, B, C
        }
        public y_Type Y { get { return _y; } private set { _y = value; } }
        [SerializeField]
        private y_Type _y;

        public override SourceType Type
        {
            get
            {
                return SourceType.Body;
            }
            protected set { }
        }

        public override void Initialize(Module creationModule)
        {
            base.Initialize(creationModule);
            Y = (y_Type)Random.Range(1, 4);
        }

        public override string GetStats()
        {
            string stats = base.GetStats();
            stats += "\nY = " + Y +
                "\nColor: " + Color +
                "\nType: " + Type;
            return stats;
        }

    }

}
