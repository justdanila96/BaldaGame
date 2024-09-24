using LiteDB;
using System.Diagnostics;

namespace BaldaApp {
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class WordItem {
        public ObjectId _id { get; set; }
        public string? Value { get; set; }
        public int Size { get; set; }
        public string? Meaning { get; set; }

        public WordItem() {
            _id = ObjectId.Empty;
        }

        public WordItem(string word) {
            _id = ObjectId.Empty;
            Value = word;
            Size = word.Length;
        }

        private string GetDebuggerDisplay() {
            string value = Value ?? "Empty";
            return $"{value} ({Size})";
        }
    }
}
