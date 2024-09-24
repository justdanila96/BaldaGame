using CommunityToolkit.Mvvm.ComponentModel;

namespace BaldaApp {

    public enum CellStatus {
        Empty, Fillable, Filled
    }

    public partial class Cell : ObservableObject {

        [ObservableProperty]
        private CellStatus status;

        [ObservableProperty]
        private char? letter;

        [ObservableProperty]
        private bool selected;

        [ObservableProperty]
        private bool incorrect;

        public Cell( char? letter) {          
            Letter = letter;
        }

        public static bool IsEmpty(Cell cell) => cell.Status == CellStatus.Empty;
        public static bool IsFillable(Cell cell) => cell.Status == CellStatus.Fillable;
        public static bool IsFilled(Cell cell) => cell.Status == CellStatus.Filled;
    }
}
