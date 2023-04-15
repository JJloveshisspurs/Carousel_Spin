using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*** By Jonathan Jennings 3-13-23

public class CarouselObjectSpawner : MonoBehaviour
{
    //*** Objects to be spawned which make up the tower
    public GameObject spawnObjects;

    //*** Radius for objects spawned to make use of the circular tower
    public float circleRadius = 5.0f;

    //*** A number corresponding ot the number of pieces hwich make up each aspect of the tower
    int numOfObjectsToSpawn = 0;

    //*** Carousel Input Controller reference
    [SerializeField] private CarouselInputController oInputcontroller;

    //*** This objects layer number helps determine if this is the top of the tower or not
    public int layernum;

    //*** List of Materials to make it easier to see different tower layers.
    public List<Material> clickableObjectMaterials;

    public float updateCameraPositionY;

    

    // Start is called before the first frame update
    void Start()
    {
        //*** Get Input controller reference
        oInputcontroller = GameObject.FindObjectOfType<CarouselInputController>();


        //*** Get and set current layer value
        layernum = oInputcontroller.layerCount;

        //***  Randomly assign how many objects to spawn to make up this layer of the tower
        numOfObjectsToSpawn = Random.Range(15, 100);

        //*** Begin instantiating clickable objets
        InitializeSpawnerObjects(numOfObjectsToSpawn);

       
    }

    

    public void InitializeSpawnerObjects(int pNumOfObjects)
    {

        //*** Get Center for this Carousel Spawner Object
        Vector3 oCarouselObjectCenter = this.transform.position;

        //*** Local References for clickable objects
        GameObject oSpawnedObject;
        CarouselClickables oClickableObjects;
        MeshRenderer oClickableObjectsMesh;

        //*** Which material should we apply to the objects on this level of the tower?
        int oMaterialIndex = Random.Range(0, clickableObjectMaterials.Count);

        //*** Set Material value
        Material oMaterialColor = clickableObjectMaterials[oMaterialIndex];

        //*** ITerate through all clickable objects
        for (int i = 0; i < pNumOfObjects; i++)
        {

            //*** Set position for all clickable objects on this level
            Vector3 oPos = GenerateCircle(oCarouselObjectCenter, circleRadius,i);
            Quaternion oRot = Quaternion.FromToRotation(Vector3.forward, oCarouselObjectCenter - oPos);
             oSpawnedObject =  Instantiate(spawnObjects, oPos, oRot);
            oSpawnedObject.transform.parent = this.gameObject.transform;


            //*** Set Color, parent, and layer numbers for this particular lcickable object
             oClickableObjects = oSpawnedObject.GetComponent<CarouselClickables>();

            oClickableObjects.SetParentObject(this.gameObject,oInputcontroller,layernum);

            oClickableObjectsMesh = oClickableObjects.GetComponent<MeshRenderer>();

            oClickableObjectsMesh.material = oMaterialColor;
        }
    }

    Vector3 GenerateCircle(Vector3 pCenter, float pRadius, int pObjectIndex)
    {

        //*** Spawn gameobjects equidistant around the radius based on the total number of spawned objects on this level
        float oAngle = ((float)pObjectIndex /(float)numOfObjectsToSpawn) * 360f;

        //Debug.Log(" Object index : " + pObjectIndex.ToString() + " angle  ==  " + ang);

        //*** Place Clickable object at correct position on the  circlaes radius.
        Vector3 oPos;
        oPos.x = pCenter.x + pRadius * Mathf.Sin(oAngle * Mathf.Deg2Rad);
        oPos.y = pCenter.y;
        oPos.z = pCenter.z + pRadius * Mathf.Cos(oAngle * Mathf.Deg2Rad);
        return oPos;
    }
}
