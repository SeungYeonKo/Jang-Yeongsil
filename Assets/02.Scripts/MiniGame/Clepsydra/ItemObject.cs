using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Pasuho1,
    Pasuho2, 
    Pasuho3,
    Susuho,
    Stick,
    Drum,
    Jing,
    Bell,
    Bead,
    Pulley
}

public class ItemObject : MonoBehaviour
{
    public ItemType ItemType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ItemSlotManager slotManager = FindObjectOfType<ItemSlotManager>();
            if (slotManager != null)
            {
                slotManager.ActivateSlot(ItemType);
                
            }
            Destroy(gameObject);
        }
    }
}
