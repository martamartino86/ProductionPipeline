using System;
using UnityEngine;

namespace ProductionPipeline
{
    public enum ModuleType
    {
        Assembler,
        Buffer,
        Conveyor,
        FlowSplitter,
        SourceProvider,
        QualityAssurance,
        SourceReceiver
    }

    public abstract class Module : MonoBehaviour
    {
        /// <summary>
        /// Each module is connected to one or more input modules.
        /// </summary>
        public Module[] InputModules;

        /// <summary>
        /// Each module is connected to one or more output modules.
        /// </summary>
        public Module[] OutputModules;

        /// <summary>
        /// Name of this module.
        /// </summary>
        [SerializeField]
        private string _moduleName;
        public string ModuleName 
        {
            get
            { 
                return _moduleName; 
            }
        }

        /// <summary>
        /// Type of module.
        /// </summary>
        public ModuleType ModuleType { get; private set; }

        /// <summary>
        /// If true (user has paused the simulation), the module temporarily stops working
        /// </summary>
        protected bool _paused;
        
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
            if (InputModules.Length > 0)
            {
                foreach (var inputModule in InputModules)
                {
                    if (inputModule == null)
                    {
                        Debug.LogError("[" + name + "]: Please assign valid InputModules!");
                        return;
                    }
                    inputModule.NewSource += InputModule_NewSource;
                }
            }
            PipelineManager.Instance.SimulationPaused += Instance_SimulationPaused;
        }

        protected void OnDisable()
        {
            if (InputModules.Length > 0)
            {
                foreach (var inputModule in InputModules)
                {
                    if (inputModule != null)
                    {
                        inputModule.NewSource -= InputModule_NewSource;
                    }
                }
            }
            if (PipelineManager.Instance != null)
                PipelineManager.Instance.SimulationPaused -= Instance_SimulationPaused;
        }

        protected virtual void Awake()
        {
            if (_moduleName == "")
            {
                Debug.LogError("[" + gameObject.name + " Please assign carefully all the properties in the inspector.");
            }
            SetModuleType();
            TextMesh t = GetComponentInChildren<TextMesh>();
            if (t != null)
                t.text = _moduleName;
        }

        private void SetModuleType()
        {
            if (this is Assembler)
            {
                ModuleType = ModuleType.Assembler;
            }
            else if (this is Buffer)
            {
                ModuleType = ModuleType.Buffer;
            }
            else if (this is Conveyor)
            {
                ModuleType = ModuleType.Conveyor;
            }
            else if (this is FlowSplitter)
            {
                ModuleType = ModuleType.FlowSplitter;
            }
            else if (this is SourceProvider)
            {
                ModuleType = ModuleType.SourceProvider;
            }
            else if (this is SourceReceiver)
            {
                ModuleType = ModuleType.SourceReceiver;
            }
            else if (this is QualityAssurance)
            {
                ModuleType = ModuleType.QualityAssurance;
            }
#if DEBUG_PRINT
            print(ModuleName + " " + ModuleType);
#endif
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

        protected void DataChanged(string newStats)
        {
            PipelineManager.Instance.ModuleDataUpdated(ModuleType, _moduleName, newStats);
        }

        public virtual string GetStats()
        {
            string stats = "<b>" + _moduleName + "</b>";
            if (InputModules.Length > 0)
            {
                stats += "\nInput module(s): ";
                for (int i = 0; i < InputModules.Length; i++)
                {
                    stats += InputModules[i]._moduleName + " ";
                }
            }
            if (OutputModules.Length > 0)
            {
                stats += "\nOutput module(s): ";
                for (int i = 0; i < OutputModules.Length; i++)
                {
                    stats += OutputModules[i]._moduleName + " ";
                }
            }
            stats += "\n";
            return stats;
        }

        public void CheckInput()
        {
            if (InputModules == null || InputModules.Length == 0)
            {
                Debug.LogError("[" + gameObject.name + "]: Please assign a valid input module");
            }
        }

        public void CheckOutput()
        {
            if (OutputModules == null || OutputModules.Length == 0)
            {
                Debug.LogError("[" + gameObject.name + "]: Please assign a valid output module");
            }
        }

        private void Instance_SimulationPaused(object sender, PipelineManager.SimulationEventArgs e)
        {
            _paused = e.IsPaused;
        }
    }

}
