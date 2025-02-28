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
            newCity.starvationDeathResetTimer = city.starvationDeathResetTimer;
            newCity.deathPenalty = city.deathPenalty;
            newCity.hungerTimer = city.hungerTimer;
            newCity.hungerDrainModifier = city.hungerDrainModifier;

            // Event Saving
            newCity.eventTimer = city.eventTimer;
            newCity.activeEvent = city.activeEvent;

            newCity.coordinates = new Vector2Int(city.coordinates.x, city.coordinates.y);

            newCityObject.transform.position = MapManager.instance.tiles[city.coordinates.x, city.coordinates.y].transform.position;
            newCityObject.transform.position += new Vector3(0, 0.75f, 0);

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

                industry.Initialize(newCity);

                // Load all unlocked skills
                foreach(SkillSerilaized skill in currentIndustry.unlockedSkills){
                    Skill newSkill = Resources.Load<Skill>(skill.skillPath);
                    industry.unlockedSkills.Add(newSkill);
                    newSkill.OnLoad(industry);
                }
                // Load all active skills
                foreach(SkillSerilaized skill in currentIndustry.activeSkills){
                    Skill newSkill = Resources.Load<Skill>(skill.skillPath);
                    industry.activeSkills.Add(newSkill);
                    newSkill.OnLoad(industry);
                }
                industry.skillPoints = currentIndustry.skillPoints;

                
                
                newCity.workersPerIndustry.Add(industry, city.workerAmount[i]); 
            }

            cities.Add(newCity);

            if (newCity.activeEvent != ""){
                newCity.SpawnEvent(newCity.activeEvent);
                newCity.SetEventBubbleTimer(city.eventBubbleTimer);
            }
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
            newCity.starvationDeathResetTimer = city.starvationDeathResetTimer;
            newCity.deathPenalty = city.deathPenalty;
            newCity.hungerTimer = city.hungerTimer;
            newCity.hungerDrainModifier = city.hungerDrainModifier;

            // Event Saving
            newCity.eventTimer = city.eventTimer;
            newCity.activeEvent = city.activeEvent;
            if (newCity.activeEvent != "") newCity.eventBubbleTimer = city.decisionBubble.timeLeft;

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
                
                List<string> unlockedSkillPaths = new();
                foreach (Skill skill in industry.unlockedSkills)
                {
                    unlockedSkillPaths.Add("Scriptable Objects/Skills/" + skill.name);
                }
                List<string> activeSkillPaths = new();
                foreach (Skill skill in industry.activeSkills)
                {
                    activeSkillPaths.Add("Scriptable Objects/Skills/" + skill.name);
                }
                
                IndustrySerialized newIndustry = new(industry.industryName, industry.level, itemNames, outputPerWorker, "Prefabs/Skills/SkillTrees/" + industry.skillTree.name, unlockedSkillPaths, activeSkillPaths, industry.skillPoints);

                newCity.industries.Add(newIndustry);
                newCity.workerAmount.Add(city.workersPerIndustry[industry]);
            }
            data.cities.Add(newCity);
        }
    }

    public City GetCity(Vector2Int coordinates){
         foreach (City city in cities)
        {
            if(city.coordinates == coordinates){
                return city;
            }
        }
        return null;
    }
    
    public City GetCity(HexTile tile){
        foreach (City city in cities)
        {
            if(city.coordinates == tile.coordinates){
                return city;
            }
        }
        return null;
    }

    public City GetCity(string name){
        foreach (City city in cities)
        {
            if(city.cityName == name){
                return city;
            }
        }
        return null;
    }

    public int GetPriority()
    {
        return 1;
    }
}
