using UnityEngine;

namespace ProductionPipeline
{
    public class AssembledSource : Source
    {
        [SerializeField]
        private Source _source1, _source2;

        public override SourceType GetSourceType()
        {
            return _sourceType;
        }

        public Source GetFirstSource()
        {
            return _source1;
        }
        public Source GetSecondSource()
        {
            return _source2;
        }

        public void Initialize(Source source1, Source source2, SourceType sourceType, Transform myParentModule)
        {
            transform.SetParent(myParentModule);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            
            name = source1.name + "+" + source2.name;
            _sourceType = sourceType;
            _source1 = source1;
            _source2 = source2;

            _source1.gameObject.transform.SetParent(transform);
            _source2.gameObject.transform.SetParent(transform);
            _source1.gameObject.transform.localRotation = Quaternion.identity;
            _source2.gameObject.transform.localRotation = Quaternion.identity;

            ArrangeSources();
        }

        private void ArrangeSources()
        {
            switch (_sourceType)
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
                    basebase = (_source1.GetSourceType() == SourceType.BaseBase) ? _source1 : _source2;
                    body     = (_source1.GetSourceType() == SourceType.Body) ? _source1 : _source2;
                    basebase.transform.localPosition = Vector3.zero;
                    body.transform.localPosition = new Vector3(0, basebase.Height, 0);
                    SetSize(body.Width, basebase.Height + body.Height, body.Depth);
                    break;
                case SourceType.BaseBaseBodyDetail:
                    Source basebasebody, detail;
                    basebasebody = (_source1.GetSourceType() == SourceType.BaseBaseBody) ? _source1 : _source2;
                    detail       = (_source1.GetSourceType() == SourceType.Detail) ? _source1 : _source2;
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
