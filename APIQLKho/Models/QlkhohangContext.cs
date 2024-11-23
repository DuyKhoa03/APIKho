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

    public virtual DbSet<SanPhamViTri> SanPhamViTris { get; set; }

    public virtual DbSet<Vitri> Vitris { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-QR70HSK\\SQLEXPRESS01;Database=QLKhohang;Trusted_Connection=True;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blog__54379E500F9E200A");

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
                .HasConstraintName("FK__Blog__MaNguoiDun__60A75C0F");
        });

        modelBuilder.Entity<ChiTietKiemKe>(entity =>
        {
            entity.HasKey(e => new { e.MaKiemKe, e.MaSanPham }).HasName("PK__ChiTietK__19BDB75D87FB76F3");

            entity.ToTable("ChiTietKiemKe");

            entity.Property(e => e.Anh).HasMaxLength(255);
            entity.Property(e => e.NguyenNhan).HasColumnType("ntext");

            entity.HasOne(d => d.MaKiemKeNavigation).WithMany(p => p.ChiTietKiemKes)
                .HasForeignKey(d => d.MaKiemKe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietKi__MaKie__66603565");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietKiemKes)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietKi__MaSan__59FA5E80");
        });

        modelBuilder.Entity<ChiTietPhieuNhapHang>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieuNhapHang, e.MaSanPham }).HasName("PK__ChiTiet___D10975E6C16E971B");

            entity.ToTable("ChiTiet_PhieuNhapHang");

            entity.Property(e => e.DonGiaNhap).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Image).HasMaxLength(255);

            entity.HasOne(d => d.MaPhieuNhapHangNavigation).WithMany(p => p.ChiTietPhieuNhapHangs)
                .HasForeignKey(d => d.MaPhieuNhapHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTiet_P__MaPhi__628FA481");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietPhieuNhapHangs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTiet_P__MaSan__5812160E");
        });

        modelBuilder.Entity<ChiTietPhieuXuatHang>(entity =>
        {
            entity.HasKey(e => new { e.MaSanPham, e.MaPhieuXuatHang }).HasName("PK__ChiTiet___5A2C69EDFDD2F682");

            entity.ToTable("ChiTiet_PhieuXuatHang");

            entity.Property(e => e.DonGiaXuat).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.NganHang).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TienMat).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaPhieuXuatHangNavigation).WithMany(p => p.ChiTietPhieuXuatHangs)
                .HasForeignKey(d => d.MaPhieuXuatHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTiet_P__MaPhi__6477ECF3");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietPhieuXuatHangs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTiet_P__MaSan__59063A47");
        });

        modelBuilder.Entity<HangSanXuat>(entity =>
        {
            entity.HasKey(e => e.MaHangSanXuat).HasName("PK__HangSanX__977119FC043EE565");

            entity.ToTable("HangSanXuat");

            entity.Property(e => e.TenHangSanXuat).HasMaxLength(255);
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__KhachHan__88D2F0E5BA6EEEEC");

            entity.ToTable("KhachHang");

            entity.Property(e => e.Diachi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.SoDt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SoDT");
            entity.Property(e => e.TenKhachHang).HasMaxLength(255);

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.MaLoai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhachHang__MaLoa__6754599E");
        });

        modelBuilder.Entity<KiemKe>(entity =>
        {
            entity.HasKey(e => e.MaKiemKe).HasName("PK__KiemKe__D611C31F216549BA");

            entity.ToTable("KiemKe");

            entity.Property(e => e.NgayKiemKe).HasColumnType("datetime");

            entity.HasOne(d => d.MaNhanVienKhoNavigation).WithMany(p => p.KiemKes)
                .HasForeignKey(d => d.MaNhanVienKho)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KiemKe__MaNhanVi__5DCAEF64");
        });

        modelBuilder.Entity<LoaiKhacHang>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK__LoaiKhac__730A57595AB58F7C");

            entity.ToTable("LoaiKhacHang");

            entity.Property(e => e.ChiPhiVanChuyen).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ChietKhauXuatHang).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TenLoai).HasMaxLength(255);
        });

        modelBuilder.Entity<LoaiSanPham>(entity =>
        {
            entity.HasKey(e => e.MaLoaiSanPham).HasName("PK__LoaiSanP__ECCF699FAAF43E9D");

            entity.ToTable("LoaiSanPham");

            entity.Property(e => e.TenLoaiSanPham).HasMaxLength(255);
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__Menu__C99ED2505230335B");

            entity.ToTable("Menu");

            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.Link).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.Menus)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Menu__MaNguoiDun__619B8048");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D762A281AD3E");

            entity.ToTable("NguoiDung");

            entity.Property(e => e.Anh).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.NgayDk)
                .HasColumnType("datetime")
                .HasColumnName("NgayDK");
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SDT");
            entity.Property(e => e.TenDangNhap).HasMaxLength(255);
            entity.Property(e => e.TenNguoiDung).HasMaxLength(255);
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNhaCungCap).HasName("PK__NhaCungC__53DA9205D8E471D9");

            entity.ToTable("NhaCungCap");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SDT");
            entity.Property(e => e.TenNhaCungCap).HasMaxLength(255);
        });

        modelBuilder.Entity<NhanVienKho>(entity =>
        {
            entity.HasKey(e => e.MaNhanVienKho).HasName("PK__NhanVien__E62587A5155EFADC");

            entity.ToTable("NhanVienKho");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Hinhanh).HasMaxLength(255);
            entity.Property(e => e.NamSinh).HasColumnType("datetime");
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SDT");
            entity.Property(e => e.TenNhanVien).HasMaxLength(255);
        });

        modelBuilder.Entity<PhieuNhapHang>(entity =>
        {
            entity.HasKey(e => e.MaPhieuNhapHang).HasName("PK__PhieuNha__1EA501A453634D33");

            entity.ToTable("PhieuNhapHang");

            entity.Property(e => e.NgayNhap).HasColumnType("datetime");
            entity.Property(e => e.PhiVanChuyen).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.PhieuNhapHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuNhap__MaNgu__5EBF139D");

            entity.HasOne(d => d.MaNhaCungCapNavigation).WithMany(p => p.PhieuNhapHangs)
                .HasForeignKey(d => d.MaNhaCungCap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuNhap__MaNha__6383C8BA");
        });

        modelBuilder.Entity<PhieuXuatHang>(entity =>
        {
            entity.HasKey(e => e.MaPhieuXuatHang).HasName("PK__PhieuXua__0EB2DC0B82D7138D");

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
                .HasConstraintName("FK__PhieuXuat__MaKha__656C112C");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.PhieuXuatHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuXuat__MaNgu__5FB337D6");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__SanPham__FAC7442D3B1AAB87");

            entity.ToTable("SanPham");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.KichThuoc).HasMaxLength(255);
            entity.Property(e => e.MaVach).HasMaxLength(4000);
            entity.Property(e => e.Mota).HasMaxLength(255);
            entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.TenSanPham).HasMaxLength(255);
            entity.Property(e => e.XuatXu).HasMaxLength(255);

            entity.HasOne(d => d.MaHangSanXuatNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaHangSanXuat)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaHangS__5CD6CB2B");

            entity.HasOne(d => d.MaLoaiSanPhamNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaLoaiSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaLoaiS__5BE2A6F2");
        });

        modelBuilder.Entity<SanPhamViTri>(entity =>
        {
            entity.HasKey(e => new { e.MaViTri, e.MaSanPham }).HasName("PK__SanPham___7F27503DE18F29C4");

            entity.ToTable("SanPham_ViTri");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.SanPhamViTris)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham_V__MaSan__5AEE82B9");

            entity.HasOne(d => d.MaViTriNavigation).WithMany(p => p.SanPhamViTris)
                .HasForeignKey(d => d.MaViTri)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham_V__MaViT__68487DD7");
        });

        modelBuilder.Entity<Vitri>(entity =>
        {
            entity.HasKey(e => e.MaViTri).HasName("PK__Vitri__B08B247FAA10E871");

            entity.ToTable("Vitri");

            entity.Property(e => e.Ke).HasMaxLength(255);
            entity.Property(e => e.KhuVuc).HasMaxLength(255);
            entity.Property(e => e.Mota).HasColumnType("ntext");
            entity.Property(e => e.Tang).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
