///// ---------------------------------------------
///// Ultimate Character Controller
///// Copyright (c) Opsive. All Rights Reserved.
///// https://www.opsive.com
///// ---------------------------------------------

//namespace Opsive.UltimateCharacterController.Integrations.FinalIK
//{
//    using Opsive.Shared.Events;
//    using Opsive.Shared.Game;
//    using Opsive.UltimateCharacterController.Character;
//    using Opsive.UltimateCharacterController.Inventory;
//    using Opsive.UltimateCharacterController.Items;
//    using Opsive.UltimateCharacterController.Motion;
//    using Opsive.UltimateCharacterController.Utility;
//    using RootMotion.FinalIK;
//    using UnityEngine;

//    /// <summary>
//    /// A bridge component which allows FinalIK to be used with the Ultimate Character Controller.
//    /// </summary>
//    public class FinalIKBridge : CharacterIKBase
//    {
//        [Tooltip("The character's head. This field will be filled in automatically if the character is using the FullBodyBipedIK component.")]
//        [SerializeField] protected Transform m_Head;
//        [Tooltip("An offset to apply to the look at direction for the body and arms.")]
//        [SerializeField] protected Vector3 m_LookAtOffset;
//        [Tooltip("The adjustment speed to apply to the Look At position.")]
//        [SerializeField] protected float m_LookAtAdjustmentSpeed = 0.5f;
//        [Tooltip("The name of the state when the character is actively looking at a target.")]
//        [SerializeField] protected string m_ActiveLookAtStateName = "LookAt";
//        [Tooltip("The positional spring used for IK movement.")]
//        [SerializeField] protected Spring m_PositionSpring = new Spring();
//        [Tooltip("The rotational spring used for IK movement.")]
//        [SerializeField] protected Spring m_RotationSpring = new Spring(0.2f, 0.05f);
//        [Tooltip("The direction of the animated weapon aiming in character space. Tweak this value to adjust the aiming.")]
//        public Vector3 m_AnimatedAimDirection = Vector3.forward;

//        public override bool UseOnAnimatorIK { get { return false; } }

//        public Vector3 LookAtOffset { get { return m_LookAtOffset; } set { m_LookAtOffset = value; } }
//        public float LookAtAdjustmentSpeed { get { return m_LookAtAdjustmentSpeed; } set { m_LookAtAdjustmentSpeed = value; } }
//        public string ActiveLookAtStateName { get { return m_ActiveLookAtStateName; } set { m_ActiveLookAtStateName = value; } }
//        public Spring PositionSpring
//        {
//            get { return m_PositionSpring; }
//            set
//            {
//                m_PositionSpring = value;
//                if (m_PositionSpring != null) { m_PositionSpring.Initialize(false, true); }
//            }
//        }
//        public Spring RotationSpring
//        {
//            get { return m_RotationSpring; }
//            set
//            {
//                m_RotationSpring = value;
//                if (m_RotationSpring != null) { m_RotationSpring.Initialize(true, true); }
//            }
//        }

//        private GameObject m_GameObject;
//        private Transform m_Transform;
//        private InventoryBase m_Inventory;
//        private ILookSource m_LookSource;

//        private FullBodyBipedIK m_FullBodyBipedIK;
//        private LookAtIK m_LookAtIK;
//        private AimIK m_AimIK;
//        private InteractionSystem m_InteractionSystem;

//        private Transform m_Target;
//        private bool m_ActiveLookAtTarget;
//        private Vector3 m_LookAtTargetPosition;
//        private bool m_Aiming;
//        private bool m_ItemInUse;
//        private int m_DominantSlotID;
//        private bool m_FixedUpdate;

//        public Transform Target => m_Target;

//        /// <summary>
//        /// Initialize the default values.
//        /// </summary>
//        protected override void Awake()
//        {
//            base.Awake();

//            // Wait to enable until a look source has been attached.
//            enabled = false;

