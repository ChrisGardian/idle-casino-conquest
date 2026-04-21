using System.Collections.Generic;
using UnityEngine;

public enum NodeType { Unlock, Upgrade }

[System.Serializable]
public class UpgradeNodeDefinition
{
    [Header("Identity")]
    public string id;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Type")]
    public NodeType nodeType;
    public int maxLevel;

    [Header("Cost")]
    public float baseCost;
    [Tooltip("Ignored if maxLevel == 1")]
    public float growthFactor = 1.8f;

    [Header("Tree")]
    public string prerequisiteId;
    public Vector2 treePosition;

    [Header("Effects")]
    public List<UpgradeEffect> effects = new();
}
