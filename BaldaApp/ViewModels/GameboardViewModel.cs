using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BaldaApp.ViewModels {
    public partial class GameboardViewModel : ObservableObject {

        private readonly CellMap cellMap;
        private readonly Stack<Cell> selectedCells;
        private readonly FastSearch fastSearch;

        [ObservableProperty]
        private Player mainPlayer;
        [ObservableProperty]
        private BotPlayer bot;

        public event Action? AlphabetRequested;
        public event Action<string?>? WordMeaningRequested;

        public IEnumerable<Cell> Cells => cellMap.Cells;

        public GameboardViewModel(WordItem startWordItem, FastSearch prefixTrie) {

            ArgumentException.ThrowIfNullOrEmpty(startWordItem.Value);
            cellMap = new CellMap(startWordItem);
            fastSearch = prefixTrie;
            MainPlayer = new Player();
            selectedCells = new Stack<Cell>();
            Bot = new BotPlayer(prefixTrie, cellMap);
            cellMap.RefreshFillableCells();
        }

        private string GetWord(char newLetter) =>
            selectedCells.Aggregate(string.Empty, (word, cell) => (cell.Letter ?? newLetter) + word);

        public async Task AddNewLetter(char? symbol) {

            if (!symbol.HasValue) {
                ClearSelectedCells();
                return;
            }

            string newWord = GetWord(symbol.Value);
            WordItem? wordItem = await fastSearch.FindWordItem(newWord);

            if (wordItem == null)
                MessageBox.Show("Такого слова нет");

            else if (!MainPlayer.TryAddNewWord(wordItem))
                MessageBox.Show("Это слово уже было найдено");

            else {
                Database.FindMeaningAsync(wordItem);
                Cell fillableCell = selectedCells.Single(Cell.IsFillable);
                fillableCell.Letter = symbol;
                fillableCell.Status = CellStatus.Filled;
                cellMap.RefreshFillableCells();
                Bot.Run(MainPlayer.FoundWords);
            }

            ClearSelectedCells();
        }

        private bool IsCorrectSequence() =>
            selectedCells.Count > 1
            && !selectedCells.Any(Cell.IsEmpty)
            && selectedCells.Count(Cell.IsFillable) == 1;

        private void Highlight() {
            bool result = IsCorrectSequence();
            foreach (Cell cell in selectedCells) {
                cell.Incorrect = !result;
            }
        }

        private void ClearSelectedCells() {
            while (selectedCells.Count > 0) {
                Cell cell = selectedCells.Pop();
                cell.Selected = false;
                cell.Incorrect = false;
            }
        }

        [RelayCommand]
        private void CellMouseDown(Cell cell) {
            if (selectedCells.Count == 0) {
                selectedCells.Push(cell);
                cell.Selected = true;
                Highlight();
            }
        }

        [RelayCommand]
        private void CellMouseEntered(Cell cell) {
            if (selectedCells.Count > 0) {
                Cell lastCell = selectedCells.Peek();
                if (cellMap.AreNeighbors(cell, lastCell)) {
                    cell.Selected = true;
                    selectedCells.Push(cell);
                    Highlight();
                }
            }
        }

        [RelayCommand]
        private void CellMouseUp() {
            if (IsCorrectSequence())
                AlphabetRequested?.Invoke();
            else
                ClearSelectedCells();
        }

        [RelayCommand]
        private void OpenMeaning(WordItem wordItem) =>
            WordMeaningRequested?.Invoke(wordItem.Meaning);
    }
}
