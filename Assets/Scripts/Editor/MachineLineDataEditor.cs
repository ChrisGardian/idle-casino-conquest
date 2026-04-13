using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MachineLineData))]
public class MachineLineDataEditor : Editor
{
    private int _numberOfLevels = 10;
    private float _baseCost = 1000f;

    public override void OnInspectorGUI()
    {
        // Affiche l'Inspector normal du SO
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("── Génération automatique ──", EditorStyles.boldLabel);

        _numberOfLevels = EditorGUILayout.IntField("Nombre de niveaux", _numberOfLevels);
        _baseCost = EditorGUILayout.FloatField("Coût de base", _baseCost);

        if (GUILayout.Button("Générer les listes"))
        {
            MachineLineData lineData = (MachineLineData)target;

            Undo.RecordObject(lineData, "Générer listes MachineLineData");

            lineData.upgradeCosts = CreateListUpgradeCost(_numberOfLevels, _baseCost);
            lineData.maxMachinesPerLevel = CreateListMaxMachines(_numberOfLevels);
            lineData.revenueMultiplierPerLevel = CreateListRevenueMultiplier(_numberOfLevels);

            EditorUtility.SetDirty(lineData);
            AssetDatabase.SaveAssets();

            Debug.Log($"[MachineLineData] Listes générées pour {lineData.lineTypeName} — {_numberOfLevels} niveaux.");
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
