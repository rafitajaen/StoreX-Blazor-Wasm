using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.Store.Suppliers;

public class SupplierAutocomplete : MudAutocomplete<Guid>
{
    [Inject]
    private IStringLocalizer<SupplierAutocomplete> L { get; set; } = default!;
    [Inject]
    private ISuppliersClient SuppliersClient { get; set; } = default!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private List<SupplierDto> _suppliers = new();

    // supply default parameters, but leave the possibility to override them
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = L["Supplier"];
        Variant = Variant.Filled;
        Dense = true;
        Margin = Margin.Dense;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchSuppliers;
        ToStringFunc = GetSupplierName;
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
                () => SuppliersClient.GetAsync(_value), Snackbar) is { } supplier)
        {
            _suppliers.Add(supplier);
            ForceRender(true);
        }
    }

    private async Task<IEnumerable<Guid>> SearchSuppliers(string value)
    {
        var filter = new SearchSuppliersRequest
        {
            PageSize = 10,
            AdvancedSearch = new() { Fields = new[] { "name" }, Keyword = value }
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => SuppliersClient.SearchAsync(filter), Snackbar)
            is PaginationResponseOfSupplierDto response)
        {
            _suppliers = response.Data.ToList();
        }

        return _suppliers.Select(x => x.Id);
    }

    private string GetSupplierName(Guid id) =>
        _suppliers.Find(b => b.Id == id)?.Name ?? string.Empty;

}