using ClassFileBackEnd.Models;

namespace ClassFileBackEnd.Common
{
    public class Utils
    {
        public static class MyQuery<T>
        {
            public static (IQueryable<T>, int)Paging(IQueryable<T> query, int pageNumber)
            {
                pageNumber = PageIndexNormalize(pageNumber);
                int totalPage = 0;

                int totalRecord = query.Count();
                totalPage = (int) Math.Ceiling((decimal) totalRecord / Const.NUMBER_RECORD_PAGE);

                IQueryable<T> queryResult = query.Skip(Const.NUMBER_RECORD_PAGE * (pageNumber - 1))
                    .Take(Const.NUMBER_RECORD_PAGE);

                return (queryResult, totalPage);
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
