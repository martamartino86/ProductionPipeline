using System;
using UnityEngine;

namespace ProductionPipeline 
{
    public class SourceProvider : Module
    {
        /// <summary>
        /// If true, it starts the production as soon as the simulation starts, without waiting for the production time.
        /// </summary>
        [SerializeField]
        private bool BeginProductionAtStart = true;
        
        [SerializeField]
        private Source.SourceType _sourceType;
        
        [SerializeField]
        private float _productionTime;

        private Base _baseSourcePrefab;
        private Body _bodySourcePrefab;
        private Detail _detailSourcePrefab;

        /// <summary>
        /// Total amount of source created from the beginning of the simulation.
        /// </summary>
        private int _nSourcesCreated;
        private float _lastCreationTime;

        protected override void Awake()
        {
            base.Awake();
            CheckOutput();
            _baseSourcePrefab = Resources.Load<Base>("Prefabs/BaseSource");
            _bodySourcePrefab = Resources.Load<Body>("Prefabs/BodySource");
            _detailSourcePrefab = Resources.Load<Detail>("Prefabs/DetailSource");
            _nSourcesCreated = 0;
        }

        private void Start()
        {
            if (BeginProductionAtStart)
            {
                CreateSource(_sourceType);
                _lastCreationTime = Time.time;
            }
        }

        void Update()
        {
            if (_paused) return;
            if (Time.time - _lastCreationTime >= _productionTime)
            {
                CreateSource(_sourceType);
                _lastCreationTime = Time.time;
            }
        }

        private void CreateSource(Source.SourceType type)
        {
            try
            {
                BasicSource newSource = null;
                switch (type)
                {
                    case Source.SourceType.Base:
                        newSource = Instantiate(_baseSourcePrefab, transform.position, Quaternion.identity);
                        break;
                    case Source.SourceType.Body:
                        newSource = Instantiate(_bodySourcePrefab, transform.position, Quaternion.identity);
                        break;
                    case Source.SourceType.Detail:
                        newSource = Instantiate(_detailSourcePrefab, transform.position, Quaternion.identity);
                        break;
                    default:
                        break;
                }
                newSource.SetCurrentParent(this, true, false);
                newSource.Initialize(this);
                _nSourcesCreated++;
#if DEBUG_PRINT
                Debug.Log("[" + name + "] generated source " + newSource.name);
#endif
                SendSourceOut(newSource, this, OutputModules[0]);
                DataChanged(GetStats());
            }
            catch (Exception e) { Debug.LogError(e); }
        }

        public override string GetStats()
        {
            string stats = base.GetStats();
            stats += "\nType of source produced: " + _sourceType.ToString() +
                "\nCreate a new source each " + _productionTime.ToString() + " seconds" +
                "\nTotal amount of sources created: " + _nSourcesCreated;
            if (BeginProductionAtStart)
            {
                stats += "\nThis provider begins the production when the simulation starts.";
            }
            return stats;
        }

        protected override void InputModule_NewSource(object sender, SourceEventArgs e)
        {}
    }
}
