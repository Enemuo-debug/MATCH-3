using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;


public class GameOver : MonoBehaviour
{
    public GameObject screenObj;
    public GameObject scoreObj;
    public TextMeshProUGUI tryAgainText;
    public TextMeshProUGUI youWinText;
    public TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        screenObj.SetActive(false);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowLose()
    {
        screenObj.SetActive(true);
        scoreObj.SetActive(false);
        youWinText.enabled = false;

        Animator anim = GetComponent<Animator>();
        if (anim)
        {
            anim.Play("GameOverAnim");
        }
    }
    public void ShowWin(int score)
    {
        screenObj.SetActive(true);
        tryAgainText.enabled = false;
        youWinText.enabled = true;

        scoreText.text = score.ToString();
        scoreText.enabled = true;

        Animator anim = GetComponent<Animator>();
        if (anim)
        {
            anim.Play("GameOverAnim");
        }
    }
    public void OnReplayClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnNextClicked()
    {
        SceneManager.LoadScene(0);
    }
}
