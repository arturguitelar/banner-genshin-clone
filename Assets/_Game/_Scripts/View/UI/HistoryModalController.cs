using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Game.Core;

namespace Game.View.UI
{
    public class HistoryModalController
    {
        private GachaController _gachaController;
        private VisualElement _modalOverlay;
        private Button _btnClose;
        private Button _btnOpenHistory;

        // Paginação
        private Button _btnPrev;
        private Button _btnNext;
        private Label _pageNumberText;
        private int _currentPage = 0;
        private const int ITEMS_PER_PAGE = 5;

        // As nossas 5 linhas fixas
        private VisualElement[] _rows = new VisualElement[5];

        public HistoryModalController(VisualElement root, GachaController gachaController)
        {
            _gachaController = gachaController;

            _modalOverlay = root.Q<VisualElement>("hystory-template"); // Mude se o seu ID for diferente
            _btnClose = root.Q<Button>("btn-close-modal");
            _btnOpenHistory = root.Q<Button>("btn-history"); // Aquele botão lá da tela do Banner

            _btnPrev = root.Q<Button>("btn-prev");
            _btnNext = root.Q<Button>("btn-next");
            _pageNumberText = root.Q<Label>("page-number");

            // Coleta as 5 linhas que você duplicou
            for (int i = 0; i < 5; i++)
            {
                _rows[i] = root.Q<VisualElement>($"Data-Row-{i}");
            }

            // Eventos
            if (_btnOpenHistory != null) _btnOpenHistory.clicked += OpenModal;
            if (_btnClose != null) _btnClose.clicked += CloseModal;
            if (_btnPrev != null) _btnPrev.clicked += PreviousPage;
            if (_btnNext != null) _btnNext.clicked += NextPage;

            // Esconde por padrão
            if (_modalOverlay != null) _modalOverlay.style.display = DisplayStyle.None;
        }

        private void OpenModal()
        {
            _currentPage = 0;
            UpdateTableView();
            if (_modalOverlay != null) _modalOverlay.style.display = DisplayStyle.Flex;
        }

        private void CloseModal()
        {
            if (_modalOverlay != null) _modalOverlay.style.display = DisplayStyle.None;
        }

        private void PreviousPage()
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                UpdateTableView();
            }
        }

        private void NextPage()
        {
            int maxPage = Mathf.Max(0, (_gachaController.PullHistory.Count - 1) / ITEMS_PER_PAGE);
            if (_currentPage < maxPage)
            {
                _currentPage++;
                UpdateTableView();
            }
        }

        private void UpdateTableView()
        {
            // O histórico mais recente tem que aparecer primeiro! Então invertemos a lista.
            List<PullRecord> history = new List<PullRecord>(_gachaController.PullHistory);
            history.Reverse();

            int maxPage = Mathf.Max(1, (history.Count + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE);
            if (_pageNumberText != null) _pageNumberText.text = $"{_currentPage + 1} / {maxPage}";

            int startIndex = _currentPage * ITEMS_PER_PAGE;

            for (int i = 0; i < 5; i++)
            {
                int itemIndex = startIndex + i;

                if (itemIndex < history.Count)
                {
                    // Tem item para mostrar nessa linha!
                    _rows[i].style.display = DisplayStyle.Flex;
                    PullRecord record = history[itemIndex];

                    var typeLabel = _rows[i].Q<Label>("col-type");
                    var nameLabel = _rows[i].Q<Label>("col-name");
                    var pity5Label = _rows[i].Q<Label>("col-pity5");
                    var pity4Label = _rows[i].Q<Label>("col-pity4");

                    if (typeLabel != null) typeLabel.text = record.ItemType;
                    if (nameLabel != null)
                    {
                        nameLabel.text = record.ItemName;
                        // Aplica a cor do Genshin no texto do nome!
                        nameLabel.style.color = GetRarityColor(record.Rarity);
                    }
                    if (pity5Label != null) pity5Label.text = record.Pity5.ToString();
                    if (pity4Label != null) pity4Label.text = record.Pity4.ToString();
                }
                else
                {
                    // Não tem item, esconde a linha pra ficar bonito
                    if (_rows[i] != null) _rows[i].style.display = DisplayStyle.None;
                }
            }
        }

        private StyleColor GetRarityColor(GachaRarity rarity)
        {
            switch (rarity)
            {
                case GachaRarity.FiveStar: return new StyleColor(new Color32(0xBD, 0x69, 0x32, 255)); // Dourado (#BD6932)
                case GachaRarity.FourStar: return new StyleColor(new Color(0.73f, 0.33f, 0.83f)); // Roxo
                default: return new StyleColor(new Color(0.4f, 0.4f, 0.4f)); // Cinza/Azul escuro
            }
        }
    }
}