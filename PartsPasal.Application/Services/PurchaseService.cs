
using PartsPasal.Application.DTOs.Purchase;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Entities;
using PartsPasal.Domain.Enums;

namespace PartsPasal.Application.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IRepositoryBase<PurchaseInvoice> _purchaseRepository;
    private readonly IRepositoryBase<PurchaseInvoiceItem> _purchaseItemRepository;
    private readonly IRepositoryBase<Vendor> _vendorRepository;
    private readonly IRepositoryBase<VehiclePart> _partRepository;

    public PurchaseService(
        IRepositoryBase<PurchaseInvoice> purchaseRepository,
        IRepositoryBase<PurchaseInvoiceItem> purchaseItemRepository,
        IRepositoryBase<Vendor> vendorRepository,
        IRepositoryBase<VehiclePart> partRepository)
    {
        _purchaseRepository = purchaseRepository;
        _purchaseItemRepository = purchaseItemRepository;
        _vendorRepository = vendorRepository;
        _partRepository = partRepository;
    }

    public async Task<int?> CreatePurchaseAsync(CreatePurchaseDto dto)
    {
        // 1. Validate vendor
        var vendor = await _vendorRepository.GetByIdAsync(dto.VendorId);
        if (vendor == null) return null;

        // 2. Validate items
        if (dto.Items == null || !dto.Items.Any())
            return null;

        decimal totalAmount = 0;

        // 3. Create invoice
        var purchase = new PurchaseInvoice
        {
            VendorId = dto.VendorId,
            PurchaseDate = DateTime.UtcNow,
            Status = InvoiceStatus.Completed
        };

        await _purchaseRepository.AddAsync(purchase);
        await _purchaseRepository.SaveChangesAsync(); // needed to get Id

        // 4. Process each item
        foreach (var item in dto.Items)
        {
            // Validate part
            var part = await _partRepository.GetByIdAsync(item.VehiclePartId);
            if (part == null) return null;

            if (item.Quantity <= 0 || item.UnitPrice < 0)
                return null;

            // Create purchase item
            var purchaseItem = new PurchaseInvoiceItem
            {
                PurchaseInvoiceId = purchase.Id,
                VehiclePartId = item.VehiclePartId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };

            await _purchaseItemRepository.AddAsync(purchaseItem);

            // 🔥 IMPORTANT: Increase stock
            part.StockQuantity += item.Quantity;
            _partRepository.Update(part);

            // Calculate total
            totalAmount += item.Quantity * item.UnitPrice;
        }

        // 5. Update total
        purchase.TotalAmount = totalAmount;
        _purchaseRepository.Update(purchase);

        // 6. Save everything
        await _purchaseItemRepository.SaveChangesAsync();

        return purchase.Id;
    }

    public async Task<List<PurchaseDto>> GetAllPurchasesAsync()
    {
        var purchases = await _purchaseRepository.GetAllAsync();
        var items = await _purchaseItemRepository.GetAllAsync();

        return purchases.Select(p => new PurchaseDto
        {
            Id = p.Id,
            VendorId = p.VendorId,
            PurchaseDate = p.PurchaseDate,
            TotalAmount = p.TotalAmount,
            Status = p.Status.ToString(),
            Items = items
                .Where(i => i.PurchaseInvoiceId == p.Id)
                .Select(i => new PurchaseItemDto
                {
                    VehiclePartId = i.VehiclePartId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
        }).ToList();
    }

    public async Task<PurchaseDto?> GetPurchaseByIdAsync(int id)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(id);
        if (purchase == null) return null;

        var items = await _purchaseItemRepository.FindAsync(i => i.PurchaseInvoiceId == id);

        return new PurchaseDto
        {
            Id = purchase.Id,
            VendorId = purchase.VendorId,
            PurchaseDate = purchase.PurchaseDate,
            TotalAmount = purchase.TotalAmount,
            Status = purchase.Status.ToString(),
            Items = items.Select(i => new PurchaseItemDto
            {
                VehiclePartId = i.VehiclePartId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}