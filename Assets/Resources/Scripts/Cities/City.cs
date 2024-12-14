using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class City : MonoBehaviour
{
    public string cityName;
    public int lastOpenTab = 0;
    [Header("Inventory and industry")]
    public Dictionary<Item, float> inventory = new Dictionary<Item, float>();
    public Dictionary<Item, float> consumingThisFrame = new Dictionary<Item, float>();
    public Dictionary<Industry, int> workersPerIndustry = new Dictionary<Industry, int>();
    public Vector2Int coordinates;
    [Header("Happiness sources")]
    public List<HappinessSource> happinessSources = new List<HappinessSource>();
    HappinessSource starvingSource;
    [Header("Stats")]
    public int population;
    public int workers;
    public float overallHappiness;
    public float lockedHappiness;
    public float hungerDrainModifier = 1;
    public float hungerTimer;
    public float eventTimer;
    public bool starvation = false;
    public List<City> connections = new();
    [Header("Event pools")]
    public EventPool eventPoolLowHappiness;
    public EventPool eventPoolHighHappiness;
    [Header("Prefabs")]
    public GameObject prefabEventBubble;
    


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

        starvingSource = new HappinessSource("Starvation", -0.3f, 1000, true);
        eventTimer = Random.Range(0f, 60f) + 120f;
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
        // Calculate happiness locked in a range between 0 and 1
        lockedHappiness = Mathf.Clamp01(overallHappiness);
        
        ConsumeResourcesFromIndustries();
        GainResourcesFromIndustries();
        UpdateSkills();
        HungerDrain();
        UpdateHappinessSourceTimers();
        SpawnRandomEventTimer();
    }

    void ConsumeResourcesFromIndustries(){
        // Consumes all resources in consumingThisFrame
        for (int i = 0; i < consumingThisFrame.Count; i++){
            KeyValuePair<Item, float> itemPair = consumingThisFrame.ElementAt(i);
            if (itemPair.Value <= 0) continue;

            ConsumeResource(itemPair.Key, itemPair.Value);
            consumingThisFrame[itemPair.Key] = 0;
        }
    }
    public void ConsumeResource(Item itemToConsume, float amount){
        // Consume a given resource
        inventory[itemToConsume] -= amount;
        if (inventory[itemToConsume] < 0) inventory[itemToConsume] = 0; 
    }
    public bool CheckInventoryFor(List<Item> itemsToCheck, List<float> neededAmounts){
        // Check if there is enough of a list of items
        bool enoughResources = true;
        for (int i = 0; i < itemsToCheck.Count; i++){
            if (inventory[itemsToCheck[i]] < neededAmounts[i]){
                enoughResources = false;
                break;
            }
        }
        return enoughResources;
                
    }
    public bool CheckInventoryFor(Item itemToCheck, float neededAmount){
        // Check if there is enough of a single item
        List<Item> itemsToCheck = new List<Item>(){itemToCheck};
        List<float> neededAmounts = new List<float>(){neededAmount};
        return CheckInventoryFor(itemsToCheck, neededAmounts);
    }

    void GainResourcesFromIndustries(){
        // Check all industries and find which resources and in what amount should be gained this turn
        for (int i = 0; i < DataBase.instance.allItems.Count; i++){
            // Check every item type
            Item itemToCheck = DataBase.instance.allItems[i];
            float amountToGain = CalculateProduction(itemToCheck) * Time.deltaTime;
            GainResource(itemToCheck, amountToGain);
        }
    }
    public void GainResource(Item itemToGain, float amount){
        // Gain a single resource
        inventory[itemToGain] += amount;
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
        // Adds a happiness source and updates the cities overall happiness
        newSource = newSource.Copy();
        foreach (HappinessSource happinessSource in happinessSources){
            if (happinessSource.sourceName == newSource.sourceName){
                happinessSource.daysLeft = newSource.daysLeft;
                return;
            }
        }
        happinessSources.Add(newSource);
        overallHappiness += newSource.happinessModifier;

        if (CityMenu.instance != null && CityMenu.instance.currentCity != null)
            CityMenu.instance.tabs[0].GetComponent<OverviewTab>().UpdateInformation();
    }
    public void RemoveHappinessSource(HappinessSource sourceToRemove){
        // Removes a happiness source as well as it's modifier
        for (int i = 0; i < happinessSources.Count; i++){
            if (happinessSources[i].sourceName == sourceToRemove.sourceName){
                happinessSources.RemoveAt(i);
                overallHappiness -= sourceToRemove.happinessModifier;
                break;
            }
        }

        if (CityMenu.instance != null && CityMenu.instance.currentCity != null)
            CityMenu.instance.tabs[0].GetComponent<OverviewTab>().UpdateInformation();
    }
    public void UpdateHappinessSourceTimers(){
        // Loop through all happiness sources and check if they should be removed
        for (int i = 0; i < happinessSources.Count; i++){
            HappinessSource source = happinessSources[i];
            // If the happiness source has infinite duration turned on don't do anything
            if (!source.infiniteDuration){
                source.daysLeft -= Time.deltaTime / DataBase.instance.dayLenghtInSeconds;
                if (source.daysLeft < 0){
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
            foreach (Skill skill in currentIndustry.activeSkills){
                skill.OnUpdate();
            }
        }
    }

    public void SpawnRandomEventTimer(){
        // Count down and spawn a random event once in a while
        eventTimer-= Time.deltaTime;
        if (eventTimer <= 0f){
            RandomEvent();
            eventTimer = 10f;//Random.Range(0, 60f) + 120f;
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
        // Rail building
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

    public void RandomEvent(){
        // Pick a random event based on the city's happiness
        Decision eventToSpawn = null;
        if (lockedHappiness <= 0.25f){
            eventToSpawn = eventPoolLowHappiness.events[Random.Range(0, eventPoolLowHappiness.events.Count)];
        }else{
            eventToSpawn = eventPoolHighHappiness.events[Random.Range(0, eventPoolHighHappiness.events.Count)];
        }
        SpawnEvent(eventToSpawn);
    }
    public void SpawnEvent(Decision eventToSpawn){
        // Spawn a certain event ot top of the city
        eventToSpawn = Instantiate(eventToSpawn);

        Vector3 bubblePosition = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        DecisionBubble decisionBubble = Instantiate(prefabEventBubble, bubblePosition, Quaternion.identity).GetComponent<DecisionBubble>();
        decisionBubble.decision = eventToSpawn;
        decisionBubble.linkedCity = this;
    }
}
