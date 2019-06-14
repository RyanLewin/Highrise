using System;   // For Exceptions
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuDropdown : MonoBehaviour
{
    public ScrollRect itemDisplayer;
    public List<RectTransform> categories;
    int curr;

    private void OnEnable()
    {
        foreach (RectTransform rt in categories)
            rt.gameObject.SetActive(false);

        if (categories.Count == 0)
            this.gameObject.SetActive(false);

        categories[curr].gameObject.SetActive(true);   // activate the first RectTransform in categories
    }

    public void ValueChanged()
    {
        int val = GetComponent<Dropdown>().value;
        
        try
        {
            foreach (RectTransform rt in categories)
                rt.gameObject.SetActive(false);
            categories[val].gameObject.SetActive(true); // !
            //itemDisplayer.content.gameObject.SetActive(false);
            //itemDisplayer.content = categories[val];
            curr = val;
        }
        catch(ArgumentOutOfRangeException e)
        {
            Debug.Log("ArgumentOutOfRange Exception caught: " + e.Data);
        }
    }

    public void OnMouseEnter(GameObject background)
    {
        background.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
    }

    public void OnMouseExit(GameObject background)
    {
        background.GetComponent<Image>().color = new Color(0, 0, 0);
    }
}
