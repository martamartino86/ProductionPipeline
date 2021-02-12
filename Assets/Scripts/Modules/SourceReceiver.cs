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
        private float _lastDestroyedTimer;
        private WaitForEndOfFrame _wait;

        public int AmountOfSourceReceived 
        {
            get { return _receivedSources.Count; } 
        }

        private void Awake()
        {
            CheckInput();
            _lastDestroyedTimer = Time.time;
            _receivedSources = new Queue<Source>();
            _wait = new WaitForEndOfFrame();
            StartCoroutine(DestroySource());
        }


        IEnumerator DestroySource()
        {
            while (true)
            {
                if (Time.time - _lastDestroyedTimer >= _destroyInterval)
                {
                    if (_receivedSources.Count > 0)
                    {
                        Source toBeDestroyedSource = _receivedSources.Dequeue();
                        Destroy(toBeDestroyedSource.gameObject);
                    }
                    _lastDestroyedTimer = Time.time;
                }
                yield return _wait;
            }
        }

        protected override void InputModule_NewSource(object sender, SourceEventArgs e)
        {
            if (e.ReceivingModule == this)
            {
                Source inputSource = e.IncomingSource;
                inputSource.transform.SetParent(transform);
                inputSource.transform.localPosition = Vector3.zero;
                _receivedSources.Enqueue(inputSource);
            }
        }
    }

}

