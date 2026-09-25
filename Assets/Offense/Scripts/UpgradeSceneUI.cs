using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UpgradeSceneUI : MonoBehaviour
{
    [SerializeField] private OffenseData data;
    [SerializeField] private string battleSceneName = "DefenseSandbox";

    [Header("UI Text Displays")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text healthBtnText;
    [SerializeField] private TMP_Text damageBtnText;
    [SerializeField] private TMP_Text spawnCountBtnText;

    private void Start()
    {
        RefreshUI();
    }

    public void OnBuyHealth()
    {
        if (data != null && data.BuyHealthUpgrade()) RefreshUI();
    }

    public void OnBuyDamage()
    {
        if (data != null && data.BuyDamageUpgrade()) RefreshUI();
    }

    public void OnBuySpawnCount()
    {
        if (data != null && data.BuySpawnCountUpgrade()) RefreshUI();
    }

    public void OnStartNextRun()
    {
        SceneManager.LoadScene(battleSceneName);
    }

    private void RefreshUI()
    {
        if (data == null) return;

        if (goldText) goldText.text = $"Gold: {FormatNumber(data.gold)}";
        if (healthBtnText) healthBtnText.text = $"+HP (Lv.{data.healthLevel})\nCost: {FormatNumber(data.GetHealthCost())}";
        if (damageBtnText) damageBtnText.text = $"+DMG (Lv.{data.damageLevel})\nCost: {FormatNumber(data.GetDamageCost())}";
        if (spawnCountBtnText) spawnCountBtnText.text = $"+1 Unit (Cap: {data.TotalSpawnCount})\nCost: {FormatNumber(data.GetSpawnCountCost())}";
    }

    private string FormatNumber(double num)
    {
        if (num >= 1000000) return (num / 1000000D).ToString("0.##") + "M";
        if (num >= 1000) return (num / 1000D).ToString("0.##") + "K";
        return num.ToString("0");
    }
}