using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityManager : MonoBehaviour, ISavable
{
    public static CityManager instance;
    public List<City> cities;

    public GameObject cityPrefab;

    private void Awake() {
        instance = this;
    }

    private void Start() {

    }
    public void LoadData(GameData data)
    {
        foreach (CitySerialized city in data.cities)
        {
            GameObject newCityObject = Instantiate(cityPrefab, Vector3.zero, Quaternion.identity);
            
            City newCity = newCityObject.GetComponent<City>();

            newCity.cityName = city.cityName;
            newCity.population = city.population;
            newCity.workers = city.workers;
            newCity.starvation = city.starving;
            newCity.hungerTimer = city.hungerTimer;
            newCity.hungerDrainModifier = city.hungerDrainModifier;

            newCity.coordinates = new Vector2Int(city.coordinates.x, city.coordinates.y);

            foreach (HappinessSource source in city.happinessSources)
            {
                newCity.AddHappinessSource(source);
            }

            for(int i = 0; i < DataBase.instance.allItems.Count; i++){
                newCity.inventory.Add(DataBase.instance.allItems[i], city.itemAmount[i]);
            }

            for(int i = 0; i < city.industries.Count; i++){
                // Industries
                IndustrySerialized currentIndustry = city.industries[i];
                Industry industry = new();
                industry.industryName = currentIndustry.industryName;
                industry.level = currentIndustry.level;
                
                industry.skillTree = Resources.Load<GameObject>(currentIndustry.skillTree.skillTreePath);

                foreach(SkillSerilaized skill in currentIndustry.skills){
                    Skill newSkill = Resources.Load<Skill>(skill.skillPath);
                    industry.unlockedSkills.Add(newSkill);
                    newSkill.OnLoad(industry);
                }
                industry.skillPoints = currentIndustry.skillPoints;

                industry.Initialize(newCity);

                for(int j = 0; j < DataBase.instance.allItems.Count; j++){
                    industry.itemOutputPerWorker[DataBase.instance.allItems[j]] = currentIndustry.outputPerWorker[j];
                }
                
                newCity.workersPerIndustry.Add(industry, city.workerAmount[i]); 
            }
            cities.Add(newCity);
        }
        foreach (City city in cities)
        {
            Debug.Log(city.workersPerIndustry.ElementAt(0));
        }
    }

    public void SaveData(GameData data)
    {
        data.cities = new();

        foreach (City city in cities)
        {   
            CitySerialized newCity = new();
            newCity.cityName = city.cityName;
            newCity.population = city.population;
            newCity.workers = city.workers;
            newCity.starving = city.starvation;
            newCity.hungerTimer = city.hungerTimer;
            newCity.hungerDrainModifier = city.hungerDrainModifier;

            newCity.coordinates = new(city.coordinates.x, city.coordinates.y);

            foreach (HappinessSource source in city.happinessSources)
            {
                newCity.happinessSources.Add(source);
            }

            foreach (Item item in city.inventory.Keys)
            {
                newCity.itemName.Add(item.itemName);
                newCity.itemAmount.Add(city.inventory[item]);
            }

            foreach (Industry industry in city.workersPerIndustry.Keys)
            {
                List<string> itemNames = new();
                List<float> outputPerWorker = new();
                foreach (Item item in industry.itemOutputPerWorker.Keys)
                {
                    itemNames.Add(item.itemName);
                    outputPerWorker.Add(industry.itemOutputPerWorker[item]);
                }
                
                List<string> skillPaths = new();
                foreach (Skill skill in industry.unlockedSkills)
                {
                    skillPaths.Add("Scriptable Objects/Skills/" + skill.name);
                }
                
                IndustrySerialized newIndustry = new(industry.industryName, industry.level, itemNames, outputPerWorker, "Prefabs/SkillTrees/" + industry.skillTree.name, skillPaths, industry.skillPoints);

                newCity.industries.Add(newIndustry);
                newCity.workerAmount.Add(city.workersPerIndustry[industry]);
            }
            data.cities.Add(newCity);
        }
    }
}
