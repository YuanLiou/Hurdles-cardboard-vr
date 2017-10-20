using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public TextMesh infoText;
    public Player player;

    private float gameTimer = 0f;
    private float restartTimer = 3f;

    // Use this for initialization
    void Start() { }

    // Update is called once per frame
    void Update() {
        if (!player.reachedFinishLine) {
            gameTimer += Time.deltaTime;
            infoText.text = "Avoid the obstacles!\nPress button to jump\nTime: " + Mathf.Floor(gameTimer);
        } else {
            infoText.text = "Game over\nYour time: " + Mathf.Floor(gameTimer);
            player.Decelerate(0.1f);
            restartTimer -= Time.deltaTime;
            if (restartTimer <= 0f) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}