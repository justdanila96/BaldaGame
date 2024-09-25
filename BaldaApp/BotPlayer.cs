using System.Text;

namespace BaldaApp {
    public partial class BotPlayer(FastSearch trie, CellMap cellMap) : Player {

        public async void Run(IEnumerable<WordItem> forbidden) {

            var validCells = cellMap.Cells.Where(cell => !Cell.IsEmpty(cell));
            var found = new HashSet<WordItem>();

            foreach (var cell in validCells) {

                var visited = new LinkedList<Cell>();
                await Search(cellMap[cell], found, visited);
            }

            FoundWords.Clear();
            found.ExceptWith(forbidden);

            var result = found.OrderByDescending(item => item.Size).ThenBy(item => item.Value);

            foreach (var item in result) {
                FoundWords.Add(item);
            }
        }

        private async Task Search((int, int) pos, HashSet<WordItem> found, LinkedList<Cell> visited) {

            Cell? cell = cellMap[pos];

            if (cell == null
                || Cell.IsEmpty(cell)
                || visited.Contains(cell)
                || (Cell.IsFillable(cell) && visited.Any(Cell.IsFillable)))
                return;

            if (visited.Last != null && !cellMap.AreNeighbors(cell, visited.Last.Value))
                return;

            visited.AddLast(cell);

            if (visited.Count(Cell.IsFillable) == 1) {

                for (char letter = 'а'; letter <= 'я'; letter++) {

                    var sb = new StringBuilder();
                    foreach (Cell c in visited) {

                        char symbol = Cell.IsFillable(c) ? letter : c.Letter!.Value;
                        sb.Append(symbol);
                    }

                    string newWord = sb.ToString();

                    if (newWord.Length > 2) {

                        WordItem? wordItem = await trie.FindWordItem(newWord);
                        if (wordItem != null) {
                            found.Add(wordItem);
                        }
                    }
                }
            }

            (int x, int y) = pos;
            await Search((x + 1, y), found, visited);
            await Search((x - 1, y), found, visited);
            await Search((x, y - 1), found, visited);
            await Search((x, y + 1), found, visited);
        }
    }
}
