using UnityEngine;
using UnityEngine.InputSystem;
using Game.Core; // Nossa matemática
using Game.Data; // Nossos dados

namespace Game.View
{
    public class GachaController : MonoBehaviour
    {
        [SerializeField] private GachaBannerSO currentBanner;

        private GachaSystem _gachaLogic;
        private WalletSystem _wallet; // <--- Nossa carteira

        public int primogemsAmount = 16000;

        private void Awake()
        {
            _gachaLogic = new GachaSystem();

            // Para teste, vamos dar 16000 gemas para o jogador (Rico!)
            // Num jogo real, isso viria de um SaveSystem
            _wallet = new WalletSystem(primogemsAmount);

            Debug.Log($"Carteira Inicializada: {_wallet.CurrentPrimogems} Gemas");
        }

        private void Update()
        {
            if (Keyboard.current == null) return;

            // Espaço = 1 Tiro
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                PullOne();
            }

            // Enter = 10 Tiros
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                PullTen();
            }
        }

        [ContextMenu("Pull 1")]
        public void PullOne()
        {
            // 1. VERIFICAÇÃO DE FUNDOS (A novidade!)
            int cost = currentBanner.costPerPull;

            if (!_wallet.TrySpend(cost))
            {
                Debug.LogWarning($"<color=red>SEM GEMAS! Você tem {_wallet.CurrentPrimogems}, precisa de {cost}.</color>");
                return; // Cancela o tiro
            }

            // Se passou, continua o fluxo normal...
            Debug.Log($"Gasto: {cost} Gemas. Restam: {_wallet.CurrentPrimogems}");

            // 2. Lógica Gacha (igual antes)
            GachaRarity rarity = _gachaLogic.Pull(Random.value);
            GachaItemSO prize = ResolveItem(rarity);
            LogResult(prize, rarity);
        }

        [ContextMenu("Pull 10")]
        public void PullTen()
        {
            int totalCost = currentBanner.costPerPull * 10;

            // Verificamos se tem grana para os 10 DE UMA VEZ
            if (!_wallet.TrySpend(totalCost))
            {
                Debug.LogWarning($"<color=red>SEM GEMAS PARA 10x! Você tem {_wallet.CurrentPrimogems}, precisa de {totalCost}.</color>");
                return;
            }

            Debug.Log($"--- MULTI-ROLL (Custo: {totalCost}) ---");
            // Se pagou, roda o loop
            for (int i = 0; i < 10; i++)
            {
                // ATENÇÃO: Aqui chamamos a lógica interna direto, 
                // não o PullOne(), senão ele cobraria de novo!
                GachaRarity rarity = _gachaLogic.Pull(Random.value);
                GachaItemSO prize = ResolveItem(rarity);
                LogResult(prize, rarity);
            }
        }

        // --- RESOLUÇÃO DO ITEM (Lógica do 50/50) ---
        private GachaItemSO ResolveItem(GachaRarity rarity)
        {
            if (rarity == GachaRarity.FiveStar)
            {
                // Core decide a sorte
                bool wonRateUp = _gachaLogic.ResolveRateUp5Star(Random.value);

                if (wonRateUp)
                {
                    Debug.Log($"<color=green>VENCEU 50/50! Veio {currentBanner.featuredUnit5Star.displayName}</color>");
                    return currentBanner.featuredUnit5Star;
                }
                else
                {
                    Debug.Log("<color=red>PERDEU 50/50... Buscando no Standard Pool.</color>");
                    return currentBanner.GetStandard5Star(); // <--- Agora pegamos do Pool via Banner
                }
            }

            if (rarity == GachaRarity.FourStar)
            {
                // Regra simplificada do 4* (50/50 também)
                // Você pode implementar a logica "ResolveRateUp4Star" no Core depois se quiser ser perfeccionista
                bool wonRateUp4 = (Random.value < 0.5f);

                // Mas precisamos checar se tem garantia de 4* também
                // (Deixo isso como desafio ou implementamos depois, senão o código fica gigante agora)

                if (wonRateUp4)
                {
                    return currentBanner.GetFeatured4Star();
                }
                else
                {
                    return currentBanner.GetStandard4Star();
                }
            }

            // 3 Estrelas
            return currentBanner.Get3Star();
        }

        private void LogResult(GachaItemSO item, GachaRarity rarity)
        {
            string colorHex = "#FFFFFF";
            switch (rarity)
            {
                case GachaRarity.FiveStar: colorHex = "#FFD700"; break; // Dourado
                case GachaRarity.FourStar: colorHex = "#DA70D6"; break; // Roxo
                case GachaRarity.ThreeStar: colorHex = "#ADD8E6"; break; // Azul
            }

            // Agora mostramos Pity5 e Pity4
            Debug.Log($"<color={colorHex}><b>[{rarity}]</b> {item.displayName} " +
                      $"(Pity 5★: {_gachaLogic.PityCounter5} | Pity 4★: {_gachaLogic.PityCounter4})</color>");
        }
    }
}