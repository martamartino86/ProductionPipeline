using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProductionPipeline
{
    public class SourceStorer : SourceReceiver
    {
        public Transform Input;

        private Queue<BasicSource> _storedSources;

        // Start is called before the first frame update
        void Awake()
        {
            _storedSources = new Queue<BasicSource>();
        }

        /// <summary>
        /// Stores the source. TODO
        /// </summary>
        public void ReceiveSource(BasicSource source)
        {
            _storedSources.Enqueue(source);
        }

        /// <summary>
        /// Receiving an input source from an InputModule TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void InputModule_NewSource(object sender, SourceEventArgs e)
        {
            Source inputSource = e.IncomingSource;
            inputSource.transform.SetParent(transform, true);
        }
    }

}
