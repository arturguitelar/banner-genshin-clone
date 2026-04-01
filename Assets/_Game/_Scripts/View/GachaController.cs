using UnityEngine;
using System;
using System.Collections.Generic;
using System.Security.Cryptography; // A magia do True Randomness!
using Game.Core;
using Game.Data;

namespace Game.View
{
    public class GachaController : MonoBehaviour
    {
        [Header("Configurações")]
        [SerializeField] private GachaBannerSO currentBanner;
        [SerializeField] private int startingGems = 16000;

        public GachaSystem Logic { get; private set; }
        public WalletSystem Wallet { get; private set; }

        public event Action OnDataUpdated;

        private void Awake()
        {
            Logic = new GachaSystem();
            Wallet = new WalletSystem(startingGems);
        }

        private void Start()
        {
            OnDataUpdated?.Invoke();
        }

        // --- O NOVO MOTOR DE SORTE (CRIPTOGRÁFICO) ---
        // É fisicamente impossível este gerador viciar com resets da Unity.
        private float GetTrueRandom()
        {
            byte[] bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            uint randomInt = BitConverter.ToUInt32(bytes, 0);
            return (float)((double)randomInt / uint.MaxValue); // Retorna sempre entre 0.0 e 1.0
        }

        public bool TryPull(int amount, out List<GachaItemSO> prizes)
        {
            prizes = new List<GachaItemSO>();
            int totalCost = currentBanner.costPerPull * amount;

            if (!Wallet.TrySpend(totalCost))
            {
                Debug.LogWarning("Sem Grana! Vai farmar baú!");
                return false;
            }

            for (int i = 0; i < amount; i++)
            {
                float roll = GetTrueRandom();

                // --- LOG DE AUDITORIA CRÍTICA ---
                // Aqui vamos capturar o estado ANTES do Pull alterar os contadores
                int pityAntes = Logic.PityCounter5;
                // Simulamos a mesma conta que o GachaSystem faz internamente para expor o erro
                float chanceComparada = (pityAntes + 1 >= 74) ? 0.006f + (0.06f * (pityAntes + 1 - 74 + 1)) : 0.006f;
                if (pityAntes + 1 >= 90) chanceComparada = 1.0f;

                GachaRarity rarity = Logic.Pull(roll);
                GachaItemSO item = ResolveItem(rarity);
                prizes.Add(item);

                // Log detalhado para o review de amanhã
                if (rarity == GachaRarity.FiveStar)
                {
                    Debug.LogWarning($"<color=#FFD700>[AUDITORIA 5★]</color> " +
                        $"DADO: {roll:F6} | " +
                        $"CHANCE CALCULADA: {chanceComparada:F6} | " +
                        $"PITY NO MOMENTO: {pityAntes}");
                }

                LogResult(item, rarity, roll); // Passei o roll para o log comum também
            }

            OnDataUpdated?.Invoke();
            return true;
        }

        private void LogResult(GachaItemSO item, GachaRarity rarity, float rollValue)
        {
            string colorHex = "#FFFFFF";
            string stars = "";

            switch (rarity)
            {
                case GachaRarity.FiveStar: colorHex = "#FFD700"; stars = "⭐⭐⭐⭐⭐"; break;
                case GachaRarity.FourStar: colorHex = "#BA55D3"; stars = "⭐⭐⭐⭐"; break;
                case GachaRarity.ThreeStar: colorHex = "#1E90FF"; stars = "⭐⭐⭐"; break;
            }

            string itemName = item != null && !string.IsNullOrEmpty(item.idName) ? item.idName : "Desconhecido";
            // Agora o log mostra o valor do dado para cada item sorteado
            Debug.Log($"<color={colorHex}>[{stars}] {itemName} (Dado: {rollValue:F4})</color>");
        }

        public void AddGems(int amount)
        {
            Wallet.AddGems(amount);
            OnDataUpdated?.Invoke();
        }

        private GachaItemSO ResolveItem(GachaRarity rarity)
        {
            if (rarity == GachaRarity.FiveStar)
            {
                float rateUpRoll = GetTrueRandom();
                bool wonRateUp = Logic.ResolveRateUp5Star(rateUpRoll);
                return wonRateUp ? currentBanner.featuredUnit5Star : currentBanner.GetStandard5Star();
            }

            if (rarity == GachaRarity.FourStar)
            {
                float rateUpRoll = GetTrueRandom();
                bool wonRateUp = Logic.ResolveRateUp4Star(rateUpRoll);
                return wonRateUp ? currentBanner.GetFeatured4Star() : currentBanner.GetStandard4Star();
            }

            return currentBanner.Get3Star();
        }

        [ContextMenu("Auditoria: Rodar 100.000 Tiros")]
        public void RunMassiveTest()
        {
            int total5Stars = 0;
            int totalPulls = 100000;

            // Criamos uma lógica temporária e isolada para não estragar seu save/pity atual
            GachaSystem testLogic = new GachaSystem();

            for (int i = 0; i < totalPulls; i++)
            {
                float roll = GetTrueRandom();
                if (testLogic.Pull(roll) == GachaRarity.FiveStar)
                {
                    total5Stars++;
                }
            }

            double percent = ((double)total5Stars / totalPulls) * 100;
            Debug.Log($"<color=#00FF00>[TESTE DE MASSA]</color> Rodados {totalPulls} tiros.");
            Debug.Log($"5 Estrelas ganhos: {total5Stars}");
            Debug.Log($"Taxa Real: {percent:F4}% (Esperado: ~0.6%)");
        }
    }
}