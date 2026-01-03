# ASP.NET Core MVC + Supabase Storage Integration

Dự án mẫu hướng dẫn tích hợp **Supabase Storage** vào ứng dụng **ASP.NET Core MVC**.  
Hệ thống cho phép quản lý tệp tin đám mây với các thao tác cơ bản: **Liệt kê, Tải lên, Tải xuống và Xóa**.

---

## Mục lục

- [Cấu hình Supabase](#cấu-hình-supabase)  
- [Cấu hình ứng dụng (appsettings.json)](#cấu-hình-ứng-dụng-appsettingsjson)  
- [Cài đặt Thư viện (NuGet)](#cài-đặt-thư-viện-nuget)  
- [Khởi tạo Service (Program.cs)](#khởi-tạo-service-programcs)  
- [Triển khai Controller (FileController.cs)](#triển-khai-controller-filecontrollercs)  
- [Giao diện người dùng (Index.cshtml)](#giao-diện-người-dùng-indexcshtml)  
- [Lưu ý quan trọng](#lưu-ý-quan-trọng)  

---

## Cấu hình Supabase

Trước khi chạy code, bạn cần thiết lập trên **Supabase Dashboard**:

### Tạo Bucket
- Vào mục **Storage**, tạo **bucket** tên là `my-bucket`.
- Chọn **Public** nếu muốn truy cập nhanh hoặc **Private** để bảo mật.

### Thiết lập RLS Policy
- Đây là bước quan trọng nhất, nếu không cấu hình bạn sẽ gặp lỗi `403 Forbidden`.
- Vào **Storage > Policies**.
- Thêm các quyền: `SELECT`, `INSERT`, `DELETE` cho role `anon` hoặc `authenticated`.

### Lấy API Keys
- Copy **Project URL** và **anon key** từ mục **Settings > API**.

---

## Cấu hình ứng dụng (appsettings.json)

Thêm các thông số kết nối vào file cấu hình:

```json
{
  "Supabase": {
    "Url": "https://your-project-id.supabase.co",
    "Key": "your-anon-key",
    "Bucket": "my-bucket"
  }
}
