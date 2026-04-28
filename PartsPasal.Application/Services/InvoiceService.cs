using PartsPasal.Application.DTOs.Sales;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;

namespace PartsPasal.Application.Services;

/// <summary>
/// Invoice service for reading and emailing customer invoices.
/// </summary>
public class InvoiceService : IInvoiceService
{
    private readonly IRepositoryBase<SalesInvoice> _invoiceRepo;
    private readonly IRepositoryBase<SalesInvoiceItem> _itemRepo;
    private readonly IRepositoryBase<VehiclePart> _partRepo;
    private readonly IRepositoryBase<User> _userRepo;
    private readonly IEmailService _emailService;

    public InvoiceService(
        IRepositoryBase<SalesInvoice> invoiceRepo,
        IRepositoryBase<SalesInvoiceItem> itemRepo,
        IRepositoryBase<VehiclePart> partRepo,
        IRepositoryBase<User> userRepo,
        IEmailService emailService)
    {
        _invoiceRepo = invoiceRepo;
        _itemRepo = itemRepo;
        _partRepo = partRepo;
        _userRepo = userRepo;
        _emailService = emailService;
    }

    public async Task<SalesInvoiceDto?> GetInvoiceByIdAsync(int id)
    {
        var inv = await _invoiceRepo.GetByIdAsync(id);
        if (inv == null) return null;

        var items = await _itemRepo.FindAsync(i => i.SalesInvoiceId == id);

        var dto = new SalesInvoiceDto
        {
            Id = inv.Id,
            CustomerId = inv.CustomerId,
            StaffId = inv.StaffId,
            SaleDate = inv.SaleDate,
            TotalAmount = inv.TotalAmount,
            DiscountAmount = inv.DiscountAmount,
            FinalAmount = inv.FinalAmount,
            IsPaid = inv.IsPaid,
            Items = new List<SalesInvoiceItemDto>()
        };

        foreach (var item in items)
        {
            var part = await _partRepo.GetByIdAsync(item.VehiclePartId);

            dto.Items.Add(new SalesInvoiceItemDto
            {
                PartId = item.VehiclePartId,
                PartName = part?.Name ?? string.Empty,
                Quantity = item.Quantity,
                SalePrice = item.SalePrice
            });
        }

        return dto;
    }

    public async Task<List<SalesInvoiceDto>> GetInvoicesByCustomerIdAsync(int customerId)
    {
        var invoices = await _invoiceRepo.FindAsync(i => i.CustomerId == customerId);

        var result = new List<SalesInvoiceDto>();
        foreach (var inv in invoices.OrderByDescending(i => i.SaleDate))
        {
            var dto = await GetInvoiceByIdAsync(inv.Id);
            if (dto != null)
                result.Add(dto);
        }

        return result;
    }

    public async Task<bool> SendInvoiceEmailAsync(int invoiceId)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);
        if (invoice == null) return false;

        var customer = await _userRepo.GetByIdAsync(invoice.CustomerId);
        var customerEmail = customer?.Email;

        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new Exception("Customer email not found.");

        await _emailService.SendInvoiceEmailAsync(customerEmail, invoiceId);
        return true;
    }
}