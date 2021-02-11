using System;
using UnityEngine;

namespace ProductionPipeline
{
    public abstract class Module : MonoBehaviour
    {
        /// <summary>
        /// Each module is connected to one or more input modules.
        /// </summary>
        public Module[] InputModule;
        /// <summary>
        /// Each module is connected to one or more output modules.
        /// </summary>
        public Module[] OutputModule;

        /// <summary>
        /// Every module emits this event when the source is ready to be passed to the Output module(s).
        /// </summary>
        public event EventHandler<SourceEventArgs> NewSource;
        protected virtual void OnNewSource(SourceEventArgs e)
        {
            EventHandler<SourceEventArgs> handler = NewSource;
            handler?.Invoke(this, e);
        }

        public class SourceEventArgs : EventArgs
        {
            public Module EmittingModule; // who is sending the source
            public Module ReceivingModule; // who is the receiver of the source
            public Source IncomingSource; // the generic source
        }

        protected void OnEnable()
        {
            if (InputModule.Length > 0)
            {
                foreach (var inputModule in InputModule)
                {
                    if (inputModule == null)
                    {
                        Debug.LogError("[" + name + "]: Please assign valid InputModules!");
                        return;
                    }
                    inputModule.NewSource += InputModule_NewSource;
                }
            }
        }
        protected void OnDisable()
        {
            if (InputModule.Length > 0)
            {
                foreach (var inputModule in InputModule)
                {
                    if (inputModule != null)
                    {
                        inputModule.NewSource -= InputModule_NewSource;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the reception of a new source from a module.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void InputModule_NewSource(object sender, SourceEventArgs e);

        /// <summary>
        /// Prepares and emits the event of the source going out from this module.
        /// </summary>
        /// <param name="outputSource"></param>
        /// <param name="inputModule"></param>
        /// <param name="outputModule"></param>
        protected void SendSourceOut(Source outputSource, Module inputModule, Module outputModule)
        {
            SourceEventArgs args = new SourceEventArgs();
            args.EmittingModule = inputModule;
            args.IncomingSource = outputSource;
            args.ReceivingModule = outputModule;
            OnNewSource(args);
        }

        public void CheckInput()
        {
            if (InputModule == null || InputModule.Length == 0)
            {
                Debug.LogError("[" + gameObject.name + "]: Please assign a valid input module");
            }
        }

        public void CheckOutput()
        {
            if (OutputModule == null || OutputModule.Length == 0)
            {
                Debug.LogError("[" + gameObject.name + "]: Please assign a valid output module");
            }
        }

    }

}
