using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EFSTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        // GET: api/<UploadController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UploadController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UploadController>
        [HttpPost]
        public void Post(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No files found to upload");


            var path = Path.Combine(Directory.GetCurrentDirectory(), "accessfiles", file.FileName);
            using (var stream = new FileStream(path,FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }

    }
}
