/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateCharacterController.Objects
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Represents a unique identifier for the object that this component is attached to, used by the Detect Object Ability Base ability.
    /// </summary>
    public class ObjectIdentifier : MonoBehaviour
    {
        [Tooltip("The value of the identifier.")]
        [SerializeField] protected uint m_ID;
        [SerializeField] GameObject myObject_item;
        [SerializeField] GameObject myObject;
        [SerializeField] float waitTimeForItem;
        public uint ID { get { return m_ID; } set { m_ID = value; } }
        private void OnTriggerEnter(Collider other)
        {
            myObject.SetActive(true);
            myObject_item.SetActive(true);
            StartCoroutine(BuildItem());
        }
        private void OnTriggerExit(Collider other)
        {
            myObject.SetActive(false);
            myObject_item.SetActive(false);
        }
        IEnumerator BuildItem()
        {
            yield return new WaitForSeconds(waitTimeForItem);
            this.GetComponent<BoxCollider>().isTrigger = false;
            yield return new WaitForSeconds(0.5f);
            Destroy(this.gameObject);
        }

    }
}