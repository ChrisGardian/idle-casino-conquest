using System.IO;
using UnityEngine;

public static class CasinoLogger
{
    public static bool Enabled = true;

    private static string _logPath;
    private static StreamWriter _writer;
    private static int _totalTransactions;
    private static float _totalCasinoNet;

    private static StreamWriter Writer
    {
        get
        {
            if (_writer == null)
                Initialize();
            return _writer;
        }
    }

    private static void Initialize()
    {
        _logPath = Path.Combine(Application.persistentDataPath, $"casino_log_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
        _writer = new StreamWriter(_logPath, append: false) { AutoFlush = true };
        _writer.WriteLine($"=== Session Casino - {System.DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
        _writer.WriteLine($"Log path: {_logPath}");
        _writer.WriteLine(new string('-', 80));
        Debug.Log($"[CasinoLogger] Logs écrits dans : {_logPath}");
    }

    public static void LogTransaction(string npcName, string machineName, float bet, bool win, float payout, float casinoNet)
    {
        if (!Enabled) return;

        _totalTransactions++;
        _totalCasinoNet += casinoNet;

        string outcome = win ? $"CASINO PERD  {payout:F1}$" : $"CASINO GAGNE {bet:F1}$";
        string line = $"[{System.DateTime.Now:HH:mm:ss}] TRANSACTION | {npcName,-20} | {machineName,-20} | Mise: {bet,6:F1}$ | {outcome} | Net: {casinoNet,+7:F1}$ | Cumul: {_totalCasinoNet,+9:F1}$";

        string coloredOutcome = win
            ? $"<color=red>CASINO PERD  {payout:F1}$</color>"
            : $"<color=green>CASINO GAGNE {bet:F1}$</color>";
        Debug.Log($"[CASINO] {npcName} | {machineName} | Mise: {bet:F1}$ | {coloredOutcome} | Cumul: {_totalCasinoNet:+0.0;-0.0}$");

        Writer.WriteLine(line);
    }

    public static void LogSessionSummary(string npcName, string machineName, int plays, int wins, float sessionNetCasino)
    {
        if (!Enabled) return;

        float winRate = plays > 0 ? (float)wins / plays * 100f : 0f;
        string line = $"[{System.DateTime.Now:HH:mm:ss}] SESSION     | {npcName,-20} quitte {machineName,-20} | {plays} mises, {wins} victoires ({winRate:F0}%) | Net session: {sessionNetCasino:+0.0;-0.0}$";

        Debug.Log($"[SESSION] {npcName} quitte {machineName} | {plays} mises, {wins} victoires ({winRate:F0}%) | Net: {sessionNetCasino:+0.0;-0.0}$");
        Writer.WriteLine(line);
        Writer.WriteLine(new string('-', 80));
    }

    public static void Close()
    {
        if (_writer == null) return;
        _writer.WriteLine(new string('=', 80));
        _writer.WriteLine($"TOTAL : {_totalTransactions} transactions | Net casino final : {_totalCasinoNet:+0.0;-0.0}$");
        _writer.Close();
        _writer = null;
    }
}
