using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Event Banner", menuName = "Genshin/Banner Config")]
    public class GachaBannerSO : ScriptableObject
    {
        public string bannerName;
        public int costPerPull = 160; // <--- Item 1 resolvido!

        [Header("Featured (Rate-Up)")]
        public GachaItemSO featuredUnit5Star;       // Ex: Nahida
        public List<GachaItemSO> featuredUnits4Star; // Ex: Kuki, Dori, Layla

        [Header("Base Pool Reference")]
        public GachaPoolSO standardPool; // <--- A Mágica do Item 3!

        // --- Helpers ---
        // Se perder o 50/50, pedimos ajuda ao Pool
        public GachaItemSO GetStandard5Star() => standardPool.GetRandomStandard5Star();

        // Verifica se um item 4* faz parte do rate-up
        public bool IsFeatured4Star(GachaItemSO item) => featuredUnits4Star.Contains(item);

        // Pega um dos destaques 4*
        public GachaItemSO GetFeatured4Star()
        {
            if (featuredUnits4Star.Count == 0) return null;
            return featuredUnits4Star[Random.Range(0, featuredUnits4Star.Count)];
        }

        // Se perder o 50/50 de 4*
        public GachaItemSO GetStandard4Star() => standardPool.GetRandomStandard4Star();

        // Lixo
        public GachaItemSO Get3Star() => standardPool.GetRandom3Star();
    }
}