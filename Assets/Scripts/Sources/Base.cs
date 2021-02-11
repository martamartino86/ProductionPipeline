using UnityEngine;

namespace ProductionPipeline 
{
    public class Base : BasicSource
    {
        [SerializeField]
        private int _x;

        public override SourceType GetSourceType()
        {
            return SourceType.Base;
        }

        public int GetX()
        {
            return _x;
        }

        public override void Initialize()
        {
            base.Initialize();
            _x = Random.Range(0, 100);
        }

    }

}
