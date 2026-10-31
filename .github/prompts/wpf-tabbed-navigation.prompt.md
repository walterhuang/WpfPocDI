---
description: Add ServiceNow-style tabbed navigation (open/switch/close tabs) to a WPF MVVM + DI app
---

# Goal

Turn the main window's single content area into a **tabbed workspace** (like ServiceNow):

- Clicking a menu item opens its page in a **new tab**.
- Clicking a menu item whose page is **already open** switches to that tab instead of opening another one.
- Each tab has a **close (✕) button** next to its title.
- The **Home** tab opens on startup and is **pinned**: it has no ✕ and can't be closed.
- Closing the **selected** tab selects the tab to its left (or Home).
- Reopening a closed page creates a **fresh view model** from DI. Nothing is cached.
- Use the **default light WPF `TabControl`** style unless told otherwise.

# Before you change anything

Read the existing code first. Don't assume anything about it:

1. Open the main window XAML. Find the menu items, their command bindings, and the content host (usually a `ContentControl` bound to something like `CurrentViewModel`). Find the implicit `DataTemplate`s that map view models to views.
2. Open the main view model, the navigation service and its interface, the base view model, the `RelayCommand` class, and the DI registrations in `App.xaml.cs`.
3. Search the whole solution for every use of the current-view-model property, so nothing is left behind.
4. If anything below doesn't match how this codebase works, adapt to the codebase's conventions and tell me what you changed.

# Implementation

1. **`TabItemViewModel`** (new, in the ViewModels folder, extends the existing base view model):
   - `string Header`, `BaseViewModel Content`, `bool IsClosable`
   - `Type ViewModelType => Content.GetType()` (used to find duplicates)
2. **`RelayCommand<T>`**: add it if the project only has a parameterless `RelayCommand`. Use the same `CommandManager.RequerySuggested` pattern.
3. **Main view model**:
   - Replace the single current-view-model property with `ObservableCollection<TabItemViewModel> Tabs` and `TabItemViewModel? SelectedTab`.
   - Add `bool TryActivate(Type viewModelType)`: if a matching tab exists, select it and return true.
   - Add `void OpenTab(string header, BaseViewModel content, bool isClosable)`: add the tab and select it.
   - Add `CloseTabCommand` (`RelayCommand<TabItemViewModel>`). It may only close tabs where `IsClosable` is true, and it selects the left neighbor when the selected tab is closed.
   - Navigation commands pass a header (keep the existing menu icons/emoji). Open Home with `isClosable: false` on startup.
4. **Navigation service and its interface**:
   - Change the signature to `NavigateTo<TViewModel>(string header, bool isClosable = true)`.
   - Call `TryActivate` first. Only resolve the view model from DI when a new tab is needed.
5. **Main window XAML**:
   - Add a `BooleanToVisibilityConverter` resource.
   - Replace the content host with `<TabControl ItemsSource="{Binding Tabs}" SelectedItem="{Binding SelectedTab}">`.
   - `ItemTemplate`: a horizontal `StackPanel` with a `TextBlock` for `Header` and a small flat ✕ `Button`:
	 - `Visibility` bound to `IsClosable` through the converter
	 - `Command="{Binding DataContext.CloseTabCommand, RelativeSource={RelativeSource AncestorType=TabControl}}"`
	 - `CommandParameter="{Binding}"`
   - `ContentTemplate`: `<ContentControl Content="{Binding Content}"/>`, so the existing implicit `DataTemplate`s still resolve the views.
   - Keep the existing menu, styles, and view `DataTemplate`s as they are.

# Verify

1. Build the solution and fix any errors.
2. Confirm no references to the old current-view-model property are left.
3. Give me a short summary of the changed and new files, plus a manual test checklist:
   - Home opens on startup with no ✕.
   - Each menu item opens one tab, and choosing it again switches to that tab.
   - ✕ closes the tab, and the left neighbor gets selected.
   - Reopening a closed page shows a fresh instance.
