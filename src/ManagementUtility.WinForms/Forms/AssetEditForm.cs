using ManagementUtility.Core.DTOs.Assets;
using ManagementUtility.Core.Enums;

namespace ManagementUtility.WinForms.Forms;

public partial class AssetEditForm : Form
{
    private readonly TextBox _inventoryNumber = new();
    private readonly TextBox _name = new();
    private readonly TextBox _category = new();
    private readonly TextBox _location = new();
    private readonly TextBox _responsiblePerson = new();
    private readonly ComboBox _status = new();
    private readonly DateTimePicker _purchaseDate = new();
    private readonly CheckBox _hasPurchaseDate = new();
    private readonly NumericUpDown _purchasePrice = new();
    private readonly CheckBox _hasPurchasePrice = new();
    private readonly TextBox _notes = new();

    public AssetEditForm() : this(null)
    {
    }

    public AssetEditForm(AssetListItemDto? asset)
    {
        InitializeComponent();

        Text = asset is null ? "Додати актив" : "Редагувати актив";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(560, 610);

        BuildForm();

        if (asset is not null)
        {
            FillForm(asset);
        }
    }

    public CreateAssetRequest ToCreateRequest()
    {
        return new CreateAssetRequest
        {
            InventoryNumber = _inventoryNumber.Text,
            Name = _name.Text,
            Category = _category.Text,
            Location = _location.Text,
            ResponsiblePerson = _responsiblePerson.Text,
            Status = GetSelectedStatus(),
            PurchaseDate = _hasPurchaseDate.Checked
                ? _purchaseDate.Value
                : null,
            PurchasePrice = _hasPurchasePrice.Checked
                ? _purchasePrice.Value
                : null,
            Notes = _notes.Text
        };
    }

    public UpdateAssetRequest ToUpdateRequest()
    {
        return new UpdateAssetRequest
        {
            InventoryNumber = _inventoryNumber.Text,
            Name = _name.Text,
            Category = _category.Text,
            Location = _location.Text,
            ResponsiblePerson = _responsiblePerson.Text,
            Status = GetSelectedStatus(),
            PurchaseDate = _hasPurchaseDate.Checked
                ? _purchaseDate.Value
                : null,
            PurchasePrice = _hasPurchasePrice.Checked
                ? _purchasePrice.Value
                : null,
            Notes = _notes.Text
        };
    }

    private void BuildForm()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            ColumnCount = 2,
            AutoScroll = true
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _status.DropDownStyle = ComboBoxStyle.DropDownList;
        _status.DataSource = Enum.GetValues<AssetStatus>();

        _purchaseDate.Format = DateTimePickerFormat.Short;
        _purchaseDate.Enabled = false;

        _hasPurchaseDate.Text = "Вказати дату";
        _hasPurchaseDate.AutoSize = true;
        _hasPurchaseDate.CheckedChanged += (_, _) =>
            _purchaseDate.Enabled = _hasPurchaseDate.Checked;

        _purchasePrice.DecimalPlaces = 2;
        _purchasePrice.Maximum = 1_000_000_000;
        _purchasePrice.Enabled = false;

        _hasPurchasePrice.Text = "Вказати ціну";
        _hasPurchasePrice.AutoSize = true;
        _hasPurchasePrice.CheckedChanged += (_, _) =>
            _purchasePrice.Enabled = _hasPurchasePrice.Checked;

        _notes.Multiline = true;
        _notes.ScrollBars = ScrollBars.Vertical;

        AddRow(layout, "Інвентарний номер *", _inventoryNumber);
        AddRow(layout, "Назва *", _name);
        AddRow(layout, "Категорія *", _category);
        AddRow(layout, "Локація", _location);
        AddRow(layout, "Відповідальний", _responsiblePerson);
        AddRow(layout, "Статус", _status);
        AddRow(layout, "Дата придбання", CreateDateInput());
        AddRow(layout, "Ціна придбання", CreatePriceInput());
        AddRow(layout, "Примітки", _notes, 80);

        var saveButton = new Button
        {
            Text = "Зберегти",
            AutoSize = true,
            DialogResult = DialogResult.OK
        };

        var cancelButton = new Button
        {
            Text = "Скасувати",
            AutoSize = true,
            DialogResult = DialogResult.Cancel
        };

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true
        };

        buttons.Controls.Add(cancelButton);
        buttons.Controls.Add(saveButton);

        layout.RowCount++;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.Controls.Add(buttons, 0, layout.RowCount - 1);
        layout.SetColumnSpan(buttons, 2);

        Controls.Add(layout);

        AcceptButton = saveButton;
        CancelButton = cancelButton;
    }

    private Control CreateDateInput()
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = false
        };

        panel.Controls.Add(_hasPurchaseDate);
        panel.Controls.Add(_purchaseDate);

        return panel;
    }

    private Control CreatePriceInput()
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = false
        };

        panel.Controls.Add(_hasPurchasePrice);
        panel.Controls.Add(_purchasePrice);

        return panel;
    }

    private static void AddRow(
        TableLayoutPanel layout,
        string labelText,
        Control control,
        int height = 38)
    {
        layout.RowCount++;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, height));

        var row = layout.RowCount - 1;
        var label = new Label
        {
            Text = labelText,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        control.Dock = control is TextBox { Multiline: true }
            ? DockStyle.Fill
            : DockStyle.Top;

        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(control, 1, row);
    }

    private AssetStatus GetSelectedStatus()
    {
        return _status.SelectedItem is AssetStatus status
            ? status
            : AssetStatus.InUse;
    }

    private void FillForm(AssetListItemDto asset)
    {
        _inventoryNumber.Text = asset.InventoryNumber;
        _name.Text = asset.Name;
        _category.Text = asset.Category;
        _location.Text = asset.Location;
        _responsiblePerson.Text = asset.ResponsiblePerson ?? string.Empty;
        _status.SelectedItem = asset.Status;
        _notes.Text = asset.Notes ?? string.Empty;

        if (asset.PurchaseDate.HasValue)
        {
            _hasPurchaseDate.Checked = true;
            _purchaseDate.Value = asset.PurchaseDate.Value;
        }

        if (asset.PurchasePrice.HasValue)
        {
            _hasPurchasePrice.Checked = true;
            _purchasePrice.Value = asset.PurchasePrice.Value;
        }
    }
}