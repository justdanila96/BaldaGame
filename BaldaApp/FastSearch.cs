using LiteDB;
using System.Collections.Immutable;

namespace BaldaApp {

    public class FastSearch(IEnumerable<WordItem> wordItems) {

        private readonly record struct Node(ImmutableDictionary<char, Node> Subnodes, ObjectId? Value);

        private static readonly Node EmptyNode = new(ImmutableDictionary<char, Node>.Empty, null);

        private static Node Insert(Node node, ObjectId id, ReadOnlySpan<char> word, int index) {

            if (index < word.Length) {

                char letter = word[index];
                (var subnodes, _) = node;
                Node foundSubnode = subnodes.TryGetValue(letter, out Node found) ? found : EmptyNode;
                Node newSubnode = Insert(foundSubnode, id, word, index + 1);
                var newSubnodes = subnodes.SetItem(letter, newSubnode);
                return node with { Subnodes = newSubnodes };
            }
            else
                return EmptyNode with { Value = id };
        }

        private readonly Node root =
            wordItems.Aggregate(EmptyNode, (node, wordItem) =>
                Insert(node, wordItem._id, wordItem.Value, 0));

        private static ObjectId? Search(Node node, ReadOnlySpan<char> word, int index) =>
            index >= word.Length
                ? node.Value
                : node.Subnodes.TryGetValue(word[index], out Node subnode)
                    ? Search(subnode, word, index + 1)
                    : null;

        public bool Contains(string word) => Search(root, word, 0) != null;

        public async Task<WordItem?> FindWordItem(string word) {
            ObjectId? id = Search(root, word, 0);
            return id == null ? null : await Database.GetWordItem(id);
        }
    }
}
