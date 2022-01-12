using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_SpriteManager : MonoBehaviour
{
    //reference to the main player script
    [SerializeField] internal Player playerScript;

    [SerializeField] internal SpriteRenderer[] leftTopRenderers;
    [SerializeField] internal SpriteRenderer[] rightTopRenderers;
    [SerializeField] internal SpriteRenderer[] leftBottomRenderers;
    [SerializeField] internal SpriteRenderer[] rightBottomRenderers;

    internal SpriteRenderer[][] spriteRenderers;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderers = new SpriteRenderer[][] { leftTopRenderers, leftBottomRenderers, rightBottomRenderers, rightTopRenderers };

        foreach (Renderer[] rendererArray in spriteRenderers)
        {
            foreach (Renderer renderer in rendererArray)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;
            }
        }
    }

    public void SetSprites(Renderer[] rendererArray, bool isActive)
    {
        foreach (Renderer renderer in rendererArray)
        {
            renderer.enabled = isActive;
        }
    }

    public int[] GetActiveRendererArrays()
    {
        List<int> activeArrays = new List<int>();

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            foreach (Renderer renderer in spriteRenderers[i])
            {
                if (renderer.enabled)
                {
                    activeArrays.Add(i);
                    break;
                }
            }
        }

        return activeArrays.ToArray();
    }
}
