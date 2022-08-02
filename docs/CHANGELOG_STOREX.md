# StoreX Blazor WebAssembly Changelog

## v0.0.6 - [Complete Customers Page]()

**branch :** _storex_

> This version is compatible with [StoreX-WebAPI v0.0.9](https://github.com/rafitajaen/StoreX-WebAPI/tree/decce606983c5dda81b725afb93097a36171f29d)

    Add:
        - src/Client/Pages:  Customers Page

    Edit:
        - src/Client/Shared: Update NavMenu with new Customers Page
        - src/Shared/Authorization/FSHPermission: Update Permissions with new Models
        - src/Client.Infrastructure/ApiClient: Update API with new Models

## v0.0.5 - [Complete Stock Page](https://github.com/rafitajaen/StoreX-Blazor-Wasm/tree/86ba1becf7fcda29ab58ca4f50234eff78055403)

**branch :** _storex_

> This version is compatible with [StoreX-WebAPI v0.0.6](https://github.com/rafitajaen/StoreX-WebAPI/tree/5d7e5e920b1ec5a1cf18aac20bbd5ecfb5a33878)

    Edit:
        - Project folder re-organization to meet CLEAN architecture standards
        - Warehouse and Suppliers Pages and Components Updated

## v0.0.4 - [Complete Suppliers Page](https://github.com/rafitajaen/StoreX-Blazor-Wasm/tree/878bf9b021fc8d85aa5fe2f0be35d1d6564321a3)

**branch :** _storex_

> This version is compatible with [StoreX-WebAPI v0.0.5](https://github.com/rafitajaen/StoreX-WebAPI/tree/1566be581cc9537c3cc2520a100a1dd59a6be320)

    Edit:
        - Warehouse Page and Components

## v0.0.3 - [Solve rendering side effects in Warehouse Page](https://github.com/rafitajaen/StoreX-Blazor-Wasm/tree/80c75ff804b5149ed846a0a9feff6e0e3a1f5b8c)

**branch :** _storex_

    Edit:
        - src/Client/Pages/Store : Warehouse Page
        - src/Client/Components/Store : All Warehouses Components

## v0.0.2 - [Add a functional Warehouse Page](https://github.com/rafitajaen/StoreX-Blazor-Wasm/tree/601a5349aaa37716c00070904d86f0252972418f)

**branch :** _storex_

    Add:
        - src/Client/Components/Store : Create SimpleOrderTable and SimpleProductTable
        - src/Shared/Authorization/ : Update new Permissions.

    Edit:
        - src/Client/Pages/Store : Warehouse Page for manage Orders
        - src/Client/EntityTable : Add Extension to support SelectedItem
        - Update ApiClient with the API Changes
        - Update NavMenu with new Pages

## v0.0.1 - [Add Supplier Card](https://github.com/rafitajaen/StoreX-Blazor-Wasm/tree/a82838a35a8010d0a17271889773f91410687242)

**branch :** _storex_

    Add:
        - docs/ : Add Readme and changelog
        - Add NavMenuLinks for Orders and Stock

    Edit:
        - src/Shared/Authorization : Update permission with new Entities
        - Update ApiClient with the API Changes
