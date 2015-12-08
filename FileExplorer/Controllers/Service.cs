using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using FileExplorer.Models;

namespace FileExplorer.Controllers
{
    /// <summary>
    /// Class to service all required manipulations with directories and files
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Gets info about directory on path with ability to get info about parent directory 
        /// </summary>
        /// <param name="path">Path to get info</param>
        /// <param name="isBack">Method returns info on parent directory if true, on path otherwise</param>
        /// <returns>DirectoryInfoModel representation of info on directory</returns>
        public DirectoryInfoModel GetInfo(string path, bool isBack)
        {
            path = makeCorrectPass(path);
            if (isBack)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return GetInfo("");
                }
                else
                {
                    path = path.Substring(0, path.LastIndexOf('\\'));
                    if (path.IndexOf('\\') == -1)
                    {
                        path = "";
                        return GetInfo(path);
                    }
                    else
                    {
                        return GetInfo(path);
                    }
                }
            }
            else
            {
                return GetInfo(path);
            }
        }

        /// <summary>
        /// Get correct path from passed path
        /// </summary>
        /// <param name="path">Path for correction</param>
        /// <returns>New correct path</returns>
        private string makeCorrectPass(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }
            else
            {
                if (path.IndexOf('\\') < path.IndexOf(':'))
                {
                    return path.Substring(path.IndexOf(':') - 1);
                }
                else
                {
                    return path;
                }
            }
        }

        /// <summary>
        /// Shows all required information about folder 
        /// </summary>
        /// <param name="path">Path of folder to get info</param>
        /// <returns>DirectoryInfoModel representation of directory</returns>
        private DirectoryInfoModel GetInfo(string path)
        {
            DirectoryInfoModel dir = new DirectoryInfoModel
            {
                path = "",
                Info = new InfoModel
                {
                    Small = "0",
                    Medium = "0",
                    Large = "0"
                },
                Directories = new List<DirectoryModel>()
            };

            if (string.IsNullOrEmpty(path))
            {
                return GetInfoOnDrives();
            }
            else
            {
                if (isFilePath(path))
                {
                    return GetInfo(path.Substring(0, path.LastIndexOf('\\')));
                }
                else
                {
                    try
                    {
                        var temp = new DirectoryInfo(path).GetDirectories();
                    }
                    catch
                    {
                        dir.path = path;
                        return dir;
                    }
                    var files = new List<FileInfo>(GetAllFiles(path));

                    dir.path = path;
                    if (files != null)
                    {
                        dir.Info.Small = GetSmall(files);
                        dir.Info.Medium = GetMedium(files);
                        dir.Info.Large = GetLarge(files);
                    }
                    dir.Directories = GetListOfDirectoriesAndFiles(path);
                    return dir;
                }
            }
        }

        /// <summary>
        /// List of subdirectories of path directory
        /// </summary>
        /// <param name="path">Directory on which info to get</param>
        /// <returns>Subdirectories of path directory</returns>
        private List<DirectoryModel> GetListOfDirectoriesAndFiles(string path)
        {
            List<DirectoryModel> result = new List<DirectoryModel>();
            foreach (var item in new DirectoryInfo(path).GetDirectories())
            {
                try
                {
                    item.GetDirectories();
                    result.Add(new DirectoryModel { Name = item.Name, Size = "" });
                }
                catch
                {

                }
            }

            foreach (var item in new DirectoryInfo(path).GetFiles())
            {
                result.Add(new DirectoryModel { Name = item.Name, Size = item.Length.ToString() });
            }

            return result;
        }

        /// <summary>
        /// Gives number of large files in collection
        /// </summary>
        /// <param name="files">Collection of files to filter</param>
        /// <returns>String representation of number</returns>
        private string GetLarge(List<FileInfo> files)
        {
            if (files == null)
            {
                return "0";
            }
            return (from item in files
                    where item != null && item.Length >= 104857600
                    select item).ToList().Count().ToString();
        }

        /// <summary>
        /// Gives number of large files in collection
        /// </summary>
        /// <param name="files">Collection of files to filter</param>
        /// <returns>String representation of number</returns>
        private string GetMedium(List<FileInfo> files)
        {
            if (files == null)
            {
                return "0";
            }
            return (from item in files
                    where item != null && item.Length > 10485760 && item.Length <= 52428800
                    select item).ToList().Count().ToString();
        }

        /// <summary>
        /// Gives number of large files in collection
        /// </summary>
        /// <param name="files">Collection of files to filter</param>
        /// <returns>String representation of number</returns>
        private string GetSmall(List<FileInfo> files)
        {
            return (from item in files
                    where item != null && item.Length <= 1048576
                    select item).ToList().Count().ToString();
        }

        /// <summary>
        /// Gets all files on current directory and all it's subdirectories
        /// </summary>
        /// <param name="path">Path of current directory</param>
        /// <returns>Collection of all files</returns>
        private List<FileInfo> GetAllFiles(string path)
        {
            List<FileInfo> result = new List<FileInfo>();
            try
            {
                Parallel.ForEach(new DirectoryInfo(path).GetFiles(), item =>
                {
                    result.Add(item);
                });

                Parallel.ForEach(new DirectoryInfo(path).GetDirectories(), item =>
                {
                    List<FileInfo> Temp = GetAllFiles(item.FullName);
                    result = result.Concat(Temp).ToList();
                });
            }
            catch
            {

            }
            return result;
        }

        /// <summary>
        /// Checks if passed path is path of file
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if passed path is filepath</returns>
        private bool isFilePath(string path)
        {
            try
            {
                long d = new FileInfo(path).Length;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets summary info on all drives
        /// </summary>
        /// <returns>DirectoryInfoModel representation of info</returns>
        private DirectoryInfoModel GetInfoOnDrives()
        {
            DirectoryInfoModel dir = new DirectoryInfoModel
            {
                path = "",
                Info = new InfoModel
                {
                    Small = "0",
                    Medium = "0",
                    Large = "0"
                },
                Directories = new List<DirectoryModel>()
            };
            foreach (var item in DriveInfo.GetDrives())
            {
                dir.Directories.Add(new DirectoryModel { Name = item.Name, Size = "" });
                var files = GetAllFiles(item.Name);
                if (files != null)
                {
                    dir.Info.Small = (Convert.ToInt32(dir.Info.Small) + Convert.ToInt32(GetSmall(files))).ToString();
                    dir.Info.Medium = (Convert.ToInt32(dir.Info.Medium) + Convert.ToInt32(GetMedium(files))).ToString();
                    dir.Info.Large = (Convert.ToInt32(dir.Info.Large) + Convert.ToInt32(GetLarge(files))).ToString();
                }
            }
            return dir;
        }

    }
}