using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class UIController : MonoBehaviour
{
    public class CertificateWhore : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    [Header("Score Settings")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public TMP_InputField userNameText;
    public TextMeshProUGUI scoreGoal;

    [Header("Game Over Settings")]
    public GameObject gameOverPanel;

    [Header("Database Settings")]
    public string tableName;
    string setScorePath = "https://localhost/Runner2D/setscore.php";

    float score = 0;
    float time = 0;

    void Start()
    {
        gameOverPanel.SetActive(false);
        Time.timeScale = 1;
    }

    void Update()
    {
        time += Time.deltaTime;
        timeText.text = time.ToString("F0");

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendScore();
        }
    }

    public void SetScore(int _score)
    {
        score += _score;
        scoreText.text = score.ToString();
    }

    public float GetScore()
    {
        return score;
    }

    public void SetScoreGoal(int _scoreGoal)
    {
        scoreGoal.text = score.ToString() + " / " + _scoreGoal.ToString();
    }

    public void SetGameOver()
    {
        ShowPanel();
        gameOverPanel.transform.Find("GameOverText").gameObject.SetActive(true);
        gameOverPanel.transform.Find("FinishedText").gameObject.SetActive(false);
    }

    public void SetWorldFinished()
    {
        ShowPanel();
        gameOverPanel.transform.Find("GameOverText").gameObject.SetActive(false);
        gameOverPanel.transform.Find("FinishedText").gameObject.SetActive(true);
    }

    private void ShowPanel()
    {
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
    }

    public void SendScore()
    {
        if (userNameText.text == "")
        {
            return;
        }

        StartCoroutine(SetScoreToDB());
    }

    IEnumerator SetScoreToDB()
    {
        WWWForm form = new WWWForm();

        form.AddField("tablename", tableName);
        form.AddField("username", userNameText.text);
        form.AddField("score", scoreText.text);
        form.AddField("time", timeText.text);

        UnityWebRequest request = UnityWebRequest.Post(setScorePath, form);

        request.certificateHandler = new CertificateWhore();

        yield return request.SendWebRequest();

        SceneManager.LoadScene(0);
    }
}
