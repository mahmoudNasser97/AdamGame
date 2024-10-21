using UnityEngine;
using AISystem.Common;
using AISystem.Common.Performance;

namespace AISystem.ItemSystem
{
    public class Item : MonoBehaviour
    {
        System.Guid id;
        public ITEMS itemName = ITEMS.NULL;
        public ITEMS_TYPE type = ITEMS_TYPE.NULL;
        public int quanity;
        [SerializeField] bool inUse = false;
        [SerializeField] Interactor interaction = null;

        RendererController rendererController;

        public void Start()
        {
            interaction = GetComponent<Interactor>();
            id = System.Guid.NewGuid();
            rendererController = new RendererController(gameObject);

            AIOrchestrator.GetInstance().AddItem(this);
        }

        public bool IsInUse()
        {
            return inUse;
        }

        public void SetInUse(bool update)
        {
            if(interaction != null)
            {
                interaction.Apply(update);
            }

            inUse = update;
        }

        public System.Guid GetId()
        {
            return id;
        }

        public void UpdateRenderState(bool value)
        {
            rendererController.SetRendered(value);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (rendererController != null)
            {
                if (!rendererController.IsRendered())
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(transform.position, 0.3f);
                }
            }
        }
#endif
    }
}
