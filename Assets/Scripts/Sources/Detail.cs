using UnityEngine;

namespace ProductionPipeline
{
    public class Detail : BasicSource
    {
        [SerializeField]
        private int _z;

        public override void Initialize()
        {
            base.Initialize();
            _z = Random.Range(-30, 30);
        }
        public override SourceType GetSourceType()
        {
            return SourceType.Detail;
        }
    }

}
