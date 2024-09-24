using BaldaApp;

namespace BaldaTests {
    public class DatabaseTests {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public async Task DatabaseRandomWord() {
            const int size = 10;
            WordItem? wordItem = await Database.GetRandomWordItemAsync(size);
            Assert.That(wordItem, Is.Not.Null);
            Assert.That(wordItem, Has.Property("Value"));
            Assert.That(wordItem.Value, Is.Not.Null);
            Assert.Multiple(() => {
                Assert.That(wordItem.Value, Has.Property("Length").EqualTo(size));
                Assert.That(wordItem, Has.Property("Size"));
            });
            Assert.That(wordItem.Size, Is.EqualTo(size));
        }

        [Test]
        public async Task DatabaseRandomWord2() {
            const int size = int.MaxValue;
            WordItem? wordItem = await Database.GetRandomWordItemAsync(size);
            Assert.That(wordItem, Is.Null);
        }
    }
}