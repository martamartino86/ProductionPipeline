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
        public float VelocityOfTransport;

        /// <summary>
        /// Sources on the conveyor
        /// </summary>
        public Queue<Source> _sourcesOnConveyor;

        /// <summary>
        /// Points of the path
        /// </summary>
        private Vector3[] _pathPositions;
        private float _pathLength;

        private Source _lastAddedSource;
        private bool _newSourceAdded;

        private void Awake()
        {
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
                Debug.Log("[" + name + "] This Source is not for me.");
                return;
            }

            Debug.Log("[" + name + "] received new source: " +
                e.EmittingModule.name + " " + e.IncomingSource.name);

            Source inputSource = e.IncomingSource;
            _sourcesOnConveyor.Enqueue(inputSource);
            _lastAddedSource = inputSource;
            _newSourceAdded = true;
        }

        void Update()
        {
            if (_newSourceAdded)
            {
                _newSourceAdded = false;
                // in realtà queste andranno poi movimentate sulla forma ...
                _lastAddedSource.EndOfConveyor += Source_HasMovedOnTheConveyor;
                float duration = _pathLength / VelocityOfTransport;
                //_lastAddedSource.Move(_waypoints[0].position, _waypoints, VelocityOfTransport);
                _lastAddedSource.Move(_pathPositions, duration);
            }
        }

        private void Source_HasMovedOnTheConveyor(object sender, EventArgs e)
        {
            ((Source)sender).EndOfConveyor -= Source_HasMovedOnTheConveyor;
            Source outputSource = _sourcesOnConveyor.Dequeue();
            // when source is arrived at destination, invoke
            SendSourceOut(outputSource, this, OutputModule[0]);
        }


    }

}
