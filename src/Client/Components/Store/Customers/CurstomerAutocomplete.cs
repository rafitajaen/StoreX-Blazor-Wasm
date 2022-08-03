using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.Store.Customers;

public class CustomerAutocomplete : MudAutocomplete<Guid>
{
    [Inject]
    private IStringLocalizer<SharedResource> L { get; set; } = default!;
    [Inject]
    private ICustomersClient CustomersClient { get; set; } = default!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private List<CustomerDto> _customers = new();

    // supply default parameters, but leave the possibility to override them
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = L["Customer"];
        Variant = Variant.Filled;
        Dense = true;
        Margin = Margin.Dense;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchCustomers;
        ToStringFunc = GetCustomerName;
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
                () => CustomersClient.GetAsync(_value), Snackbar) is { } customer)
        {
            _customers.Add(customer);
            ForceRender(true);
        }
    }

    private async Task<IEnumerable<Guid>> SearchCustomers(string value)
    {
        var filter = new SearchCustomersRequest
        {
            PageSize = 10,
            AdvancedSearch = new() { Fields = new[] { "name" }, Keyword = value }
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CustomersClient.SearchAsync(filter), Snackbar)
            is PaginationResponseOfCustomerDto response)
        {
            _customers = response.Data.ToList();
        }

        return _customers.Select(x => x.Id);
    }

    private string GetCustomerName(Guid id) =>
        _customers.Find(b => b.Id == id)?.Name ?? string.Empty;

}