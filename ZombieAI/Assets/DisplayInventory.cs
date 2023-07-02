using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayInventory : MonoBehaviour
{
    public InventoryObject inventory;
    
    int spaceBetweenItems = 264;
    int numberOfColumns = 5;
    int xStart = -524;
    int yStart = 141;


    TextMeshProUGUI nameText;
    TextMeshProUGUI descriptionText;

    // Start is called before the first frame update
    void Awake()
    {
        nameText = GameObject.Find("NameTMP").GetComponent<TextMeshProUGUI>();
        descriptionText = GameObject.Find("DescriptionTMP").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateItemText(string name, string description)
    {
        nameText.text = name;
        descriptionText.text = description;
    }

    void ResetText()
    {
        nameText.text = "";
        descriptionText.text = "";
    }

    void Display()
    {
        for(int i = 0; i < inventory.container.Count; i++) 
        {
            var slot = inventory.container[i];
            var obj = Instantiate(slot.item.uiPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            var eventTrigger = obj.GetComponent<EventTrigger>();
            AddEventTriggerListener(
                eventTrigger,
                EventTriggerType.PointerEnter,
                (eventData) => { UpdateItemText(slot.item.itemName, slot.item.description); });
            AddEventTriggerListener(
                eventTrigger,
                EventTriggerType.PointerExit,
                (eventData) => { UpdateItemText(string.Empty, string.Empty); });
        }
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(xStart + (spaceBetweenItems * (i % numberOfColumns)), yStart, 0);
    }

    void AddEventTriggerListener(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        ResetText();
        Display();
        
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

}
