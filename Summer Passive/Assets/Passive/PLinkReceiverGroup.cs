using Unity.VisualScripting;
using UnityEngine;
using Util;

namespace  Passive {
    public class PLinkReceiverGroup : MonoBehaviour {
    
        [Serialize]
        public SObservableList<PLinkReceiver> linkReceivers;

        private void Awake() {

        }
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}