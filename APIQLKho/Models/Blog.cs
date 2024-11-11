using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace APIQLKho.Models;

public partial class Blog
{
    public int BlogId { get; set; }

    public string? Anh { get; set; }

    public string? Mota { get; set; }

    public string? Link { get; set; }

    public bool? Hide { get; set; }

    public int MaNguoiDung { get; set; }
    [JsonIgnore] // Bỏ qua thuộc tính này khi chuyển đổi từ JSON
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
