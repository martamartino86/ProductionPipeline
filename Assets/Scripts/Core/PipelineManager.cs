using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProductionPipeline
{
    public class PipelineManager : MonoBehaviour
    {
        public static PipelineManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PipelineManager>();

                }
                return _instance;
            }
        }
        private static PipelineManager _instance;

        public bool SimulationIsPaused {
            get
            {
                return _simulationIsPaused; 
            } 
            set
            {
                _simulationIsPaused = value;
                SimulationEventArgs e = new SimulationEventArgs();
                e.IsPaused = value;
                OnSimulationPaused(e);
            }
        }
        private bool _simulationIsPaused;

        private Dictionary<string, Module> _modules;
        private int _nModules;
        private List<string>[] _modulesNames;
        
        private Dictionary<string, Source> _sourcesInPipeline;

        private WaitForEndOfFrame wait;

        /// <summary>
        /// Emits this event when the structures have been filled (useful for UI).
        /// </summary>
        public event EventHandler PipelineObjectsLoaded;
        protected virtual void OnPipelineObjectsLoaded(EventArgs e)
        {
            EventHandler handler = PipelineObjectsLoaded;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Emits this event when the module data has changed (useful for UI).
        /// </summary>
        public event EventHandler<ModuleEventArgs> ChangedModuleData;
        protected virtual void OnChangedModuleData(ModuleEventArgs e)
        {
            EventHandler<ModuleEventArgs> handler = ChangedModuleData;
            handler?.Invoke(this, e);
        }
        public class ModuleEventArgs : EventArgs
        {
            public ModuleType moduleType;
            public string moduleName;
            public string newStats;
        }

        /// <summary>
        /// Emits this event when a source has been created/destroyed (useful for UI).
        /// </summary>
        public event EventHandler<SourceEventArgs> ChangedSourceData;
        protected virtual void OnChangedSourceData(SourceEventArgs e)
        {
            EventHandler<SourceEventArgs> handler = ChangedSourceData;
            handler?.Invoke(this, e);
        }
        public class SourceEventArgs : EventArgs
        {
            public string sourceId;
            public bool newSource;
            public string newStats;
        }

        /// <summary>
        /// Emits this event when the user has clicked on the PauseSimulation checkbox (useful for Modules and Sources).
        /// </summary>
        public event EventHandler<SimulationEventArgs> SimulationPaused;
        protected virtual void OnSimulationPaused(SimulationEventArgs e)
        {
            EventHandler<SimulationEventArgs> handler = SimulationPaused;
            handler?.Invoke(this, e);
        }
        public class SimulationEventArgs : EventArgs
        {
            public bool IsPaused;
        }


        public event EventHandler<MouseModuleClickedArgs> MouseClickedModule;
        public event EventHandler<MouseSourceClickedArgs> MouseClickedSource;
        protected virtual void OnMouseClickedModule(MouseModuleClickedArgs e)
        {
            EventHandler<MouseModuleClickedArgs> handler = MouseClickedModule;
            handler?.Invoke(this, e);
        }
        protected virtual void OnMouseClickedSource(MouseSourceClickedArgs e)
        {
            EventHandler<MouseSourceClickedArgs> handler = MouseClickedSource;
            handler?.Invoke(this, e);
        }
        public class MouseSourceClickedArgs : EventArgs
        {
            public string id;
        }
        public class MouseModuleClickedArgs : EventArgs
        {
            public int type;
            public string name;
        }

        private void Awake()
        {
            StartCoroutine(FillLists());
        }

        IEnumerator FillLists()
        {
            _sourcesInPipeline = new Dictionary<string, Source>();

            _modules = new Dictionary<string, Module>();
            foreach (var m in FindObjectsOfType<Module>())
            {
                _modules.Add(m.ModuleName, m);
                yield return wait;
            }
            _nModules = Enum.GetValues(typeof(ModuleType)).Length;
            _modulesNames = new List<string>[_nModules];
            foreach (var m in _modules)
            {
                Module module = m.Value;
                if (_modulesNames[(int)module.ModuleType] == null)
                {
                    _modulesNames[(int)module.ModuleType] = new List<string>();
                }
                _modulesNames[(int)module.ModuleType].Add(module.ModuleName);
                yield return wait;
            }
            for (int i = 0; i < _nModules - 1; i++)
            {
                // hierarchy order
                _modulesNames[i].Reverse();
                yield return wait;
            }
            OnPipelineObjectsLoaded(new EventArgs());
        }

        public void AddSource(Source source)
        {
            _sourcesInPipeline.Add(source.Id, source);
            SourceEventArgs e = new SourceEventArgs();
            e.sourceId = source.Id;
            e.newSource = true;
            e.newStats = source.GetStats();
            OnChangedSourceData(e);
        }

        public void ModifySource(string sourceId, string newStats)
        {
            SourceEventArgs e = new SourceEventArgs();
            e.sourceId = sourceId;
            e.newSource = false;
            e.newStats = newStats;
            OnChangedSourceData(e);
        }

        public void RemoveSource(string sourceId)
        {
            // check dictionary because of OnDestroy
            if (_sourcesInPipeline != null)
            {
                _sourcesInPipeline.Remove(sourceId);
                SourceEventArgs e = new SourceEventArgs();
                e.sourceId = sourceId;
                e.newSource = false;
                e.newStats = "";
                OnChangedSourceData(e);
            }
        }

        /// <summary>
        /// Returns the names of the Module Types.
        /// </summary>
        /// <returns></returns>
        public List<string> GetModuleTypesNames()
        {
            int nTypes = Enum.GetValues(typeof(ModuleType)).Length;
            List<string> modTypeNames = new List<string>();
            for (int i = 0; i < nTypes; i++)
            {
                modTypeNames.Add(Enum.GetValues(typeof(ModuleType)).GetValue(i).ToString());
            }
            return modTypeNames;
        }

        /// <summary>
        /// Given the name of the type of module, returns all the names of the instances of that module.
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public List<string> GetModulesNames(int moduleType)
        {
            return _modulesNames[moduleType];
        }

        public List<string> GetSourcesNames()
        {
            return new List<string>(_sourcesInPipeline.Keys);
        }

        public string GetSourceStats(string sourceId)
        {
            if (_sourcesInPipeline.ContainsKey(sourceId))
            {
                return _sourcesInPipeline[sourceId].GetStats();
            }
            else return "";
        }

        public string GetModuleStats(int moduleType, string moduleName)
        {
            return _modules[moduleName].GetStats();
        }

        public void ModuleDataUpdated(ModuleType moduleType, string moduleName, string newStats)
        {
            ModuleEventArgs args = new ModuleEventArgs();
            args.moduleType = moduleType;
            args.moduleName = moduleName;
            args.newStats = newStats;
            OnChangedModuleData(args);
        }

        public void MouseSelectedSource(string id)
        {
            MouseSourceClickedArgs e = new MouseSourceClickedArgs();
            e.id = id;
            OnMouseClickedSource(e);
        }
        public void MouseSelectedModule(ModuleType moduleType, string moduleName)
        {
            MouseModuleClickedArgs e = new MouseModuleClickedArgs();
            e.type = (int)moduleType;
            e.name = moduleName;
            OnMouseClickedModule(e);
        }
    }

}
