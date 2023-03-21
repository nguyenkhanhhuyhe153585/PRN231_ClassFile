using ClassFileBackEnd.Models;
using System.Text;

namespace ClassFileBackEnd.Common
{
    public class Utils
    {
        public static string GetDomain(HttpContext context)
        {
            string domainName = context.Request.Host.Value;
            return domainName;

        }

        public static class MyQuery<T>
        {
            public static (IQueryable<T>, int, int)Paging(IQueryable<T> query, int pageNumber)
            {
                pageNumber = PageIndexNormalize(pageNumber);
                int totalPage = 0;

                int totalRecord = query.Count();
                totalPage = (int) Math.Ceiling((decimal) totalRecord / Const.NUMBER_RECORD_PAGE);

                IQueryable<T> queryResult = query.Skip(Const.NUMBER_RECORD_PAGE * (pageNumber - 1))
                    .Take(Const.NUMBER_RECORD_PAGE);

                return (queryResult, totalPage, pageNumber);
            }
        }

        public static int PageIndexNormalize(int page)
        {
            if(page <= 0)
            {
                page = 1;
            }
            return page;
        }

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
        
        public async Task FileUpload(IFormCollection form, Post post, ClassfileContext db)
        {
            try
            {
                #region Lưu Files

                string folderName = Const.ROOT_FOLDER_NAME;
                string filePath = "";
                string fileNameForSaving = "";

                string subFolder = Const.folederModeMapping[form["fileMode"]];
                if (!Const.folederModeMapping.ContainsKey(form["fileMode"]))
                {
                    throw new Exception("Folder Mode Not Accepted");
                };

                foreach (var file in form.Files)
                {
                    string fileName = file.FileName;
                    string fileType = Utils.GetFileExtension(fileName);
                    string fileNameWithoutExtension = fileName.Split("." + fileType)[0];

                    // Triển khai khởi tạo tên file tới khi không có file nào trùng trong Dir
                    int index = 0;
                    string indexString = "";
                    do
                    {
                        if (index != 0)
                        {
                            indexString = $"_({index})_";
                        }
                        fileNameForSaving = $"{fileNameWithoutExtension}{indexString}{DateTime.Now.ToString("HHmmssddMMyyyy")}.{fileType}";
                        filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName, subFolder, fileNameForSaving);
                    }
                    while (System.IO.File.Exists(filePath));

                    // Lưu file vào tệp của Server
                    Stream fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);
                    fileStream.Close();

                    // Tạo đối tượng file gắn với Post
                    ClassFileBackEnd.Models.File fileDb = new()
                    {
                        FileType = Utils.GetMimeType(fileType),
                        FileName = fileNameForSaving,
                        FileNameRoot = fileName,
                        PostId = post.Id
                    };

                    db.Files.Add(fileDb);
                }

                await db.SaveChangesAsync();

                #endregion
            }
            catch (Exception)
            { throw; }

        }
    }
}
