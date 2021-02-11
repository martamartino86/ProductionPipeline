using UnityEngine;

namespace ProductionPipeline
{
    public class SourceReceiver : Module
    {
        public enum TypeOfReceiver
        {
            Storer,
            Destroyer
        }
        public TypeOfReceiver _type;

        private void Awake()
        {
            CheckInput();
            switch (_type)
            {
                case TypeOfReceiver.Storer:
                    gameObject.AddComponent<SourceStorer>();
                    break;
                case TypeOfReceiver.Destroyer:
                    gameObject.AddComponent<SourceDestroyer>();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Receiving an input source from an InputModule TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void InputModule_NewSource(object sender, SourceEventArgs e) { }
    }
}
