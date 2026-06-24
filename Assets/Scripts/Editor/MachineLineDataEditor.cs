using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MachineLineData))]
public class MachineLineDataEditor : Editor
{
    private int _numberOfLevels = 10;
    private float _baseCost = 1000f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Auto-Generate", EditorStyles.boldLabel);

        _numberOfLevels = EditorGUILayout.IntField("Number of levels", _numberOfLevels);
        _baseCost = EditorGUILayout.FloatField("Base cost", _baseCost);

        if (GUILayout.Button("Generate lists"))
        {
            MachineLineData lineData = (MachineLineData)target;

            Undo.RecordObject(lineData, "Generate MachineLineData Lists");

            lineData.upgradeCosts = CreateListUpgradeCost(_numberOfLevels, _baseCost);
            lineData.maxMachinesPerLevel = CreateListMaxMachines(_numberOfLevels);
            lineData.revenueMultiplierPerLevel = CreateListRevenueMultiplier(_numberOfLevels);

            EditorUtility.SetDirty(lineData);
            AssetDatabase.SaveAssets();

            Debug.Log($"[MachineLineData] Lists generated for {lineData.lineTypeName} — {_numberOfLevels} levels.");
        }
    }

    private float[] CreateListUpgradeCost(int levels, float baseCost)
    {
        float[] costs = new float[levels];
        for (int i = 0; i < levels; i++)
            costs[i] = baseCost * (i + 1);
        return costs;
    }

    private int[] CreateListMaxMachines(int levels)
    {
        int[] maxMachines = new int[levels];
        for (int i = 0; i < levels; i++)
            maxMachines[i] = i + 1;
        return maxMachines;
    }

    private float[] CreateListRevenueMultiplier(int levels)
    {
        float[] multipliers = new float[levels];
        for (int i = 0; i < levels; i++)
            multipliers[i] = 1f + (i * 0.1f);
        return multipliers;
    }
}