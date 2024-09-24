using BaldaApp;

namespace BaldaTests {
    public class FastSearchTests {

        private FastSearch? fastSearch;
        private readonly List<WordItem> randomWords = [];

        [SetUp]
        public async Task Setup() {
            fastSearch = await Database.GetPrefixTrie();

            for (int i = 0; i < 100; i++) {
                var wordItem = await Database.GetRandomWordItemAsync(Random.Shared.Next(1, 10));
                if (wordItem != null)
                    randomWords.Add(wordItem);
            }
        }

        [Test]
        public void FastSearchCanFindWords() {

            Assert.That(fastSearch, Is.Not.Null);

            foreach (WordItem item in randomWords) {
                Assert.That(item.Value, Is.Not.Null);
                Assert.That(fastSearch.Contains(item.Value), Is.True);
            }
        }
    }
}
