namespace APIQLKho.Dtos
{
    public class NhanVienKhoDto
    {
        public int MaNhanVienKho { get; set; }
        public string? TenNhanVien { get; set; }
        public string? Email { get; set; }
        public int? Sdt { get; set; }
        public DateTime? NamSinh { get; set; }
        public string? Hinhanh { get; set; }
        public IFormFile? Img { get; set; } // Trường này để nhận file ảnh tải lên
    }
}
