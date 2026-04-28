namespace PartsPasal.Application.DTOs.System;

public class LowStockPartDto
{
    public int PartId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public int MinStockThreshold { get; set; }
}

public class LowStockCheckResultDto
{
    public int Count { get; set; }
    public List<LowStockPartDto> Parts { get; set; } = new();
}