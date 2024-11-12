using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APIQLKho.Models;

public partial class QlkhohangContext : DbContext
{
    public QlkhohangContext()
    {
    }

    public QlkhohangContext(DbContextOptions<QlkhohangContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<ChiTietKiemKe> ChiTietKiemKes { get; set; }

    public virtual DbSet<ChiTietPhieuNhapHang> ChiTietPhieuNhapHangs { get; set; }

    public virtual DbSet<ChiTietPhieuXuatHang> ChiTietPhieuXuatHangs { get; set; }

    public virtual DbSet<DoanhThu> DoanhThus { get; set; }

    public virtual DbSet<HangSanXuat> HangSanXuats { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KiemKe> KiemKes { get; set; }

    public virtual DbSet<LoaiKhacHang> LoaiKhacHangs { get; set; }

    public virtual DbSet<LoaiSanPham> LoaiSanPhams { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<NhanVienKho> NhanVienKhos { get; set; }

    public virtual DbSet<PhieuNhapHang> PhieuNhapHangs { get; set; }

    public virtual DbSet<PhieuXuatHang> PhieuXuatHangs { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=TANDUNG;Database=QLKhohang;Trusted_Connection=True;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blog__54379E5056F7424E");

            entity.ToTable("Blog");

            entity.Property(e => e.BlogId).HasColumnName("BlogID");
            entity.Property(e => e.Anh).HasMaxLength(255);
            entity.Property(e => e.Hide).HasColumnName("hide");
            entity.Property(e => e.Link)
                .HasMaxLength(255)
                .HasColumnName("link");
            entity.Property(e => e.Mota).HasMaxLength(255);

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Blog__MaNguoiDun__5EBF139D");
        });

        modelBuilder.Entity<ChiTietKiemKe>(entity =>
        {
            entity.HasKey(e => new { e.MaKiemKe, e.MaSanPham }).HasName("PK__ChiTietK__19BDB75D1C3F3340");

            entity.ToTable("ChiTietKiemKe");

            entity.HasOne(d => d.MaKiemKeNavigation).WithMany(p => p.ChiTietKiemKes)
                .HasForeignKey(d => d.MaKiemKe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietKi__MaKie__6477ECF3");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietKiemKes)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietKi__MaSan__5812160E");
        });

        modelBuilder.Entity<ChiTietPhieuNhapHang>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieuNhapHang, e.MaSanPham }).HasName("PK__ChiTiet___D10975E6A79495FA");

            entity.ToTable("ChiTiet_PhieuNhapHang");

