using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BaldaApp {
    public partial class Player : ObservableObject {

        [ObservableProperty]
        private ObservableCollection<WordItem> foundWords;

        public Player() {
            FoundWords = [];
        }

        public int Score =>
            FoundWords.Aggregate(0, (sum, wordItem) => sum + wordItem.Size);

        public bool TryAddNewWord(WordItem wordItem) {

            WordItem? item = FoundWords.FirstOrDefault(w => w._id == wordItem._id);

            if (item == null) {
                FoundWords.Add(wordItem);
                return true;
            }
            else {
                return false;
            }
        }
    }
}