//            var characterLocomotion = gameObject.GetComponentInParent<UltimateCharacterLocomotion>();
//            m_GameObject = characterLocomotion.gameObject;
//            m_Transform = characterLocomotion.transform;
//            m_FullBodyBipedIK = GetComponent<FullBodyBipedIK>();
//            m_LookAtIK = GetComponent<LookAtIK>();
//            m_AimIK = GetComponent<AimIK>();
//            m_InteractionSystem = GetComponent<InteractionSystem>();

//            // Initialize the IK components. Disable the FinalIK components as they will be updated manually.
//            if (m_FullBodyBipedIK != null) {
//                m_FullBodyBipedIK.enabled = false;
//                if (m_Head == null) {
//                    m_Head = m_FullBodyBipedIK.references.head;
//                }
//            }
//            if (m_AimIK != null || m_LookAtIK != null) {
//                m_Target = new GameObject(gameObject.name + "IKTarget").transform;
//                if (m_LookAtIK != null) {
//                    m_LookAtIK.solver.target = m_Target;
//                    m_LookAtIK.enabled = false;
//                }
//                if (m_AimIK != null) {
//                    m_AimIK.solver.target = m_Target;
//                    m_AimIK.enabled = false;
//                }
//                if (m_Head == null) {
//                    Debug.LogError("Error: No head is specified.");
//                    enabled = false;
//                    return;
//                }
//            }

//            m_PositionSpring.Initialize(false, true);
//            m_RotationSpring.Initialize(true, true);

//            EventHandler.RegisterEvent<ILookSource>(m_GameObject, "OnCharacterAttachLookSource", OnAttachLookSource);
//            EventHandler.RegisterEvent<bool, Character.Abilities.Items.Use>(m_GameObject, "OnUseAbilityStart", OnUseStart);
//            EventHandler.RegisterEvent<bool>(m_GameObject, "OnAimAbilityAim", OnAim);
//            EventHandler.RegisterEvent<CharacterItem, int>(m_GameObject, "OnInventoryEquipItem", OnEquipItem);
//            EventHandler.RegisterEvent<CharacterItem, int>(m_GameObject, "OnInventoryUnequipItem", OnUnequipItem);
//            EventHandler.RegisterEvent<int, Vector3, Vector3, bool>(m_GameObject, "OnAddSecondaryForce", OnAddForce);
//            EventHandler.RegisterEvent<Vector3, Vector3, GameObject>(m_GameObject, "OnDeath", OnDeath);
//            EventHandler.RegisterEvent(m_GameObject, "OnRespawn", OnRespawn);
//        }

//        /// <summary>
//        /// A new ILookSource object has been attached to the character.
//        /// </summary>
//        /// <param name="lookSource">The ILookSource object attached to the character.</param>
//        private void OnAttachLookSource(ILookSource lookSource)
//        {
//            m_LookSource = lookSource;
//            if (m_LookSource != null && m_LookAtIK != null) {
//                m_Target.position = m_Head.position + (GetDefaultLookAtPosition() - m_Head.position).normalized;
//            }
//            enabled = m_LookSource != null;
//        }

//        /// <summary>
//        /// An item was equipepd or unequipped. Determine the new dominant hand.
//        /// </summary>
//        private void DetermineDominantHand()
//        {
//            if (m_FullBodyBipedIK == null) {
//                return;
//            }

//            if (m_Inventory == null) {
//                m_Inventory = m_GameObject.GetCachedComponent<InventoryBase>();
//            }
//            CharacterItem dominantItem = null;
//            for (int i = 0; i < m_Inventory.SlotCount; ++i) {
//                var item = m_Inventory.GetActiveCharacterItem(i);
//                if (item != null && item.DominantItem) {
//                    dominantItem = item;
//                    break;
//                }
//            }

//            // The hands should act independently if there are no items.
//            if (dominantItem == null) {
//                m_DominantSlotID = -1;
//            } else {
//                m_DominantSlotID = dominantItem.SlotID;
//            }
//        }

//        /// <summary>
//        /// An item has been equipped.
//        /// </summary>
//        /// <param name="characterItem">The equipped item.</param>
//        /// <param name="slotID">The slot that the item now occupies.</param>
//        private void OnEquipItem(CharacterItem characterItem, int slotID)
//        {
//            DetermineDominantHand();
//        }

