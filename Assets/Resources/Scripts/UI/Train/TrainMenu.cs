using TMPro;
using UnityEngine;

public class TrainMenu : MonoBehaviour
{
    public static TrainMenu instance;

    [Header("Objects and Holders")]
    public GameObject trainMenuObject;
    public GameObject routeHolder;
    public GameObject stopHolder;

    [Header("Route Window")]
    public Route selectedRoute;
    public TMP_InputField routeName;

    [Header("Instantiables")]
    public GameObject routeObject;
    public GameObject stopObject;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        LoadRoutes();
    }
    
    public void OpenMenu(){
        trainMenuObject.SetActive(true);
    }

    public void CloseMenu(){
        trainMenuObject.SetActive(false);
    }


    public void LoadRoutes(){
        foreach (Route route in TrainManager.instance.routes)
        {
            GameObject newRoute = Instantiate(routeObject);
            newRoute.transform.SetParent(routeHolder.transform);
            newRoute.GetComponent<RoutePlateUI>().Initialize(route.name);
        }
    }

    public void AddRoute(){
        GameObject newRoute = Instantiate(routeObject);
        newRoute.transform.SetParent(routeHolder.transform);
        TrainManager.instance.CreateRoute("New Route");
        newRoute.GetComponent<RoutePlateUI>().Initialize(TrainManager.instance.routes[^1].name);
    }

    public void DeleteRoute(RoutePlateUI routeToDelete){
        for(int i = 0; i < routeHolder.transform.childCount; i++){
            if(routeHolder.transform.GetChild(i).GetComponent<RoutePlateUI>() == routeToDelete){
                if(routeToDelete.routeName.text == selectedRoute.name){
                    if(i == 0){
                        if(routeHolder.transform.childCount == 1){
                            DeleteStops();
                            routeName.text = "NO ROUTE CHOSEN";
                            selectedRoute = new();
                        }else{
                            routeHolder.transform.GetChild(1).GetComponent<RoutePlateUI>().SelectRoute();
                        }
                    }else{
                        routeHolder.transform.GetChild(i - 1).GetComponent<RoutePlateUI>().SelectRoute();
                    }
                }
                Destroy(routeHolder.transform.GetChild(i).gameObject);
                TrainManager.instance.routes.Remove(TrainManager.instance.GetRoute(routeToDelete.routeName.text));
            }
        }
    }

    public void AddStop(){
        if(selectedRoute.name == "") return;
        selectedRoute.stops.Add(new("No City"));
        GameObject newStop = Instantiate(stopObject, stopHolder.transform);
        newStop.GetComponent<StopPlateUI>().Initialize(stopHolder.transform.childCount - 1);
    }

    public void AddStops(){
        for(int i = 0; i < selectedRoute.stops.Count; i++){
            GameObject newStop = Instantiate(stopObject, stopHolder.transform);
            StopPlateUI stopScript = newStop.GetComponent<StopPlateUI>();
            stopScript.Initialize(i);
            for(int j = 0; j < selectedRoute.stops[i].conditions.Count; j++){
                stopScript.CreateConditionObject(selectedRoute.stops[i].conditions[j]);
            }
        }
    }

    public void DeleteStop(StopPlateUI stopToDelete){
        for(int i = 0; i < stopHolder.transform.childCount; i++){
            if(stopHolder.transform.GetChild(i).GetComponent<StopPlateUI>() == stopToDelete){
                StopPlateUI stopPlate = stopHolder.transform.GetChild(i).GetComponent<StopPlateUI>();
                string stopName = stopPlate.cityList.options[stopPlate.cityList.value].text;
                selectedRoute.stops.Remove(TrainManager.instance.GetStop(selectedRoute, stopName));
                Destroy(stopHolder.transform.GetChild(i).gameObject);
            }
        }
    }

    public void DeleteStops(){
        for(int i = 0; i < stopHolder.transform.childCount; i++){
            Destroy(stopHolder.transform.GetChild(i).gameObject);
        }
    }

    public void EditRouteName(){
        if(routeName.text == ""){
            routeName.text = selectedRoute.name;
            return;
        } 
            
        
        for(int i = 0; i < routeHolder.transform.childCount; i++){
            if(routeHolder.transform.GetChild(i).GetComponent<RoutePlateUI>().routeName.text == selectedRoute.name){
                string checker = routeName.text;
                int counter = 0;
                while(TrainManager.instance.GetRoute(checker) != null){
                    counter++;
                    checker = routeName.text + "_" + counter;
                }
                routeName.text = checker;
                routeHolder.transform.GetChild(i).GetComponent<RoutePlateUI>().routeName.text = routeName.text;
            }
        }
        
        selectedRoute.name = routeName.text;
    }

    public void BuildMode(){
        TrainManager.instance.buildMode = true;
    }
}
