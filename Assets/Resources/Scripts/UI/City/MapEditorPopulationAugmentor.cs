using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapEditorPopulationAugmentor : MonoBehaviour
{
    public TMP_InputField textBox;
    public void UpdatePopulation(){
        if (textBox.text == "") return;

        // Parse int
        int newPopulation = int.Parse(textBox.text);
        textBox.text = "";

        // Update information
        CityMenu.instance.currentCity.SetPopulation(newPopulation);
        CityMenu.instance.populationText.text = newPopulation.ToString();
    }
}
