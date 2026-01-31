using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIImageAlphaRaycast : MonoBehaviour
{
    [Range(0f, 1f)]
    public float alphaThreshold = 0.5f;

    void Awake()
    {
        Image img = GetComponent<Image>();
        img.alphaHitTestMinimumThreshold = alphaThreshold;
    }
}
