using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.Store.Suppliers;

public class OrderAutocomplete : MudAutocomplete<Guid>
{
    [Inject]
    private IStringLocalizer<OrderAutocomplete> L { get; set; } = default!;
    [Inject]
    private IOrdersClient OrdersClient { get; set; } = default!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private List<OrderDto> _orders = new();

    // supply default parameters, but leave the possibility to override them
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = L["Order"];
        Variant = Variant.Filled;
        Dense = true;
        Margin = Margin.Dense;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchOrder;
        ToStringFunc = GetOrderName;
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
                () => OrdersClient.GetAsync(_value), Snackbar) is { } order)
        {
            _orders.Add(order);
            ForceRender(true);
        }
    }

    private async Task<IEnumerable<Guid>> SearchOrder(string value)
    {
        var filter = new SearchOrdersRequest
        {
            PageSize = 10,
            AdvancedSearch = new() { Fields = new[] { "name" }, Keyword = value }
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => OrdersClient.SearchAsync(filter), Snackbar)
            is PaginationResponseOfOrderDto response)
        {
            _orders = response.Data.ToList();
        }

        return _orders.Select(x => x.Id);
    }

    private string GetOrderName(Guid id) =>
        _orders.Find(b => b.Id == id)?.Name ?? string.Empty;

}