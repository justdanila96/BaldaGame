using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Documents;

namespace BaldaApp.ViewModels {
    internal partial class WordDescriptionViewModel(string? meaning) : ObservableObject {

        public FlowDocument Content {
            get {
                string txt = meaning ?? "Описание не найдено";
                var p = new Paragraph();
                p.Inlines.Add(txt);
                return new FlowDocument(new Section(p));
            }
        }

        public event Action? BackToGame;

        [RelayCommand]
        private void Exit() => BackToGame?.Invoke();
    }
}
