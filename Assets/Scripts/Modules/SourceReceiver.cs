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
        public TypeOfReceiver ReceiverType { get; private set; }
        private int _nSourceReceived;

        private void Awake()
        {
            CheckInput();
            _nSourceReceived = 0;
        }


        /// <summary>
        /// Behaves depending from the type.
        /// </summary>
        private void ReceiveSource(Source source)
        {
            switch (ReceiverType)
            {
                case TypeOfReceiver.Storer:

                    break;
                case TypeOfReceiver.Destroyer:
                    
                    break;
                default:
                    break;
            }
            _nSourceReceived++;
        }

        /// <summary>
        /// Receiving an input source from an InputModule TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void InputModule_NewSource(object sender, SourceEventArgs e)
        {
            Source inputSource = e.IncomingSource;
            inputSource.transform.SetParent(transform);
            inputSource.transform.localPosition = Vector3.zero;
            ReceiveSource(inputSource);
        }
    }

}

