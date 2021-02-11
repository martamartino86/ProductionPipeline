using System;
using UnityEngine;

namespace ProductionPipeline 
{
    public class SourceProvider : Module
    {
        bool PRODUCE = true;

        public Source.SourceType _sourceType;
        public float _intervalInSeconds;

        private Base _baseSourcePrefab;
        private Body _bodySourcePrefab;
        private Detail _detailSourcePrefab;

        private float _lastCreationTime;
        private int _nSources;

        private void Awake()
        {
            CheckOutput();
            _baseSourcePrefab = Resources.Load<Base>("Prefabs/BaseSource");
            _bodySourcePrefab = Resources.Load<Body>("Prefabs/BodySource");
            _detailSourcePrefab = Resources.Load<Detail>("Prefabs/DetailSource");
            _nSources = 0;
        }

        private void Start()
        {
            _lastCreationTime = Time.time;
        }

        void Update()
        {
            // every deltaCreationSeconds, it generates a new source // DEBUG
            if (PRODUCE && Time.time - _lastCreationTime >= _intervalInSeconds)
            {
                CreateSource(_sourceType);
                _nSources++;
                _lastCreationTime = Time.time;
                PRODUCE = false;
            }
        }

        private void CreateSource(Source.SourceType type)
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
            newSource.Initialize();
            Debug.Log("[" + name + "] generated source " + newSource.name);
            // notify the output that I'm passing a source
            // ASSUMPTION: FOR NOW, SourceProvider HANDLES ONLY ONE OUTPUT MODULE.
            // IF MORE THAN ONE MODULE IS SET VIA EDITOR, EVERY OUTPUT MODULE WILL BE IGNORED EXCEPT THE FIRST ONE.
            SendSourceOut(newSource, this, OutputModule[0]);
        }
       
        // SourceProvider non ha moduli in Input, per cui non riceverà mai niente.
        // Implementa il metodo solo perché deve.
        protected override void InputModule_NewSource(object sender, SourceEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