            entity.Property(e => e.DonGiaNhap).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Image).HasMaxLength(255);

            entity.HasOne(d => d.MaPhieuNhapHangNavigation).WithMany(p => p.ChiTietPhieuNhapHangs)
                .HasForeignKey(d => d.MaPhieuNhapHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTiet_P__MaPhi__60A75C0F");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietPhieuNhapHangs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTiet_P__MaSan__5629CD9C");
        });

        modelBuilder.Entity<ChiTietPhieuXuatHang>(entity =>
        {
            entity.HasKey(e => new { e.MaSanPham, e.MaPhieuXuatHang }).HasName("PK__ChiTiet___5A2C69ED0F08BF92");

            entity.ToTable("ChiTiet_PhieuXuatHang");

            entity.Property(e => e.DonGiaXuat).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.NganHang).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TienMat).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaPhieuXuatHangNavigation).WithMany(p => p.ChiTietPhieuXuatHangs)
                .HasForeignKey(d => d.MaPhieuXuatHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTiet_P__MaPhi__628FA481");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietPhieuXuatHangs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTiet_P__MaSan__571DF1D5");
        });

        modelBuilder.Entity<DoanhThu>(entity =>
        {
            entity.HasKey(e => e.MaDoanhThu).HasName("PK__DoanhThu__F06823DB1794ABCD");

            entity.ToTable("DoanhThu");

            entity.Property(e => e.DoanhThuNgay).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");
            entity.Property(e => e.PhiVanHanh).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TongPhiNhap).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TongPhiXuat).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.DoanhThus)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DoanhThu__MaSanP__59063A47");
        });

        modelBuilder.Entity<HangSanXuat>(entity =>
        {
            entity.HasKey(e => e.MaHangSanXuat).HasName("PK__HangSanX__977119FC5048D98A");

            entity.ToTable("HangSanXuat");

            entity.Property(e => e.TenHangSanXuat).HasMaxLength(255);
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__KhachHan__88D2F0E5DBC1ADEA");

            entity.ToTable("KhachHang");

            entity.Property(e => e.TenKhachHang).HasMaxLength(255);

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.MaLoai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhachHang__MaLoa__656C112C");
        });

        modelBuilder.Entity<KiemKe>(entity =>
        {
            entity.HasKey(e => e.MaKiemKe).HasName("PK__KiemKe__D611C31FCC424DFB");

            entity.ToTable("KiemKe");

            entity.Property(e => e.NgayKiemKe).HasColumnType("datetime");

            entity.HasOne(d => d.MaNhanVienKhoNavigation).WithMany(p => p.KiemKes)
                .HasForeignKey(d => d.MaNhanVienKho)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KiemKe__MaNhanVi__5BE2A6F2");
        });

        modelBuilder.Entity<LoaiKhacHang>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK__LoaiKhac__730A5759052C074C");

            entity.ToTable("LoaiKhacHang");

            entity.Property(e => e.ChiPhiVanChuyen).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ChietKhauXuatHang).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TenLoai).HasMaxLength(255);
        });

        modelBuilder.Entity<LoaiSanPham>(entity =>
        {
            entity.HasKey(e => e.MaLoaiSanPham).HasName("PK__LoaiSanP__ECCF699F21C13257");

            entity.ToTable("LoaiSanPham");

            entity.Property(e => e.TenLoaiSanPham).HasMaxLength(255);
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__Menu__C99ED250B913643A");

            entity.ToTable("Menu");

            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.Link).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.Menus)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Menu__MaNguoiDun__5FB337D6");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D7625EA37FBE");

            entity.ToTable("NguoiDung");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.NgayDk)
                .HasColumnType("datetime")
                .HasColumnName("NgayDK");
            entity.Property(e => e.Sdt).HasColumnName("SDT");
            entity.Property(e => e.TenDangNhap).HasMaxLength(255);
            entity.Property(e => e.TenNguoiDung).HasMaxLength(255);
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNhaCungCap).HasName("PK__NhaCungC__53DA9205395DFDEC");

            entity.ToTable("NhaCungCap");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Sdt).HasColumnName("SDT");
            entity.Property(e => e.TenNhaCungCap).HasMaxLength(255);
        });

        modelBuilder.Entity<NhanVienKho>(entity =>
        {
            entity.HasKey(e => e.MaNhanVienKho).HasName("PK__NhanVien__E62587A50F6B8C2E");

            entity.ToTable("NhanVienKho");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Hinhanh).HasMaxLength(255);
            entity.Property(e => e.NamSinh).HasColumnType("datetime");
            entity.Property(e => e.Sdt).HasColumnName("SDT");
            entity.Property(e => e.TenNhanVien).HasMaxLength(255);
        });

        modelBuilder.Entity<PhieuNhapHang>(entity =>
        {
            entity.HasKey(e => e.MaPhieuNhapHang).HasName("PK__PhieuNha__1EA501A45AD9753B");

            entity.ToTable("PhieuNhapHang");

            entity.Property(e => e.NgayNhap).HasColumnType("datetime");
            entity.Property(e => e.PhiVanChuyen).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.PhieuNhapHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuNhap__MaNgu__5CD6CB2B");

            entity.HasOne(d => d.MaNhaCungCapNavigation).WithMany(p => p.PhieuNhapHangs)
                .HasForeignKey(d => d.MaNhaCungCap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuNhap__MaNha__619B8048");
        });

        modelBuilder.Entity<PhieuXuatHang>(entity =>
        {
            entity.HasKey(e => e.MaPhieuXuatHang).HasName("PK__PhieuXua__0EB2DC0B761CC046");

            entity.ToTable("PhieuXuatHang");

            entity.Property(e => e.HinhThucThanhToan)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NgayXuat).HasColumnType("datetime");
            entity.Property(e => e.PhiVanChuyen)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.PhieuXuatHangs)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuXuat__MaKha__6383C8BA");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.PhieuXuatHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuXuat__MaNgu__5DCAEF64");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__SanPham__FAC7442DFAA9D3E2");

            entity.ToTable("SanPham");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Mota).HasMaxLength(255);
            entity.Property(e => e.TenSanPham).HasMaxLength(255);
            entity.Property(e => e.XuatXu).HasMaxLength(255);

            entity.HasOne(d => d.MaHangSanXuatNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaHangSanXuat)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaHangS__5AEE82B9");

            entity.HasOne(d => d.MaLoaiSanPhamNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaLoaiSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaLoaiS__59FA5E80");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
