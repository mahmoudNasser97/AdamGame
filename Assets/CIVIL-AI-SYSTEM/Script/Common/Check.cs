using AISystem.Civil;
using AISystem.ItemSystem;

namespace AISystem.Common
{
    public static class Check
    {
        public static bool CheckAgainstRequirements(NodeConnection node, Controller controller, AIDataBoard databoard)
        {
            RequirementData requirement = node.GetRequirementData();

            if(requirement == null)
            {
                return true;
            }

            return requirement.RequirementsMet(controller, databoard);
        }

        public static bool? ApplyActionRequirement(NodeConnection node, Controller controller, AIDataBoard databoard)
        {
            if(node == null)
            {
                return true;
            }

            RequirementAction CheckData = (RequirementAction)node.GetRequirementData();

            if (CheckData != null)
            {
                if (!controller.GetAppliedActionRequirement())
                {
                    if (CheckData.customAnimation != null)
                    {
                        databoard.SetDynamicAnimation("Dynamic01", CheckData.customAnimation);
                        databoard.SetDynamicAnimation(true);
                    }

                    if (CheckData.waitTime > 0f)
                    {
                        databoard.ResetTimer();
                    }

                    controller.SetAppliedActionRequirement(true);
                }
                else
                {
                    if (CheckData.waitTime < databoard.GetTimer())
                    {
                        databoard.SetDynamicAnimation(false);
                        controller.SetAppliedActionRequirement(false);
                        return true;
                    }
                }

                return null;
            }

            return true;
        }

        public static bool CheckItemNeededIsMet(AIDataBoard databoard)
        {
            var currentAction = databoard.GetCurrentAction();

            if (currentAction.ActionType == ACTIONS_TYPES.IDLE)
            {
                ITEMS itemNeeded = currentAction.ITEMSNeeded();

                if (itemNeeded != ITEMS.NULL)
                {
                    if (databoard.GetCarriedItem() != null)
                    {
                        if (databoard.GetCarriedItem().GetComponent<Item>().itemName == itemNeeded)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            return true;
        }
    }
}