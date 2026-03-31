using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Game.Core;
using Game.Data;

namespace Game.View
{
    public class GachaController : MonoBehaviour
    {
        [Header("Data")][SerializeField] private GachaBannerSO currentBanner;

        [Header("UI Dependencies")]
        [SerializeField] private GachaResultsView resultsView;
        [SerializeField] private UIDocument bannerUIDocument; [Header("Debug / Wallet")]
        [SerializeField] private int startingGems = 16000;

        private GachaSystem _gachaLogic;
        private WalletSystem _wallet;

        // --- Elementos de UI: Tela Principal ---
        private Label _gemsText;
        private Label _pityValue;
        private Label _guaranteedValue;
        private Button _btnPull1;
        private Button _btnPull10;
        private Button _btnHistory;
        private Button _btnOpenStore;

        // --- Elementos de UI: Modal de Compra ---
        private VisualElement _buyGemsModal;
        private Label _modalValueGems;
        private Button _btnSub1Gem;
        private Button _btnAdd1Gem;
        private Button _btnSub10Gems;
        private Button _btnAdd10Gems;
        private Button _btnCancel;
        private Button _btnApprove;

        // Estado temporário do modal
        private int _currentPurchaseAmount = 160;

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
            UnbindUI();
        }

        private void BindUI()
        {
            if (bannerUIDocument == null) return;
            var root = bannerUIDocument.rootVisualElement;

            // 1. Tela Principal
            _gemsText = root.Q<Label>("gems-text");
            _pityValue = root.Q<Label>("pity-value");
            _guaranteedValue = root.Q<Label>("guaranteed-value");
            _btnPull1 = root.Q<Button>("btn-pull-1");
            _btnPull10 = root.Q<Button>("btn-pull-10");
            _btnHistory = root.Q<Button>("btn-history");
            _btnOpenStore = root.Q<Button>("BtnOpenStore");

            if (_btnPull1 != null) _btnPull1.clicked += OnPullSingle;
            if (_btnPull10 != null) _btnPull10.clicked += OnPullTen;
            if (_btnHistory != null) _btnHistory.clicked += OnOpenHistory;
            if (_btnOpenStore != null) _btnOpenStore.clicked += OpenStoreModal;

            // 2. Modal de Compra
            _buyGemsModal = root.Q<VisualElement>("buy-gems-template");
            _modalValueGems = root.Q<Label>("value-gems");
            _btnSub1Gem = root.Q<Button>("btn-sub-1-gem");
            _btnAdd1Gem = root.Q<Button>("btn-add-1-gem");
            _btnSub10Gems = root.Q<Button>("btn-sub-10-gems");
            _btnAdd10Gems = root.Q<Button>("btn-add-10-gems");
            _btnCancel = root.Q<Button>("btn-cancel");
            _btnApprove = root.Q<Button>("btn-approve");

            if (_btnSub1Gem != null) _btnSub1Gem.clicked += () => ChangePurchaseAmount(-160);
            if (_btnAdd1Gem != null) _btnAdd1Gem.clicked += () => ChangePurchaseAmount(160);
            if (_btnSub10Gems != null) _btnSub10Gems.clicked += () => ChangePurchaseAmount(-1600);
            if (_btnAdd10Gems != null) _btnAdd10Gems.clicked += () => ChangePurchaseAmount(1600);
            if (_btnCancel != null) _btnCancel.clicked += CloseStoreModal;
            if (_btnApprove != null) _btnApprove.clicked += ConfirmPurchase;
        }

        private void UnbindUI()
        {
            // Remover assinaturas para evitar memory leak
            if (_btnPull1 != null) _btnPull1.clicked -= OnPullSingle;
            if (_btnPull10 != null) _btnPull10.clicked -= OnPullTen;
            if (_btnHistory != null) _btnHistory.clicked -= OnOpenHistory;
            if (_btnOpenStore != null) _btnOpenStore.clicked -= OpenStoreModal;

            if (_btnSub1Gem != null) _btnSub1Gem.clicked -= () => ChangePurchaseAmount(-160);
            if (_btnAdd1Gem != null) _btnAdd1Gem.clicked -= () => ChangePurchaseAmount(160);
            if (_btnSub10Gems != null) _btnSub10Gems.clicked -= () => ChangePurchaseAmount(-1600);
            if (_btnAdd10Gems != null) _btnAdd10Gems.clicked -= () => ChangePurchaseAmount(1600);
            if (_btnCancel != null) _btnCancel.clicked -= CloseStoreModal;
            if (_btnApprove != null) _btnApprove.clicked -= ConfirmPurchase;
        }

        // --- LÓGICA DO MODAL ---

        private void OpenStoreModal()
        {
            if (_buyGemsModal == null) return;
            _currentPurchaseAmount = 160; // Reseta sempre pra 1 tiro
            UpdateModalValue();
            _buyGemsModal.style.display = DisplayStyle.Flex; // Mostra o modal!
        }

        private void CloseStoreModal()
        {
            if (_buyGemsModal == null) return;
            _buyGemsModal.style.display = DisplayStyle.None; // Esconde o modal!
        }

        private void ChangePurchaseAmount(int amount)
        {
            _currentPurchaseAmount += amount;

            // Impede de comprar 0 ou valor negativo
            if (_currentPurchaseAmount < 160)
                _currentPurchaseAmount = 160;

            UpdateModalValue();
        }

        private void UpdateModalValue()
        {
            if (_modalValueGems != null)
                _modalValueGems.text = _currentPurchaseAmount.ToString();
        }

        private void ConfirmPurchase()
        {
            _wallet.AddGems(_currentPurchaseAmount); // Método do WalletSystem!
            Debug.Log($"Resgatou {_currentPurchaseAmount} gemas! Saldo novo: {_wallet.CurrentPrimogems}");
            UpdateUI(); // Atualiza o topo da tela principal
            CloseStoreModal();
        }

        // --- LÓGICA DO BANNER ---

        private void OnPullSingle() => PerformPull(1);
        private void OnPullTen() => PerformPull(10);
        private void OnOpenHistory() => Debug.Log("Pop-up de Histórico (Próximas Sprints)");

        private void PerformPull(int amount)
        {
            int totalCost = currentBanner.costPerPull * amount;

            if (!_wallet.TrySpend(totalCost))
            {
                Debug.LogWarning("Sem Grana! Vai farmar baú!");
                OpenStoreModal(); // Opcional de QoL: abre a loja direto se faltar grana!
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

            UpdateUI();
            // resultsView.ShowResults(prizes.ToArray()); // Comentado por enquanto
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
            string colorHex = "#FFFFFF";
            string stars = "";

            switch (rarity)
            {
                case GachaRarity.FiveStar: colorHex = "#FFD700"; stars = "⭐⭐⭐⭐⭐"; break;
                case GachaRarity.FourStar: colorHex = "#BA55D3"; stars = "⭐⭐⭐⭐"; break;
                case GachaRarity.ThreeStar: colorHex = "#1E90FF"; stars = "⭐⭐⭐"; break;
            }

            string itemName = !string.IsNullOrEmpty(item.idName) ? item.idName : item.name;
            Debug.Log($"<color={colorHex}>[{stars}] Ganhou: {itemName}</color>");
        }
    }
}