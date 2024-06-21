using UnityEngine;

public class ShieldHitboxScript : MonoBehaviour{
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other){
        playerController.OnShieldCollisionEnter(other);
    }
}