using System.Collections.Immutable;

namespace BaldaApp {
    public class CellMap {

        private readonly ImmutableSortedDictionary<(int, int), Cell> main;
        private readonly ImmutableDictionary<Cell, (int, int)> reversed;

        private static IEnumerable<KeyValuePair<(int, int), Cell>> GenerateForward(string word) {

            int sz = word.Length;

            for (int y = 0; y < sz; y++) {
                for (int x = 0; x < sz; x++) {

                    char? letter = x == sz / 2 ? word[y] : null;
                    Cell cell = new(letter);

                    if (letter.HasValue)
                        cell.Status = CellStatus.Filled;

                    yield return new KeyValuePair<(int, int), Cell>((x, y), cell);
                }
            }
        }

        private static KeyValuePair<B, A> Swap<A, B>(KeyValuePair<A, B> pair) => new(pair.Value, pair.Key);

        public CellMap(WordItem wordItem) {

            string word = wordItem.Value!;
            main = GenerateForward(word).ToImmutableSortedDictionary();
            reversed = main.Select(Swap).ToImmutableDictionary();
        }

        public Cell? this[(int, int) position] {
            get {
                _ = main.TryGetValue(position, out Cell? cell);
                return cell;
            }
        }

        public (int, int) this[Cell cell] => reversed[cell];

        public IEnumerable<Cell> Cells => main.Values;

        public bool AreNeighbors(Cell A, Cell B) {
            (int ax, int ay) = this[A];
            (int bx, int by) = this[B];
            return Math.Abs(ax - bx) == 1 && ay == by || Math.Abs(ay - by) == 1 && ax == bx;
        }

        public void RefreshFillableCells() {

            foreach (var cell in Cells) {

                if (!Cell.IsEmpty(cell))
                    continue;

                (int x, int y) = reversed[cell];
                IEnumerable<(int, int)> neighbors = [(x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)];

                if (neighbors.Any(pos => main.TryGetValue(pos, out Cell? neighbor) && Cell.IsFilled(neighbor)))
                    cell.Status = CellStatus.Fillable;
            }
        }
    }
}
