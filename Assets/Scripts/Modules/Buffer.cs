using System.Collections.Generic;
using UnityEngine;

namespace ProductionPipeline
{
    public class Buffer : Module
    {
        [Range(0.1f,100f)]
        public float _intervalInSeconds;

        private float _lastReleaseTime;
        private Queue<Source> _currentlyStoredSources;

        private void Awake()
        {
            CheckInput();
            CheckOutput();
            _currentlyStoredSources = new Queue<Source>();
        }

        private void Start()
        {
            _lastReleaseTime = Time.time;
        }

        /// <summary>
        /// Temporarily stores a new source.
        /// </summary>
        /// <param name="s"></param>
        private void TemporarilyStoreSource(Source s)
        {
            _currentlyStoredSources.Enqueue(s);
        }

        
        void Update()
        {
            if (Time.time - _lastReleaseTime >= _intervalInSeconds)
            {
                if (_currentlyStoredSources.Count > 0)
                {
                    Source outputSource = _currentlyStoredSources.Dequeue();
                    SendSourceOut(outputSource, this, OutputModule[0]);
                }
                _lastReleaseTime = Time.time;
            }
        }


        /// <summary>
        /// Receiving an input source from an InputModule TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void InputModule_NewSource(object sender, SourceEventArgs e)
        {
            Source inputSource = e.IncomingSource;
            inputSource.transform.SetParent(transform);
            inputSource.transform.localPosition = Vector3.zero;
            TemporarilyStoreSource(inputSource);
        }
    }

}
