using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class Slider
{
    public int SliderId { get; set; }

    public string? Name { get; set; }

    public string? Image { get; set; }

    public int? Order { get; set; }

    public string? Link { get; set; }

    public bool? Hide { get; set; }
}
