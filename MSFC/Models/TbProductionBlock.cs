using System;

namespace MSFC.Models;

public partial class TbProductionBlock
{
    public string Pid { get; set; } = null!;

    public string? WoName { get; set; }

    public string? McId { get; set; }

    public string? McType { get; set; }

    public string? Reason { get; set; }

    public string? UserId { get; set; }

    /// <summary>
    /// 1 = Blocked (Chặn), 0 = Released (QA mở khóa)
    /// </summary>
    public sbyte Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
