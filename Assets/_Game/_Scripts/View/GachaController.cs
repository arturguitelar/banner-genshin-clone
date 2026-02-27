using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Game.Core;
using Game.Data;

namespace Game.View
{
    public class GachaController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private GachaBannerSO currentBanner;

        [Header("UI Dependencies")]
        [SerializeField] private GachaResultsView resultsView;

        [Header("Debug / Wallet")]
        [SerializeField] private int startingGems = 16000; // Define aqui quanto quer
        [SerializeField] private int currentGemsDisplay;    // Só olha! (Não edite em Play Mode)

        // Lógica Interna
        private GachaSystem _gachaLogic;
        private WalletSystem _wallet;

        private void Awake()
        {
            _gachaLogic = new GachaSystem();

            // Inicializa a carteira com o valor que você colocou no Inspector
            _wallet = new WalletSystem(startingGems);

            // Sincroniza o display visual
            UpdateDebugDisplay();

            Debug.Log($"Sistema Iniciado. Gemas: {_wallet.CurrentPrimogems}");
        }

        private void Update()
        {
            if (Keyboard.current == null) return;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                PerformPull(1);
            }

            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                PerformPull(10);
            }
        }

        private void PerformPull(int amount)
        {
            int totalCost = currentBanner.costPerPull * amount;

            // 1. Tem dinheiro?
            if (!_wallet.TrySpend(totalCost))
            {
                Debug.LogWarning("Sem Grana! Vai farmar baú!");
                return;
            }

            // Atualiza o display do Inspector para você ver caindo
            UpdateDebugDisplay();

            Debug.Log($"Gastou {totalCost} Gemas. Saldo: {_wallet.CurrentPrimogems}");

            // 2. Lista temporária
            List<GachaItemSO> prizes = new List<GachaItemSO>();

            // 3. Roda a roleta
            for (int i = 0; i < amount; i++)
            {
                GachaRarity rarity = _gachaLogic.Pull(Random.value);
                GachaItemSO item = ResolveItem(rarity);
                prizes.Add(item);
            }

            // 4. Manda a UI desenhar
            resultsView.ShowResults(prizes.ToArray());
        }

        private GachaItemSO ResolveItem(GachaRarity rarity)
        {
            if (rarity == GachaRarity.FiveStar)
            {
                bool wonRateUp = _gachaLogic.ResolveRateUp5Star(Random.value);
                return wonRateUp ? currentBanner.featuredUnit5Star : currentBanner.GetStandard5Star();
            }

            if (rarity == GachaRarity.FourStar)
            {
                bool wonRateUp = (Random.value < 0.5f);
                return wonRateUp ? currentBanner.GetFeatured4Star() : currentBanner.GetStandard4Star();
            }

            return currentBanner.Get3Star();
        }

        // Método auxiliar para atualizar o Inspector
        private void UpdateDebugDisplay()
        {
            currentGemsDisplay = _wallet.CurrentPrimogems;
        }
    }
}