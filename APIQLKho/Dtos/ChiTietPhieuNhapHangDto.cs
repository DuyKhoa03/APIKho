namespace APIQLKho.Dtos
{
    public class ChiTietPhieuNhapHangDto
    {
        public int MaPhieuNhapHang { get; set; }
        public int MaSanPham { get; set; }
        public string? TenSanPham { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public int? TrangThai { get; set; }
        public string? Image { get; set; }
        public string? Image2 { get; set; }

        public string? Image3 { get; set; }

        public string? Image4 { get; set; }

        public string? Image5 { get; set; }

        public string? Image6 { get; set; }
        public List<IFormFile>? Images { get; set; } // Danh sách file ảnh tải lên

    }

}
