using LiteDB;
using System.Diagnostics;

namespace BaldaApp {
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class WordItem : IEquatable<WordItem?> {
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

        public override bool Equals(object? obj) {
            return Equals(obj as WordItem);
        }

        public bool Equals(WordItem? other) {
            return other is not null &&
                   EqualityComparer<ObjectId>.Default.Equals(_id, other._id);
        }

        public override int GetHashCode() {
            return HashCode.Combine(_id);
        }

        public static bool operator ==(WordItem? left, WordItem? right) {
            return EqualityComparer<WordItem>.Default.Equals(left, right);
        }

        public static bool operator !=(WordItem? left, WordItem? right) {
            return !(left == right);
        }
    }
}
