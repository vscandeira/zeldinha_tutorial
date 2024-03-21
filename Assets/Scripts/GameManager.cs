using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    public bool isGameOver {get;private set;}
    private static readonly string NAME_HIGHEST_SCORE = "HighestScore";
    [Header("Audio")]
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource gameOverPlayer;
    [Header("Score")]
    [SerializeField] private float score;
    [SerializeField] private int highestScore;
    void Awake() {
        if(Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
            isGameOver = false;
        }
        score = 0;
        highestScore = PlayerPrefs.GetInt(NAME_HIGHEST_SCORE);
    }

    void Update() {
        if(!isGameOver) {
            score += Time.deltaTime;
            if(GetScore() >= GetHighestScore()) {
                highestScore = GetScore();
            }
        }
    }

    public int GetScore(){
        return (int) Mathf.Floor(score);
    }

    public int GetHighestScore() {
        return highestScore;
    }

    public void EndGame(){
        if(isGameOver) return;
        isGameOver = true;
        musicPlayer.Stop();
        gameOverPlayer.Play();

        int prevHigh = PlayerPrefs.GetInt(NAME_HIGHEST_SCORE);
        if(prevHigh < GetHighestScore()) {
            PlayerPrefs.SetInt(NAME_HIGHEST_SCORE, GetHighestScore());
        }

        StartCoroutine(ReloadScene(7));
    }

    private IEnumerator ReloadScene(float delay){
        yield return new WaitForSeconds(delay);

        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }
}
