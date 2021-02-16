using UnityEngine;

namespace ProductionPipeline
{
    public class AssembledSource : Source
    {
        /// <summary>
        /// First source that compose the assembled source.
        /// </summary>
        public Source Source1 
        { 
            get { return _source1; } 
            private set { _source1 = value; } 
        }

        /// <summary>
        /// Second source that compose the assembled source.
        /// </summary>
        public Source Source2
        {
            get { return _source2; }
            private set { _source2 = value; }
        }
        [SerializeField]
        private Source _source1, _source2;

        /// <summary>
        /// Type of the assembled source
        /// </summary>
        public override SourceType Type
        {
            get
            {
                return _sourceType;
            }
            protected set
            {
                _sourceType = value;
            }
        }
        private SourceType _sourceType;


        public override string GetStats()
        {
            string stats = base.GetStats();
            stats += "\nAssembled Type: " + Type.ToString() + "\n";
            stats += "\n" + _source1.GetStats();
            stats += "\n" + _source2.GetStats();
            return stats;
        }

        public void Initialize(Source source1, Source source2, SourceType sourceType, Transform myParentModule)
        {
            CreationModule = myParentModule.GetComponent<Module>();
            SetCurrentParent(CreationModule, true, true);
            
            Id = source1.name + "+" + source2.name;
            name = Id;
            Type = sourceType;
            _source1 = source1;
            _source2 = source2;

            _source1.gameObject.transform.SetParent(transform);
            _source2.gameObject.transform.SetParent(transform);
            _source1.gameObject.transform.localRotation = Quaternion.identity;
            _source2.gameObject.transform.localRotation = Quaternion.identity;

            ArrangeSources();
            SourceCreated();
        }

        private void ArrangeSources()
        {
            switch (Type)
            {
                case SourceType.BaseBase:
                    float source1Width = _source1.Width;
                    float source2Width = _source2.Width;
                    _source1.transform.localPosition = new Vector3(- source1Width / 2, 0, 0);
                    _source2.transform.localPosition = new Vector3(  source2Width / 2, 0, 0);
                    // set correct dimensions of this new assembled sources
                    SetSize(source1Width + source2Width, _source1.Height, _source1.Depth);
                    break;
                case SourceType.BaseBaseBody:
                    Source basebase, body;
                    basebase = (_source1.Type == SourceType.BaseBase) ? _source1 : _source2;
                    body     = (_source1.Type == SourceType.Body) ? _source1 : _source2;
                    basebase.transform.localPosition = Vector3.zero;
                    body.transform.localPosition = new Vector3(0, basebase.Height, 0);
                    SetSize(body.Width, basebase.Height + body.Height, body.Depth);
                    break;
                case SourceType.BaseBaseBodyDetail:
                    Source basebasebody, detail;
                    basebasebody = (_source1.Type == SourceType.BaseBaseBody) ? _source1 : _source2;
                    detail       = (_source1.Type == SourceType.Detail) ? _source1 : _source2;
                    basebasebody.transform.localPosition = Vector3.zero;
                    detail.transform.localPosition = new Vector3(0, basebasebody.Height, 0);
                    SetSize(basebasebody.Width, basebasebody.Height + detail.Height, basebasebody.Depth);
                    break;
                default:
                    break;
            }
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        private void SetSize(float w, float h, float d)
        {
            Width = w;
            Height = h;
            Depth = d;
        }
    }
}
