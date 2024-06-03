using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnerController : MonoBehaviour
{
    [Header("Collectibles Object Variables")]
    [SerializeField] private Transform EnergyDrinkParent;
    private List<GameObject> ListOfEnergyDrinks;
    [SerializeField] private Transform PainKillerParent;
    private List<GameObject> ListOfPainKillers;

    [Header("Physical Object Variables")]
    [SerializeField] private Transform ObstacleParent;
    private List<GameObject> ListOfObstacles;

    private void Start()
    {
        RegisterObjects();
        ResetObstacles();
    }

    void RegisterObjects()
    {
        //Register Energy Drinks
        ListOfEnergyDrinks = new List<GameObject>();
        for (int i = 0; i < EnergyDrinkParent.childCount; i++)
        {
            ListOfEnergyDrinks.Add(EnergyDrinkParent.GetChild(i).gameObject);
        }
        //Register Pain Killers
        ListOfPainKillers = new List<GameObject>();
        for (int i = 0; i < PainKillerParent.childCount; i++)
        {
            ListOfPainKillers.Add(PainKillerParent.GetChild(i).gameObject);
        }

        //Register Obstacles
        ListOfObstacles = new List<GameObject>();
        for (int i = 0; i < ObstacleParent.childCount; i++)
        {
            ListOfObstacles.Add(ObstacleParent.GetChild(i).gameObject);
        }
    }

    public void ResetObstacles()
    {
        //Respawn Energy Drinks
        foreach (var obstacle in ListOfEnergyDrinks)
        {
            obstacle.SetActive(true);
        }
        //Respawn Pain Killers
        foreach (var obstacle in ListOfPainKillers)
        {
            obstacle.SetActive(true);
        }

        //TO DO: Respawn Obstacles with animations
        foreach (var obstacle in ListOfObstacles)
        {
            obstacle.SetActive(true);
        }
    }
}
