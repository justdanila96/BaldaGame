using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BaldaApp.ViewModels {
    public partial class MainWindowViewModel : ObservableObject {

        [ObservableProperty]
        private object? content;

        [RelayCommand]
        private async Task Load() {

            await Database.Refresh();

            const int gameboardSize = 7;

            WordItem? startWord =
                await Database.GetRandomWordItemAsync(gameboardSize)
                ?? throw new ArgumentNullException("Start word not found. Check your database");

            FastSearch prefixTrie = await Database.GetPrefixTrie();

            var gameboard = new GameboardViewModel(startWord, prefixTrie);

            gameboard.AlphabetRequested += () => {
                var alphabet = new AlphabetViewModel();
                alphabet.LetterPicked += async symbol => {
                    await gameboard.AddNewLetter(symbol);
                    Content = gameboard;
                };
                Content = alphabet;
            };

            gameboard.WordMeaningRequested += word => {
                var wordDescr = new WordDescriptionViewModel(word);
                wordDescr.BackToGame += () => Content = gameboard;
                Content = wordDescr;
            };

            Content = gameboard;
        }
    }
}
