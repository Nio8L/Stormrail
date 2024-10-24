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
    float lockedHappiness;
    public float hungerDrainModifier = 1;
    float hungerTimer;
    HappinessSource starvingSource;
    bool starvation = false;
    


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
        lockedHappiness = overallHappiness;
             if (lockedHappiness < 0f) lockedHappiness = 0f;
        else if (lockedHappiness > 1f) lockedHappiness = 1f;

        GainResources();
        HungerDrain();
        UpdateHappinessSourceTimers();
    }

    void GainResources(){
        for (int i = 0; i < workersPerIndustry.Count; i++){
            // Get every industry in this city
            KeyValuePair<Industry, int> industryPair = workersPerIndustry.ElementAt(i);

            float modifier = lockedHappiness;
            if (modifier < 0.1f) modifier = 0.1f;

            if (industryPair.Key.level == 0) continue;

            for (int product = 0; product < industryPair.Key.itemOutputPerWorker.Count; product++){
                // Get every product of that industry
                KeyValuePair<Item, float> itemPair = industryPair.Key.itemOutputPerWorker.ElementAt(product);

                // Add the product to the inventory of the city
                float amountToGain = itemPair.Value * industryPair.Value * Time.deltaTime * modifier;
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
        Debug.Log("Removing");
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
        float hungerDrain = DataBase.instance.baseFoodConsumedPerDayPerPerson * population * hungerDrainModifier / DataBase.instance.dayLenghtInSeconds * Time.deltaTime;
        if (inventory[DataBase.instance.allItems[0]] < hungerDrain){
            // No food
            hungerTimer += Time.deltaTime;
        }else{
            // Some food
            inventory[DataBase.instance.allItems[0]] -= hungerDrain;
            if (hungerTimer > 0f){
                hungerTimer -= Time.deltaTime/2;
            }
        }

        // Add or remove happiness modifier
        if (hungerTimer > 15f){
            if (!starvation){
                starvingSource = new HappinessSource("Starvation", -0.3f, 1000, true);
                AddHappinessSource(starvingSource);
                starvation = true;
            }
        }else if (hungerTimer <= 0f){
            if (starvation){
                RemoveHappinessSource(starvingSource);
                starvation = false;
            }
        }
    }
}
