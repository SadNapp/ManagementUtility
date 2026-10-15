using ManagementUtility.Core.DTOs.Assets;
using ManagementUtility.Core.Interfaces;

namespace ManagementUtility.WinForms.Forms;

public partial class MainForm : Form
{
    private readonly IAssetService? _assetService;
    private readonly TextBox _searchText = new();
    private readonly DataGridView _grid = new();

    public MainForm() : this(null)
    {
    }

    public MainForm(IAssetService? assetService)
    {
        InitializeComponent();

        _assetService = assetService;

        Text = "Management Utility";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 550);
        Size = new Size(1150, 700);

        BuildForm();

        Shown += async (_, _) => await LoadAssetsAsync();
    }

    private void BuildForm()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            ColumnCount = 1,
            RowCount = 3
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var title = new Label
        {
            Text = "Облік активів будівельної компанії",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            AutoScroll = true,
            FlowDirection = FlowDirection.LeftToRight
        };

        _searchText.Width = 280;
        _searchText.PlaceholderText = "Пошук за номером, назвою, категорією";
        _searchText.Margin = new Padding(3, 7, 8, 3);
        _searchText.KeyDown += async (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await LoadAssetsAsync();
            }
        };

        var searchButton = CreateButton("Пошук");
        searchButton.Click += async (_, _) => await LoadAssetsAsync();

        var refreshButton = CreateButton("Оновити");
        refreshButton.Click += async (_, _) =>
        {
            _searchText.Clear();
            await LoadAssetsAsync();
        };

        var addButton = CreateButton("Додати");
        addButton.Click += async (_, _) => await AddAssetAsync();

        var editButton = CreateButton("Редагувати");
        editButton.Click += async (_, _) => await EditSelectedAssetAsync();

        var deleteButton = CreateButton("Видалити");
        deleteButton.Click += async (_, _) => await DeleteSelectedAssetAsync();

        toolbar.Controls.Add(_searchText);
        toolbar.Controls.Add(searchButton);
        toolbar.Controls.Add(refreshButton);
        toolbar.Controls.Add(addButton);
        toolbar.Controls.Add(editButton);
        toolbar.Controls.Add(deleteButton);

        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AutoGenerateColumns = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = false;
        _grid.RowHeadersVisible = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        AddTextColumn("InventoryNumber", "Інвентарний номер", 120);
        AddTextColumn("Name", "Назва", 150);
        AddTextColumn("Category", "Категорія", 100);
        AddTextColumn("Location", "Локація", 100);
        AddTextColumn("Status", "Статус", 90);
        AddTextColumn("ResponsiblePerson", "Відповідальний", 120);

        var priceColumn = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AssetListItemDto.PurchasePrice),
            HeaderText = "Ціна",
            Name = "PurchasePrice",
            FillWeight = 80,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = "N2"
            }
        };
        _grid.Columns.Add(priceColumn);

        _grid.CellDoubleClick += async (_, e) =>
        {
            if (e.RowIndex >= 0)
            {
                await EditSelectedAssetAsync();
            }
        };

        layout.Controls.Add(title, 0, 0);
        layout.Controls.Add(toolbar, 0, 1);
        layout.Controls.Add(_grid, 0, 2);

        Controls.Add(layout);
    }

    private void AddTextColumn(
        string propertyName,
        string headerText,
        float fillWeight)
    {
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = propertyName,
            HeaderText = headerText,
            Name = propertyName,
            FillWeight = fillWeight
        });
    }

    private static Button CreateButton(string text)
    {
        return new Button
        {
            Text = text,
            AutoSize = true,
            Height = 34,
            Margin = new Padding(3, 5, 3, 3)
        };
    }

    private async Task LoadAssetsAsync()
    {
        if (_assetService is null)
        {
            return;
        }

        try
        {
            var assets = await _assetService.GetAllAsync(
                _searchText.Text,
                CancellationToken.None);

            _grid.DataSource = assets.ToList();
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private async Task AddAssetAsync()
    {
        using var form = new AssetEditForm();

        if (form.ShowDialog(this) != DialogResult.OK || _assetService is null)
        {
            return;
        }

        try
        {
            await _assetService.CreateAsync(
                form.ToCreateRequest(),
                CancellationToken.None);

            await LoadAssetsAsync();
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private async Task EditSelectedAssetAsync()
    {
        if (_assetService is null || GetSelectedAsset() is not { } selectedAsset)
        {
            MessageBox.Show(this, "Спочатку вибери актив у списку.");
            return;
        }

        try
        {
            var asset = await _assetService.GetByIdAsync(
                selectedAsset.Id,
                CancellationToken.None);

            if (asset is null)
            {
                MessageBox.Show(this, "Актив не знайдено.");
                await LoadAssetsAsync();
                return;
            }

            using var form = new AssetEditForm(asset);

            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            await _assetService.UpdateAsync(
                asset.Id,
                form.ToUpdateRequest(),
                CancellationToken.None);

            await LoadAssetsAsync();
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private async Task DeleteSelectedAssetAsync()
    {
        if (_assetService is null || GetSelectedAsset() is not { } asset)
        {
            MessageBox.Show(this, "Спочатку вибери актив у списку.");
            return;
        }

        var result = MessageBox.Show(
            this,
            $"Видалити актив «{asset.Name}»?",
            "Підтвердження",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        try
        {
            await _assetService.DeleteAsync(
                asset.Id,
                CancellationToken.None);

            await LoadAssetsAsync();
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private AssetListItemDto? GetSelectedAsset()
    {
        return _grid.CurrentRow?.DataBoundItem as AssetListItemDto;
    }

    private void ShowError(Exception exception)
    {
        MessageBox.Show(
            this,
            exception.Message,
            "Помилка",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}