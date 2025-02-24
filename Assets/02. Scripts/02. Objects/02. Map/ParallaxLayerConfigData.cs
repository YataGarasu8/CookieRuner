using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParallaxLayerConfigData", menuName = "Parallax/LayerConfigData", order = 0)]
public class ParallaxLayerConfigData : ScriptableObject
{
    public ParallaxLayerConfig backgroundLayer;
    public ParallaxLayerConfig midgroundLayer;
    public ParallaxLayerConfig foregroundLayer;
}
