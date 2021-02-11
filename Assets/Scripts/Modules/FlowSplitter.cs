using System.Collections.Generic;
using UnityEngine;

namespace ProductionPipeline
{
    public class FlowSplitter : Module
    {
        [SerializeField]
        [Range(0,1)]
        private float[] _outputWeights;

        private bool _receivedSource;

        private Queue<Source> _receivedSources;

        // Start is called before the first frame update
        void Awake()
        {
            CheckInput();
            CheckOutput();
            if (_outputWeights.Length != OutputModule.Length)
            {
                Debug.LogError("[FlowSplitter] Please assign a weight to each output of this splitter.");
                return;
            }
            _receivedSources = new Queue<Source>();
            _receivedSource = false;
        }


        private void Update()
        {
            if (_receivedSource)
            {
                _receivedSource = false;
                Source inputSource = _receivedSources.Dequeue();
                int moduleIndex = ChooseOutput();
                SendSourceOut(inputSource, this, OutputModule[moduleIndex]);
            }
        }

        private int ChooseOutput()
        {
            // potrei lasciare questa parte qui, in questo modo
            // se i pesi cambiano a runtime (da editor) viene comunque aggiornato
            int[] moduleIndexes = new int[_outputWeights.Length];
            for (int i = 0; i < _outputWeights.Length; i++)
            {
                moduleIndexes[i] = i;
            }
            float weightsSum = 0;
            foreach (var weight in _outputWeights)
                weightsSum += weight;
            // (...fino a qui)

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
            inputSource.transform.SetParent(transform, true);
            _receivedSources.Enqueue(inputSource);
            _receivedSource = true;
        }
    }

}
