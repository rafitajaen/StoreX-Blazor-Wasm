# StoreX - Store Management made simple

## Getting Started

### Login or Register

StoreX is a multitenant application. This means that all users of the application share a single database between them.

This database is divided into fractions, which we can commonly call `Tenants`.

Each `Tenant` will have an administrator. This administrator will be in charge of granting `read`, `write`, `update` and `delete` permissions to the users.

A user can only access the data belonging to his tenant, through `Authentication` and `Permissions`.

In case you need to consult the implementation about Users Authentication:

- `/login` and `/users/self-register` in [src/Client/Pages/Authentication](https://github.com/rafitajaen/StoreX-Blazor-Wasm/tree/storex/src/Client/Pages/Authentication)

In order to provide a demo of the application, you can find a button called "**FILL ADMINISTRATOR CREDENTIALS**" in `/login` page, which automatically fills the login form with the credentials of the application's super administrator.

### Homepage and Sections

Once the user is authenticated, the sections of the application will be displayed in the drawer (on the left side) depending on the permissions that each specific user has been assigned.

The app has four main sections:

- **Start**
  - Home
  - Dashboard
- **Alerts**
  - Stock Alerts
- **Store Management**
  - Suppliers
  - Warehouse
  - Customers
- **Administration**
  - Logs
  - Users
  - Roles
  - Tenants

The **Start** section is related to the user's landing in the application. In **`Home`** page you can find a quick guide to start using the app, and other useful links about the project and the authors. You will also find the **`/dashboard`** page, where you can check the status of your database and the number of available entities at a glance.

The **Alerts** section is about products in a state of shortage. With a simple table, the user can quickly see which products in the warehouse need to be re-stocked, depending on the alerts that have been programmed. `Alerts` can be programmed here and on the warehouse page.

The **Store Management** section is where the magic happens. On the **`/suppliers`** page, the user can manage the `Orders` of each supplier. On the **`/warehouse`** page, the user can control the stock of each `Product` and set `Alarms` individually for each product. And finally, on the **`/customers`** page. The user can have control over the `Deliveries` made to each customer's `Project`.

Finally, in the **Administration** section, which is only available to users with an `Admin Role`. Here, you can access the logs, as well as manage users, roles and tenats of the application.
