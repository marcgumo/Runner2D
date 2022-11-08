using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MenuManager : MonoBehaviour
{
    public class CertificateWhore : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    [System.Serializable]
    public class WorldProperties
    {
        public string WorldName;
        public int idScene;
        public bool isEnable;
        public Button button;

        public string tableName;

        public List<ScoreProperties> scoreList;
    }

    [System.Serializable]
    public class ScoreProperties
    {
        public string userName;
        public int score;
        public int time;
    }

    bool titleScreen;

    public TextMeshProUGUI worldName;
    public int currentWorld = 0;

    public List<WorldProperties> worldList;

    public GameObject worldInfo;

    string getScorePath = "https://localhost/Runner2D/getscore.php";

    public Transform contentPosition;
    public GameObject userScorePrefab;

    void Start()
    {
        currentWorld = PlayerPrefs.GetInt("CurrentWorld");

        SelectWorld(currentWorld);

        if (currentWorld == 0)
        {
            titleScreen = true;
        }

        SetWorlds();
    }

    private void Update()
    {
        TitleScreen();
    }

    public void SelectWorld(int world)
    {
        currentWorld = world;

        worldName.text = worldList[world].WorldName;

        StartCoroutine(GetScoreFromDB());
    }

    public void ShowWorldInfo()
    {
        if (!worldInfo.activeSelf)
        {
            worldInfo.SetActive(true);
        }
    }

    public void SetWorlds()
    {
        PlayerPrefs.SetInt("World" + 0, 1);
        
        for (int i = 0; i < worldList.Count; i++)
        {
            worldList[i].isEnable = PlayerPrefs.GetInt("World" + i) == 1;


            worldList[i].button.interactable = worldList[i].isEnable;
        }
    }

    public void DeleteSaveWorld()
    {
        for (int i = 0; i < worldList.Count; i++)
        {
            PlayerPrefs.DeleteKey("World" + i);
        }

        SetWorlds();
        SelectWorld(0);

        PlayerPrefs.DeleteKey("CurrentWorld");
    }

    public void PlayWorld()
    {
        PlayerPrefs.SetInt("CurrentWorld", currentWorld);
        
        SceneManager.LoadScene(worldList[currentWorld].idScene);
    }

    private void TitleScreen()
    {
        if (Input.GetButtonDown("Jump"))
        {
            titleScreen = false;
            GameObject.Find("TitleScreen").SetActive(false);
        }
    }

    IEnumerator GetScoreFromDB()
    {
        WWWForm form = new WWWForm();

        form.AddField("tablename", worldList[currentWorld].tableName);

        UnityWebRequest request = UnityWebRequest.Post(getScorePath, form);

        request.certificateHandler = new CertificateWhore();

        yield return request.SendWebRequest();

        SplitTable(request.downloadHandler.text);

        request.Dispose();
    }

    private void SplitTable(string text)
    {
        DestroyChildsContent(contentPosition);
        
        worldList[currentWorld].scoreList.Clear();

        string[] rows = text.Split(new string[] { "/()/" }, System.StringSplitOptions.None);

        for (int i = 0; i < rows.Length - 1; i++)
        {
            string[] columns = rows[i].Split(new string[] { "/_/" }, System.StringSplitOptions.None);

            ScoreProperties userToSubmit = new ScoreProperties();

            userToSubmit.userName = columns[0];
            userToSubmit.score = int.Parse(columns[1]);
            userToSubmit.time = int.Parse(columns[2]);

            worldList[currentWorld].scoreList.Add(userToSubmit);

            GameObject scoreToSubmit = Instantiate(userScorePrefab, contentPosition);

            scoreToSubmit.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = userToSubmit.userName;
            scoreToSubmit.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Score: " + userToSubmit.score.ToString();
            scoreToSubmit.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Time: " + userToSubmit.time.ToString();
        }
    }

    private void DestroyChildsContent(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
