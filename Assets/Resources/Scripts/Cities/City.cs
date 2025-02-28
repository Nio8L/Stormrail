using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class City : Station
{
    public int lastOpenTab = 0;
    public Dictionary<Item, float> consumingThisFrame = new Dictionary<Item, float>();
    public Dictionary<Industry, int> workersPerIndustry = new Dictionary<Industry, int>();
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
    public float starvationDeathResetTimer;
    public float deathPenalty = 1;
    int lastDeathPenalty;
    
    [Header("Event pools")]
    public EventPool eventPoolLowHappiness;
    public EventPool eventPoolHighHappiness;
    [Header("Events")]
    public string activeEvent;
    public Decision starvationEvent;
    public Decision deathFromHungerEvent;
    [Header("Prefabs")]
    public GameObject prefabEventBubble;
    [Header("Object")]
    public DecisionBubble decisionBubble;

    protected override void Start(){
        base.Start();
        MapManager.instance.tiles[coordinates.x, coordinates.y].SetType(HexTile.Type.City);

        for (int i = 0; i < DataBase.instance.allItems.Count; i++){
            consumingThisFrame.Add(DataBase.instance.allItems[i], 0);
        }

        starvingSource = new HappinessSource("Starvation", -0.3f, 1000, true);

        // Reveal city
        if (!MapLoader.instance.loadingEditor){
            HexTile thisTile = MapManager.instance.StationToTile(this);
            thisTile.Reveal();
            List<HexTile> surroundingTiles = thisTile.GetNeighbors(1);
            foreach (HexTile tile in surroundingTiles){
                tile.Reveal();
            }
        }
    }

    public void Initialize(Vector2Int coordinates, string cityName, int population){
        Initialize(coordinates, cityName);
        for (int i = 0; i < DataBase.instance.allIndustries.Count; i++){
            Industry newIndustry = Instantiate(DataBase.instance.allIndustries[i]);
            newIndustry.Initialize(this);
            workersPerIndustry.Add(newIndustry, 0);
        }

        this.population = population;

        HappinessSource baseHappiness = new HappinessSource("Base city happiness", 0.15f, 1000f, true);
        AddHappinessSource(baseHappiness);
    }

    public void OnFirstCreate(){
        eventTimer = Random.Range(0f, 60f) + 180f;

        inventory[DataBase.instance.allItems[4]] = 20;
        inventory[DataBase.instance.allItems[0]] = 10;
    }

    protected override void Update(){
        // Clamp happiness in a range between 0 and 1
        lockedHappiness = Mathf.Clamp01(overallHappiness);
        
        // City actions
        ConsumeResourcesFromIndustries();
        GainResourcesFromIndustries();
        UpdateSkills();
        HungerDrain();
        UpdateHappinessSourceTimers();
        SpawnRandomEventTimer();
        UpdateDeathPenalty();

        // Check if this city should be destroyed
        if (population <= 0) DestroyCity();
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

    public float CalculateProduction(Item itemToCheck){
        // Calculates 1 seconds worth of production for a certain item
        float productionThisSecond = 0;

        for (int i = 0; i < workersPerIndustry.Count; i++){
            Industry industry = workersPerIndustry.ElementAt(i).Key;
            
            // If the current industry is unupgraded continue
            if (industry.level == 0) continue;
            for (int j = 0; j < industry.activeSkills.Count; j++){
                productionThisSecond += industry.activeSkills[j].productionPerSecond[itemToCheck] * workersPerIndustry[industry];
            }
        }

        // Calculate the happiness modifier
        float modifier = 0.8f + 2*lockedHappiness/5 * deathPenalty;

        // Calculate the final production
        productionThisSecond *= modifier/DataBase.instance.dayLenghtInSeconds;
        
        return productionThisSecond;
    }

    public float CalculateConsumption(float amountPerSecond, float workers){
        // Calculates consumption per second

        // Calculate the happiness modifier
        float modifier = 0.8f + 2*lockedHappiness/5 * deathPenalty;

        return amountPerSecond * modifier * workers / DataBase.instance.dayLenghtInSeconds;
    }

    public void DestroyCity(){
        if (!MapLoader.instance.loadingEditor)
            MapManager.instance.StationToTile(this).SetTypeDecoration(HexTile.Type.Empty);
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
        RemoveHappinessSource(sourceToRemove.sourceName);
    }

    public void RemoveHappinessSource(string sourceName){
        // Removes a happiness source as well as it's modifier
        for (int i = 0; i < happinessSources.Count; i++){
            if (happinessSources[i].sourceName == sourceName){
                overallHappiness -= happinessSources[i].happinessModifier;
                happinessSources.RemoveAt(i);
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
        if (activeEvent != "") return;

        eventTimer-= Time.deltaTime;
        if (eventTimer <= 0f){
            RandomEvent();
            eventTimer = Random.Range(0f, 60f) + 180f;
        }
    }

    public void HungerDrain(){
        // Calculate hunger drain per frame
        float hungerDrain = DataBase.instance.baseFoodConsumedPerDayPerPerson * population * hungerDrainModifier / DataBase.instance.dayLenghtInSeconds * Time.deltaTime;
        // Consume food
        consumingThisFrame[DataBase.instance.allItems[0]] += hungerDrain;

        if (inventory[DataBase.instance.allItems[0]] < hungerDrain){
            // No food
            if (hungerTimer < DataBase.instance.dayLenghtInSeconds * 2){
                hungerTimer += Time.deltaTime;
            }

            if (hungerTimer >= DataBase.instance.dayLenghtInSeconds * 2){
                // Starving
                starvationDeathResetTimer += Time.deltaTime;
                if (starvationDeathResetTimer >= DataBase.instance.dayLenghtInSeconds * 1){
                    starvationDeathResetTimer = 0f;
                    SpawnEvent(deathFromHungerEvent);
                }
            }
        }else{
            // Some food
            if (hungerTimer > 0f){
                hungerTimer -= Time.deltaTime/2;
            }

            if (starvationDeathResetTimer > 0f){
                starvationDeathResetTimer -= Time.deltaTime*2;
            }
        }

        // Add or remove happiness modifier
        if (hungerTimer > DataBase.instance.dayLenghtInSeconds * 2){
            if (!starvation){
                SpawnEvent(starvationEvent);
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
        if (eventToSpawn == null) return;

        string eventName = eventToSpawn.name;
        eventToSpawn = Instantiate(eventToSpawn);
        eventToSpawn.name = eventName;
        activeEvent = eventToSpawn.subFolder + "/" + eventToSpawn.name;

        Vector3 bubblePosition = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        decisionBubble = Instantiate(prefabEventBubble, bubblePosition, Quaternion.identity).GetComponent<DecisionBubble>();
        decisionBubble.decision = eventToSpawn;
        decisionBubble.linkedCity = this;
    }

    public void SpawnEvent(string eventName){
        // Event name format must be: event sub folder name + event name. Ex: Low happiness/LHSlums
        if (eventName == "") return;

        Decision eventToSpawn = Resources.Load<Decision>("Scriptable Objects/Events/" + eventName);
        SpawnEvent(eventToSpawn);
    }

    public void SetEventBubbleTimer(float time){
        decisionBubble.timeLeft = time;
    }

    public void SetPopulation(int amount){
        // Sets this city's population to amount
        int diff = amount - population;
        ChangePopulation(diff, false);
    }
    public void ChangePopulation(int amount, bool death){
        // Modifies this cities population by amount and updates worker numbers
        population += amount;

        if (CityMenu.instance.currentCity == this){
            CityMenu.instance.populationText.text = population.ToString();
        }

        // Update population
        if (amount < 0){
            int diff = population - workers;
            if (diff < 0){
                workers += diff;
                for (int i = 0; i < workersPerIndustry.Count; i++){
                    KeyValuePair<Industry, int> keyValuePair = workersPerIndustry.ElementAt(i);
                    if (keyValuePair.Value >= diff){
                        workersPerIndustry[keyValuePair.Key] += diff;
                        break;
                    }else if (keyValuePair.Value < diff){
                        diff -= keyValuePair.Value;
                        workersPerIndustry[keyValuePair.Key] = 0;
                    }
                }
            }

            if (population  < 0) population = 0;
        }

        // Death case
        if (death && amount < 0){
            deathPenalty += amount * 0.05f;
        }
    }

    void UpdateDeathPenalty(){
        // Update the death penalty and add the necessary happiness source
        if (deathPenalty < 1) deathPenalty += 0.1f * Time.deltaTime / DataBase.instance.dayLenghtInSeconds;

        
        if (deathPenalty < 0.05f){
            // Game over
            if (lastDeathPenalty == 0) return;
            RemoveDeathHappinessSources();
            lastDeathPenalty = 0;
            HappinessSource deaths = new HappinessSource("Anarchy", -2f, 1000f, true);
            AddHappinessSource(deaths);
        }else if (deathPenalty < 0.35f){
            // High penalty
            if (lastDeathPenalty == 1) return;
            RemoveDeathHappinessSources();
            lastDeathPenalty = 1;
            HappinessSource deaths = new HappinessSource("Collapsing city", -0.65f, 1000f, true);
            AddHappinessSource(deaths);
        }else if (deathPenalty < 0.7f){
            // Mid penalty
            if (lastDeathPenalty == 2) return;
            RemoveDeathHappinessSources();
            lastDeathPenalty = 2;
            HappinessSource deaths = new HappinessSource("Mass deaths", -0.30f, 1000f, true);
            AddHappinessSource(deaths);
        }else if (deathPenalty < 1f){
            // Low penalty
            if (lastDeathPenalty == 3) return;
            RemoveDeathHappinessSources();
            lastDeathPenalty = 3;
            HappinessSource deaths = new HappinessSource("Recent losses", -0.10f, 1000f, true);
            AddHappinessSource(deaths);
        }
    }

    void RemoveDeathHappinessSources(){
        RemoveHappinessSource("Recent losses");
        RemoveHappinessSource("Mass deaths");
        RemoveHappinessSource("Collapsing city");
    }

    public override City GetCity(){
        return this;
    }
}
