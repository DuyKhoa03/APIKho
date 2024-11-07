using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Position { get; set; }

    public int? Role { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public DateTime? Registerdate { get; set; }

    public DateTime? LastLogin { get; set; }

    public bool? Hide { get; set; }

    public virtual ICollection<EmployeePurchaseOrder> EmployeePurchaseOrders { get; set; } = new List<EmployeePurchaseOrder>();

    public virtual ICollection<EmployerOrder> EmployerOrders { get; set; } = new List<EmployerOrder>();
}