//        /// <summary>
//        /// An item has been unequipped.
//        /// </summary>
//        /// <param name="characterItem">The item that was unequipped.</param>
//        /// <param name="slotID">The slot that the item was unequipped from.</param>
//        private void OnUnequipItem(CharacterItem characterItem, int slotID)
//        {
//            DetermineDominantHand();
//        }

//        /// <summary>
//        /// The Aim ability has started or stopped.
//        /// </summary>
//        /// <param name="start">Has the Aim ability started?</param>
//        /// <param name="inputStart">Was the ability started from input?</param>
//        private void OnAim(bool aim)
//        {
//            m_Aiming = aim;
//        }

//        /// <summary>
//        /// The Use ability has started or stopped using an item.
//        /// </summary>
//        /// <param name="start">Has the Use ability started?</param>
//        /// <param name="useAbility">The Use ability that has started or stopped.</param>
//        private void OnUseStart(bool start, Character.Abilities.Items.Use useAbility)
//        {
//            if (useAbility.SlotID == -1 || useAbility.SlotID == m_DominantSlotID) {
//                m_ItemInUse = start;
//            }
//        }

//        /// <summary>
//        /// Adds a positional and rotational force to the ViewTypes.
//        /// </summary>
//        /// <param name="slotID">The Slot ID that is adding the secondary force.</param>
//        /// <param name="positionalForce">The positional force to add.</param>
//        /// <param name="rotationalForce">The rotational force to add.</param>
//        /// <param name="globalForce">Is the force applied to the entire character?</param>
//        private void OnAddForce(int slotID, Vector3 positionalForce, Vector3 rotationalForce, bool globalForce)
//        {
//            m_PositionSpring.AddForce(positionalForce);
//            m_RotationSpring.AddForce(rotationalForce);
//        }

//        /// <summary>
//        /// Sets the target that the character should look at.
//        /// </summary>
//        /// <param name="active">Should the character look at the target position?</param>
//        /// <param name="position">The position that the character should look at.</param>
//        public override void SetLookAtPosition(bool active, Vector3 position)
//        {
//            if (m_LookAtIK == null) {
//                return;
//            }

//            if (active) {
//                m_LookAtTargetPosition = position;
//            }
//            if (m_ActiveLookAtTarget != active) {
//                if (!string.IsNullOrEmpty(m_ActiveLookAtStateName)) {
//                    Shared.StateSystem.StateManager.SetState(m_GameObject, m_ActiveLookAtStateName, active);
//                }
//                m_ActiveLookAtTarget = active;
//            }
//        }

//        /// <summary>
//        /// Returns the default position that the character should look at.
//        /// </summary>
//        /// <returns>The default position that the character should look at.</returns>
//        public override Vector3 GetDefaultLookAtPosition()
//        {
//            if (m_LookSource == null) {
//                return m_Head.position;
//            }
//            var lookDirection = m_LookSource.LookDirection(m_Head.position, false, 0, true, false);
//            // Multiply the local offset by the distance so the same relative offset will be applied for both the upper body and head.
//            var localOffset = m_LookAtOffset * m_LookSource.LookDirectionDistance;
//            localOffset.z += m_LookSource.LookDirectionDistance;
//            return MathUtility.TransformPoint(m_Head.position, Quaternion.LookRotation(lookDirection), localOffset);
//        }

