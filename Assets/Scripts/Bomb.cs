using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float ExplosionDelay = 3f;
    public List<GameObject> Exp;
    public GameObject WoodBreakingPrefab;
    public float BlastRadius = 3f;
    public float BlastDamage=6f;
    
    void Start() {
        StartCoroutine(ExpDelay(ExplosionDelay));   
    }

    private IEnumerator ExpDelay(float delay){
        yield return new WaitForSeconds(delay);
        Explosion();
    }

    private void Explosion(){
        // Destroy bomb
        Destroy(gameObject);

        // Create explosion
        Vector3 pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        Quaternion q = new Quaternion(0,0,0,0);
        Instantiate(Exp[0], pos, q);

        // Destroy plataforms
        Collider[] colliders = Physics.OverlapSphere(transform.position, BlastRadius);
        foreach(Collider collider in colliders) {
            GameObject hitObject = collider.gameObject;
            if (hitObject.CompareTag("Plataform")) {
                Life life = hitObject.GetComponent<Life>();
                if(life != null) {
                    float distance = (hitObject.transform.position - transform.position).magnitude;
                    float distanceRate = Mathf.Clamp(distance/BlastRadius, 0, 1);
                    float damageRate = 1f - Mathf.Pow(distanceRate, 4);
                    int damage = (int) Mathf.Ceil(damageRate * BlastDamage);
                    life.health -= damage;
                    if(life.health <= 0) {
                        Instantiate(WoodBreakingPrefab, hitObject.transform.position, WoodBreakingPrefab.transform.rotation);
                        Destroy(hitObject);
                    }
                }
            }
        }

    }
}
