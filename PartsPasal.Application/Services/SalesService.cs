using PartsPasal.Application.DTOs.Sales;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;

namespace PartsPasal.Application.Services;

public class SalesService : ISalesService
{
    private readonly IRepositoryBase<SalesInvoice> _invoiceRepo;
    private readonly IRepositoryBase<SalesInvoiceItem> _itemRepo;
    private readonly IRepositoryBase<VehiclePart> _partRepo;

    public SalesService(
        IRepositoryBase<SalesInvoice> invoiceRepo,
        IRepositoryBase<SalesInvoiceItem> itemRepo,
        IRepositoryBase<VehiclePart> partRepo)
    {
        _invoiceRepo = invoiceRepo;
        _itemRepo = itemRepo;
        _partRepo = partRepo;
    }

    public async Task<SalesInvoiceDto> SellPartsAsync(CreateSaleDto dto)
    {
        var invoice = new SalesInvoice
        {
            CustomerId = dto.CustomerId,
            StaffId = dto.StaffId,
            SaleDate = DateTime.UtcNow,
            IsPaid = true
        };

        decimal totalAmount = 0;

        await _invoiceRepo.AddAsync(invoice);
        await _invoiceRepo.SaveChangesAsync();

        foreach (var item in dto.Items)
        {
            var part = await _partRepo.GetByIdAsync(item.PartId);

            if (part == null)
                throw new Exception("Part not found");

            if (part.StockQuantity < item.Quantity)
                throw new Exception($"Not enough stock for {part.Name}");

            var lineTotal = part.Price * item.Quantity;

            totalAmount += lineTotal;

            part.StockQuantity -= item.Quantity;
            _partRepo.Update(part);

            var invoiceItem = new SalesInvoiceItem
            {
                SalesInvoiceId = invoice.Id,
                VehiclePartId = part.Id,
                Quantity = item.Quantity,
                SalePrice = part.Price
            };

            await _itemRepo.AddAsync(invoiceItem);
        }

        await _partRepo.SaveChangesAsync();
        await _itemRepo.SaveChangesAsync();

        invoice.TotalAmount = totalAmount;

        if (totalAmount > 5000)
        {
            invoice.DiscountAmount = totalAmount * 0.10m;
        }
        else
        {
            invoice.DiscountAmount = 0;
        }

        invoice.FinalAmount = invoice.TotalAmount - invoice.DiscountAmount;

        _invoiceRepo.Update(invoice);
        await _invoiceRepo.SaveChangesAsync();

        return await GetSaleByIdAsync(invoice.Id)
               ?? throw new Exception("Invoice error");
    }

    public async Task<List<SalesInvoiceDto>> GetAllSalesAsync()
    {
        var invoices = await _invoiceRepo.GetAllAsync();

        var result = new List<SalesInvoiceDto>();

        foreach (var inv in invoices)
        {
            var items = await _itemRepo.FindAsync(i => i.SalesInvoiceId == inv.Id);

            result.Add(new SalesInvoiceDto
            {
                Id = inv.Id,
                CustomerId = inv.CustomerId,
                StaffId = inv.StaffId,
                SaleDate = inv.SaleDate,
                TotalAmount = inv.TotalAmount,
                DiscountAmount = inv.DiscountAmount,
                FinalAmount = inv.FinalAmount,
                IsPaid = inv.IsPaid,

                Items = items.Select(i => new SalesInvoiceItemDto
                {
                    PartId = i.VehiclePartId,
                    Quantity = i.Quantity,
                    SalePrice = i.SalePrice
                }).ToList()
            });
        }

        return result;
    }

    public async Task<SalesInvoiceDto?> GetSaleByIdAsync(int id)
    {
        var inv = await _invoiceRepo.GetByIdAsync(id);

        if (inv == null) return null;

        var items = await _itemRepo.FindAsync(i => i.SalesInvoiceId == id);

        return new SalesInvoiceDto
        {
            Id = inv.Id,
            CustomerId = inv.CustomerId,
            StaffId = inv.StaffId,
            SaleDate = inv.SaleDate,
            TotalAmount = inv.TotalAmount,
            DiscountAmount = inv.DiscountAmount,
            FinalAmount = inv.FinalAmount,
            IsPaid = inv.IsPaid,

            Items = items.Select(i => new SalesInvoiceItemDto
            {
                PartId = i.VehiclePartId,
                Quantity = i.Quantity,
                SalePrice = i.SalePrice
            }).ToList()
        };
    }
}