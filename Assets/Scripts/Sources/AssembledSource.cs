using System.Collections;
using System.Collections.Generic;
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

        public void Initialize(Source source1, Source source2, SourceType sourceType)
        {
            name = source1.name + "+" + source2.name;
            _sourceType = sourceType;
            _source1 = source1;
            _source2 = source2;

            _source1.gameObject.transform.SetParent(transform);
            _source2.gameObject.transform.SetParent(transform);

            ArrangeSources();
        }

        private void ArrangeSources()
        {
            switch (_sourceType)
            {
                case SourceType.BaseBase:
                    float source1Width = _source1.GetWidth();
                    float source2Width = _source2.GetWidth();
                    _source1.transform.localPosition = new Vector3(- source1Width / 2, 0, 0);
                    _source2.transform.localPosition = new Vector3(  source2Width / 2, 0, 0);
                    // set correct dimensions of this new assembled sources
                    SetSize(source1Width + source2Width, _source1.GetHeight(), _source1.GetDepth());
                    break;
                case SourceType.BaseBaseBody:
                    Source basebase, body;
                    if (_source1.GetSourceType() == SourceType.BaseBase)
                    {
                        basebase = _source1;
                        body = _source2;
                    }
                    else
                    {
                        basebase = _source2;
                        body = _source1;
                    }
                    basebase.transform.localPosition = Vector3.zero;
                    body.transform.localPosition = new Vector3(0, body.GetHeight() / 2 + basebase.GetHeight() / 2, 0);
                    break;
                case SourceType.BaseBaseBodyDetail:

                    break;
                default:
                    break;
            }
        }

        private void SetSize(float w, float h, float d)
        {
            _width = w;
            _height = h;
            _depth = d;
        }
    }

}
