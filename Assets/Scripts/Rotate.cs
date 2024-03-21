using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float DegreesPerSecond = 10f;
    void Update() {
        if(GameManager.Instance.isGameOver) return;
        float deg = DegreesPerSecond * Time.deltaTime;
        transform.Rotate(0, -deg, 0);
    }
}
