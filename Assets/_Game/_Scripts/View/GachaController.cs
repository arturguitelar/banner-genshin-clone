using UnityEngine;
using System;
using System.Collections.Generic;
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

        public bool TryPull(int amount, out List<GachaItemSO> prizes)
        {
            prizes = new List<GachaItemSO>();
            int totalCost = currentBanner.costPerPull * amount;

            if (!Wallet.TrySpend(totalCost)) return false;

            for (int i = 0; i < amount; i++)
            {
                GachaRarity rarity = Logic.Pull(UnityEngine.Random.value);
                GachaItemSO item = ResolveItem(rarity);
                prizes.Add(item);

                // Nossos logs coloridos voltaram! ✨
                LogResult(item, rarity);
            }

            OnDataUpdated?.Invoke();
            return true;
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
                bool wonRateUp = Logic.ResolveRateUp5Star(UnityEngine.Random.value);
                return wonRateUp ? currentBanner.featuredUnit5Star : currentBanner.GetStandard5Star();
            }

            if (rarity == GachaRarity.FourStar)
            {
                // CORREÇÃO: Respeitando o 50/50 do 4 estrelas do seu GachaSystem!
                bool wonRateUp = ResolveRateUp4Star(UnityEngine.Random.value);
                return wonRateUp ? currentBanner.GetFeatured4Star() : currentBanner.GetStandard4Star();
            }

            return currentBanner.Get3Star();
        }

        // Como o ResolveRateUp4Star não existia no GachaSystem, implementei a ponte dele aqui
        private bool ResolveRateUp4Star(float diceRoll)
        {
            if (Logic.IsNext4StarGuaranteed)
            {
                Logic.SetNext4StarGuaranteed(false);
                return true;
            }

            if (diceRoll < 0.5f)
            {
                Logic.SetNext4StarGuaranteed(false);
                return true;
            }

            Logic.SetNext4StarGuaranteed(true);
            return false;
        }

        private void LogResult(GachaItemSO item, GachaRarity rarity)
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
            Debug.Log($"<color={colorHex}>[{stars}] Ganhou: {itemName}</color>");
        }
    }
}