using Supabase.Storage;

namespace Supabase.Services;

public class SupabaseStorageService
{
    private readonly Client _storage;
    private const string BucketName = "pdf-bucket.";

    public SupabaseStorageService(IConfiguration configuration)
    {
        var url = configuration["Supabase:Url"]!;
        var key = configuration["Supabase:ServiceKey"]!;

        // Đảm bảo URL kết thúc bằng /storage/v1
        var storageUrl = url.EndsWith("/storage/v1") ? url : $"{url.TrimEnd('/')}/storage/v1";

        // Sử dụng Service Key (như một api key) để có toàn quyền quản lý
        var headers = new Dictionary<string, string>
        {
            { "apikey", key }, // Quan trọng: Supabase yêu cầu cả apikey và Authorization
            { "Authorization", $"Bearer {key}" }
        };

        _storage = new Client(storageUrl, headers);
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        // Giữ nguyên logic tạo tên file duy nhất
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();

            // Thêm FileOptions để tự động xác định loại file (PDF, Zip, Docx...)
            var options = new Storage.FileOptions
            {
                ContentType = file.ContentType, // Quan trọng để trình duyệt đọc được file
                Upsert = true
            };

            await _storage.From(BucketName).Upload(fileBytes, fileName, options);
        }

        return _storage.From(BucketName).GetPublicUrl(fileName);
    }

    public async Task DeleteAsync(string fileName)
    {
        // Truyền một danh sách các đường dẫn cần xóa
        await _storage.From(BucketName).Remove(new List<string> { fileName });
    }
    public async Task<List<Supabase.Storage.FileObject>> ListFilesAsync()
    {
        // SỬA: Phải gọi _storage (biến Client), không phải _storageService
        // Và dùng List() để lấy danh sách file
        var files = await _storage.From(BucketName).List();

        // Nếu kết quả null, trả về danh sách rỗng để tránh lỗi ở View
        return files ?? new List<Supabase.Storage.FileObject>();
    }
}