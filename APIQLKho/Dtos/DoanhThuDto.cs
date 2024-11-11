namespace APIQLKho.Dtos
{
    public class DoanhThuDto
    {
        public int MaDoanhThu { get; set; }
        public int MaSanPham { get; set; }
        public decimal? TongPhiNhap { get; set; }
        public decimal? TongPhiXuat { get; set; }
        public decimal? PhiVanHanh { get; set; }
        public decimal? DoanhThuNgay { get; set; }
        public DateTime? NgayCapNhat { get; set; }

        // Thông tin cơ bản về sản phẩm
        
        //public string? TenSanPham { get; set; } // Tên sản phẩm từ SanPham
    }

}