//        /// <summary>
//        /// Specifies the location of the left or right hand IK target and IK hint target.
//        /// </summary>
//        /// <param name="itemTransform">The transform of the item.</param>
//        /// <param name="itemHand">The hand that the item is parented to.</param>
//        /// <param name="nonDominantHandTarget">The target of the left or right hand. Can be null.</param>
//        /// <param name="nonDominantHandElbowTarget">The target of the left or right elbow. Can be null.</param>
//        public override void SetItemIKTargets(Transform itemTransform, Transform itemHand, Transform nonDominantHandTarget, Transform nonDominantHandElbowTarget)
//        {
//            if (m_AimIK != null) {
//                m_AimIK.solver.transform = itemTransform;
//            }
//            if (m_FullBodyBipedIK != null) {
//                IKEffector effector, elbowEffector;
//                // If the item belongs to the right hand then the left hand is the non-dominant hand.
//                if (m_FullBodyBipedIK.references.rightHand == itemHand) {
//                    effector = m_FullBodyBipedIK.solver.leftHandEffector;
//                    elbowEffector = m_FullBodyBipedIK.solver.leftShoulderEffector;
//                } else {
//                    effector = m_FullBodyBipedIK.solver.rightHandEffector;
//                    elbowEffector = m_FullBodyBipedIK.solver.rightShoulderEffector;
//                }
//                effector.target = nonDominantHandTarget;
//                effector.positionWeight = effector.rotationWeight = effector.target != null ? 1 : 0;
//                elbowEffector.target = nonDominantHandElbowTarget;
//                elbowEffector.positionWeight = elbowEffector.target != null ? 1 : 0;
//            }
//        }

//        /// <summary>
//        /// Specifies the target location of the limb.
//        /// </summary>
//        /// <param name="target">The target location of the limb.</param>
//        /// <param name="ikGoal">The limb affected by the target location.</param>
//        /// <param name="duration">The amount of time it takes to reach the goal.</param>
//        public override void SetAbilityIKTarget(Transform target, IKGoal ikGoal, float duration)
//        {
//            if (m_InteractionSystem == null) {
//                Debug.LogError("Error: The character must have the Interaction System component in order for the ability to set an IK target.");
//                return;
//            }

//            if (target == null) {
//                m_InteractionSystem.StopInteraction(IKGoalToEffector(ikGoal));
//                return;
//            }

//            var interactionObject = target.gameObject.GetCachedComponent<InteractionObject>();
//            if (interactionObject == null) {
//                Debug.LogError("Error: The target " + target.name + " must have the FinalIK Interaction Object attached.");
//                return;
//            }

//            m_InteractionSystem.StartInteraction(IKGoalToEffector(ikGoal), interactionObject, false);
//        }

//        /// <summary>
//        /// Maps the IKGoal enum to the FullBodyBipedEffector enum.
//        /// </summary>
//        /// <param name="ikGoal">The IKGoal enum that should be mapped to a FullBodyBipedEffector.</param>
//        /// <returns>The mapped FullBodyBipedEfffector.</returns>
//        private FullBodyBipedEffector IKGoalToEffector(IKGoal ikGoal)
//        {
//            switch (ikGoal) {
//                case IKGoal.LeftHand:
//                    return FullBodyBipedEffector.LeftHand;
//                case IKGoal.LeftElbow:
//                    return FullBodyBipedEffector.LeftShoulder;
//                case IKGoal.RightHand:
//                    return FullBodyBipedEffector.RightHand;
//                case IKGoal.RightElbow:
//                    return FullBodyBipedEffector.RightShoulder;
//                case IKGoal.LeftFoot:
//                    return FullBodyBipedEffector.LeftFoot;
//                case IKGoal.LeftKnee:
//                    return FullBodyBipedEffector.LeftThigh;
//                case IKGoal.RightFoot:
//                    return FullBodyBipedEffector.RightFoot;
//                case IKGoal.RightKnee:
//                    return FullBodyBipedEffector.RightThigh;
//            }
//            return FullBodyBipedEffector.Body;
//        }

//        /// <summary>
//        /// The Animator is updated within a fixed update. Prevent FinalIK from updating too quickly when the framerate is high.
//        /// </summary>
//        private void FixedUpdate()
//        {
//            m_FixedUpdate = true;
//        }

//        /// <summary>
//        /// Updates the IK.
//        /// </summary>
//        private void LateUpdate()
//        {
//            if (!m_FixedUpdate) {
//                return;
//            }
//            m_FixedUpdate = false;

//            UpdateSolvers(0);
//        }

//        public override void UpdateSolvers(int layer)
//        {
//            if (m_Target != null)
//            {
//                var lookAtPosition = m_ActiveLookAtTarget ? m_LookAtTargetPosition : GetDefaultLookAtPosition();
//                lookAtPosition = m_Head.position + (lookAtPosition - m_Head.position).normalized;
//                m_Target.position = Vector3.Lerp(m_Target.position, lookAtPosition, m_LookAtAdjustmentSpeed);

