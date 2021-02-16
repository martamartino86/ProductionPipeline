using System.Collections.Generic;
using UnityEngine;

namespace ProductionPipeline
{
    public class Buffer : Module
    {
        /// <summary>
        /// Buffer time
        /// </summary>
        [SerializeField]
        [Range(0.1f, 100f)]
        private float _intervalInSeconds;

        private float _lastReleaseTime;
        private Queue<Source> _currentlyStoredSources;

        protected override void Awake()
        {
            base.Awake();
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
            if (_paused) return;
            if (Time.time - _lastReleaseTime >= _intervalInSeconds)
            {
                if (_currentlyStoredSources.Count > 0)
                {
                    Source outputSource = _currentlyStoredSources.Dequeue();
                    SendSourceOut(outputSource, this, OutputModules[0]);
                    DataChanged(GetStats());
                }
                _lastReleaseTime = Time.time;
            }
        }

        /// <summary>
        /// Receiving an input source from an InputModule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void InputModule_NewSource(object sender, SourceEventArgs e)
        {
            Source inputSource = e.IncomingSource;
            inputSource.SetCurrentParent(this);
            TemporarilyStoreSource(inputSource);
            DataChanged(GetStats());
        }

        public override string GetStats()
        {
            string stats = base.GetStats();
            stats += "\nBuffering time: " + _intervalInSeconds.ToString() + " sec.";
            if (_currentlyStoredSources.Count > 0)
            {
                stats += "\nCurrently stored sources: ";
                foreach (Source s in _currentlyStoredSources)
                {
                    stats += s.name + " ";
                }
            }
            return stats;
        }

    }

}
