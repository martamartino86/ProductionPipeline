using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProductionPipeline
{
    public class SourceReceiver : Module
    {
        public enum TypeOfReceiver
        {
            Storer,
            Destroyer
        }
        [SerializeField]
        public TypeOfReceiver ReceiverType;
        private Queue<Source> _receivedSources;

        [SerializeField]
        [Range(0f, 5f)]
        private float _destroyInterval;
        private WaitForEndOfFrame _wait;
        private bool _coroutineRunning;
        private int _nSourcesReceived;

        protected override void Awake()
        {
            base.Awake();
            CheckInput();
            _receivedSources = new Queue<Source>();
            _wait = new WaitForEndOfFrame();
            _coroutineRunning = false;
            _nSourcesReceived = 0;
        }

        void Update()
        {
            if (_receivedSources.Count > 0)
            {
                if (!_coroutineRunning)
                    StartCoroutine(DestroySource());
            }
        }

        IEnumerator DestroySource()
        {
            _coroutineRunning = true;
            bool _destroyed = false;
            float timer = Time.time;
            while (!_destroyed)
            {
                if (Time.time - timer >= _destroyInterval)
                {
                    Source toBeDestroyedSource = _receivedSources.Dequeue();
                    Destroy(toBeDestroyedSource.gameObject);
                    _destroyed = true;
                }
                yield return _wait;
            }
            DataChanged(GetStats());
            _coroutineRunning = false;
        }

        protected override void InputModule_NewSource(object sender, SourceEventArgs e)
        {
            if (e.ReceivingModule == this)
            {
                Source inputSource = e.IncomingSource;
                inputSource.transform.SetParent(transform);
                inputSource.transform.localPosition = Vector3.zero;
                _receivedSources.Enqueue(inputSource);
                _nSourcesReceived++;
                DataChanged(GetStats());
            }
        }
        public override string GetStats()
        {
            string stats = base.GetStats();
            stats += "\nType of receiver: " + ReceiverType.ToString() +
                "\nDestroy GameObjects after " + _destroyInterval + " seconds" +
                "\nTotal amount of sources received: " + _nSourcesReceived;
            if (_receivedSources.Count > 0)
            {
                stats += "\nSources currently in the receiver: ";
                foreach (var s in _receivedSources)
                {
                    stats += s.ToString() + " ";
                }
            }
            return stats;
        }
    }
}

