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

        public TypeOfReceiver ReceiverType { get { return _receiverType; } private set { _receiverType = value; } }
        [SerializeField]
        private TypeOfReceiver _receiverType;

        private Queue<Source> _receivedSources;
        private bool _newSourceReceived;
        /// <summary>
        /// Total amount of sources received from the beginning of the simulation.
        /// </summary>
        private int _nSourcesReceived;

        protected override void Awake()
        {
            base.Awake();
            CheckInput();
            _receivedSources = new Queue<Source>();
            _nSourcesReceived = 0;
            _newSourceReceived = false;
        }

        void Update()
        {
            if (_paused) return;
            if (_newSourceReceived)
            {
                if (_receivedSources.Count > 1)
                {
                    Source toBeDeleted = _receivedSources.Dequeue();
                    Destroy(toBeDeleted.gameObject);
                    DataChanged(GetStats());
                }
                _newSourceReceived = false;
            }
        }

        protected override void InputModule_NewSource(object sender, SourceEventArgs e)
        {
            if (e.ReceivingModule == this)
            {
                Source inputSource = e.IncomingSource;
                inputSource.SetCurrentParent(this);
                _receivedSources.Enqueue(inputSource);
                _newSourceReceived = true;
                _nSourcesReceived++;
                DataChanged(GetStats());
            }
        }
        public override string GetStats()
        {
            string stats = base.GetStats();
            stats += "\nType of receiver: " + ReceiverType.ToString() +
                "\nTotal amount of sources received: " + _nSourcesReceived;
            if (_receivedSources.Count > 0)
            {
                stats += "\nSources currently in the receiver: ";
                foreach (var s in _receivedSources)
                {
                    stats += s.Id.ToString() + " ";
                }
            }
            return stats;
        }
    }
}

