using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProductionPipeline
{
    public class QualityAssurance : Module
    {
        private Module _outputForGoodQuality, _outputForBadQuality;
        private bool _receivedNewSource;
        private Source _newSource;

        private void Awake()
        {
            if (OutputModule[0].GetComponent<SourceDestroyer>() != null)
            {
                _outputForBadQuality = OutputModule[0];
                _outputForGoodQuality = OutputModule[1];
            }
            else
            {
                _outputForBadQuality = OutputModule[1];
                _outputForGoodQuality = OutputModule[0];
            }
            _receivedNewSource = false;
        }

        protected override void InputModule_NewSource(object sender, SourceEventArgs e)
        {
            _receivedNewSource = true;
            Source inputSource = e.IncomingSource;
            inputSource.transform.SetParent(transform, true);
            _newSource = inputSource;
        }

        private void Update()
        {
            if (_receivedNewSource)
            {
                _receivedNewSource = false;
                try
                {
                    AssembledSource outputSource = (AssembledSource)_newSource;
                    Base b1 = (Base)outputSource.GetFirstSource();
                    Base b2 = (Base)outputSource.GetSecondSource();
                    bool qualityCondition = b1.GetX() + b2.GetX() <= 100;
                    SendSourceOut(outputSource, this, qualityCondition ? _outputForGoodQuality : _outputForBadQuality);
                }
                catch (InvalidCastException exc)
                {
                    Debug.LogError("[" + name + "] " + exc + " // Cannot check quality of this Source.");
                }
            }
        }
    }

}
