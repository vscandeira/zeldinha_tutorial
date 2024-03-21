using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour
{
    public int maxHealth;
    //[HideInInspector] 
    public int health;
    void Start() {
        health = maxHealth;       
    }

    void Update(){
    }
}
