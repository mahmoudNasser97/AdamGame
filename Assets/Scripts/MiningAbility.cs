using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character.Abilities.Items;

public class MiningAbility : DetectObjectAbilityBase
{
    public override bool ShouldBlockAbilityStart(Ability startingAbility)
    {
        return startingAbility is ItemAbility;
    }
    public override void OnTriggerExit(Collider other)
    {
        if (other.gameObject == m_DetectedObject)
        {
            StopAbility();
        }
        base.OnTriggerExit(other);
    }
}
