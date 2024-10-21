using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISystem.Common.Performance
{
    public class RendererController
    {
        [SerializeField] bool isRendered = true;
        [SerializeField] MeshRenderer[] renderComponent;
        [SerializeField] SkinnedMeshRenderer[] renderComponentSkins;

        public RendererController(List<MeshRenderer> meshRenderers, List<SkinnedMeshRenderer> renderSkins)
        {
            renderComponent = meshRenderers.ToArray();
            renderComponentSkins = renderSkins.ToArray();
        }

        public RendererController(GameObject candidate)
        {
            renderComponent = new List<MeshRenderer>(candidate.GetComponentsInChildren<MeshRenderer>()).ToArray();
            renderComponentSkins = new List<SkinnedMeshRenderer>(candidate.GetComponentsInChildren<SkinnedMeshRenderer>()).ToArray();
        }

        public bool IsRendered()
        {
            return isRendered;
        }

        public void SetRendered(bool value)
        {
            if (value != isRendered)
            {
                foreach (MeshRenderer renderer in renderComponent)
                {
                    if (renderer != null)
                    {
                        renderer.enabled = value;
                    }
                }

                foreach (SkinnedMeshRenderer renderer in renderComponentSkins)
                {
                    if (renderer != null)
                    {
                        renderer.enabled = value;
                    }
                }

                isRendered = value;
            }
        }
    }
}