using CodePlusBlog.IService;
using CodePlusBlog.Model;
using HtmlAgilityPack;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace CodePlusBlog.Service
{
    public class DailyNewsArticleService : IDailyNewsArticleService
    {
        public async Task<DailyArticle> GetRandomArticle()
        {
            string folderPath = Directory.GetCurrentDirectory(); // Get the current directory
            string fileName = "Paragraph.json"; // Replace this with the name of your JSON file
            DailyArticle dailyArticle = null;
            string filePath = Path.Combine(folderPath, "JSON", fileName);
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);

                    // Deserialize the JSON content into an object
                    List<string> jsonObject = JsonConvert.DeserializeObject<List<string>>(jsonContent);
                    dailyArticle = new DailyArticle
                    {
                        Content = await GetRandomParagraph(jsonObject),
                        Title = "Paragraph"
                    };
                }
                return await Task.FromResult(dailyArticle);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task GetTodayNewsService()
        {
            var newsApiClient = new NewsApiClient("8f1ccde175754e0db64fa03645776254");
            var articlesResponse = newsApiClient.GetEverything(new EverythingRequest
            {
                Sources = new List<string> { "techcrunch" },
                SortBy = SortBys.Popularity,
                Language = Languages.EN,
                From = DateTime.Now.AddDays(-30)
            });
            if (articlesResponse.Status == Statuses.Ok)
            {
                // total results found
                Console.WriteLine(articlesResponse.TotalResults);
                // here's the first 20
                foreach (var article in articlesResponse.Articles)
                {
                    // title
                    Console.WriteLine(article.Title);
                    // author
                    Console.WriteLine(article.Author);
                    // description
                    Console.WriteLine(article.Description);
                    // url
                    Console.WriteLine(article.Url);
                    // published at
                    Console.WriteLine(article.PublishedAt);
                }
            }

            await Task.CompletedTask;
        }

        public async Task<DailyArticle> scapingText()
        {
            var articlePage = await GetDailyRandomNumber();
            string url = $"https://bodheeprep.com/daily-rc-article-{articlePage}";
            DailyArticle dailyArticle = null;
            try
            {
                // Create HtmlWeb instance
                HtmlWeb web = new HtmlWeb();
                // Load the webpage
                HtmlDocument doc = web.Load(url);
                string keyword = "paragraph";
                // Scrape the full webpage
                var selectedParagraphs = doc.DocumentNode.Descendants("h4").Where(p => p.InnerHtml.ToLower().Contains(keyword)).ToList();
                List<string> contents = new List<string>();
                if (selectedParagraphs.Count > 0)
                {
                    selectedParagraphs.ForEach(async p =>
                    {
                        var isHeading = await FilterParagrahHeader(p.InnerText);
                        if (isHeading)
                        {
                            var nextParagraph = p.SelectSingleNode("following-sibling::p[1]");
                            if (nextParagraph != null)
                                contents.Add(nextParagraph.InnerHtml);
                        }
                    });
                    if (contents.Count > 0)
                    {
                        dailyArticle = new DailyArticle
                        {
                            Content = await GetRandomParagraph(contents),
                            Title = keyword.ToUpper(),
                            Link = url,
                            Note = "Note"
                        };
                        return dailyArticle;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return dailyArticle;
        }

        private async Task<bool> FilterParagrahHeader(string text)
        {
            // Regular expression to match any integer
            Regex regex = new Regex(@"\d+");

            // Check if the text contains any match
            return await Task.FromResult(regex.IsMatch(text));
        }

        private async Task<string> GetRandomParagraph(List<string> paragraphs)
        {
            Random rand = new Random();
            int index = rand.Next(paragraphs.Count); // Generate a random index
            string randomParagraph = paragraphs[index]; // Access the item at the random index
            return await Task.FromResult(randomParagraph);
        }

        private async Task<int> GetDailyRandomNumber()
        {
            // Use the current date as a seed for the random number generator
            int seed = DateTime.Today.GetHashCode();
            Random rand = new Random(seed);

            // Generate a random number between 1 and 80
            return await Task.FromResult(rand.Next(1, 81));
        }
    }
}
