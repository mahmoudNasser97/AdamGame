using System;
using UnityEngine;

namespace AISystem.Civil.Objects
{
    [System.Serializable]
    public class Action
    {
        public string id; // Guid - limitation with serialization in unity
        public ACTIONS_TYPES ActionType; // Action type
        public ITEMS itemsNeeded; // ITEMS needed
        public GameObject itemOutput;
        public LOOKING_TYPES LookAt = LOOKING_TYPES.NONE;
        public bool SetItemInUse = false;
        public bool NoResetItemOnEnd = false;

        public Action(
            ACTIONS_TYPES ActionType_temp,
            ITEMS ITEMS_temp,
            GameObject ItemOutput_temp,
            LOOKING_TYPES lookAt_temp,
            bool SetItemInUseTemp,
            bool NoResetItemOnEndTemp
        )
        {
            id = Guid.NewGuid().ToString();
            ActionType = ActionType_temp;
            itemsNeeded = ITEMS_temp;
            itemOutput = ItemOutput_temp;
            LookAt = lookAt_temp;
            SetItemInUse = SetItemInUseTemp;
            NoResetItemOnEnd = NoResetItemOnEndTemp;
        }

        public Action(
            string entry_id,
            ACTIONS_TYPES ActionType_temp,
            ITEMS ITEMS_temp,
            GameObject ItemOutput_temp,
            bool SetItemInUseTemp,
            bool NoResetItemOnEndTemp
        )
        {
            id = entry_id;
            ActionType = ActionType_temp;
            itemsNeeded = ITEMS_temp;
            itemOutput = ItemOutput_temp;
            SetItemInUse = SetItemInUseTemp;
            NoResetItemOnEnd = NoResetItemOnEndTemp;
        }

        public ACTIONS_TYPES actionType()
        {
            return ActionType;
        }

        public ITEMS ITEMSNeeded()
        {
            return itemsNeeded;
        }

        public GameObject ItemOutput()
        {
            return itemOutput;
        }

    }
}
