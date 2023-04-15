using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*** By Jonathan Jennings 3-13-23
//*** This class controls all of the mouse input and selection interactions for the clickable carousel objects

public class CarouselInputController : MonoBehaviour
{

    //*** Speed of given layers rotation
    [SerializeField] private float rotatingSpeed = 5f;

    //*** How quickly a spinning layer should decelerate
    [SerializeField] private float decelerationSpeed = .1f;


    //*** Time related Variables to track update ticks
    [SerializeField] private float updatetime = .1f;
    [SerializeField] private float updateTimer;

    //*** Pre-cache vector Reference to reduce number of copies created for translation purposes
    [SerializeField] public Vector3 speedVector = new Vector3(0f,0f,1f);

    //*** Current position of mouse on screen used for delta calculation
    [SerializeField] private Vector3 mousePosition;

    //*** Last tracked position of mouse on screen used for delta calculation
    [SerializeField] private Vector3 lastMousePosition;

    //*** Layer currently selected to be rotated
    public GameObject activeRotationLayer;

    public int currentlySelectedLayer;


    //*** The difference between the current and last mouses position!
    [SerializeField] public float mouseXPosition_Delta;

    //*** Min and max acceleration values to maintain some control when spinning a layer.
    public float maxAcceleration = 20f;
    public float minAcceleration = -20f;

    //*** Prefab reference to duplicated layers to create our full towers height
    public GameObject layersPrefab;

    //*** Number of layers currently instantiated in scene
    public int layerCount;

    //*** Control used to make it easier to split a selection interation and spin interaction for the tower layers.
    public float mouseButtonHeldTime;

    public float position_Y_Offset = 17.9f;
    public float position_Y_Multiplier = 3f;
    public float target_Position_y = 0f;
    public Vector3 newTargetCameraPositionVector;
    public Vector3 tempTargetCameraPositionVector;

    public GameObject camera;
    public float cameraTransitionSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //*** General timer used to optimize how often we process items within the update loop
        updateTimer += Time.deltaTime;


        //*** control to refine interactions by tracking mouse click length
        if (Input.GetMouseButton(0))
        {

            mouseButtonHeldTime += Time.deltaTime;
        }
        else
        {

            mouseButtonHeldTime = 0f;
        }


        
        //*** Get last saved mouse position to help with Mouse Delta Calculations
        lastMousePosition = mousePosition;


        //*** Control how often we update our core functionality loop to reduce overal number of process calls
        if (updateTimer < updatetime)
        {
            return;

        }
        else
        {
            //*** Process loop and reset timer 
            updateTimer = 0f;

        }

       
        //*** Call to get collision of raycast with in scene objects to determine Carousel item selection.
        GetMouseRaycastCollision();


        //*** Check if Moust is being clicked , whether tower is rotating, and how long ap layer has held a click to determine if mouse input should be tracked
       if (Input.GetMouseButton(0) && Mathf.Abs(rotatingSpeed) <= 1f && mouseButtonHeldTime >= .25f)
        {
            mousePosition = Input.mousePosition;


            //*** Calulate difference in last mouse Position and new mouse position to get Delta values for rotation
            GetMousePositionDelta();
           
        }

            //*** Apply momentum based on mouse delta
            ApplyMomentum();



        //*** Update Camera Position
        target_Position_y = (position_Y_Multiplier * (float)currentlySelectedLayer) + position_Y_Offset;

        newTargetCameraPositionVector = new Vector3(camera.transform.position.x, target_Position_y, camera.transform.position.z);

        tempTargetCameraPositionVector = Vector3.Lerp(camera.transform.position, newTargetCameraPositionVector, cameraTransitionSpeed * Time.deltaTime);

        camera.transform.position = new Vector3(tempTargetCameraPositionVector.x, tempTargetCameraPositionVector.y, tempTargetCameraPositionVector.z);
    }

    public void GetMousePositionDelta()
    {

        if (lastMousePosition.x == mousePosition.x)
        {
            mouseXPosition_Delta = 0f;

        }
        else
        {

            mouseXPosition_Delta = lastMousePosition.x - mousePosition.x;
            Debug.Log("Mouse Position Delta : " + mouseXPosition_Delta.ToString());

            rotatingSpeed = mouseXPosition_Delta;
            if (mouseXPosition_Delta != 0)
                rotatingSpeed = Mathf.Clamp(mouseXPosition_Delta, minAcceleration, maxAcceleration);
        }
    }

    public void GetMouseRaycastCollision()
    {

        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hitInfo;

            Vector2 mousePosition = Input.mousePosition;

            Ray rayOrigin = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(rayOrigin, out hitInfo))
            {
                Debug.Log("Raycast hit object " + hitInfo.transform.name + " at the position of " + hitInfo.transform.position);

                if(hitInfo.transform.gameObject.name == "CarouselClickables(Clone)")
                {

                    hitInfo.transform.gameObject.SendMessage("AssignClickableTier");

                }

            }

        }


     }

    public void ApplyMomentum()
    {

        //*** Apply Deceleration value to speed variable for deceleration , flip sign based on which way the layer is psinning
        if (rotatingSpeed > 0f)
        { 
            rotatingSpeed = (rotatingSpeed - decelerationSpeed);
        }
        else
        {
            rotatingSpeed = (rotatingSpeed + decelerationSpeed);

        }

        //*** Determine if we superceded the minimum deceleration speed.
        if (Mathf.Abs(rotatingSpeed) < Mathf.Abs(decelerationSpeed))
        {
            //*** If Our sliding speed is lower than the rate of deceleration than just stop the objects momentum entirely
            rotatingSpeed = 0f;
        }

        //*** Take currenct active layer and begin rotating
        if ( Mathf.Abs( rotatingSpeed) >= 2f)
        {

            //testcube.transform.Translate((speedVector * slidingSpeed) * Time.deltaTime);
            activeRotationLayer.transform.Rotate((speedVector * rotatingSpeed), Space.Self);
        }

    }

    //*** Change which layer is being spun and check if this is the top of the tower so far to determine if new layer hsould be added
    public void ChangeActiveClickableTier( GameObject pActivecliackableTier, int pSelectedLayer)
    {
        //***
        currentlySelectedLayer = pSelectedLayer;

        //*** Set NEw Actively rotatable layer
        activeRotationLayer = pActivecliackableTier;

        if (pSelectedLayer == layerCount)
        {
            //** Add new tier

            layerCount = layerCount + 1;


            //*** Take current layer transform values
            Transform oCurrentLayerTransform = this.gameObject.transform;

            //*** Increment transform height based on current layer count and set as currently active layer
            activeRotationLayer = Instantiate(layersPrefab, new Vector3(oCurrentLayerTransform.transform.position.x, 5f * layerCount, oCurrentLayerTransform.position.z), Quaternion.identity);
        }
    }
}
