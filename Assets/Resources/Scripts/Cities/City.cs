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
    public Dictionary<Item, float> consumingThisFrame = new Dictionary<Item, float>();
    public Dictionary<Industry, int> workersPerIndustry = new Dictionary<Industry, int>();
    public Vector2Int coordinates;
    public List<HappinessSource> happinessSources = new List<HappinessSource>();
    public float overallHappiness;
    public float lockedHappiness;
    public float hungerDrainModifier = 1;
    public float hungerTimer;
    HappinessSource starvingSource;
    public bool starvation = false;
    public List<City> connections = new();
    


    void Start(){
        transform.position = MapManager.instance.tiles[coordinates.x, coordinates.y].transform.position;
        transform.position += new Vector3(0, 0.75f, 0);
        MapManager.instance.tiles[coordinates.x, coordinates.y].SetType(HexTile.Type.City);

        for (int i = 0; i < DataBase.instance.allItems.Count; i++){
            consumingThisFrame.Add(DataBase.instance.allItems[i], 0);
        }

        foreach (City connection in connections)
            {
                CityManager.instance.BuildRailConnection(this, connection);
            }
    }

    public void Initialize(Vector2Int coordinates, string cityName, int population){
        for (int i = 0; i < DataBase.instance.allItems.Count; i++){
            inventory.Add(DataBase.instance.allItems[i], 0);
        }
        for (int i = 0; i < DataBase.instance.allIndustries.Count; i++){
            Industry newIndustry = Instantiate(DataBase.instance.allIndustries[i]);
            newIndustry.Initialize(this);
            workersPerIndustry.Add(newIndustry, 0);
        }
        
        this.coordinates = coordinates;
        this.cityName = cityName;
        this.population = population;

        HappinessSource baseHappiness = new HappinessSource("Base city happiness", 0.15f, 1000f, true);
        AddHappinessSource(baseHappiness);
    }

    void Update(){
        lockedHappiness = overallHappiness;
             if (lockedHappiness < 0f) lockedHappiness = 0f;
        else if (lockedHappiness > 1f) lockedHappiness = 1f;

        
        ConsumeResources();
        GainResources();
        UpdateSkills();
        HungerDrain();
        UpdateHappinessSourceTimers();
    }

    void ConsumeResources(){
        // Consumes all resources in consumingThisFrame
        for (int i = 0; i < consumingThisFrame.Count; i++){
            KeyValuePair<Item, float> itemPair = consumingThisFrame.ElementAt(i);
            if (itemPair.Value <= 0) continue;
            inventory[itemPair.Key] -= itemPair.Value;
            //Debug.Log("Consuming " + itemPair.Key.itemName + ": " + itemPair.Value);
            if (inventory[itemPair.Key] < 0){
                inventory[itemPair.Key] = 0;
            }

            consumingThisFrame[itemPair.Key] = 0;
        }
    }

    void GainResources(){
        for (int i = 0; i < DataBase.instance.allItems.Count; i++){
            // Get every industry in this city
            Item itemToCheck = DataBase.instance.allItems[i];
            float amountToGain = CalculateProduction(itemToCheck) * Time.deltaTime;
            inventory[itemToCheck] += amountToGain;
        }
    }

    public float CalculateProduction(Item itemToCheck){
        // Calculates 1 seconds worth of production for a certain item
        float productionThisSecond = 0;

        for (int i = 0; i < workersPerIndustry.Count; i++){
            Industry industry = workersPerIndustry.ElementAt(i).Key;
            
            // If the current industry is unupgraded continue
            if (industry.level == 0) continue;

            productionThisSecond += industry.itemOutputPerWorker[itemToCheck] * workersPerIndustry[industry];
        }

        // Calculate the happiness modifier
        float modifier = 0.8f + 2*lockedHappiness/5;

        // Calculate the final production
        productionThisSecond *= modifier/DataBase.instance.dayLenghtInSeconds;
        
        return productionThisSecond;
    }

    public float CalculateConsumption(float amountPerSecond, float workers){
        // Calculates consumption per second

        // Calculate the happiness modifier
        float modifier = 0.8f + 2*lockedHappiness/5;

        return amountPerSecond * modifier * workers / DataBase.instance.dayLenghtInSeconds;
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
        if (newSource == null) return;
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

    public void UpdateSkills(){
        // Loop though all industries and call update on their skills
        for (int i = 0; i < workersPerIndustry.Count; i++){
            Industry currentIndustry = workersPerIndustry.ElementAt(i).Key;
            foreach (Skill skill in currentIndustry.unlockedSkills){
                skill.OnUpdate();
            }
        }
    }

    public void HungerDrain(){
        // Calculate hunger drain per frame
        float hungerDrain = DataBase.instance.baseFoodConsumedPerDayPerPerson * population * hungerDrainModifier / DataBase.instance.dayLenghtInSeconds * Time.deltaTime;
        // Consume food
        consumingThisFrame[DataBase.instance.allItems[0]] += hungerDrain;

        if (inventory[DataBase.instance.allItems[0]] < hungerDrain && hungerTimer < 20f){
            // No food
            hungerTimer += Time.deltaTime;
        }else{
            // Some food
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

    private void OnMouseUp() {
        if (TrainManager.instance.buildMode)
        {
            if(TrainManager.instance.citiesToConnect[0] == null){
                //Pathfinder.instance.tile1 = MapManager.instance.tiles[coordinates.x, coordinates.y];
                TrainManager.instance.citiesToConnect[0] = this;
            }else{
                //Pathfinder.instance.tile2 = MapManager.instance.tiles[coordinates.x, coordinates.y];
                TrainManager.instance.citiesToConnect[1] = this;
                CityManager.instance.ConnectCities(TrainManager.instance.citiesToConnect[0], TrainManager.instance.citiesToConnect[1]);
                
                TrainManager.instance.citiesToConnect[0] = null;
                TrainManager.instance.citiesToConnect[1] = null;
            }
        }
    }
}
