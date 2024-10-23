using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class City : MonoBehaviour
{
    public string cityName;
    public int population;
    public int workers;
    public int lastOpenTab = 0;
    public Dictionary<Item, float> inventory = new Dictionary<Item, float>();
    public Dictionary<Industry, int> workersPerIndustry = new Dictionary<Industry, int>();
    public Vector2Int coordinates;
    public List<HappinessSource> happinessSources = new List<HappinessSource>();
    public float overallHappiness;
    void Start(){
        transform.position = MapManager.instance.tiles[coordinates.x, coordinates.y].transform.position;
        transform.position += new Vector3(0, 0.75f, 0);
        MapManager.instance.tiles[coordinates.x, coordinates.y].SetType(HexTile.Type.City);

        HappinessSource baseHappiness = new HappinessSource("Base city happiness", 0.15f, 1000f, true);
        AddHappinessSource(baseHappiness);
    }

    public void Initialize(Vector2Int coordinates, string cityName, int population){
        for (int i = 0; i < DataBase.instance.allItems.Count; i++){
            inventory.Add(DataBase.instance.allItems[i], 0);
        }
        for (int i = 0; i < DataBase.instance.allIndustries.Count; i++){
            Industry newIndustry = Instantiate(DataBase.instance.allIndustries[i]);
            newIndustry.Initialize();
            workersPerIndustry.Add(newIndustry, 0);
        }
        
        this.coordinates = coordinates;
        this.cityName = cityName;
        this.population = population;
    }

    void Update(){
        GainResources();
        UpdateHappinessSourceTimers();
    }

    void GainResources(){
        for (int i = 0; i < workersPerIndustry.Count; i++){
            // Get every industry in this city
            KeyValuePair<Industry, int> industryPair = workersPerIndustry.ElementAt(i);

            if (industryPair.Key.level == 0) continue;

            for (int product = 0; product < industryPair.Key.itemOutputPerWorker.Count; product++){
                // Get every product of that industry
                KeyValuePair<Item, float> itemPair = industryPair.Key.itemOutputPerWorker.ElementAt(product);

                // Add the product to the inventory of the city
                float amountToGain = itemPair.Value * industryPair.Value * Time.deltaTime * overallHappiness;
                inventory[itemPair.Key] += amountToGain;
            }
        }
    }

    public void DestroyCity(){
        CityManager.instance.cities.Remove(this);
        Destroy(gameObject);
    }

    public void AddHappinessSource(HappinessSource newSource){
        // Removes a happiness source and updates the cities overall happiness
        happinessSources.Add(newSource);
        overallHappiness += newSource.happinessModifier;
    }
    public void RemoveHappinessSource(HappinessSource newSource){
        // Removes a happiness source as well as it's modifier
        happinessSources.Remove(newSource);
        overallHappiness -= newSource.happinessModifier;
    }

    public void UpdateHappinessSourceTimers(){
        // Loop through all happiness sources and check if they should be removed
        for (int i = 0; i < happinessSources.Count; i++){
            HappinessSource source = happinessSources[i];
            // If the happiness source has infinite duration turned on don't do anything
            if (!source.infiniteDuration){
                source.timeLeft -= Time.deltaTime;
                if (source.timeLeft < 0){
                    RemoveHappinessSource(source);
                    i--;
                }
            }
        }
    }

    public void HungerDrain(){
        
    }
}
