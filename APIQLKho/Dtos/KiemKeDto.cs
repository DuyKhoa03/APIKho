namespace APIQLKho.Dtos
{
    public class KiemKeDto
    {
        public int MaKiemKe { get; set; }
        public DateTime? NgayKiemKe { get; set; }

        // Thông tin cơ bản về nhân viên kho
        public int MaNhanVienKho { get; set; }
        public string? TenNhanVienKho { get; set; } // Tên nhân viên kho từ NhanVienKho
    }

}
