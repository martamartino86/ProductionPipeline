using UnityEngine;

namespace ProductionPipeline
{
    public class Body : BasicSource
    {
        private enum y_Type
        {
            A, B, C
        }
        [SerializeField]
        private y_Type _y;

        public override void Initialize()
        {
            base.Initialize();
            _y = (y_Type)Random.Range(1, 4);
        }

        public override SourceType GetSourceType()
        {
            return SourceType.Body;
        }
    }

}
