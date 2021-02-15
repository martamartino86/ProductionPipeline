using UnityEngine;

namespace ProductionPipeline 
{
    public class Base : BasicSource
    {
        public int X { get { return _x; } private set { _x = value; } }
        [SerializeField]
        private int _x;
        
        public override SourceType Type { get; protected set; }

        public override void Initialize(Module creationModule)
        {
            base.Initialize(creationModule);
            Type = SourceType.Base;
            X = Random.Range(0, 100);
        }

        public override string GetStats()
        {
            string stats = base.GetStats();
            stats += "\nX = " + X +
                "\nColor: " + Color +
                "\nType: " + Type;
            return stats;
        }
    }

}
