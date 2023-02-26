using UnityEngine;



public static class LayerMaskExtensions
{
    public static bool HasLayer(this LayerMask mask, int layer)
    {
        return (mask & 1 << layer) == 1 << layer;
    }
    
    public static bool HasLayer(this LayerMask mask, string layer)
    {
        return mask.HasLayer(LayerMask.NameToLayer(layer));
    }
}