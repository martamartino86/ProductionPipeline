using UnityEngine;

namespace ProductionPipeline 
{
    public class MouseInteraction : MonoBehaviour
    {
        Module m;
        AssembledSource a_s;
        Source s;

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
                PipelineManager.Instance.MouseSelectedSource(a_s.Id);
            }
            else if (s != null)
            {
                string id = s.Id;
                PipelineManager.Instance.MouseSelectedSource(id);
            }
            else
            {
                PipelineManager.Instance.MouseSelectedModule(m.ModuleType, m.ModuleName);
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
