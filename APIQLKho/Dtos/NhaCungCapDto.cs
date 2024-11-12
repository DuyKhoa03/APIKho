namespace APIQLKho.Dtos
{
    public class NhaCungCapDto
    {
        public string? TenNhaCungCap { get; set; }
        public string? DiaChi { get; set; }
        public string? Email { get; set; }
        public int? Sdt { get; set; }
        public string? Image { get; set; }
        public IFormFile? Img { get; set; } // Trường này để nhận file ảnh tải lên
    }
}
