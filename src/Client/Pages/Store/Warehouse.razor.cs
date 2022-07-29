using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Common;
using FSH.WebApi.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Store;

public partial class Warehouse
{
    [Inject]
    protected IStoreProductsClient StoreProductsClient { get; set; } = default!;
    [Inject]
    protected IBrandsClient BrandsClient { get; set; } = default!;

    protected EntityServerTableContext<StoreProductDto, Guid, StoreProductViewModel> Context { get; set; } = default!;

    private EntityTable<StoreProductDto, Guid, StoreProductViewModel> _table = default!;

    protected override void OnInitialized() =>
        Context = new(
            entityName: L["Product"],
            entityNamePlural: L["Products"],
            entityResource: FSHResource.StoreProducts,
            fields: new()
            {
                new(prod => prod.StockUnits, L["Uds"], "StockUnits", typeof(int)),
                new(prod => prod.UnitType, L["Type"], "UnitType"),
                new(prod => prod.Name, L["Name"], "Name"),
                new(prod => prod.Description, L["Description"], "Description"),
                new(prod => prod.BasePrice, L["Base Price"], "BasePrice", typeof(decimal)),
                new(prod => prod.M2, L["M2"], "M2", typeof(decimal)),
                new(prod => prod.StockAlert, L["Stock Alert"], "StockAlert", typeof(int))
            },
            enableAdvancedSearch: true,
            idFunc: prod => prod.Id,
            searchFunc: async filter =>
            {
                var productFilter = filter.Adapt<SearchStoreProductsRequest>();

                productFilter.MinPrice = SearchMinPrice;
                productFilter.MaxPrice = SearchMaxPrice;

                var result = await StoreProductsClient.SearchAsync(productFilter);
                return result.Adapt<PaginationResponse<StoreProductDto>>();
            },
            createFunc: async prod =>
            {
                if (!string.IsNullOrEmpty(prod.ImageInBytes))
                {
                    prod.Image = new FileUploadRequest() { Data = prod.ImageInBytes, Extension = prod.ImageExtension ?? string.Empty, Name = $"{prod.Name}_{Guid.NewGuid():N}" };
                }

                await StoreProductsClient.CreateAsync(prod.Adapt<CreateStoreProductRequest>());
                prod.ImageInBytes = string.Empty;
            },
            updateFunc: async (id, prod) =>
            {
                if (!string.IsNullOrEmpty(prod.ImageInBytes))
                {
                    prod.DeleteCurrentImage = true;
                    prod.Image = new FileUploadRequest() { Data = prod.ImageInBytes, Extension = prod.ImageExtension ?? string.Empty, Name = $"{prod.Name}_{Guid.NewGuid():N}" };
                }

                await StoreProductsClient.UpdateAsync(id, prod.Adapt<UpdateStoreProductRequest>());
                prod.ImageInBytes = string.Empty;
            },
            exportFunc: async filter =>
            {
                var exportFilter = filter.Adapt<ExportStoreProductsRequest>();

                exportFilter.MinPrice = SearchMinPrice;
                exportFilter.MaxPrice = SearchMaxPrice;

                return await StoreProductsClient.ExportAsync(exportFilter);
            },
            deleteFunc: async id => await StoreProductsClient.DeleteAsync(id));

    // Advanced Search

    private decimal _searchMinPrice;
    private decimal SearchMinPrice
    {
        get => _searchMinPrice;
        set
        {
            _searchMinPrice = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private decimal _searchMaxPrice = 9999;
    private decimal SearchMaxPrice
    {
        get => _searchMaxPrice;
        set
        {
            _searchMaxPrice = value;
            _ = _table.ReloadDataAsync();
        }
    }

    // TODO : Make this as a shared service or something? Since it's used by Profile Component also for now, and literally any other component that will have image upload.
    // The new service should ideally return $"data:{ApplicationConstants.StandardImageFormat};base64,{Convert.ToBase64String(buffer)}"
    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        if (e.File != null)
        {
            string? extension = Path.GetExtension(e.File.Name);
            if (!ApplicationConstants.SupportedImageFormats.Contains(extension.ToLower()))
            {
                Snackbar.Add("Image Format Not Supported.", Severity.Error);
                return;
            }

            Context.AddEditModal.RequestModel.ImageExtension = extension;
            var imageFile = await e.File.RequestImageFileAsync(ApplicationConstants.StandardImageFormat, ApplicationConstants.MaxImageWidth, ApplicationConstants.MaxImageHeight);
            byte[]? buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream(ApplicationConstants.MaxAllowedSize).ReadAsync(buffer);
            Context.AddEditModal.RequestModel.ImageInBytes = $"data:{ApplicationConstants.StandardImageFormat};base64,{Convert.ToBase64String(buffer)}";
            Context.AddEditModal.ForceRender();
        }
    }

    public void ClearImageInBytes()
    {
        Context.AddEditModal.RequestModel.ImageInBytes = string.Empty;
        Context.AddEditModal.ForceRender();
    }

    public void SetDeleteCurrentImageFlag()
    {
        Context.AddEditModal.RequestModel.ImageInBytes = string.Empty;
        Context.AddEditModal.RequestModel.ImagePath = string.Empty;
        Context.AddEditModal.RequestModel.DeleteCurrentImage = true;
        Context.AddEditModal.ForceRender();
    }
}

public class StoreProductViewModel : UpdateStoreProductRequest
{
    public string? ImagePath { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
}