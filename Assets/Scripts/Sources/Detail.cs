using UnityEngine;

namespace ProductionPipeline
{
    public class Detail : BasicSource
    {
        [SerializeField]
        private int _z;
        public int Z { get { return _z; } private set { _z = value; } }

        public override SourceType Type
        {
            get
            {
                return SourceType.Detail;
            }
            protected set { }
        }

        public override void Initialize(Module creationModule)
        {
            base.Initialize(creationModule);
            Z = Random.Range(-30, 30);
        }

        public override string GetStats()
        {
            string stats = base.GetStats();
            stats += "\nZ = " + Z +
                "\nColor: " + Color +
                "\nType: " + Type;
            return stats;
        }
    }

}
