using PartsPasal.Application.DTOs.Sales;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;

namespace PartsPasal.Application.Services;

public class SalesService : ISalesService
{
    private readonly IRepositoryBase<SalesInvoice> _invoiceRepo;
    private readonly IRepositoryBase<SalesInvoiceItem> _itemRepo;
    private readonly IRepositoryBase<VehiclePart> _partRepo;
    private readonly IRepositoryBase<User> _userRepo;

    public SalesService(
        IRepositoryBase<SalesInvoice> invoiceRepo,
        IRepositoryBase<SalesInvoiceItem> itemRepo,
        IRepositoryBase<VehiclePart> partRepo,
        IRepositoryBase<User> userRepo)
    {
        _invoiceRepo = invoiceRepo;
        _itemRepo = itemRepo;
        _partRepo = partRepo;
        _userRepo = userRepo;
    }

    public async Task<SalesInvoiceDto> SellPartsAsync(CreateSaleDto dto)
    {
        // Loyalty program: discount is applied if the customer is loyal.
        var customer = await _userRepo.GetByIdAsync(dto.CustomerId);
        if (customer == null)
            throw new Exception("Customer not found");

        var invoice = new SalesInvoice
        {
            CustomerId = dto.CustomerId,
            StaffId = dto.StaffId,
            SaleDate = DateTime.UtcNow,
            IsPaid = dto.IsPaid ?? true
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

        // Apply loyalty discount when invoice is created.
        // Eligible if customer has already crossed threshold,
        // or if this paid invoice pushes them over the threshold.
        var isLoyalCustomer = customer.TotalServiceSpent > 5000m ||
                              (invoice.IsPaid && (customer.TotalServiceSpent + invoice.TotalAmount) > 5000m);

        // Applying loyalty discount (10%).
        invoice.DiscountAmount = isLoyalCustomer ? invoice.TotalAmount * 0.10m : 0m;

        invoice.FinalAmount = invoice.TotalAmount - invoice.DiscountAmount;

        _invoiceRepo.Update(invoice);
        await _invoiceRepo.SaveChangesAsync();

        // Updating customer lifetime spent amount (used to determine loyalty for future purchases).
        // Only counting paid invoices as "spent"
        if (invoice.IsPaid)
        {
            customer.TotalServiceSpent += invoice.FinalAmount;
            _userRepo.Update(customer);
            await _userRepo.SaveChangesAsync();
        }

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

            var invoiceDto = new SalesInvoiceDto
            {
                Id = inv.Id,
                CustomerId = inv.CustomerId,
                StaffId = inv.StaffId,
                SaleDate = inv.SaleDate,
                TotalAmount = inv.TotalAmount,
                DiscountAmount = inv.DiscountAmount,
                FinalAmount = inv.FinalAmount,
                IsPaid = inv.IsPaid
            };

            foreach (var item in items)
            {
                var part = await _partRepo.GetByIdAsync(item.VehiclePartId);

                invoiceDto.Items.Add(new SalesInvoiceItemDto
                {
                    PartId = item.VehiclePartId,
                    PartName = part?.Name ?? string.Empty,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                });
            }

            result.Add(invoiceDto);
        }

        return result;
    }

    public async Task<SalesInvoiceDto?> GetSaleByIdAsync(int id)
    {
        var inv = await _invoiceRepo.GetByIdAsync(id);

        if (inv == null) return null;

        var items = await _itemRepo.FindAsync(i => i.SalesInvoiceId == id);

        var invoiceDto = new SalesInvoiceDto
        {
            Id = inv.Id,
            CustomerId = inv.CustomerId,
            StaffId = inv.StaffId,
            SaleDate = inv.SaleDate,
            TotalAmount = inv.TotalAmount,
            DiscountAmount = inv.DiscountAmount,
            FinalAmount = inv.FinalAmount,
            IsPaid = inv.IsPaid
        };

        foreach (var item in items)
        {
            var part = await _partRepo.GetByIdAsync(item.VehiclePartId);

            invoiceDto.Items.Add(new SalesInvoiceItemDto
            {
                PartId = item.VehiclePartId,
                PartName = part?.Name ?? string.Empty,
                Quantity = item.Quantity,
                SalePrice = item.SalePrice
            });
        }

        return invoiceDto;
    }
}