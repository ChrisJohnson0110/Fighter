using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CJ
{
    public class WeaponSlotManager : MonoBehaviour
    {
        WeaponHolderSlot leftHandSlot;
        WeaponHolderSlot rightHandSlot;

        private void Awake()
        {
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots)
            {
                if (weaponSlot.isLeftHandSlot == true)
                {
                    leftHandSlot = weaponSlot;
                }
                else if (weaponSlot.isRightHandSlot == true)
                {
                    rightHandSlot = weaponSlot;
                }
            }
        }

        public void LoadWeaponOnSlot(WeaponItem a_weaponItem, bool a_bIsLeft)
        {
            if (a_bIsLeft == true)
            {
                leftHandSlot.LoadWeaponModel(a_weaponItem);
            }
            else
            {
                rightHandSlot.LoadWeaponModel(a_weaponItem);
            }
        }


    }
}