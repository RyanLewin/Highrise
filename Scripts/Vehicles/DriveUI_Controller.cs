using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class DriveUI_Controller : MonoBehaviour {

    Transform bulletsUI;
    int bullets = 6;

    void Awake()
    {
        bulletsUI = transform.GetChild(1);
    }

    IEnumerator BulletsUI ()
    {
        float duration;
        if (bullets != 6)
        {
            //Lerp rotation of shown image based on how many bullets in gun
            duration = 0.2f;
            for (float t = 0.0f; t < duration; t += Time.deltaTime)
            {
                bulletsUI.GetChild(1).GetComponent<Image>().fillAmount = Mathf.Lerp(currentFill, toFill, t / duration);
                yield return null;
            }
        }
        else
        {
            //Out of bullets, reset image to show all and set to red till bullets are full
            bulletsUI.GetChild(1).GetComponent<Image>().fillAmount = 1f;
            bulletsUI.GetChild(1).GetComponent<Image>().color = Color.red;
            yield return new WaitForSeconds(3f);
            bulletsUI.GetChild(1).GetComponent<Image>().color = Color.white;
        }
    }
    
    float currentFill = 1;
    float toFill = 1;

    public int Bullets
    {
        get { return bullets; }
        set
        {
            bullets = value;
            //Change UI to show how many bullets are available
            bulletsUI.GetChild(0).GetComponent<Text>().text = bullets.ToString();

            //how much of the bullets image is currently filled and how much needs to be filled
            currentFill = bullets != 6 ? (bullets + 1) / 6f : 1f;
            toFill = bullets / 6f;
            StartCoroutine(BulletsUI());
        }
    }

    public void Cop (bool cop)
    {
       if (cop)
            transform.GetChild(1).gameObject.SetActive(true);
       else
            transform.GetChild(1).gameObject.SetActive(false);
    }
}
