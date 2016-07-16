﻿using System;
using System.Collections.Generic;

using TestOpenGL.VisualObjects;
using TestOpenGL.VisualObjects.ChieldsItem;

namespace TestOpenGL.BeingContents
{
    class Inventory
    {
        // Неэкипированные вещи, хранящиеся в мешке.
        List<Item> bag;

        // Экипированные вещи.
        List<Item> equipment;

        public EventInventory eventsInventory;
        //-------------


        public Inventory()
        {
            bag = new List<Item>();
            equipment = new List<Item>();
            eventsInventory = new EventInventory();
        }
        //=============


        public void PutBagItem(Item i)
        {
            bag.Add(i);
            eventsInventory.InventoryChangeBag();
        }
        public void ThrowBagItem(int num)
        {
            try
            {
                bag.RemoveAt(num);
                eventsInventory.InventoryChangeBag();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public List<Item> GetBagItems()
        {
            return bag;
        }
        public List<Item> GetEquipmentItems()
        {
            return equipment;
        }

        private Item GetItemByPart(Part part, List<Item> listItems)
        {
            foreach (Item i in listItems)
                foreach (Part p in i.Parts)
                    if (p == part)
                        return i;
            return null;
        }
        public Item GetEquipWeapon()
        {
            return GetItemByPart(Part.RHand, equipment);
        }
        public int GetEquipWeaponLevel()
        {
            Item i = GetEquipWeapon();
            if (i != null)
                return i.Level;
            else
                return 0;
        }
        public Item GetEquipShield()
        {
            Item i = GetItemByPart(Part.LHand, equipment);
            if (i is Shield)
                return i;
            return null;
        }
        public int GetEquipShieldLevel()
        {
            Item i = GetEquipShield();
            if (i != null)
                return i.Level;
            else
                return 0;
        }
        public List<Item> GetEquipArmors()
        {
            List<Item> listArmor = new List<Item>();
            for (int i = 0; i < 3; i++)
            {
                listArmor.Add(GetItemByPart((Part)i, equipment));
            }
            listArmor.RemoveAll(x => x == null);
            return listArmor;
        }
        public int GetEquipArmorsLevel()
        {
            List<Item> li = GetEquipArmors();
            if (li.Count == 0)
                return 0;
            else
            {
                int count = 0;
                foreach (Item i in li)
                    count += i.Level;
                return count;
            }
        }

        public bool EquipItem(int num)
        {
            try
            {
                Item ei = bag[num];


                foreach (Item i in equipment)
                {
                    foreach (Part p in i.Parts)
                    {
                        foreach (Part pp in ei.Parts)
                        {
                            if (p == pp)
                            {
                                return false;
                            }
                        }
                    }
                }

                equipment.Add(ei);
                bag.Remove(ei);
                eventsInventory.InventoryChangeBag();
                eventsInventory.InventoryChangeEquipment();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void UnequipItem(int num)
        {
            try
            {
                bag.Add(equipment[num]);
                equipment.RemoveAt(num);
                eventsInventory.InventoryChangeBag();
                eventsInventory.InventoryChangeEquipment();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    class EventInventory
    {
        public event VoidEventDelegate EventInventoryChangeEquipment;
        public event VoidEventDelegate EventInventoryChangeBag;

        public void InventoryChangeBag()
        {
            if (EventInventoryChangeBag != null)
                EventInventoryChangeBag();
        }
        public void InventoryChangeEquipment()
        {
            if (EventInventoryChangeEquipment != null)
                EventInventoryChangeEquipment();
        }
    }
}
