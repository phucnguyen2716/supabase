using Microsoft.AspNetCore.Mvc;
using Supabase.Models;
using Supabase.Storage;
using System.Diagnostics;

namespace Supabase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Client _storageClient;
        private readonly string _bucketName;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;

            var url = configuration["Supabase:Url"]!;
            var key = configuration["Supabase:ServiceKey"]!;
            _bucketName = configuration["Supabase:BucketName"]!;

            // Đảm bảo URL kết thúc bằng /storage/v1
            var storageUrl = url.EndsWith("/storage/v1") ? url : $"{url.TrimEnd('/')}/storage/v1";

            var headers = new Dictionary<string, string>
            {
                { "apikey", key },
                { "Authorization", $"Bearer {key}" }
            };

            _storageClient = new Client(storageUrl, headers);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Vui lòng chọn một file!";
                return View("Index");
            }

            try
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();

                    var options = new Storage.FileOptions
                    {
                        ContentType = file.ContentType,
                        Upsert = true
                    };

                    await _storageClient.From(_bucketName).Upload(fileBytes, fileName, options);
                }

                string publicUrl = _storageClient.From(_bucketName).GetPublicUrl(fileName);

                ViewBag.Message = "Upload thành công!";
                ViewBag.ImageUrl = publicUrl;
                ViewBag.OriginalFileName = file.FileName;
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Lỗi: " + ex.Message;
            }

            return View("Index");
        }

        public async Task<IActionResult> Privacy()
        {
            try
            {
                var files = await _storageClient.From(_bucketName).List();
                return View(files ?? new List<Supabase.Storage.FileObject>());
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Lỗi khi lấy danh sách file: " + ex.Message;
                return View(new List<Supabase.Storage.FileObject>());
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
