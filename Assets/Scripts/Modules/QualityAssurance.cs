using System;
using UnityEngine;

namespace ProductionPipeline
{
    public class QualityAssurance : Module
    {
        private SourceReceiver _outputForGoodQuality, _outputForBadQuality;
        private bool _receivedNewSource;
        private Source _newSource;

        private void Awake()
        {
            CheckInput();
            CheckOutput();
            SourceReceiver outputModule1 = OutputModule[0].GetComponent<SourceReceiver>();
            SourceReceiver outputModule2 = OutputModule[1].GetComponent<SourceReceiver>();
            _outputForBadQuality = (outputModule1.ReceiverType == SourceReceiver.TypeOfReceiver.Destroyer) ? outputModule1 : outputModule2;
            _outputForGoodQuality = (outputModule1.ReceiverType == SourceReceiver.TypeOfReceiver.Storer) ? outputModule1 : outputModule2;
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
                    string s = !qualityCondition ? "NOT" : "";
                    Debug.Log("[" + name + "] The source " + outputSource.name + " did " + s + " satisfy our quality standards.");
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
