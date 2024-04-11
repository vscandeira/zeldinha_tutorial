using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    public bool isGameOver {get;private set;}
    [Header("Physics")]
    public LayerMask groundLayer;
    void Awake() {
        if(Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
            isGameOver = false;
        }
    }

    void Update() {
    }

}
