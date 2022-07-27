using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Common;
using FSH.BlazorWebAssembly.Client.Pages.Catalog;
using FSH.WebApi.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.Store.Orders;

public partial class OrdersTable
{
    [Inject]
    protected IOrdersClient OrdersClient { get; set; } = default!;
    [Inject]
    protected ISuppliersClient SuppliersClient { get; set; } = default!;

    protected EntityServerTableContext<OrderDto, Guid, OrderViewModel> Context { get; set; } = default!;

    private EntityTable<OrderDto, Guid, OrderViewModel> _table = default!;




    protected override void OnInitialized()
    {


        Context = new(
            entityName: L["Order"],
            entityNamePlural: L["Orders"],
            entityResource: FSHResource.Orders,
            fields: new()
            {
                new(order => order.Id, L["Id"], "Id"),
                new(order => order.Name, L["Name"], "Name"),
                new(order => order.SupplierName, L["Supplier"], "Supplier.Name"),
                new(order => order.Description, L["Description"], "Description"),
                //new(order => order.BasePrice, L["Base Price"], "BasePrice")
            },
            enableAdvancedSearch: true,
            idFunc: order => order.Id,
            searchFunc: async filter =>
            {
                var productFilter = filter.Adapt<SearchOrdersRequest>();

                productFilter.SupplierId = SearchSupplierId == default ? null : SearchSupplierId;
                //productFilter.MinimumRate = SearchMinimumRate;
                //productFilter.MaximumRate = SearchMaximumRate;

                var result = await OrdersClient.SearchAsync(productFilter);
                return result.Adapt<PaginationResponse<OrderDto>>();
            },
            createFunc: async prod =>
            {
                if (!string.IsNullOrEmpty(prod.ImageInBytes))
                {
                    //prod.Image = new FileUploadRequest() { Data = prod.ImageInBytes, Extension = prod.ImageExtension ?? string.Empty, Name = $"{prod.Name}_{Guid.NewGuid():N}" };
                }

                await OrdersClient.CreateAsync(prod.Adapt<CreateOrderRequest>());
                prod.ImageInBytes = string.Empty;
            },
            updateFunc: async (id, prod) =>
            {
                if (!string.IsNullOrEmpty(prod.ImageInBytes))
                {
                    //prod.DeleteCurrentImage = true;
                    //prod.Image = new FileUploadRequest() { Data = prod.ImageInBytes, Extension = prod.ImageExtension ?? string.Empty, Name = $"{prod.Name}_{Guid.NewGuid():N}" };
                }

                await OrdersClient.UpdateAsync(id, prod.Adapt<UpdateOrderRequest>());
                prod.ImageInBytes = string.Empty;
            },

            deleteFunc: async id => await OrdersClient.DeleteAsync(id));



    }


    // Advanced Search

    [Parameter]
    public EventCallback<Guid> OnSupplierSelected { get; set; }




    private Guid _searchSupplierId;

    private Guid SearchSupplierId
    {
        get => _searchSupplierId;
        set
        {
            _searchSupplierId = value;
            _ = _table.ReloadDataAsync();

            OnSupplierSelected.InvokeAsync(value);
        }
    }



    private decimal _searchMinimumRate;
    private decimal SearchMinimumRate
    {
        get => _searchMinimumRate;
        set
        {
            _searchMinimumRate = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private decimal _searchMaximumRate = 9999;
    private decimal SearchMaximumRate
    {
        get => _searchMaximumRate;
        set
        {
            _searchMaximumRate = value;
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
        //Context.AddEditModal.RequestModel.DeleteCurrentImage = true;
        Context.AddEditModal.ForceRender();
    }
}

public class OrderViewModel : UpdateOrderRequest
{
    public string? ImagePath { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
}