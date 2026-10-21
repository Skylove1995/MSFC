using System;

namespace MSFC.Models;

public partial class TbXrayScanLog
{
    public long Id { get; set; }

    public string? BatchCode { get; set; }

    public string? Pid1 { get; set; }
    public string? Pid2 { get; set; }
    public string? Pid3 { get; set; }
    public string? Pid4 { get; set; }
    public string? Pid5 { get; set; }
    public string? Pid6 { get; set; }

    public DateTime? ScanAt { get; set; }

    public string? ClientIp { get; set; }

    public DateTime? PrintedAt { get; set; }
}
