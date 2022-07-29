using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.Store.Orders;

public class ProductAutocomplete : MudAutocomplete<Guid>
{
    [Inject]
    private IStringLocalizer<ProductAutocomplete> L { get; set; } = default!;
    [Inject]
    private IStoreProductsClient StoreProductsClient { get; set; } = default!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private List<StoreProductDto> _products = new();

    // supply default parameters, but leave the possibility to override them
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = L["Product"];
        Variant = Variant.Filled;
        Dense = true;
        Margin = Margin.Dense;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchProduct;
        ToStringFunc = GetProductName;
        Clearable = true;
        return base.SetParametersAsync(parameters);
    }

    // when the value parameter is set, we have to load that one brand to be able to show the name
    // we can't do that in OnInitialized because of a strange bug (https://github.com/MudBlazor/MudBlazor/issues/3818)
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender &&
            _value != default &&
            await ApiHelper.ExecuteCallGuardedAsync(
                () => StoreProductsClient.GetAsync(_value), Snackbar) is { } product)
        {
            _products.Add(product);
            ForceRender(true);
        }
    }

    private async Task<IEnumerable<Guid>> SearchProduct(string value)
    {
        var filter = new SearchStoreProductsRequest
        {
            PageSize = 10,
            AdvancedSearch = new() { Fields = new[] { "name" }, Keyword = value }
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => StoreProductsClient.SearchAsync(filter), Snackbar)
            is PaginationResponseOfStoreProductDto response)
        {
            _products = response.Data.ToList();
        }

        return _products.Select(x => x.Id);
    }

    private string GetProductName(Guid id) =>
        _products.Find(b => b.Id == id)?.Name ?? string.Empty;

}