using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeTreeData", menuName = "Casino/Upgrade Tree Data")]
public class UpgradeTreeData : ScriptableObject
{
    [Header("Economy")]
    public float globalCostMultiplier = 1f;

    [Header("Nodes")]
    public string rootNodeId = "open_casino";
    public List<UpgradeNodeDefinition> nodes = new();

    public UpgradeNodeDefinition GetNode(string id) =>
        nodes.Find(n => n.id == id);

    public List<UpgradeNodeDefinition> GetChildren(string parentId) =>
        nodes.FindAll(n => n.prerequisiteId == parentId);
}
