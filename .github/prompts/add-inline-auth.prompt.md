---
description: Add in-app login and permission-based page access (authentication + authorization) to a .NET desktop/MVVM app, with POC data in appsettings.json that can later be swapped for a Web API or SQL.
agent: agent
---

# Add In-App Login and Permission-Based Page Access

## Goal

Add authentication and authorization to this application.

- A **public start page** is visible to everyone without signing in.
- When the user navigates to any **other page**, a **login form is shown inside the same window** (no popup window).
- After sign-in, the user's **access rights are retrieved** and used to decide whether they may open that page.
- A signed-in user without the right sees an **Access Denied** view in place of the page.
- All auth data (users, passwords, roles, access rights) lives in **`appsettings.json` for now**, behind interfaces, so it can be replaced later by a Web API or SQL tables **without touching UI or navigation code**.

## Project-specific inputs (fill in or let the agent discover)

Replace the values below. Anything left as `<...>` must be discovered from the codebase or asked about.

| Input | Value |
|---|---|
| Public start page (no login needed) | `<e.g. HomeViewModel>` |
| Protected pages and the permission each requires | `<e.g. Dashboard -> "Dashboard", Settings -> "Settings">` |
| Navigation entry point | `<e.g. INavigationService.NavigateTo<T>>` |
| Config file for POC data | `appsettings.json` |
| Test users (user / password / roles) | `<e.g. admin/admin123/Admin, alice/alice123/Analyst, bob/bob123/Guest>` |
| Roles -> permissions | `<e.g. Admin: [Dashboard, Settings]; Analyst: [Dashboard]; Guest: []>` |
| Password storage | `<plain text for POC | hashed>` |
| Login placement | `<inside the requested tab/page, then swap to the page | dedicated login page>` |
| Unauthorized behavior | `<inline Access Denied view, menus unchanged | hide menu items>` |
| Sign-out | `<Account menu with Sign out, closing protected tabs | none>` |

## How to work

1. **Explore first.** Read the project file, DI/startup code, navigation code, shell view model and shell view, and existing command/base-class helpers. Do not assume file names. Identify the single place where pages are opened.
2. **Ask before guessing.** If any input above is unknown and it changes the design (login placement, unauthorized behavior, password storage, sign-out), ask the user with concise multiple-choice questions. Do not ask what the codebase already answers.
3. **Plan, then implement.** This spans config, a new auth module, view models, views, navigation, and DI. Create a step-by-step plan, then execute one step at a time, marking progress as you go.
4. **Verify.** Build after the changes, check for warnings, confirm the output folder's config file contains the new section, and re-read the key files for logic errors. State clearly what was not run (UI walkthrough, tests).
5. Make minimal changes to existing code, follow existing conventions, and add no new NuGet packages unless unavoidable. `Microsoft.Extensions.Hosting` already provides `Configure<T>`, `IOptionsMonitor<T>` and logging.

## Required design

### 1. Configuration (POC data)

Add an `Auth` section to `appsettings.json`:

- `Users`: list of `{ UserName, Password, DisplayName, Roles[] }`
- `Roles`: list of `{ Name, Permissions[] }`

Bind it with `services.Configure<AuthOptions>(configuration.GetSection("Auth"))`. Initialize list properties to empty collections (`= []`) so empty JSON arrays do not break binding. Make sure `appsettings.json` is copied to the output directory.

### 2. Auth module (own folder and namespace, e.g. `<App>.Auth`)

Keep it self-contained so it can be lifted into another project.

- `AppUser` record: `UserName`, `DisplayName`, `Roles`.
- `PageAccess` enum: `Allowed`, `LoginRequired`, `Forbidden`.
- `Permissions` constants class: one `const string` per permission.
- Attributes (class-only, not multiple, inherited):
  - `[AllowAnonymous]`: page is public.
  - `[RequirePermission("...")]`: page needs that access right.
- Data-access interfaces (the future swap points):
  - `IUserStore.ValidateCredentialsAsync(userName, password, ct)` returns `AppUser?`.
  - `IAccessRightsStore.GetAccessRightsAsync(user, ct)` returns the set of permissions (case-insensitive).
- POC implementations `ConfigurationUserStore` and `ConfigurationAccessRightsStore` using `IOptionsMonitor<AuthOptions>`:
  - Trim the user name, reject empty input, match user name case-insensitively, password case-sensitively.
  - Access rights are the union of the permissions of the user's roles.
