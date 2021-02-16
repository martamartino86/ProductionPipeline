using UnityEngine;

namespace ProductionPipeline 
{
    public class MouseInteraction : MonoBehaviour
    {
        private Module m;
        private AssembledSource a_s;
        private Source s;
        private PipelineManager _pipelineManager { get { return PipelineManager.Instance; } }

        private void Start()
        {
            m = GetComponentInParent<Module>();
            s = GetComponentInParent<Source>();
        }

        protected void OnMouseDown()
        {
            CheckType();
            if (a_s != null)
            {
                _pipelineManager.MouseSelectedSource(a_s.Id);
            }
            else if (s != null)
            {
                string id = s.Id;
                _pipelineManager.MouseSelectedSource(id);
            }
            else
            {
                _pipelineManager.MouseSelectedModule(m.ModuleType, m.ModuleName);
            }
        }

        private void CheckType()
        {
            if (s != null)
            {
                a_s = GetComponentInParent<AssembledSource>();
            }
        }
    }
    
}
