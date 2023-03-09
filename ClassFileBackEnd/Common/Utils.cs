using System.Text;

namespace ClassFileBackEnd.Common
{
    public class Utils
    {
        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string mime;
            return Const._mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        public static string GetFileExtension(string fileName)
        {
            string[] fileNameArrayByDot = fileName.Split(".");
            string fileType = fileNameArrayByDot[fileNameArrayByDot.Length - 1];
            return fileType.ToLower();
        }

        #region generate random classcode
        private static readonly Random _random = new Random();

        // Generates a random number within a range.
        public static int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.

            // char is a single Unicode character
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        public static string RandomClassCode()
        {
            var passwordBuilder = new StringBuilder();

            // 4-Letters lower case
            passwordBuilder.Append(RandomString(4, true));

            // 4-Digits between 1000 and 9999
            passwordBuilder.Append(RandomNumber(1000, 9999));

            // 2-Letters upper case
            passwordBuilder.Append(RandomString(2));
            return passwordBuilder.ToString();
        }
        #endregion
    }
}
