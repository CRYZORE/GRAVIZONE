using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGravity : MonoBehaviour {

    public float gravity = -10;

    public void Attract(Transform objBody)
    {
        // set planet gravity direction for the object body
        Vector3 gravityDir = (objBody.position - transform.position).normalized;
        Vector3 bodyUp = objBody.up;
        // apply gravity to objects rigidbody
        objBody.GetComponent<Rigidbody>().AddForce(gravityDir * gravity);
        // update the objects rotation in relation to the planet
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityDir) * objBody.rotation;
        // smooth rotation
        objBody.rotation = Quaternion.Slerp(objBody.rotation, targetRotation, 50 * Time.fixedDeltaTime);
    }
}
