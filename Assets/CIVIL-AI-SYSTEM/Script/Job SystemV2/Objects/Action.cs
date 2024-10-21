using AISystem.Civil.Ownership;
using System;
using UnityEngine;

namespace AISystem.Civil.Objects.V2
{
    [System.Serializable]
    public class Action : BaseNode
    {
        public ACTIONS_TYPES ActionType; // Action type
        public ITEMS itemsNeeded; // ITEMS needed
        public ITEMS_TYPE itemType;
        public GameObject itemOutput;
        public LOOKING_TYPES LookAt = LOOKING_TYPES.NONE;
        public OWNERSHIP ownershipMode = OWNERSHIP.ALL;
        public bool SetItemInUse = false;
        public bool NoResetItemOnEnd = false;
        public bool UpdateEachLoop = false;

        public Action()
        {

        }

        public Action(
            ACTIONS_TYPES ActionType_temp,
            ITEMS ITEMS_temp,
            ITEMS_TYPE itemType,
            GameObject ItemOutput_temp,
            LOOKING_TYPES lookAt_temp,
            OWNERSHIP ownershipMode,
            bool SetItemInUseTemp,
            bool NoResetItemOnEndTemp
        )
        {
            id = Guid.NewGuid().ToString();
            ActionType = ActionType_temp;
            itemsNeeded = ITEMS_temp;
            this.itemType = itemType;
            itemOutput = ItemOutput_temp;
            LookAt = lookAt_temp;
            this.ownershipMode = ownershipMode;
            SetItemInUse = SetItemInUseTemp;
            NoResetItemOnEnd = NoResetItemOnEndTemp;
        }

        public Action(
            string entry_id,
            ACTIONS_TYPES ActionType_temp,
            ITEMS ITEMS_temp,
            ITEMS_TYPE itemType,
            GameObject ItemOutput_temp,
            bool SetItemInUseTemp,
            bool NoResetItemOnEndTemp
        )
        {
            id = entry_id;
            ActionType = ActionType_temp;
            itemsNeeded = ITEMS_temp;
            this.itemType = itemType;
            itemOutput = ItemOutput_temp;
            SetItemInUse = SetItemInUseTemp;
            NoResetItemOnEnd = NoResetItemOnEndTemp;
        }

        public Action(
            string entry_id,
            ACTIONS_TYPES ActionType_temp,
            ITEMS ITEMS_temp,
            ITEMS_TYPE itemType,
            OWNERSHIP ownershipMode,
            GameObject ItemOutput_temp,
            LOOKING_TYPES lookAt_temp,
            bool SetItemInUseTemp,
            bool NoResetItemOnEndTemp
        )
        {
            id = entry_id;
            ActionType = ActionType_temp;
            itemsNeeded = ITEMS_temp;
            this.itemType = itemType;
            itemOutput = ItemOutput_temp;
            this.ownershipMode = ownershipMode;
            LookAt = lookAt_temp;
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

        public OWNERSHIP OwnershipMode()
        {
            return ownershipMode;
        }

        public GameObject ItemOutput()
        {
            return itemOutput;
        }
        public static Action Convert(Objects.Action action)
        {
            Action candidate = new Action(action.id,
                action.ActionType,
                action.itemsNeeded,
                ITEMS_TYPE.NULL,
                OWNERSHIP.ALL,
                action.itemOutput,
                action.LookAt,
                action.SetItemInUse,
                action.NoResetItemOnEnd);

            return candidate;
        }

    }
}
