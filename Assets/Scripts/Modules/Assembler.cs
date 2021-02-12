using System.Collections.Generic;
using UnityEngine;

namespace ProductionPipeline
{
    public class Assembler : Module
    {
        [SerializeField]
        private Source.SourceType _sourceType1;
        [SerializeField]
        private Source.SourceType _sourceType2;

        /// <summary>
        /// Production time in seconds
        /// </summary>
        [SerializeField]
        private float _productionTime;
        private float _lastProductionStartedTime;
        [SerializeField]
        private bool _isAssembling = false;

        /// <summary>
        /// In this simplified implementation, the Assembler always have 2 inputs.
        /// So, each Queue contains one type of Source to be Dequeued when there are enough sources ready
        /// and when the Assembler is ready.
        /// </summary>
        private Queue<Source> _firstInputSourcesQueue, _secondInputSourcesQueue;
        private int queueSwitchCounter = 0;

        private void Awake()
        {
            _firstInputSourcesQueue = new Queue<Source>();
            _secondInputSourcesQueue = new Queue<Source>();
        }

        private void Start()
        {
            _lastProductionStartedTime = 0;
        }

        private void Update()
        {
            // when there is at least 1 element for each queue, take some _productionTime
            // and then a new AssembledSource is released
            if (_firstInputSourcesQueue.Count > 0 && _secondInputSourcesQueue.Count > 0)
            {
                if (!_isAssembling)
                {
                    _isAssembling = true;
                    _lastProductionStartedTime = Time.time;
                }
                if (_isAssembling && (Time.time - _lastProductionStartedTime) >= _productionTime)
                {
                    _isAssembling = false;
                    Source source1 = _firstInputSourcesQueue.Dequeue();
                    Source source2 = _secondInputSourcesQueue.Dequeue();
                    AssembledSource assembledSource = Instantiate(Resources.Load<AssembledSource>("Prefabs/AssembledSources"));
                    assembledSource.Initialize(source1, source2, GetAssembledSourceType(), transform);
                    SendSourceOut(assembledSource, this, OutputModule[0]);
                }
            }
        }


        protected override void InputModule_NewSource(object sender, SourceEventArgs e)
        {
            Source inputSource = e.IncomingSource;
            inputSource.transform.SetParent(transform);
            inputSource.transform.localPosition = Vector3.zero;
            // if the Assembler takes the same type of input, put the source alternatively
            if (_sourceType1 == _sourceType2)
            {
                int x = queueSwitchCounter % 2;
                if (x == 0)
                    _firstInputSourcesQueue.Enqueue(inputSource);
                else
                    _secondInputSourcesQueue.Enqueue(inputSource);
                queueSwitchCounter++;
                return;
            }

            if (inputSource.GetSourceType() == _sourceType1)
            {
                _firstInputSourcesQueue.Enqueue(inputSource);
            }
            else if (inputSource.GetSourceType() == _sourceType2)
            {
                _secondInputSourcesQueue.Enqueue(inputSource);
            }
            else
            {
                Debug.LogError("["+name+"] not receiving the expected type of source: " + inputSource.name + " " + inputSource.GetSourceType());
            }
        }

        /// <summary>
        /// Defines the rules for the assembled source type.
        /// </summary>
        /// <returns>The source type of the assembled (please note: if "Base" is returned, 
        /// it means that the combination of these sources does not have a rule yet.</returns>
        private Source.SourceType GetAssembledSourceType()
        {
            if (_sourceType1 == Source.SourceType.Base && _sourceType2 == Source.SourceType.Base)
            {
                return Source.SourceType.BaseBase;
            }
            else if (_sourceType1 == Source.SourceType.BaseBase && _sourceType2 == Source.SourceType.Body)
            {
                return Source.SourceType.BaseBaseBody;
            }
            else if (_sourceType1 == Source.SourceType.BaseBaseBody && _sourceType2 == Source.SourceType.Detail)
            {
                return Source.SourceType.BaseBaseBodyDetail;
            }
            else
            {
                return Source.SourceType.Base;
            }
        }

    }

}
