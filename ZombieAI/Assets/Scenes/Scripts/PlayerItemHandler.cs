using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemHandler : MonoBehaviour
{
    [SerializeField]
    Camera camera;

    //changing crosshair
    [SerializeField]
    Texture basicCrosshair;
    [SerializeField]
    Texture itemGrabCrosshair;
    [SerializeField]
    RawImage crosshairImage;

    public InventoryObject inventory;

    bool interactable = false;
    public bool Interactable
    {
        get { return interactable; }
        private set
        {
            interactable = value;
        }
    }

    GameObject lastLookedAtItem;
    InputController inputController;
    GameObject inventoryDisplay;

    //variables for controlling emission
    MeshRenderer mainRenderer;
    List<MeshRenderer> childrenRenderers = new List<MeshRenderer>();
    const string emission_str = "_EMISSION";

    void Awake()
    {
        inputController = GetComponent<InputController>();
        inventoryDisplay = GetComponentInChildren<DisplayInventory>().gameObject;
        inventoryDisplay.SetActive(false);
    }

    void PickUpItem()
    { 
        if(inputController.GetWeaponShotInput(false))
        {
            var item = lastLookedAtItem.GetComponent<Item>();
            if(item != null)
            {
                if(inventory.AddItem(item.item))
                {
                    childrenRenderers.Clear();
                    Destroy(lastLookedAtItem);
                    //Debug.Log("Player picked up 1 " + item.item.itemName);
                    interactable = false;
                }
            }
        }
    }

    void HandleItemHighlight()
    {
        if(interactable)
        {
            GameObject item = lastLookedAtItem;
            mainRenderer = item.GetComponent<MeshRenderer>();
            childrenRenderers = item.GetComponentsInChildren<MeshRenderer>().ToList();

            if(mainRenderer !=null)
                mainRenderer.material.EnableKeyword(emission_str);
    
            foreach (var renderer in childrenRenderers)
            {
                renderer.material.EnableKeyword(emission_str);
            }

            crosshairImage.texture = itemGrabCrosshair;

        }
        else
        {
            if (mainRenderer != null)
            {
                mainRenderer.material.DisableKeyword(emission_str);
            }
            foreach (var renderer in childrenRenderers)
            {
                if(renderer != null)
                    renderer.material.DisableKeyword(emission_str);
            }
            crosshairImage.texture = basicCrosshair;
        }
    }

    void CheckRaycast()
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * 3, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 3f) && hit.transform.gameObject.GetComponent<Item>() != null)
        {
            //Debug.Log("raycast hit");
            lastLookedAtItem = hit.transform.gameObject;
            interactable = true;
        }
        else
        {
            interactable = false;
        }
    }

    void HandleInventoryOpen()
    {
        if(inputController.GetInventoryOpenInput())
        {
            bool enabled = inventoryDisplay.activeSelf;
            inventoryDisplay.SetActive(!enabled);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckRaycast();
        HandleItemHighlight();

        if(interactable)
            PickUpItem();

        HandleInventoryOpen();
    }

    private void OnApplicationQuit()
    {
        inventory.Clear();
    }
}
