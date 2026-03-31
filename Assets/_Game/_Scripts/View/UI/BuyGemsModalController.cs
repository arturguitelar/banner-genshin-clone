using UnityEngine.UIElements;

namespace Game.View.UI
{
    public class BuyGemsModalController
    {
        private GachaController _gachaController;

        private VisualElement _modalOverlay;
        private Label _modalValueGems;
        private Button _btnOpenStore;
        private Button _btnSub1Gem, _btnAdd1Gem, _btnSub10Gems, _btnAdd10Gems;
        private Button _btnCancel, _btnApprove;

        private int _currentAmount = 160;

        public BuyGemsModalController(VisualElement root, GachaController GachaController)
        {
            _gachaController = GachaController;

            _btnOpenStore = root.Q<Button>("BtnOpenStore");
            _modalOverlay = root.Q<VisualElement>("buy-gems-template");
            _modalValueGems = root.Q<Label>("value-gems");

            _btnSub1Gem = root.Q<Button>("btn-sub-1-gem");
            _btnAdd1Gem = root.Q<Button>("btn-add-1-gem");
            _btnSub10Gems = root.Q<Button>("btn-sub-10-gems");
            _btnAdd10Gems = root.Q<Button>("btn-add-10-gems");
            _btnCancel = root.Q<Button>("btn-cancel");
            _btnApprove = root.Q<Button>("btn-approve");

            // Eventos
            if (_btnOpenStore != null) _btnOpenStore.clicked += OpenModal;
            if (_btnCancel != null) _btnCancel.clicked += CloseModal;
            if (_btnApprove != null) _btnApprove.clicked += ConfirmPurchase;

            if (_btnSub1Gem != null) _btnSub1Gem.clicked += () => ChangeAmount(-160);
            if (_btnAdd1Gem != null) _btnAdd1Gem.clicked += () => ChangeAmount(160);
            if (_btnSub10Gems != null) _btnSub10Gems.clicked += () => ChangeAmount(-1600);
            if (_btnAdd10Gems != null) _btnAdd10Gems.clicked += () => ChangeAmount(1600);
        }

        private void ChangeAmount(int amount)
        {
            _currentAmount += amount;
            if (_currentAmount < 160) _currentAmount = 160;
            if (_modalValueGems != null) _modalValueGems.text = _currentAmount.ToString();
        }

        private void OpenModal()
        {
            _currentAmount = 160;
            ChangeAmount(0);
            if (_modalOverlay != null) _modalOverlay.style.display = DisplayStyle.Flex;
        }

        private void CloseModal()
        {
            if (_modalOverlay != null) _modalOverlay.style.display = DisplayStyle.None;
        }

        private void ConfirmPurchase()
        {
            _gachaController.AddGems(_currentAmount);
            CloseModal();
        }
    }
}