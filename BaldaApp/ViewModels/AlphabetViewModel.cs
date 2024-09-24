using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BaldaApp.ViewModels {
    public partial class AlphabetViewModel : ObservableObject {

        public event Action<char?>? LetterPicked;

        public List<char> Alphabet { get; }

        private static IEnumerable<char> GenerateAlphabet() {
            for (char c = '\x0430'; c <= '\x044f'; c++) {
                yield return c;
            }

            yield return '\x21b5';
        }

        public AlphabetViewModel() {
            Alphabet = GenerateAlphabet().ToList();
        }

        [RelayCommand]
        private void ButtonPressed(char symbol) =>
            LetterPicked?.Invoke(char.IsLetter(symbol) ? symbol : null);
    }
}
