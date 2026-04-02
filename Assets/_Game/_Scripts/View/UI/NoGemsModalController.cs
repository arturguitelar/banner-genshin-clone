using UnityEngine.UIElements;

namespace Game.View.UI
{
    public class NoGemsModalController
    {
        private VisualElement _modalOverlay;
        private Button _btnOk;

        public NoGemsModalController(VisualElement root)
        {
            // Pescamos o template usando o ID exato que você criou
            _modalOverlay = root.Q<VisualElement>("no-gems-template");

            if (_modalOverlay != null)
            {
                // Buscamos o botão DENTRO do modal para evitar conflito com outros modais
                _btnOk = _modalOverlay.Q<Button>("btn-approve");

                if (_btnOk != null)
                    _btnOk.clicked += HideModal;

                HideModal(); // Começa o jogo invisível
            }
        }

        public void ShowModal()
        {
            if (_modalOverlay != null)
                _modalOverlay.style.display = DisplayStyle.Flex;
        }

        private void HideModal()
        {
            if (_modalOverlay != null)
                _modalOverlay.style.display = DisplayStyle.None;
        }

        public void Dispose()
        {
            // Limpeza de memória
            if (_btnOk != null)
                _btnOk.clicked -= HideModal;
        }
    }
}