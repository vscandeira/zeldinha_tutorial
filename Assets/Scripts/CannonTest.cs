using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CannonTest : MonoBehaviour {
    public GameObject prefab;
    public float interval;
    public float impulse=10f;
    private float cooldown;
    void Start() {
        cooldown = interval;
    }
    void Update(){
        if((cooldown-=Time.deltaTime) < 0){
            cooldown = interval;
            var projectile = Instantiate(prefab, transform.position, transform.rotation);
            var impulseVector = projectile.transform.rotation * Vector3.forward * impulse;
            projectile.GetComponent<Rigidbody>().AddForce(impulseVector,ForceMode.Impulse);
            Destroy(projectile, 6);
        }
    }
}
