using LiteDB;
using LiteDB.Async;
using MwParserFromScratch;
using MwParserFromScratch.Nodes;
using System.IO;
using System.Net.Http;

namespace BaldaApp {
    public static class Database {

        private static readonly string dbName = "words.db";
        private static readonly string wordsTabName = "words";

        private static IEnumerable<WordItem> GetWordsFromSrc(string src) {

            using var memStream = new MemoryStream();
            var streamWriter = new StreamWriter(memStream);
            streamWriter.Write(src);
            streamWriter.Flush();

            memStream.Position = 0;

            var streamReader = new StreamReader(memStream);
            while (!streamReader.EndOfStream) {
                string? word = streamReader.ReadLine();

                if (!string.IsNullOrEmpty(word))
                    yield return new WordItem(word);
            }
        }

        public async static Task Refresh() {

            using var db = new LiteDatabaseAsync(dbName);
            var collections = await db.GetCollectionNamesAsync();
            if (collections.Any())
                return;

            var collection = db.GetCollection<WordItem>(wordsTabName);
            string src = Properties.Resources.russian_nouns;
            IEnumerable<WordItem> wordItems = GetWordsFromSrc(src);
            await collection.InsertAsync(wordItems);
        }

        public static async Task<WordItem?> GetRandomWordItemAsync(int size) {

            using var db = new LiteDatabaseAsync(dbName);
            var collection = db.GetCollection<WordItem>(wordsTabName);
            await collection.EnsureIndexAsync(wordItem => wordItem.Size);

            var wordItems =
                from item in collection.Query()
                where item.Size == size
                select item;

            int count = await wordItems.CountAsync();

            if (count == 0)
                return null;

            int randomIndex = Random.Shared.Next(count);
            return await wordItems.Skip(randomIndex).FirstAsync();
        }

        public static async Task<FastSearch> GetPrefixTrie() {

            using var db = new LiteDatabaseAsync(dbName);
            var collection = db.GetCollection<WordItem>(wordsTabName);
            IEnumerable<WordItem> allWords = await collection.FindAllAsync();
            return new FastSearch(allWords);
        }

        public static async Task<WordItem> GetWordItem(ObjectId id) {

            using var db = new LiteDatabaseAsync(dbName);
            var collection = db.GetCollection<WordItem>(wordsTabName);
            return await collection.FindByIdAsync(id);
        }

        private static async Task<string?> FindWikitextAsync(string word) {
            try {
                using var client = new HttpClient();
                string uri = $@"https://ru.wiktionary.org/w/index.php?title={word}&action=raw";
                return await client.GetStringAsync(uri);
            }
            catch {
                return null;
            }
        }

        private static string? ParseWikitext(string wikitext) {

            var parser = new WikitextParser();
            Wikitext parsedData = parser.Parse(wikitext);
            const string nodeName = "Значение";

            Heading? heading =
                parsedData.EnumDescendants()
                .OfType<Heading>()
                .FirstOrDefault(heading => {
                    string text = heading.ToPlainText();
                    string name = MwParserUtility.NormalizeTemplateArgumentName(text);
                    return name == nodeName;
                });

            if (heading == null)
                return null;

            string? meaning =
                heading.EnumNextNodes()
                .OfType<ListItem>()
                .FirstOrDefault()
                ?.ToPlainText();

            return meaning;
        }

        public static async void FindMeaningAsync(WordItem wordItem) {

            if (!string.IsNullOrEmpty(wordItem.Meaning))
                return;

            string? wikitext = await FindWikitextAsync(wordItem.Value!);

            if (string.IsNullOrEmpty(wikitext))
                return;

            string? meaning = ParseWikitext(wikitext);

            if (string.IsNullOrEmpty(meaning))
                return;

            using var db = new LiteDatabaseAsync(dbName);
            var collection = db.GetCollection<WordItem>(wordsTabName);
            wordItem.Meaning = meaning;
            await collection.UpdateAsync(wordItem);
        }
    }
}
