using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client.Extensions.Msal;
using Supabase.Models;
using Supabase.Services;
using System.Diagnostics;

namespace Supabase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SupabaseStorageService _storageService;
        private const string BucketName = "pdf-bucket.";


        public HomeController(ILogger<HomeController> logger, SupabaseStorageService storageService)
        {
            _logger = logger;
            _storageService = storageService;
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
                // Gọi service đã viết ở bước trước
                string publicUrl = await _storageService.UploadAsync(file);

                ViewBag.Message = "Upload thành công!";
                ViewBag.ImageUrl = publicUrl;
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Lỗi: " + ex.Message;
            }

            return View("Index");
        }
        // Thêm vào file SupabaseStorageService.cs
        public async Task<IActionResult> Privacy()
        {
            // Gọi phương thức list từ Service đã viết ở trên
            var files = await _storageService.ListFilesAsync();

            return View(files);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
