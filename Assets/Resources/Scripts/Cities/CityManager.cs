using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityManager : MonoBehaviour, ISavable
{
    public static CityManager instance;
    public List<City> cities;

    public GameObject cityPrefab;

    public Vector2Int coord1;
    public Vector2Int coord2;

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
            newCity.hungerTimer = city.hungerTimer;
            newCity.hungerDrainModifier = city.hungerDrainModifier;

            // Event Saving
            newCity.eventTimer = city.eventTimer;
            newCity.eventActive = city.eventActive;

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

                industry.Initialize(newCity);

                for(int j = 0; j < DataBase.instance.allItems.Count; j++){
                    industry.itemOutputPerWorker[DataBase.instance.allItems[j]] = currentIndustry.outputPerWorker[j];
                }
                
                newCity.workersPerIndustry.Add(industry, city.workerAmount[i]); 
            }

            cities.Add(newCity);
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

            // Event Saving
            newCity.eventTimer = city.eventTimer;
            newCity.eventActive = city.eventActive;

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

    public City GetCityByTile(HexTile tile){
        foreach (City city in cities)
        {
            if(city.coordinates == tile.coordinates){
                return city;
            }
        }
        return null;
    }

    public City GetCityByName(string name){
        foreach (City city in cities)
        {
            if(city.cityName == name){
                return city;
            }
        }
        return null;
    }

    public void ConnectCities(City city1, City city2){
        BuildRailConnection(city1, city2);
    }

    public void BuildRailConnection(HexTile tile1, HexTile tile2){
        List<HexTile> path = Pathfinder.instance.Pathfind(tile1, tile2);
        for(int i = 0; i < path.Count - 1; i++){
            BuildRail(path[i], path[i + 1]);
        }
    }

    public void BuildRailConnection(City city1, City city2){
        HexTile tile1 = MapManager.instance.tiles[city1.coordinates.x, city1.coordinates.y];
        HexTile tile2 = MapManager.instance.tiles[city2.coordinates.x, city2.coordinates.y];

        BuildRailConnection(tile1, tile2);
    }

    public void BuildRail(HexTile tile1, HexTile tile2){
        int angle = MapManager.instance.GetAngle(tile1, tile2);
        int opposite = MapManager.FixAngle(angle - 180);
        if(tile1.angles.Contains(opposite)){
            return;
        }
        if(tile2.angles.Contains(angle)){
            return;
        }
        Instantiate(MapManager.instance.railPrefab, tile1.transform.position, Quaternion.Euler(0, opposite, 0));
        Instantiate(MapManager.instance.railPrefab, tile2.transform.position,  Quaternion.Euler(0, angle, 0));
        tile1.angles.Add(opposite);
        tile2.angles.Add(angle);
    }
}
