using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class News : MonoBehaviour
{
    public Transform newsBar;
    public GameObject news;
    public Text article;
    public TimeUIController timeUIController;
    public GameObject textPrefab;
    GameObject latestNews;
    //float timeSpeed;

    CameraController cameraController;
    string cellString;

    public List<string> newsItems = new List<string>();

    void Awake ()
    {
        foreach (GameObject controller in GameObject.FindGameObjectsWithTag("GameController"))
            if (controller.GetComponent<CameraController>())
                cameraController = controller.GetComponent<CameraController>();
        newsItems.Insert(0, "Hello Mayor, Welcome to your city!");
    }

    public void AddToNews (string info)
    {
        if (!latestNews)
        {
            latestNews = Instantiate(textPrefab.gameObject, newsBar); // Instantiate a new headline of given text to the right of the screen.
            latestNews.transform.localPosition = new Vector3(405, 20, 0);
            latestNews.transform.localRotation = new Quaternion(0, 0, 0, 0);
            latestNews.transform.localScale = Vector3.one;
            latestNews.GetComponent<Text>().text = "Breaking News: " + info + " - " + timeUIController.GetTime;

            cellString = info.Split('[')[1];
            cellString = cellString.Split(']')[0];
            cellString.Trim();
            
            //timeSpeed = Time.timeScale; //Reset time till the headline has passed
            timeUIController.ResetSpeed();

            StartCoroutine(ScrollText());

        }
        if (newsItems.Count >= 10)
            newsItems.RemoveAt(0);
        newsItems.Add(info);
    }

    public void View ()
    {
        int x = int.Parse(cellString.Split(',')[0]);
        int y = int.Parse(cellString.Split(',')[1]);
        cameraController.transform.position = new Vector3(-x * 10, 0, -y * 10);
    }

    public void OpenNews ()
    {
        news.SetActive(true);
        news.GetComponent<Selectable>().Select();
        string data = "";
        foreach (string n in newsItems)
            data += " --- " + n + "\n";
        article.text = data;
    }

    public void CloseNews ()
    {
        news.SetActive(false);
    }

    float duration;

    IEnumerator ScrollText ()
    {
        newsBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);   //Moves the News Bar Up onto the screen

        duration = 7f;  //how long it takes to move the text across the screen
        Vector3 currentPos = latestNews.GetComponent<RectTransform>().anchoredPosition;

        //Move the headline across the screen over the duration time
        for (float t = 0.0f; t < 1; t += Time.deltaTime / duration)
        {
            latestNews.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(currentPos, new Vector3(-2000, 20, 0), t);
            yield return null;
        }

        //Lower the news UI and reset the speed, then destroy the news headline thing
        newsBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40);
        //Time.timeScale = timeSpeed;
        Destroy(latestNews.gameObject, 1f);

    }
}
