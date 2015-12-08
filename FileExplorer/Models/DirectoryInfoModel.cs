using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileExplorer.Models
{
    public class DirectoryInfoModel
    {
        public string path { get; set; }
        public List<DirectoryModel> Directories { get; set; }
        public InfoModel Info { get; set; }
    }
}