using System;
using UnityEngine;

namespace ProductionPipeline
{
    public class QualityAssurance : Module
    {
        // ASSUMPTION: since the output of the quality assurance could be two Conveyor (and we would not know where to send the source)
        // let's make the assumption that AT LEAST one of the two is a SourceReceiver (either good or bad).
        private Module _outputForGoodQuality, _outputForBadQuality;
        private bool _receivedNewSource;
        private Source _newSource;

        private void Awake()
        {
            CheckInput();
            CheckOutput();
            CheckOutputType(OutputModule[0].GetComponent<SourceReceiver>(), OutputModule[1].GetComponent<SourceReceiver>());
            _receivedNewSource = false;
        }

        private void CheckOutputType(SourceReceiver outputModule0, SourceReceiver outputModule1)
        {
            if (outputModule0 != null)
            {
                if (outputModule0.ReceiverType == SourceReceiver.TypeOfReceiver.Destroyer)
                {
                    _outputForBadQuality = outputModule0;
                    _outputForGoodQuality = OutputModule[1].GetComponent<Module>();
                }
                else
                {
                    _outputForBadQuality = OutputModule[1].GetComponent<Module>();
                    _outputForGoodQuality = outputModule0;
                }
            }
            else if (outputModule1 != null)
            {
                if (outputModule1.ReceiverType == SourceReceiver.TypeOfReceiver.Destroyer)
                {
                    _outputForBadQuality = outputModule1;
                    _outputForGoodQuality = OutputModule[0].GetComponent<Module>();
                }
                else
                {
                    _outputForBadQuality = OutputModule[0].GetComponent<Module>();
                    _outputForGoodQuality = outputModule1;
                }
            }
        }

        protected override void InputModule_NewSource(object sender, SourceEventArgs e)
        {
            Source inputSource = e.IncomingSource;
            inputSource.transform.SetParent(transform);
            inputSource.transform.localPosition = Vector3.zero;
            _newSource = inputSource;
            _receivedNewSource = true;
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
                    string s = !qualityCondition ? " NOT" : "";
                    Module destinationModule = qualityCondition ? _outputForGoodQuality : _outputForBadQuality;
                    Debug.Log("[" + name + "] The source " + outputSource.name + " did" + s + " satisfy our quality standards. Sending it to " + destinationModule.name);
                    SendSourceOut(outputSource, this, destinationModule);
                }
                catch (InvalidCastException exc)
                {
                    Debug.LogError("[" + name + "] " + exc + " // Cannot check quality of this Source.");
                }
            }
        }
    }

}
