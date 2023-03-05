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
    }
}
