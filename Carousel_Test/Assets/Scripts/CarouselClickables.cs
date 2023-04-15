using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*** By Jonathan Jennings 3-13-23
//*** This class controls all of the clickable objects on a carousel

public class CarouselClickables : MonoBehaviour
{
    //*** Central Input controller for Tower
    public CarouselInputController carouselController;

    //*** Container for this objects parent
    public GameObject parentObjectSpawnerContainer;

    //*** Prefab for the individual spawner Level for the Tower
    [SerializeField] private GameObject carouselObjectSpawnerPrefabs;

    //*** Layer for currently selected object , helps determine which layer is being pressed for higher  tower level spawning
    public int layer;

    //*** Assign PArent object and corresponding layer values for this particular clickable object
    public void SetParentObject(GameObject pParentObject , CarouselInputController pCarouselInputController, int pLayer)
    {
        parentObjectSpawnerContainer = pParentObject;
        carouselController = pCarouselInputController;

        layer = pLayer;
    }

    //*** Set Which Tier of the tower will be Active for rotation
    public void AssignClickableTier()
    {
        //Debug.Log("I was clicked!!!!");
        carouselController.ChangeActiveClickableTier(parentObjectSpawnerContainer,layer);
    }
}
