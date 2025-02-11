using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AgeDropdown : MonoBehaviour
{
    [SerializeField] TMP_Dropdown ageDropdown;
    public int minAge = 1;
    public int maxAge = 100;
    void Start()
    {
        FillAgesDropdown();
    }
    void FillAgesDropdown()
    {
        ageDropdown.ClearOptions();
        for (int i = minAge; i < maxAge; i++)
        {
            ageDropdown.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }
        ageDropdown.value = 0;
        ageDropdown.captionText.text = ageDropdown.options[0].text;
    }
}
