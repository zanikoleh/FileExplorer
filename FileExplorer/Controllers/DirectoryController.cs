using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FileExplorer.Models;

namespace FileExplorer.Controllers
{
    /// <summary>
    /// Main controller
    /// </summary>
    public class DirectoryController : ApiController
    {
        /// <summary>
        /// Service to handle requests
        /// </summary>
        private readonly Service _directoryService = new Service();

        // GET: api/Directory
        public DirectoryInfoModel Get(string path, bool is_back)
        {
            return _directoryService.GetInfo(path, is_back);
        }

        public DirectoryInfoModel Get()
        {
            return _directoryService.GetInfo("", false);
        }

        // GET: api/Directory/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Directory
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Directory/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Directory/5
        public void Delete(int id)
        {
        }
    }
}
