using UnityEngine;
using UnityEngine.UIElements;

namespace Game.View.UI
{
    public class UI_BannerController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private GachaController _gachaController;

        // Nossos sub-controladores levinhos
        private BannerScreenController _bannerScreen;
        private BuyGemsModalController _buyGemsModal;
        private HistoryModalController _historyModal;
        private NoGemsModalController _noGemsModal;

        private void OnEnable()
        {
            // Proteção de null reference ao recompilar a Unity
            if (_uiDocument == null || _gachaController == null) return;

            VisualElement root = _uiDocument.rootVisualElement;

            // Instancia os operários e passa as ferramentas pra eles
            _bannerScreen = new BannerScreenController(root, _gachaController);
            _buyGemsModal = new BuyGemsModalController(root, _gachaController);
            _historyModal = new HistoryModalController(root, _gachaController);

            _noGemsModal = new NoGemsModalController(root);
            _gachaController.OnInsufficientGems += _noGemsModal.ShowModal;
        }

        private void OnDisable()
        {
            // Limpa a sujeira pra não dar memory leak
            _bannerScreen?.Dispose();

            // Desassina o evento e limpa a memória
            if (_noGemsModal != null)
            {
                _noGemsModal.Dispose();
                if (_gachaController != null)
                    _gachaController.OnInsufficientGems -= _noGemsModal.ShowModal;
            }
        }
    }
}