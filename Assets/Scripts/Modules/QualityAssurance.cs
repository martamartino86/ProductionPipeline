using System;
using UnityEngine;

namespace ProductionPipeline
{
    public class QualityAssurance : Module
    {
        private Module _outputForGoodQuality, _outputForBadQuality;
        private bool _receivedNewSource;
        private Source _newSource;

        protected override void Awake()
        {
            base.Awake();
            CheckInput();
            CheckOutput();
            CheckOutputType(OutputModules[0].GetComponent<SourceReceiver>(), OutputModules[1].GetComponent<SourceReceiver>());
            _receivedNewSource = false;
        }

        private void CheckOutputType(SourceReceiver outputModule0, SourceReceiver outputModule1)
        {
            if (outputModule0 != null)
            {
                if (outputModule0.ReceiverType == SourceReceiver.TypeOfReceiver.Destroyer)
                {
                    _outputForBadQuality = outputModule0;
                    _outputForGoodQuality = OutputModules[1].GetComponent<Module>();
                }
                else
                {
                    _outputForBadQuality = OutputModules[1].GetComponent<Module>();
                    _outputForGoodQuality = outputModule0;
                }
            }
            else if (outputModule1 != null)
            {
                if (outputModule1.ReceiverType == SourceReceiver.TypeOfReceiver.Destroyer)
                {
                    _outputForBadQuality = outputModule1;
                    _outputForGoodQuality = OutputModules[0].GetComponent<Module>();
                }
                else
                {
                    _outputForBadQuality = OutputModules[0].GetComponent<Module>();
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
            DataChanged(GetStats());
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
                    Module destinationModule = qualityCondition ? _outputForGoodQuality : _outputForBadQuality;
#if DEBUG_PRINT
                    string s = !qualityCondition ? " NOT" : "";
                    Debug.Log("[" + name + "] The source " + outputSource.name + " did" + s + " satisfy our quality standards. Sending it to " + destinationModule.name);
#endif
                    SendSourceOut(outputSource, this, destinationModule);
                    DataChanged(GetStats());
                    _newSource = null;
                }
                catch (InvalidCastException exc)
                {
                    Debug.LogError("[" + name + "] " + exc + " // Cannot check quality of this Source.");
                }
            }
        }
        public override string GetStats()
        {
            string stats = base.GetStats();
            stats += "Good quality output: " + _outputForGoodQuality.ModuleName +
                "\nBad quality output: " + _outputForBadQuality.ModuleName +
                "\nSource: " + _newSource;
            return stats;
        }
    }

}
