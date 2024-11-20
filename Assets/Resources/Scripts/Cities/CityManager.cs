using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityManager : MonoBehaviour, ISavable
{
    public static CityManager instance;
    public List<City> cities;

    public GameObject cityPrefab;
    public GameObject railPrefab;

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

        for(int i = 0; i < data.cities.Count; i++){
            foreach(string cityName in data.cities[i].connections){
                cities[i].connections.Add(GetCityByName(cityName));
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

            foreach (City connectedCity in city.connections)
            {
                newCity.connections.Add(connectedCity.cityName);
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

    public void ConnectCities(HexTile tile1, HexTile tile2){
        GetCityByTile(tile1).connections.Add(GetCityByTile(tile2));
        GetCityByTile(tile2).connections.Add(GetCityByTile(tile1));
        BuildRailConnection(tile1, tile2);
    }

    public void ConnectCities(City city1, City city2){
        city1.connections.Add(city2);
        city2.connections.Add(city1);
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
        Vector2 reference = Vector2.up;
        Vector2 newVector = new Vector2(tile2.transform.position.x - tile1.transform.position.x, tile2.transform.position.z - tile1.transform.position.z);
        int angle = Mathf.RoundToInt(-Vector2.SignedAngle(reference, newVector));
        int offset = 180;
        if(tile1.angles.Contains(angle - offset)){
            return;
        }
        if(tile2.angles.Contains(angle)){
            return;
        }
        Instantiate(railPrefab, tile1.transform.position, Quaternion.Euler(0, angle - offset, 0));
        Instantiate(railPrefab, tile2.transform.position,  Quaternion.Euler(0, angle, 0));
        tile1.angles.Add(angle - offset);
        tile2.angles.Add(angle);
    }
}