- `IAuthService` / `AuthService` (singleton):
  - `CurrentUser`, `IsAuthenticated`, `event AuthStateChanged`, `LoginAsync`, `Logout`, `HasPermission`, `CheckAccess(Type pageType)`.
  - `LoginAsync`: validate, fetch rights, set session, raise `AuthStateChanged`. A failed login returns `false` and leaves any existing session unchanged.
  - `Logout`: no-op if nobody is signed in; otherwise clear the session and raise the event.
  - Policy in `CheckAccess` (secure by default):
	1. `[AllowAnonymous]` -> `Allowed`
	2. not signed in -> `LoginRequired`
	3. `[RequirePermission]` present and right missing -> `Forbidden`
	4. otherwise -> `Allowed`
  - Log sign-in, failure and sign-out (never log passwords).

### 3. Pages

Mark each page view model with `[AllowAnonymous]` or `[RequirePermission(Permissions.X)]`. Pages without an attribute require sign-in.

### 4. Navigation and tabs

- A tab identifies its page by `PageType`; its displayed `Content` is a separate, settable, change-notifying property.
- The navigation service decides the content of a tab from `CheckAccess`:
  - `Allowed`: the page view model (keep an existing instance so state survives a user switch).
  - `LoginRequired`: a `LoginViewModel` with a prompt such as "Please sign in to open X."
  - `Forbidden`: a new `AccessDeniedViewModel` naming the current user and page, with a "Sign in as a different user" command that swaps that tab's content to a login form (keep the current session until the new sign-in succeeds).
- The navigation service subscribes to `AuthStateChanged` and re-evaluates **every open tab**:
  - Signed out: close every tab that is not `Allowed`, then select the first remaining tab.
  - Signed in: re-evaluate content for all tabs.
- The login view model contains **no navigation logic**. It only calls `LoginAsync`; the tab swaps through `AuthStateChanged`.

### 5. Login and Access Denied UI

- `LoginViewModel`: `Prompt`, `UserName`, `Password`, `ErrorMessage`/`HasError`, `IsBusy`, `LoginCommand` (async). Failure message: "Invalid user name or password." Exceptions: "Sign-in failed: {message}".
- Add an `AsyncRelayCommand` (re-entry guard, `try/finally`, requery on completion) if the project only has synchronous commands.
- `LoginView`: centered, fixed-width form with title, prompt, user name box, password box, error text, and a default "Sign in" button. Focus the user name box on load.
- `AccessDeniedView`: title, message, and the "Sign in as a different user" button.
- Register `ViewModel -> View` data templates in the shell window/resources.
- Shell: an **Account** menu showing "Guest" or the display name, a disabled "Signed in as {name} ({roles})" line, and a **Sign out** item bound to a command enabled only when signed in.

### 6. Dependency injection

Register: `Configure<AuthOptions>`, `IUserStore`, `IAccessRightsStore`, `IAuthService` (singletons), `LoginViewModel` and `AccessDeniedViewModel` (transient). Keep existing registrations.

## Pitfalls to avoid (learned the hard way)

- **UI thread:** `AuthStateChanged` handlers modify UI-bound collections. Do not use `ConfigureAwait(false)` in `LoginAsync`, so the event is raised on the UI thread.
- **PasswordBox:** `Password` cannot be data-bound. Sync it to the view model in code-behind (`PasswordChanged`), and also copy the view model's value into the box on `DataContextChanged`, because a `TabControl` may reuse view instances.
- **Resource lookup:** `StaticResource` inside a `UserControl` resolves during `InitializeComponent`, so it cannot see resources defined on the parent window. Put shared converters (e.g. `BooleanToVisibilityConverter`) in `App.xaml`.
- **Stale diagnostics:** IDE errors can lag right after an edit. Re-read the file and re-run the error check before changing working code.
- **Do not hide menus** unless the user chose that; the default here is to show Access Denied inline.

## Acceptance checklist (report each as verified-by-build, verified-by-reading, or needs-manual-test)

1. Startup shows the public page; Account shows Guest; Sign out is disabled.
2. Opening a protected page signed out shows the login form in that tab; a wrong password shows an error and stays.
3. Signing in with sufficient rights swaps the tab to the page; Account shows the user.
4. Opening a page the user lacks rights for shows Access Denied with no login prompt.
5. "Sign in as a different user" then a user with rights swaps the tab to the page; other tabs stay open.
6. With two login tabs open, signing in from one updates both.
7. Sign out closes protected tabs, selects the public page, and Account shows Guest.
8. A user with no rights who signs in from a protected page sees Access Denied.

## Final message to the user

Keep it short. Include: what was added, a table of test accounts, the manual test checklist, how to swap to a Web API or SQL (implement the two store interfaces, change two DI registrations, delete the configuration stores, `AuthOptions` and the `Auth` section), how to protect a new page (add a permission constant, put `[RequirePermission]` on the view model, grant it to roles), and a warning that plain-text passwords in config are for the POC only.
