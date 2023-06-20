using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CJ
{
    public class PlayerInventory : MonoBehaviour
    {
        WeaponSlotManager WeaponSlotManagerRef;

        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;

        private void Awake()
        {
            WeaponSlotManagerRef = GetComponentInChildren<WeaponSlotManager>();
        }

        private void Start()
        {
            WeaponSlotManagerRef.LoadWeaponOnSlot(rightWeapon, false);
            WeaponSlotManagerRef.LoadWeaponOnSlot(leftWeapon, true);
        }

    }
}
