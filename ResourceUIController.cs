using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUIController : MonoBehaviour
{
    ResourceManager resources;

    [SerializeField]
    private GameObject coinPanel, populationPanel, happinessPanel;
    private Text coinText, populationText, happinessText;

    private void Start()
    {
        AnimationSetUp();

        resources = GameObject.FindGameObjectWithTag("GameController").GetComponent<ResourceManager>();
        if(!resources)
            this.gameObject.SetActive(false);

        coinText = coinPanel.GetComponentInChildren(typeof(Text), true) as Text;
        populationText = populationPanel.GetComponentInChildren(typeof(Text), true) as Text;
        happinessText = happinessPanel.GetComponentInChildren(typeof(Text), true) as Text;
    }

    private void Update()
    {
        //  Maybe make changes to ResourceManager script so that when a value changes it notifies this script to update
        coinText.text = resources.creative ? "Coins: Inf" : "Coins: " + resources.money;
        populationText.text = "Population: " + resources.population + " / " + resources.maxPopulation;
        happinessText.text = "Happiness: " + resources.happiness + "%";
    }

    public void ToggleDisplay(Animator anim)
    {
        if (anim)
            anim.SetBool("display", !anim.GetBool("display"));
    }

    public void ToggleLock(Animator anim)
    {
        if (anim)
            anim.SetBool("locked", !anim.GetBool("locked"));
    }

    private void AnimationSetUp()
    {
        AnimationStartState(coinPanel);
        AnimationStartState(populationPanel);
        AnimationStartState(happinessPanel);
    }

    private void AnimationStartState(GameObject setStateOf)
    {
        Animator anim = null;

        if (setStateOf)
        {
            anim = setStateOf.GetComponentInParent<Animator>();
            if (anim)
            {
                anim.SetBool("display", false);
                anim.SetBool("locked", false);
            }
        }
    }
}
