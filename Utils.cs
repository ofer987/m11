using System.Text;

namespace M11;

public static class Utils {
    public static void CreateFile(string path) {
        if (File.Exists(path)) {
            return;
        }

        File.Open(path, FileMode.Create);
    }

    public static string ConvertToBase36(long input) {
        var i = 7;
        var sb = new StringBuilder();
        var remaining = input;
        var remainder = 0L;

        while (i >= 0 && remaining > 0) {
            remainder = remaining % 36;
            remaining = remaining / 36;

            if (remainder >= 10) {
                char ch = ConvertNumberToLowerCaseCharacter(remainder);
                sb.Append(ch);
            }
            else {
                sb.Append(remainder);
            }

            i -= 1;
        }

        var length = sb.Length;

        for (i = length; i < 8; i += 1) {
            sb.Append(0);
        }

        var result = sb.ToString();
        var charArray = result.ToCharArray();
        Array.Reverse(charArray);

        var reversedResult = new string(charArray);

        return reversedResult;
    }

    private static char ConvertNumberToLowerCaseCharacter(long input) {
        // a is 97
        // but input starts @ 10
        // So 10 will be 'a' or 97
        return (char)(input + 87);
    }
}
