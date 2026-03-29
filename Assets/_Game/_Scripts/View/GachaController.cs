using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Game.Core;
using Game.Data;

namespace Game.View
{
    public class GachaController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private GachaBannerSO currentBanner; [Header("UI Dependencies")]
        [SerializeField] private GachaResultsView resultsView;
        [SerializeField] private UIDocument bannerUIDocument;

        [Header("Debug / Wallet")]
        [SerializeField] private int startingGems = 16000;

        private GachaSystem _gachaLogic;
        private WalletSystem _wallet;

        // --- Elementos de UI Cacheados ---
        private Label _gemsText;
        private Label _pityValue;
        private Label _guaranteedValue;
        private Button _btnPull1;
        private Button _btnPull10;
        private Button _btnHistory;
        private Button _btnOpenStore;

        private void Awake()
        {
            _gachaLogic = new GachaSystem();
            _wallet = new WalletSystem(startingGems);
        }

        private void OnEnable()
        {
            BindUI();
            UpdateUI();
        }

        private void OnDisable()
        {
            // Prevenção contra Memory Leaks do UI Toolkit
            UnbindUI();
        }

        private void BindUI()
        {
            if (bannerUIDocument == null) return;
            var root = bannerUIDocument.rootVisualElement;

            // Buscando EXATAMENTE pelos nomes do seu print!
            _gemsText = root.Q<Label>("gems-text");
            _pityValue = root.Q<Label>("pity-value");
            _guaranteedValue = root.Q<Label>("guaranteed-value");

            _btnPull1 = root.Q<Button>("btn-pull-1");
            _btnPull10 = root.Q<Button>("btn-pull-10");
            _btnHistory = root.Q<Button>("btn-history");
            _btnOpenStore = root.Q<Button>("BtnOpenStore");

            // Assinando eventos de clique
            if (_btnPull1 != null) _btnPull1.clicked += OnPullSingle;
            if (_btnPull10 != null) _btnPull10.clicked += OnPullTen;
            if (_btnHistory != null) _btnHistory.clicked += OnOpenHistory;
            if (_btnOpenStore != null) _btnOpenStore.clicked += OnOpenStore;
        }

        private void UnbindUI()
        {
            if (_btnPull1 != null) _btnPull1.clicked -= OnPullSingle;
            if (_btnPull10 != null) _btnPull10.clicked -= OnPullTen;
            if (_btnHistory != null) _btnHistory.clicked -= OnOpenHistory;
            if (_btnOpenStore != null) _btnOpenStore.clicked -= OnOpenStore;
        }

        // Ações dos botões
        private void OnPullSingle() => PerformPull(1);
        private void OnPullTen() => PerformPull(10);
        private void OnOpenHistory() => Debug.Log("Pop-up de Histórico (Próximas Sprints)");
        private void OnOpenStore() => Debug.Log("Pop-up de Loja (Próximas Sprints)");

        private void PerformPull(int amount)
        {
            int totalCost = currentBanner.costPerPull * amount;

            if (!_wallet.TrySpend(totalCost))
            {
                Debug.LogWarning("Sem Grana! Vai farmar baú!");
                return;
            }

            List<GachaItemSO> prizes = new List<GachaItemSO>();

            for (int i = 0; i < amount; i++)
            {
                GachaRarity rarity = _gachaLogic.Pull(Random.value);
                GachaItemSO item = ResolveItem(rarity);
                prizes.Add(item);
                LogResult(item, rarity);
            }

            // Atualiza os textos da tela com a nova contagem de Pity e Gemas!
            UpdateUI();

            // SPRINT 2 vai entrar aqui no futuro (Animação do Meteoro antes de mostrar resultado)
            // resultsView.ShowResults(prizes.ToArray());
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

        private void UpdateUI()
        {
            if (_gemsText != null)
                _gemsText.text = _wallet.CurrentPrimogems.ToString();

            if (_pityValue != null)
                _pityValue.text = _gachaLogic.PityCounter5.ToString();

            if (_guaranteedValue != null)
                _guaranteedValue.text = _gachaLogic.IsNext5StarGuaranteed ? "SIM" : "NÃO";
        }

        private void LogResult(GachaItemSO item, GachaRarity rarity)
        {
            string colorHex = "#FFFFFF"; // Branco padrão
            string stars = "";

            // Define a cor e as estrelinhas baseadas na raridade
            switch (rarity)
            {
                case GachaRarity.FiveStar:
                    colorHex = "#FFD700"; // Dourado
                    stars = "⭐⭐⭐⭐⭐";
                    break;
                case GachaRarity.FourStar:
                    colorHex = "#BA55D3"; // Roxo
                    stars = "⭐⭐⭐⭐";
                    break;
                case GachaRarity.ThreeStar:
                    colorHex = "#1E90FF"; // Azul
                    stars = "⭐⭐⭐";
                    break;
            }

            // Usa Rich Text da Unity para colorir o Console!
            // Exemplo de saída: [⭐⭐⭐⭐⭐] Ganhou: Venti
            string itemName = !string.IsNullOrEmpty(item.idName) ? item.idName : item.name;
            Debug.Log($"<color={colorHex}>[{stars}] Ganhou: {itemName}</color>");
        }
    }
}