using System.Collections.Generic;
using UnityEngine;

namespace ProductionPipeline
{
    public class FlowSplitter : Module
    {
        /// <summary>
        /// Weights of the outputs
        /// </summary>
        [SerializeField]
        [Range(0,1)]
        private float[] _outputWeights;

        private bool _receivedSource;

        private Queue<Source> _receivedSources;

        protected override void Awake()
        {
            base.Awake();
            CheckInput();
            CheckOutput();
            if (OutputModules[0] == null || OutputModules[1] == null)
            {
                Debug.LogError("[" + ModuleName + "] Please assign the Output modules.");
            }
            if (_outputWeights.Length != OutputModules.Length)
            {
                Debug.LogError("[" + ModuleName + "] Please assign a weight to each output of this splitter.");
                return;
            }
            _receivedSources = new Queue<Source>();
            _receivedSource = false;
        }

        private void Update()
        {
            if (_paused) return;
            if (_receivedSource)
            {
                _receivedSource = false;
                Source inputSource = _receivedSources.Dequeue();
                int moduleIndex = ChooseOutput();
#if DEBUG_PRINT
                Debug.Log("Going to " + OutputModule[moduleIndex].name);
#endif
                SendSourceOut(inputSource, this, OutputModules[moduleIndex]);
                DataChanged(GetStats());
            }
        }

        private int ChooseOutput()
        {
            int[] moduleIndexes = new int[_outputWeights.Length];
            for (int i = 0; i < _outputWeights.Length; i++)
            {
                moduleIndexes[i] = i;
            }
            float weightsSum = 0;
            foreach (var weight in _outputWeights)
                weightsSum += weight;
            
            float x = Random.Range(0, weightsSum);

            float cumulativeWeight = 0;
            int j = 0;
            foreach (var weight in _outputWeights)
            {
                cumulativeWeight += weight;
                if (x < cumulativeWeight)
                    return moduleIndexes[j];
                j++;
            }
            return -1;
        }

        /// <summary>
        /// Receives a source and brings it to one of the two outputs depending on their weights.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void InputModule_NewSource(object sender, SourceEventArgs e)
        {
            Source inputSource = e.IncomingSource;
            inputSource.SetCurrentParent(this);
            _receivedSources.Enqueue(inputSource);
            _receivedSource = true;
            DataChanged(GetStats());
        }

        public override string GetStats()
        {
            string stats = base.GetStats();
            for (int i = 0; i < _outputWeights.Length; i++)
            {
                stats += "\nOutput " + i + " weight: " + _outputWeights[i].ToString() + " ";
            }
            if (_receivedSources.Count > 0)
            {
                stats += "\nReceived sources: ";
                foreach (Source s in _receivedSources)
                {
                    stats += s.name + " ";
                }
            }
            return stats;
        }
    }

}
