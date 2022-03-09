using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;

namespace SymaCord.TryOnMirror.UI.Web.Utils
{
    public static class Helper
    {
        public static string AppRootPath
        {
            get { return HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"]; }
        }

        public static string AppRootParentPath
        {
            get
            {
                var dir = new DirectoryInfo(AppRootPath);

                return dir.FullName; //.Parent.FullName;
            }
        }

        public static string PhotoFilePath
        {
//            get { return AppRootParentPath + "\\Files\\Photos\\"; }
            get { return AppRootParentPath + @"\Images\Photos\"; }
        }

        public static string HairstyleFilePath
        {
            //get { return AppRootParentPath + "\\Files\\Photos\\Hairstyles\\"; }
            get { return AppRootParentPath + @"\Images\Hairstyles\"; }
        }

        public static string HairColorFilePath
        {
            get { return AppRootParentPath + @"\Images\Hairstyles\Colors\"; }
        }

        public static string ModelPhotoeFilePath
        {
            //get { return AppRootParentPath + "\\Files\\Photos\\Models\\"; }
            get { return AppRootParentPath + @"\Images\Models\"; }
        }
        
        public static string EyeWearFilePath
        {
            //get { return AppRootParentPath + "\\Files\\Photos\\EyeWears\\"; }
            get { return AppRootParentPath + @"\Images\EyeWears\"; }
        }

        public static string SharedMakeoverFilePath
        {
            get { return AppRootParentPath + @"\Images\Shared\"; }
        }

        public static string MakeoverImageFilePath
        {
            get { return AppRootParentPath + @"\Images\Makeovers\"; }
        }

        public static string TempFilePath
        {
            //get { return AppRootParentPath + "\\Files\\Temp\\"; }
            get { return AppRootParentPath + @"\Images\Temp\"; }
        }

        public static string GetFullFilePath(string fn /*fileName*/, string ext, bool temp, string size)
        {
            var fileName = fn; // Path.GetFileNameWithoutExtension(fn);
            fileName += !string.IsNullOrEmpty(size) ? "_" + size : string.Empty;
            fileName += ext.StartsWith(".") ? ext : "." + ext;
            string filePath = null;

            if (temp)
            {
                filePath = TempFilePath;
            }
            else
            {
                var fileFolderParent = fileName.Substring(0, fileName.IndexOf("_"));
                var fileFolder = string.Empty;

                switch(fileFolderParent)
                {
                    case "model":
                        fileFolder = @"Models\";
                        break;
                    case "photo":
                        fileFolder = @"Photos\";
                        break;
                    case "hairstyle":
                        fileFolder = @"Hairstyles\";
                        break;
                    case "eyewear":
                        fileFolder = @"EyeWears\";
                        break;
                    default:
                        break;
                }

                fileFolder += (fileFolderParent.ToLower() == "model") ? string.Empty :
                    fileFolderParent.Length > 2 ?
                    fileName.Remove(0, fileName.IndexOf("_") + 1).Substring(0, 2) : 
                    fileName.Substring(0, 2);

                filePath = PhotoFilePath + fileFolder;// +@"\";
            }

            string fullFilePath = filePath + fileName;

            if (File.Exists(fullFilePath))
                return fullFilePath;

            return null;
        }

        public static string RelativeFromAbsolutePath(string path, HttpRequestBase request)
        {
            //return (path.Replace(request.ServerVariables["APPL_PHYSICAL_PATH"], "/").Replace(@"\\", "/")).ToLower();

            var applicationPath = request.PhysicalApplicationPath;
            var virtualDir = request.ApplicationPath;
            virtualDir = virtualDir == "/" ? virtualDir : (virtualDir + "/");
            var result = path.Replace(applicationPath, virtualDir).Replace(@"\", "/").ToLower();

            if(!request.IsLocal)
            {
                return request.Url.GetLeftPart(UriPartial.Authority) + result;
            }

            return result;
            //throw new InvalidOperationException("Cannot map absolute back to relative path.");
        }

        public static string ConstructFilePath(string fileName, string parentPath, bool temp)
        {
            string fullPath = null;

            if (temp)
            {
                string topPath = TempFilePath;
                fullPath = topPath + fileName;
            }
            else
            {
                var fileSection = fileName.Substring(0, fileName.IndexOf("_"));
                string fileFolder = fileSection.Length > 2?
                    fileName.Remove(0, fileName.IndexOf("_") + 1).Substring(0, 2) + @"\" : fileName.Substring(0, 2) + @"\";
                string topPath = parentPath + fileFolder;
                fullPath = topPath + fileName;
            }

            return fullPath;
        }

        //public static string ConstructFilePath(string fileName, string parentPath, bool temp)
        //{
        //    string fullPath = null;

        //    if (temp)
        //    {
        //        string topPath = parentPath;
        //        fullPath = topPath + @"\" + fileName;
        //    }
        //    else
        //    {
        //        var fileSection = fileName.Substring(0, fileName.IndexOf("_"));
        //        string fileFolder = fileSection.Length > 2 ?
        //            fileName.Remove(0, fileName.IndexOf("_") + 1).Substring(0, 2) + @"\" : fileName.Substring(0, 2) + @"\";
        //        string topPath = parentPath + fileFolder;
        //        fullPath = topPath + @"\" + fileName;
        //    }

        //    return fullPath;
        //}

        public static void DeletePhoto(string fileName)
        {
            //string fileName = id.ToString("N");
            string photoPath = PhotoFilePath + fileName.Substring(0, 2) + "/";

            if (File.Exists(photoPath + fileName + ".jpg"))
            {
                new List<string>(Directory.GetFiles(photoPath)).ForEach(file =>
                    {
                        Regex re = new Regex(fileName, RegexOptions.IgnoreCase);
                        if (re.IsMatch(file))
                            File.Delete(file);
                    });

                if (Directory.GetFiles(photoPath).Length == 0)
                    Directory.Delete(photoPath);
            }
        }

        public static void DeleteFile(string fileName, string extention, string parentPath)
        {
            string pathWithFileName = ConstructFilePath(fileName, parentPath, false);
            var path = pathWithFileName.Substring(0, pathWithFileName.LastIndexOf("\\") + 1);

            //var exist = System.IO.File.Exists(pathWithFileName + ".png");

            if (File.Exists(pathWithFileName + extention))
            {
                new List<string>(Directory.GetFiles(path)).ForEach(file =>
                {
                    var re = new Regex(fileName, RegexOptions.IgnoreCase);
                    if (re.IsMatch(file))
                        File.Delete(file);
                });

                if (Directory.GetFiles(path).Length == 0)
                    Directory.Delete(path);
            }
        }

        public static void CreateDirectoryIfNotExist(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        public static string Protect(string unprotectedText, string purpose)
        {
            var unprotectedBytes = Encoding.UTF8.GetBytes(unprotectedText);
            var protectedBytes = MachineKey.Protect(unprotectedBytes, purpose);
            var protectedText = Convert.ToBase64String(protectedBytes);
            return protectedText;
        }

        public static string Unprotect(string protectedText, string purpose)
        {
            var protectedBytes = Convert.FromBase64String(protectedText);
            var unprotectedBytes = MachineKey.Unprotect(protectedBytes, purpose);
            var unprotectedText = Encoding.UTF8.GetString(unprotectedBytes);
            return unprotectedText;
        }

        public static string RandomString(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "length cannot be less than zero.");
            if (string.IsNullOrEmpty(allowedChars)) throw new ArgumentException("allowedChars may not be empty.");

            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length) throw new ArgumentException(String.Format("allowedChars may contain no more than {0} characters.", byteSize));

            // Guid.NewGuid and System.Random are not particularly random. By using a
            // cryptographically-secure random number generator, the caller is always
            // protected, regardless of use.
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < length)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < length; ++i)
                    {
                        // Divide the byte into allowedCharSet-sized groups. If the
                        // random value falls into the last group and the last group is
                        // too small to choose from the entire allowedCharSet, ignore
                        // the value in order to avoid biasing the result.
                        var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i]) continue;
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }
                return result.ToString();
            }
        }
    }

}