//                if (m_AimIK != null && m_AimIK.solver.transform != null && (m_Aiming || m_ItemInUse))
//                {
//                    m_AimIK.solver.axis = m_AimIK.solver.transform.InverseTransformVector(m_Transform.rotation * Quaternion.Euler(m_RotationSpring.Value) * m_AnimatedAimDirection);
//                    m_AimIK.solver.Update();
//                }
//                if (m_LookAtIK != null)
//                {
//                    m_LookAtIK.solver.Update();
//                }
//            }

//            if (m_FullBodyBipedIK != null)
//            {
//                m_FullBodyBipedIK.solver.leftHandEffector.positionOffset = m_FullBodyBipedIK.solver.rightHandEffector.positionOffset = m_PositionSpring.Value;

//                // Other objects have the chance of modifying the final position value.
//                if (m_OnUpdateIKPosition != null || m_OnUpdateIKRotation != null)
//                {
//                    UpdateEffectorPositionRotation(IKGoal.LeftHand, m_FullBodyBipedIK.solver.leftHandEffector);
//                    UpdateEffectorPositionRotation(IKGoal.RightHand, m_FullBodyBipedIK.solver.rightHandEffector);
//                    UpdateEffectorPositionRotation(IKGoal.LeftFoot, m_FullBodyBipedIK.solver.leftFootEffector);
//                    UpdateEffectorPositionRotation(IKGoal.RightFoot, m_FullBodyBipedIK.solver.rightFootEffector);
//                }

//                m_FullBodyBipedIK.solver.Update();
//            }
//        }

//        /// <summary>
//        /// Updates the effector position and rotation.
//        /// </summary>
//        /// <param name="ikGoal">The limb affected by the target location.</param>
//        /// <param name="effector">The effector that should be updated</param>
//        private void UpdateEffectorPositionRotation(IKGoal ikGoal, IKEffector effector)
//        {
//            if (m_OnUpdateIKPosition != null) {
//                effector.position = m_OnUpdateIKPosition(ikGoal, effector.position, effector.rotation);
//            }
//            if (m_OnUpdateIKRotation != null) {
//                effector.rotation = m_OnUpdateIKRotation(ikGoal, effector.rotation, effector.position);
//            }
//        }

//        /// <summary>
//        /// The character has died.
//        /// </summary>
//        /// <param name="position">The position of the force.</param>
//        /// <param name="force">The amount of force which killed the character.</param>
//        /// <param name="attacker">The GameObject that killed the character.</param>
//        private void OnDeath(Vector3 position, Vector3 force, GameObject attacker)
//        {
//            enabled = false;
//        }

//        /// <summary>
//        /// The character has respawned.
//        /// </summary>
//        private void OnRespawn()
//        {
//            enabled = true;
//        }

//        /// <summary>
//        /// The character has been destroyed.
//        /// </summary>
//        private void OnDestroy()
//        {
//            EventHandler.UnregisterEvent<ILookSource>(m_GameObject, "OnCharacterAttachLookSource", OnAttachLookSource);
//            EventHandler.UnregisterEvent<bool, Character.Abilities.Items.Use>(m_GameObject, "OnUseAbilityStart", OnUseStart);
//            EventHandler.UnregisterEvent<bool>(m_GameObject, "OnAimAbilityAim", OnAim);
//            EventHandler.UnregisterEvent<CharacterItem, int>(m_GameObject, "OnInventoryEquipItem", OnEquipItem);
//            EventHandler.UnregisterEvent<CharacterItem, int>(m_GameObject, "OnInventoryUnequipItem", OnUnequipItem);
//            EventHandler.UnregisterEvent<int, Vector3, Vector3, bool>(m_GameObject, "OnAddSecondaryForce", OnAddForce);
//            EventHandler.UnregisterEvent<Vector3, Vector3, GameObject>(m_GameObject, "OnDeath", OnDeath);
//            EventHandler.UnregisterEvent(m_GameObject, "OnRespawn", OnRespawn);
//        }
//    }
//}