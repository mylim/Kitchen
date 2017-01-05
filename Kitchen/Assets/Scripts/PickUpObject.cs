﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/** 
Handles objects pickup and dropping
*/

public class PickUpObject : MonoBehaviour
{
    private GameObject mainCamera;
    private GameObject objectsHandler;
    private RaycastHit rayCastHit;

    // Object being carried
    private bool carrying;
    private GameObject carriedObject;
    public float distance;
    public float smooth;

    // Bin manipulation
    private GameObject binLid;
    private GameObject bin;
    //private GameObject singleItem;
    private GameObject tiedBag;
    private GameObject binBag;
    private bool lidOn;
    private bool binEmpty;
    private bool newBag;
    private Vector3 lidPosition;
    private Vector3 binPosition;
    private Vector3 binBagPosition;
    private Vector3 tiedBagPosition;

    // Sound
    public AudioClip dropSound;
    private AudioSource source;

    private GameObject kettle;
    public GameObject sandwich;
    private Object sandwichParent;
    //private GameObject singleTeaBag;
    //private bool teaBagIn;

    private static int NUM_BREAD = 2;
    //private ObjectsHandler objectHandler;

    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
        objectsHandler = GameObject.FindWithTag("ObjectsHandler");
        carrying = false;

        // Garbage removal
        bin = GameObject.FindWithTag("Bin");
        binLid = GameObject.FindWithTag("BinLid");
        binBag = GameObject.FindWithTag("BinBag");
        tiedBag = GameObject.FindWithTag("TiedBag");
        

        /*if (GameObject.FindWithTag("Bin"))
        {
            bin = GameObject.FindWithTag("Bin");
        }

        if (GameObject.FindWithTag("BinLid"))
        {
            binLid = GameObject.FindWithTag("BinLid");           
        }       
      
        if (GameObject.FindWithTag("BinBag"))
        {
            binBag = GameObject.FindWithTag("BinBag");
        }

        if (GameObject.FindWithTag("TiedBag"))
        {
            Debug.Log("Tied Bag found");
            tiedBag = GameObject.FindWithTag("TiedBag");
        }*/

