using System.Collections;
using System.Collections.Generic;
using Passive;
using UnityEngine;
using UnityEngine.UI;

public class PassiveTree : MonoBehaviour {
    
    public  List<PassivePoint> passivePoints;
    public List<PassiveLink> PassiveLinks;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var passiveLink in PassiveLinks) {
            passivePoints[passiveLink.id0].linked.Add(passivePoints[passiveLink.id1]);
            passivePoints[passiveLink.id1].linked.Add(passivePoints[passiveLink.id0]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Serializable]
    public struct PassiveLink {
        public int id0;
        public int id1;
    }
}
