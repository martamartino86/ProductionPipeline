using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProductionPipeline
{
    public class SourceDestroyer : SourceReceiver
    {
        public Transform Input;

        /// <summary>
        /// Total number of sources destroyed
        /// </summary>
        private int _nDestroyedSources = 0;

        /// <summary>
        /// Destroy the source. TODO
        /// </summary>
        private void ReceiveSource(Source source)
        {
            _nDestroyedSources++;
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
            ReceiveSource(inputSource);
        }
    }

}
