using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGravityBody : MonoBehaviour {

    public FakeGravity attractor;
    private Transform objTransform;
    
    // Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        GetComponent<Rigidbody>().useGravity = false;
        objTransform = transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        attractor.Attract(objTransform);
	}
}
