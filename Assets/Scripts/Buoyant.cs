using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyant : MonoBehaviour {
    public float underWaterDrag = 3f;
    public float underWaterAngularDrag = 1f;
    public float airDrag = 0f;
    public float airAngularDrag = 0.05f;
    public float buoyancyForce = 40f;
    private Rigidbody thisRigidbody;
    private bool hasTouchedWater;
    void Awake() {
        thisRigidbody = GetComponent<Rigidbody>();
        hasTouchedWater = false;
    }

    void FixedUpdate() {
        float diffY = transform.position.y;
        bool isUnderwater = diffY < 0;

        if(isUnderwater) hasTouchedWater = true;
        if(!hasTouchedWater) return;

        if(isUnderwater){
            Vector3 vector = Vector3.up * buoyancyForce * -diffY;
            thisRigidbody.AddForce(vector, ForceMode.Acceleration);
        }
        thisRigidbody.drag = isUnderwater ? underWaterDrag : airDrag;
        thisRigidbody.angularDrag = isUnderwater ? underWaterAngularDrag: airAngularDrag;
    }
}
