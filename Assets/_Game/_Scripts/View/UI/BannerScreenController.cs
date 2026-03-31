using UnityEngine.UIElements;

namespace Game.View.UI
{
    public class BannerScreenController
    {
        private GachaController _gachaController;

        private Label _gemsText;
        private Label _pityValue;
        private Label _guaranteedValue;
        private Button _btnPull1;
        private Button _btnPull10;

        public BannerScreenController(VisualElement root, GachaController gachaManager)
        {
            _gachaController = gachaManager;

            // Pescando elementos
            _gemsText = root.Q<Label>("gems-text");
            _pityValue = root.Q<Label>("pity-value");
            _guaranteedValue = root.Q<Label>("guaranteed-value");
            _btnPull1 = root.Q<Button>("btn-pull-1");
            _btnPull10 = root.Q<Button>("btn-pull-10");

            // Assinando eventos dos botões
            if (_btnPull1 != null) _btnPull1.clicked += () => PerformPull(1);
            if (_btnPull10 != null) _btnPull10.clicked += () => PerformPull(10);

            // Assinando a fofoca do Manager (Quando os dados mudarem, atualizamos o texto)
            _gachaController.OnDataUpdated += UpdateUI;
        }

        public void Dispose()
        {
            // Limpeza de memória
            _gachaController.OnDataUpdated -= UpdateUI;
            // Dica: No UI Toolkit, classes C# puras perdem a referência ao serem destruídas pelo pai, 
            // mas é sempre elegante limpar os eventos do Manager.
        }

        private void PerformPull(int amount)
        {
            if (_gachaController.TryPull(amount, out var prizes))
            {
                // SPRINT 2: AQUI VAMOS CHAMAR O METEORO DEPOIS!
            }
        }

        private void UpdateUI()
        {
            if (_gemsText != null) _gemsText.text = _gachaController.Wallet.CurrentPrimogems.ToString();
            if (_pityValue != null) _pityValue.text = _gachaController.Logic.PityCounter5.ToString();
            if (_guaranteedValue != null) _guaranteedValue.text = _gachaController.Logic.IsNext5StarGuaranteed ? "SIM" : "NÃO";
        }
    }
}