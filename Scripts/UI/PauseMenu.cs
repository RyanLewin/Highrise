using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;

[XmlRoot("ItemCollection")]
public class ItemContainer
{
    [XmlArray("Items")]
    [XmlArrayItem("Item")]
    public List<Item> items = new List<Item>();

    public static ItemContainer Load(string path)
    {
        TextAsset _xml = Resources.Load<TextAsset>(path);
        XmlSerializer serializer = new XmlSerializer(typeof(ItemContainer));
        StringReader reader = new StringReader(_xml.text);
        ItemContainer items = serializer.Deserialize(reader) as ItemContainer;
        reader.Close();
        return items;
    }
}

public class Item
{
    [XmlAttribute("title")]
    public string title;
    [XmlElement("Info")]
    public string info;
    [XmlElement("Image")]
    public string imagePath;
}

public class PauseMenu : MonoBehaviour {

    public GameObject ui;
    public GameObject pauseUI;
    public GameObject helpUI;
    public GameObject notHelpUI;

    bool paused = false;
    bool help = false;
    
    float time = 1;
    
    public Text title;
    public Text info;
    public Image image;
    const string path = "Scripts/UI/HelpData";
    ItemContainer ic;

    void Start ()
    {
        ic = ItemContainer.Load(path);

        Item item = ic.items[0];
        title.text = item.title;
        info.text = item.info;
        if (item.imagePath != "")
        {
            image.enabled = true;
            Sprite newSprite = Resources.Load<Sprite>(item.imagePath);
            if (newSprite == null)
                Debug.Log(item.imagePath);
            image.sprite = newSprite;
        }
        else
            image.enabled = false;
    }

    public void SetHelp(GameObject titleObj)
    {
        foreach (Item item in ic.items)
        {
            if (item.title == titleObj.name)
            {
                title.text = item.title;
                info.text = item.info;
                if (item.imagePath != "")
                {
                    image.enabled = true;
                    Sprite newSprite = Resources.Load<Sprite>(item.imagePath);
                    image.sprite = newSprite;
                }
                else
                    image.enabled = false;
                return;
            }
        }
    }
    
    public void ToMainMenu()
    {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

    public void ExitGame ()
    {
        Application.Quit();
    }

    public void SwapMenus()
    {
        paused = !paused;
        if (paused)
        {
            time = Time.timeScale;
            Time.timeScale = 0;
            ui.SetActive(false);
            pauseUI.SetActive(true);
            gameObject.GetComponent<Statistics>().TimePlayed = System.DateTime.Now;
        }
        else
        {
            Time.timeScale = time;
            ui.SetActive(true);
            pauseUI.SetActive(false);
        }
    }

    public void SwapHelp(GameObject ui)
    {
        help = !help;
        
        notHelpUI.SetActive(!help);
        ui.SetActive(help);
        /*
        if (help)
        {
            notHelpUI.SetActive(false);
            ui.SetActive(true);
        }
        else
        {
            notHelpUI.SetActive(true);
            ui.SetActive(false);
        }
        */
    }
}