        // Tea making
        kettle = GameObject.FindWithTag("Kettle");
        /*if (GameObject.FindWithTag("Kettle"))
        {
            Debug.Log("Kettle");
            kettle = GameObject.FindWithTag("Kettle");
        }*/
        /*if (GameObject.FindWithTag("SingleTeaBag"))
        {
            singleTeaBag = GameObject.FindWithTag("SingleTeaBag");
            singleTeaBag.SetActive(false);
            teaBagIn = false;
        }*/
        source = GetComponent<AudioSource>();  
    }

    void Update()
    {
        if (carrying)
        {
            Debug.Log("Carrying");
            Carry(carriedObject);
            CheckDrop();
        }
        else
        {
            Debug.Log("Pickup");
            Pickup();
        }
    }

    void Carry(GameObject cObject)
    {
        cObject.transform.position = Vector3.Lerp(cObject.transform.position, mainCamera.transform.position + mainCamera.transform.forward * distance, Time.deltaTime * smooth);
        //cObject.transform.rotation = Quaternion.identity;
    }

    void Pickup()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Collider collider = GetComponent<MouseHoverObject>().GetMouseHoverObject(2);
            if (collider != null)
            {
                Debug.Log("In Pickup()");
                if (collider.GetComponent<Pickupable>())
                {
                    Pickupable p = collider.GetComponent<Pickupable>();
                    // Kettle is clicked 
                    if (p.gameObject.tag.Equals("Kettle") && (kettle.GetComponent<FilledWithWater>().filledWithWater) && (!kettle.GetComponent<BoiledWater>().boiledWater))
                    {
                        // The kettle has been filled with water
                        Debug.Log("Boiling water");
                        kettle.GetComponent<BoiledWater>().boiledWater = true;                       
                    }                    
                    else
                    {
                        CarriedObject(p.gameObject);

                        // Bin lid has been removed
                        if (p.gameObject.tag.Equals("BinLid"))
                        {
                            if (bin.GetComponent<LidOn>().lidOn)
                            {
                                bin.GetComponent<LidOn>().lidOn = false;
                            }
                        }
                        // Tied bag has been removed
                        else if (p.gameObject.tag.Equals("TiedBag"))
                        {
                            if (!bin.GetComponent<LidOn>().lidOn)
                            {
                                bin.GetComponent<BinEmpty>().binEmpty = true;
                            }
                        }
                    }
                    if (p.gameObject.transform.parent != null)
                    {
                        objectsHandler.GetComponent<ObjectsHandler>().addPickedObject(p.gameObject.tag, p.gameObject.transform.parent.tag, p.gameObject);
                    } 
                    else
                    {
                        objectsHandler.GetComponent<ObjectsHandler>().addPickedObject(p.gameObject.tag, p.gameObject.tag, p.gameObject);
                    }

                    // Picked the correct item
                    if (p.gameObject.GetComponent<CorrectItem>())
                    {
                        Debug.Log("Correct " + p.gameObject.tag + " picked");
                    }
                }
                // Picking up a single item of a collection, eg. bin bag, napkin
                else if (collider.GetComponent<PickupableSingle>())
                {
                    PickupableSingle pickSingle = collider.GetComponent<PickupableSingle>();
                    GameObject singleItem = Instantiate(pickSingle.singleItem);
                    singleItem.GetComponent<Renderer>().material.SetColor("_Color", pickSingle.itemColor);
                    CarriedObject(singleItem);

                    objectsHandler.GetComponent<ObjectsHandler>().addPickedObject(pickSingle.gameObject.tag, pickSingle.gameObject.gameObject.tag, pickSingle.gameObject);

                    // Picked the correct item
                    if (pickSingle.gameObject.GetComponent<CorrectItem>())
                    {
                        Debug.Log("Correct " + pickSingle.gameObject.tag + " picked");
                    }

                    /*singleItem.GetComponent<Renderer>().material.SetColor("_Color", pickSingle.itemColor);
                    singleItem.SetActive(true);
                    CarriedObject(singleItem);*/
                }
                // Picking up a single tea bag
                else if (collider.GetComponent<PickupableTeaBag>())
                {
                    PickupableTeaBag pickTeaBag = collider.GetComponent<PickupableTeaBag>();
                    GameObject singleTeaBag = Instantiate(pickTeaBag.singleTeaBag);
                    CarriedObject(singleTeaBag);

                    objectsHandler.GetComponent<ObjectsHandler>().addPickedObject(pickTeaBag.gameObject.tag, pickTeaBag.gameObject.tag, pickTeaBag.gameObject);

                    // Picked the correct item
                    if (pickTeaBag.gameObject.GetComponent<CorrectItem>())
                    {
                        Debug.Log("Correct " + pickTeaBag.gameObject.tag + " picked");
                    }
                    //singleTeaBag.SetActive(true);
                    //CarriedObject(singleTeaBag);
                }
                else
                {
                    // Bin bag clicked
                    Debug.Log("Collider " + collider.gameObject.name);
                    if (collider.tag.Equals("BinBag"))
                    {
                        if (!bin.GetComponent<LidOn>().lidOn)
                        {
                            //Make the bin bag invisible
                            Debug.Log("Bin Bag clicked");
                            binBag.SetActive(false);

                            // Make tied bag visible 
                            if (!bin.GetComponent<NewBag>().newBag && tiedBag)
                            {
                                Debug.Log("Tied Bag found");
                                tiedBag.SetActive(true);
                            }
                            else if (bin.GetComponent<NewBag>().newBag)
                            {
                                bin.GetComponent<NewBag>().newBag = false;
                            }
                        }
                    }
                    /*else if (collider.tag.Equals("SingleNapkin") || collider.tag.Equals("HamSlice") ||
                     collider.tag.Equals("CheeseSlice") || collider.tag.Equals("SingleSlice")
                     collider.tag.Equals("CheeseSlice") || collider.tag.Equals("SingleSlice")))*/
                    else if ((collider.transform.parent != null) && collider.transform.parent.tag.Equals("Sandwich"))
                    {
                       
                        {
                            Debug.Log("Sandwich exists");
                            CarriedObject(sandwich);
                        }
                    }
                }
            }
        }
    }

    void CarriedObject(GameObject gameObject)
    {
        Debug.Log("pickupable " + gameObject);
        Debug.Log("carried object" + gameObject.tag);
        carriedObject = gameObject;
        carrying = true;
        if (carriedObject.GetComponent<Rigidbody>())
        {
            carriedObject.GetComponent<Rigidbody>().useGravity = false;
        }
        //carriedObject.GetComponentInParent<Rigidbody>().useGravity = false;
    }

    void CheckDrop()
    {
        if (Input.GetMouseButtonDown(1))
        {
            DropObject();
        }
    }

    void DropObject()
    {
        Collider collider = GetComponent<MouseHoverObject>().GetMouseHoverObject(2);
     
        if (collider != null)
        {
            Debug.Log("Collider for drop object " + collider.gameObject.tag);
            // Tap is clicked while carrying the kettle = filling water
            if (collider.tag.Equals("Tap"))
            {
                if (carriedObject.tag.Equals("Kettle"))
                {
                    Debug.Log("Filling water ");
                    kettle.GetComponent<FilledWithWater>().filledWithWater = true;
                }
                else if (carriedObject.tag.Equals("Bowl") || carriedObject.tag.Equals("PicnicPlate") || carriedObject.tag.Equals("SmallPlate") || carriedObject.tag.Equals("Saucer"))
                {
                    Debug.Log("Rinsing dirty bowl ");
                }
            }
            else if (collider.tag.Equals("Mug") || collider.tag.Equals("Cup") || collider.tag.Equals("Jar") || collider.tag.Equals("CoffeeMug"))
            {
                if (carriedObject.tag.Equals("Kettle"))
                {
                    Debug.Log("Pouring water ");
                    kettle.GetComponent<PouredWater>().pouredWater = true;
                    collider.GetComponent<HasContent>().hasWater = true;
                }
                else if (carriedObject.tag.Equals("SingleTeaBag"))
                {
                    //Drop(carriedObject.GetComponent<Collider>());
                    carriedObject.SetActive(false);
                    carrying = false;
                    carriedObject = null;  
                    Debug.Log("Tea bag is in");
                    /*singleTeaBag.GetComponent<TeaBagIn>().teaBagIn = true;
                    singleTeaBag.SetActive(false);                   */
                }
            }
            else if (collider.tag.Equals("Bowl") || collider.tag.Equals("PicnicPlate") || collider.tag.Equals("SmallPlate") || collider.tag.Equals("Saucer"))
            {
                if (carriedObject.tag.Equals("Cereal"))
                {
                    Debug.Log("Pouring Cereal");
                }
                else if (carriedObject.tag.Equals("Milk"))
                {
                    Debug.Log("Pouring Milk");
                }
                else if (carriedObject.tag.Equals("Honey"))
                {
                    Debug.Log("Pouring Honey ");
                }
                else if (carriedObject.tag.Equals("Sponge"))
                {
                    Debug.Log("Washing dirty bowl ");
                }
                else
                {
                    Drop(collider);
                }
            }
            else if (collider.tag.Equals("BinBody"))
            {
                if ((carriedObject.tag.Equals("SingleItem")) && (bin.GetComponent<BinEmpty>().binEmpty) && (!bin.GetComponent<LidOn>().lidOn))
                {
                    binBag.SetActive(true);
                    //singleItem.SetActive(false);                   
                    Debug.Log("Bin bag default color " + binBag.GetComponent<Renderer>().material.GetColor("_Color"));
                    binBag.GetComponent<Renderer>().material.SetColor("_Color", carriedObject.GetComponent<Renderer>().material.GetColor("_Color"));
                    bin.GetComponent<NewBag>().newBag = true;
                    carriedObject.SetActive(false);
                }
            }
            else if (collider.tag.Equals("BinLid"))
            {
                if (carriedObject.tag.Equals("Bowl") || carriedObject.tag.Equals("PicnicPlate") || carriedObject.tag.Equals("SmallPlate") || carriedObject.tag.Equals("Saucer"))
                {
                    Debug.Log("Disposing Cereal");
                }
            }
            else if (collider.tag.Equals("Sponge"))
            {
                if (collider.tag.Equals("DishwashingLiquid"))
                {
                    Debug.Log("Putting dishwashing liquid on sponge");
                }
            }
            else
            {
                Drop(collider);
            }
        }
    }

    void Drop(Collider collider)
    {
        Vector3 originalPosition = carriedObject.GetComponent<Pickupable>().getOriginalPosition();
        float distance = Vector3.Distance(originalPosition, collider.transform.position);
        if (distance < 0.2f)
        {
            Debug.Log(carriedObject.name + " Distance " + distance);
            if (carriedObject == binLid)
            {
                Debug.Log("Lid distance " + distance);
                bin.GetComponent<LidOn>().lidOn = true;
                Debug.Log("Lid on");
            }
            else if (carriedObject == tiedBag)
            {
                Debug.Log("Bag distance " + distance);
                if (!bin.GetComponent<LidOn>().lidOn && !bin.GetComponent<NewBag>().newBag)
                {
                    bin.GetComponent<BinEmpty>().binEmpty = false;
                    Debug.Log("Bin is not empty");
                }
            }
            carriedObject.transform.position = originalPosition;
        }
        else
        {
            carriedObject.transform.position = GetComponent<MouseHoverObject>().GetHitPoint();
            if (carriedObject.tag.Equals("SingleNapkin") || carriedObject.tag.Equals("SingleSlice") ||
                carriedObject.tag.Equals("SingleWrap") || carriedObject.tag.Equals("SinglePitta") || carriedObject.tag.Equals("SingleRoll") ||
                carriedObject.tag.Equals("HamSlice") || carriedObject.tag.Equals("CheeseSlice"))
            {
                carriedObject.transform.parent = sandwich.transform;
                Destroy(carriedObject.GetComponent<Rigidbody>());
                Destroy(carriedObject.GetComponent<Pickupable>());
            }
            else
            {
                if (carriedObject.tag.Equals("Bread") || carriedObject.tag.Equals("Breadroll") || carriedObject.tag.Equals("Pitta") ||
                    carriedObject.tag.Equals("Wrap") || carriedObject.tag.Equals("Ham") || carriedObject.tag.Equals("Cheese"))
                {
                    carriedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    //Vector3 movement = carriedObject.transform.position + GetComponent<MouseHoverObject>().GetHitPoint();
                    Debug.Log("carried object tag" + carriedObject.tag);

                    if (collider.tag.Equals("KitchenTop") || collider.tag.Equals("Table"))
                    {
                        Debug.Log("collider tag" + collider.tag);
                        // Instantiate items only if the object is placed on the table                            
                        if (carriedObject.GetComponent<InstantiateItem>())
                        {
                            carriedObject.GetComponent<OnTable>().onTable = true;
                            //carriedObject.GetComponentInParent<InstantiateItem>().Instantiate((carriedObject.transform.position + (transform.up * (carriedObject.GetComponent<Collider>().bounds.size.y / 2))), 2);
                            carriedObject.GetComponent<InstantiateItem>().Instantiate((carriedObject.transform.position + (transform.up * (carriedObject.GetComponent<Collider>().bounds.size.y))), 2);
                        }
                    }
                }
                else if (carriedObject.transform.parent != null)
                {
                    if (carriedObject.transform.parent.tag.Equals("Cutlery") || carriedObject.transform.parent.tag.Equals("BeverageContainers") || carriedObject.transform.parent.tag.Equals("Sandwich"))
                    {
                        Collider[] hitColliders = Physics.OverlapSphere(carriedObject.transform.position, 0.1f);                       
                        foreach (Collider hitCollider in hitColliders)
                        {                 
                            if (hitCollider.tag.Equals("Bowl") || hitCollider.tag.Equals("PicnicPlate") || hitCollider.tag.Equals("SmallPlate") || hitCollider.tag.Equals("Saucer"))
                            {
                                // direction from the cutlery to the dish
                                /*Vector3 dir = (hitCollider.transform.position - carriedObject.transform.position);
                                float angle = Vector3.Angle(dir, mainCamera.transform.forward);


                                Debug.DrawLine(carriedObject.transform.position, mainCamera.transform.forward, Color.red, 2f);
                                Debug.DrawLine(hitCollider.transform.position, carriedObject.transform.position, Color.green, 2f);
                                   
                                Debug.Log("Angle + collider " + angle + hitCollider.tag);
                                float angleDir = AngleDir(mainCamera.transform.forward, dir, mainCamera.transform.up);
                                Debug.Log("AngleDir " + angleDir);
                                if (angleDir < 0f)
                                {
                                    Debug.Log("Cutlery is at the right of dishes " + hitCollider.tag);
                                }
                                else if (angleDir > 0f)
                                {
                                    Debug.Log("Cutlery is at the wrong side of " + hitCollider.tag);
                                }
                                else
                                {
                                    Debug.Log("Cutlery is at the front or back of " + hitCollider.tag);
                                }*/

                                // direction from the dish to the cutlery
                                Vector3 dir = (carriedObject.transform.position - hitCollider.transform.position);
                                float angle = Vector3.Angle(dir, mainCamera.transform.forward);
                                    
                                //Debug.DrawLine(hitCollider.transform.position, mainCamera.transform.forward, Color.red, 2f);
                                //Debug.DrawLine(carriedObject.transform.position, hitCollider.transform.position, Color.green, 2f);
                                Debug.Log("Angle + collider " + angle + hitCollider.tag);
                                float angleDir = AngleDir(mainCamera.transform.forward, dir, mainCamera.transform.up);
                                Debug.Log("AngleDir " + angleDir);
                                if (angleDir > 0.0f && (angle > 45f && angle < 135f))
                                {
                                    Debug.Log(carriedObject.transform.parent.tag + " is at the right of dishes " + hitCollider.tag);
                                }
                                else if (angleDir < 0.0f)
                                {
                                    Debug.Log(carriedObject.transform.parent.tag + "  is at the wrong side of " + hitCollider.tag);
                                }
                                else
                                {
                                    Debug.Log(carriedObject.transform.parent.tag + " is at the front or back of " + hitCollider.tag);
                                }                             
                            }
                        }  
                        /* Finding the closest object
                        Collider[] hitColliders = Physics.OverlapSphere(carriedObject.transform.position, 0.1f);
                        Collider closest = null;
                        foreach (Collider hitCollider in hitColliders)
                        {
                            Debug.Log("Hit collider " + hitCollider.tag);
                            if (hitCollider != carriedObject.GetComponent<Collider>())
                            {
                                // if closest is null
                                if (closest == null)
                                {
                                    closest = hitCollider;
                                }
                                if (Vector3.Distance(carriedObject.transform.position, hitCollider.transform.position) <
                                    Vector3.Distance(carriedObject.transform.position, closest.transform.position))
                                {
                                    closest = hitCollider;
                                }
                            }
                        }
                        Debug.Log("Closest object " + closest.tag);*/                
                    }
                    /*else if (carriedObject.transform.parent.tag.Equals("BeverageContainers"))
                    {
                        Collider[] hitColliders = Physics.OverlapSphere(carriedObject.transform.position, 0.1f);
                        foreach (Collider hitCollider in hitColliders)
                        {
                            if (hitCollider.tag.Equals("Bowl") || hitCollider.tag.Equals("PicnicPlate") || hitCollider.tag.Equals("SmallPlate") || hitCollider.tag.Equals("Saucer"))
                            {
                                // direction from the dish to the cutlery
                                Vector3 dir = (carriedObject.transform.position - hitCollider.transform.position);
                                float angle = Vector3.Angle(dir, mainCamera.transform.forward);

                                //Debug.DrawLine(hitCollider.transform.position, mainCamera.transform.forward, Color.red, 2f);
                                //Debug.DrawLine(carriedObject.transform.position, hitCollider.transform.position, Color.green, 2f);
                                Debug.Log("Angle + collider " + angle + hitCollider.tag);
                                float angleDir = AngleDir(mainCamera.transform.forward, dir, mainCamera.transform.up);
                                Debug.Log("AngleDir " + angleDir);
                                if (angleDir > 0.0f && (angle > 45f && angle < 135f))
                                {
                                    Debug.Log("Cutlery is at the right of dishes " + hitCollider.tag);
                                }
                                else if (angleDir < 0.0f)
                                {
                                    Debug.Log("Cutlery is at the wrong side of " + hitCollider.tag);
                                }
                                else
                                {
                                    Debug.Log("Cutlery is at the front or back of " + hitCollider.tag);
                                }
                            }
                        }                    
                    }*/

                }
            }
        }

        carrying = false;
        Debug.Log("carrying is false");
        //carriedObject.GetComponent<Rigidbody>().AddForce(-transform.up * 20f);
        //carriedObject.GetComponent<Rigidbody>().AddTorque(transform.forward);
        /*if (carriedObject.GetComponent<IsKinematic>())
        {
            carriedObject.GetComponent<Rigidbody>().isKinematic = true;
        }*/
        if (carriedObject.GetComponent<Rigidbody>())
        {
            carriedObject.GetComponent<Rigidbody>().useGravity = true;
        }
        //source.PlayOneShot(dropSound);
        carriedObject = null;
    }

    private float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        //returns negative when to the left, positive to the right, and 0 for forward/backward
        Vector3 right = Vector3.Cross(up, fwd);        // right vector
        float dir = Vector3.Dot(right, targetDir);
        Debug.Log("AngleDir " + dir);
        return dir;

        /*returns -1 when to the left, 1 to the right, and 0 for forward/backward
        if (dir > 0.0f)
        {
            Debug.Log("Returning 1");
            return 1f;
        }
        else if (dir < 0.0f)
        {
            Debug.Log("Returning -1");
            return -1f;
        }
        else
        {
            Debug.Log("Returning 0");
            return 0f;
        }*/
    }

}
