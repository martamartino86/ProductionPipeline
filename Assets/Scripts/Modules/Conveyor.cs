using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProductionPipeline
{
    public class Conveyor : Module
    {
        /// <summary>
        /// Velocity of transport in m/s
        /// </summary>
        [Range(0.1f, 10f)]
        public float VelocityOfTransport;

        /// <summary>
        /// Sources on the conveyor
        /// </summary>
        public Queue<Source> _sourcesOnConveyor;

        /// <summary>
        /// Points of the path
        /// </summary>
        [SerializeField]
        private Vector3[] _pathPositions;
        private float _pathLength;

        private Source _lastAddedSource;
        private bool _newSourceAdded;

        protected override void Awake()
        {
            base.Awake();
            CheckInput();
            CheckOutput();
            _sourcesOnConveyor = new Queue<Source>();
            _lastAddedSource = null;
            _newSourceAdded = false;
            Transform pathChild = transform.Find("Path");
            _pathPositions = new Vector3[pathChild.childCount];
            for (int i = 0; i < pathChild.childCount; i++)
            {
                Transform child = pathChild.GetChild(i);
                _pathPositions[i] = child.position;
            }
            for (int j = 0; j < _pathPositions.Length - 1; j++)
            {
                _pathLength += Vector3.Distance(_pathPositions[j + 1], _pathPositions[j]);
            }
        }

        protected override void InputModule_NewSource(object sender, SourceEventArgs e)
        {
            // Since I'm a conveyor, if I am one of the multiple output modules of a module,
            // I could be receiving a notification for a Source that is not for me. I need to check.
            if (e.ReceivingModule != this)
            {
#if DEBUG_PRINT
                Debug.Log("[" + name + "] This Source is not for me.");
#endif
                return;
            }
#if DEBUG_PRINT
            Debug.Log("[" + name + "] received new source from " +
                e.EmittingModule.name + ": " + e.IncomingSource.name);
#endif
            Source inputSource = e.IncomingSource;
            inputSource.transform.SetParent(transform, true);
            _sourcesOnConveyor.Enqueue(inputSource);
            _lastAddedSource = inputSource;
            _newSourceAdded = true;
            DataChanged(GetStats());
        }

        void Update()
        {
            if (_newSourceAdded)
            {
                _newSourceAdded = false;
                _lastAddedSource.EndOfConveyor += Source_HasMovedOnTheConveyor;
                float duration = _pathLength / VelocityOfTransport;
                _lastAddedSource.Move(_pathPositions, duration);
            }
        }

        private void Source_HasMovedOnTheConveyor(object sender, EventArgs e)
        {
            ((Source)sender).EndOfConveyor -= Source_HasMovedOnTheConveyor;
            Source outputSource = _sourcesOnConveyor.Dequeue();
            // when source is arrived at destination, invoke
            SendSourceOut(outputSource, this, OutputModules[0]);
            DataChanged(GetStats());
        }
        public override string GetStats()
        {
            string stats = base.GetStats();
            stats += "\nVelocity of transport: " + VelocityOfTransport.ToString() + " m/s";
            stats += "\nLength of the conveyor: " + _pathLength.ToString() + " m";
            if (_sourcesOnConveyor.Count > 0)
            {
                stats += "\nTransported sources: ";
                foreach (Source s in _sourcesOnConveyor)
                {
                    stats += s.name + " ";
                }
            }
            return stats;
        }
    }

}
