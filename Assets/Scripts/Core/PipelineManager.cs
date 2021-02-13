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

        public bool DataIsReady;

        private Dictionary<string, Module> _modules;
        private int _nModules;
        private List<string>[] _modulesNames;
        private WaitForEndOfFrame wait;

        public event EventHandler PipelineObjectsLoaded;
        protected virtual void OnPipelineObjectsLoaded(EventArgs e)
        {
            EventHandler handler = PipelineObjectsLoaded;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Emits this event when the data has changed (useful for UI).
        /// </summary>
        public event EventHandler<DataEventArgs> ChangedData;
        protected virtual void OnChangedData(DataEventArgs e)
        {
            EventHandler<DataEventArgs> handler = ChangedData;
            handler?.Invoke(this, e);
        }
        public class DataEventArgs : EventArgs
        {
            public ModuleType moduleType;
            public string moduleName;
            public string newStats;
        }

        private void Awake()
        {
            StartCoroutine(FillLists());
        }

        IEnumerator FillLists()
        {
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

        public string GetStats(int moduleType, string moduleName)
        {
            return _modules[moduleName].GetStats();
        }

        public void DataUpdated(ModuleType moduleType, string moduleName, string newStats)
        {
            DataEventArgs args = new DataEventArgs();
            args.moduleType = moduleType;
            args.moduleName = moduleName;
            args.newStats = newStats;
            OnChangedData(args);
        }
    }

}
