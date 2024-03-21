//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public List<GameObject> bombPrefabs;
    public Vector2 timeInterval = new Vector2(1,1);
    public GameObject spawnPoint;
    public GameObject target;
    public float rangeInDegrees;
    public Vector2 force;
    public float HeightForce = 45;
    private float coolDown;
    void Start(){
        coolDown = Random.Range(timeInterval.x, timeInterval.y);
    }

    void Update(){
        if(GameManager.Instance.isGameOver) return;
        coolDown -= Time.deltaTime;
        if(coolDown<=0){
            coolDown = Random.Range(timeInterval.x, timeInterval.y);
            Fire();
        }
    }
    private void Fire() {
        GameObject bombPrefab = bombPrefabs[Random.Range(0,bombPrefabs.Count)];
        GameObject bomb = Instantiate(bombPrefab, spawnPoint.transform.position, bombPrefab.transform.rotation);

        Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
        Vector3 impulseVector = target.transform.position - spawnPoint.transform.position;
        impulseVector.Scale(new Vector3(1,0,1));
        impulseVector.Normalize();
        impulseVector += new Vector3(0,HeightForce/180f,0);
        impulseVector.Normalize();
        impulseVector = Quaternion.AngleAxis(rangeInDegrees * Random.Range(-1f, 1f), Vector3.up) * impulseVector;
        impulseVector *= Random.Range(force.x, force.y);
        bombRigidbody.AddForce(impulseVector, ForceMode.Impulse);
    }
}
