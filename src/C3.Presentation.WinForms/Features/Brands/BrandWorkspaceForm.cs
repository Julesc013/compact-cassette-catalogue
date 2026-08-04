using C3.Catalogue.Brands;
using C3.Presentation.WinForms.Interaction;
using C3.Presentation.WinForms.Workspace;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace C3.Presentation.WinForms.Features.Brands
{
    public partial class BrandWorkspaceForm : Form
    {
        private BrandWorkspacePresenter presenter;
        private WorkspaceController workspace;
        private bool beginCreateOnLoad;
        private bool rendering;

        /// <summary>Required by the Visual Studio WinForms designer.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public BrandWorkspaceForm()
        {
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;
            headingLabel.Font = new Font(
                Font.FontFamily,
                Font.SizeInPoints + 6F,
                FontStyle.Bold,
                GraphicsUnit.Point);
            listHeadingLabel.Font = new Font(Font, FontStyle.Bold);
            inspectorHeadingLabel.Font = new Font(Font, FontStyle.Bold);
            WireEvents();
        }

        public BrandWorkspaceForm(
            BrandService service,
            WorkspaceController workspace,
            bool beginCreate)
            : this()
        {
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            presenter = new BrandWorkspacePresenter(
                service ?? throw new ArgumentNullException(nameof(service)),
                workspace);
            presenter.StateChanged += PresenterStateChanged;
            beginCreateOnLoad = beginCreate;
        }

        public event EventHandler CatalogueChanged;

        public void BeginCreate()
        {
            RequireRuntimeComposition();
            if (!TryCancelEditor())
            {
                return;
            }

            presenter.BeginCreate();
            RenderState();
            brandNameTextBox.Focus();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            RequireRuntimeComposition();
            presenter.Refresh(string.Empty);
            if (beginCreateOnLoad)
            {
                beginCreateOnLoad = false;
                BeginCreate();
            }
            else
            {
                RenderState();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!DesignMode && presenter != null && !TryCancelEditor())
            {
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keyData)
        {
            if (presenter == null)
            {
                return base.ProcessCmdKey(ref message, keyData);
            }

            if (keyData == (Keys.Control | Keys.N))
            {
                BeginCreate();
                return true;
            }

            if (keyData == (Keys.Control | Keys.Z) && presenter.Editor == null)
            {
                Undo();
                return true;
            }

            if (keyData == (Keys.Control | Keys.Y) && presenter.Editor == null)
            {
                Redo();
                return true;
            }

            if (keyData == Keys.Delete && presenter.Editor == null)
            {
                DeleteSelected();
                return true;
            }

            if (keyData == Keys.F5)
            {
                ApplyFilter();
                return true;
            }

            if (keyData == (Keys.Control | Keys.F))
            {
                filterTextBox.Focus();
                filterTextBox.SelectAll();
                return true;
            }

            if (keyData == Keys.Escape && presenter.Editor != null)
            {
                TryCancelEditor();
                return true;
            }

            return base.ProcessCmdKey(ref message, keyData);
        }

        private void WireEvents()
        {
            applyFilterButton.Click += (sender, arguments) => ApplyFilter();
            clearFilterButton.Click += (sender, arguments) => ClearFilter();
            newButton.Click += (sender, arguments) => BeginCreate();
            editButton.Click += (sender, arguments) => BeginEdit();
            deleteButton.Click += (sender, arguments) => DeleteSelected();
            undoButton.Click += (sender, arguments) => Undo();
            redoButton.Click += (sender, arguments) => Redo();
            applyButton.Click += (sender, arguments) => ApplyEditor();
            cancelButton.Click += (sender, arguments) => TryCancelEditor();
            brandListView.SelectedIndexChanged += BrandSelectionChanged;
            brandListView.DoubleClick += (sender, arguments) => BeginEdit();
            brandListView.KeyDown += BrandListKeyDown;
            brandListView.SizeChanged += (sender, arguments) => ResizeBrandColumns();
            brandNameTextBox.TextChanged += EditorTextChanged;
            brandCodeTextBox.TextChanged += EditorTextChanged;
            brandNotesTextBox.TextChanged += EditorTextChanged;
        }

        private void ApplyFilter()
        {
            RequireRuntimeComposition();
            presenter.Refresh(filterTextBox.Text);
            brandListView.Focus();
        }

        private void ClearFilter()
        {
            filterTextBox.Clear();
            ApplyFilter();
        }

        private void BrandSelectionChanged(object sender, EventArgs e)
        {
            if (rendering || presenter == null)
            {
                return;
            }

            presenter.Select(brandListView.SelectedItems.Cast<ListViewItem>().Select(
                item => Convert.ToString(item.Tag, CultureInfo.InvariantCulture)));
        }

        private void BrandListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && presenter != null && presenter.CanEdit)
            {
                BeginEdit();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void EditorTextChanged(object sender, EventArgs e)
        {
            if (rendering || presenter == null || presenter.Editor == null)
            {
                return;
            }

            presenter.UpdateDraft(
                brandNameTextBox.Text,
                brandCodeTextBox.Text,
                brandNotesTextBox.Text);
            ClearValidationPresentation();
            documentStatusLabel.Text = "Editor has unapplied changes";
        }

        private void BeginEdit()
        {
            if (!TryCancelEditor() || !presenter.BeginEdit())
            {
                return;
            }

            RenderState();
            brandNameTextBox.Focus();
            brandNameTextBox.SelectAll();
        }

        private void ApplyEditor()
        {
            if (presenter.Apply())
            {
                RaiseCatalogueChanged();
            }

            RenderState();
        }

        private void DeleteSelected()
        {
            if (presenter == null || !presenter.CanDelete)
            {
                return;
            }

            var selectedCount = presenter.SelectedCount;
            var selectionDescription = selectedCount == 1 && presenter.Inspector.HasSelection
                ? presenter.Inspector.Value.Name + " (" + presenter.Inspector.Value.Code + ")"
                : selectedCount.ToString(CultureInfo.CurrentCulture) + " selected brands";
            var decision = MessageBox.Show(
                this,
                "Delete " + selectionDescription + "?" +
                    Environment.NewLine + Environment.NewLine +
                    "The catalogue is not saved immediately. You can use Undo to restore the brand.",
                "Delete brand",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (decision != DialogResult.Yes)
            {
                return;
            }

            if (presenter.DeleteSelected())
            {
                RaiseCatalogueChanged();
            }

            RenderState();
        }

        private void Undo()
        {
            if (presenter.CanUndo && presenter.Undo())
            {
                RaiseCatalogueChanged();
            }

            RenderState();
        }

        private void Redo()
        {
            if (presenter.CanRedo && presenter.Redo())
            {
                RaiseCatalogueChanged();
            }

            RenderState();
        }

        private bool TryCancelEditor()
        {
            if (presenter == null || presenter.Editor == null)
            {
                return true;
            }

            if (workspace.State.EditorDraft.IsDirty)
            {
                var decision = MessageBox.Show(
                    this,
                    "Discard the unapplied changes in this Brand editor?",
                    "Discard Brand changes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
                if (decision != DialogResult.Yes)
                {
                    return false;
                }
            }

            presenter.CancelEditor();
            RenderState();
            return true;
        }

        private void PresenterStateChanged(object sender, EventArgs e)
        {
            RenderState();
        }

        private void RenderState()
        {
            if (presenter == null || rendering)
            {
                return;
            }

            rendering = true;
            try
            {
                filterTextBox.Text = presenter.FilterText;
                RenderList();
                RenderInspector();
                RenderCommands();
                RenderFeedback();
            }
            finally
            {
                rendering = false;
            }
        }

        private void RenderList()
        {
            brandListView.BeginUpdate();
            try
            {
                brandListView.Items.Clear();
                foreach (var brand in presenter.List.Items)
                {
                    var item = new ListViewItem(brand.Code) { Tag = brand.Code };
                    item.SubItems.Add(brand.Name);
                    item.SubItems.Add(brand.Notes);
                    brandListView.Items.Add(item);
                    if (workspace.State.Selection.Feature == BrandWorkspacePresenter.FeatureKey &&
                        workspace.State.Selection.SelectedIds.Any(code => string.Equals(
                            code,
                            brand.Code,
                            StringComparison.OrdinalIgnoreCase)))
                    {
                        item.Selected = true;
                        item.Focused = true;
                    }
                }
            }
            finally
            {
                brandListView.EndUpdate();
            }

            var empty = presenter.List.EmptyState;
            emptyStateLabel.Visible = empty.IsVisible;
            emptyStateLabel.Text = empty.IsVisible
                ? empty.Title + ". " + empty.Guidance
                : string.Empty;
            countStatusLabel.Text = presenter.List.Count == 1
                ? "1 brand"
                : presenter.List.Count.ToString(CultureInfo.CurrentCulture) + " brands";
            ResizeBrandColumns();
        }

        private void ResizeBrandColumns()
        {
            var available = brandListView.ClientSize.Width - 4;
            if (available < 300)
            {
                return;
            }

            codeColumn.Width = Math.Max(54, available * 15 / 100);
            nameColumn.Width = Math.Max(120, available * 32 / 100);
            notesColumn.Width = Math.Max(120, available - codeColumn.Width - nameColumn.Width);
        }

        private void RenderInspector()
        {
            var editing = presenter.Editor != null;
            if (editing)
            {
                inspectorHeadingLabel.Text = presenter.Editor.IsNew ? "New brand" : "Edit brand";
                brandNameTextBox.Text = presenter.Editor.Name;
                brandCodeTextBox.Text = presenter.Editor.Code;
                brandNotesTextBox.Text = presenter.Editor.Notes;
                addedAtValueLabel.Text = presenter.Editor.IsNew
                    ? "Assigned when applied"
                    : presenter.Inspector.HasSelection
                        ? presenter.Inspector.Value.AddedAt.ToString("g", CultureInfo.CurrentCulture)
                        : "-";
            }
            else if (presenter.Inspector.HasSelection)
            {
                var value = presenter.Inspector.Value;
                inspectorHeadingLabel.Text = "Brand details";
                brandNameTextBox.Text = value.Name;
                brandCodeTextBox.Text = value.Code;
                brandNotesTextBox.Text = value.Notes;
                addedAtValueLabel.Text = value.AddedAt.ToString("g", CultureInfo.CurrentCulture);
            }
            else
            {
                inspectorHeadingLabel.Text = "Inspector";
                brandNameTextBox.Clear();
                brandCodeTextBox.Clear();
                brandNotesTextBox.Clear();
                addedAtValueLabel.Text = presenter.SelectedCount > 1
                    ? presenter.SelectedCount.ToString(CultureInfo.CurrentCulture) + " brands selected"
                    : "Select a brand to inspect it";
            }

            brandNameTextBox.ReadOnly = !editing;
            brandCodeTextBox.ReadOnly = !editing || !presenter.Editor.IsNew;
            brandNotesTextBox.ReadOnly = !editing;
            applyButton.Visible = editing;
            cancelButton.Visible = editing;
            validationLabel.Text = presenter.Validation.Summary;
            errorProvider.SetError(
                brandNameTextBox,
                presenter.Validation.ForField(BrandWorkspacePresenter.NameField));
            errorProvider.SetError(
                brandCodeTextBox,
                presenter.Validation.ForField(BrandWorkspacePresenter.CodeField));
            errorProvider.SetError(
                brandNotesTextBox,
                presenter.Validation.ForField(BrandWorkspacePresenter.NotesField));
            AcceptButton = editing ? applyButton : applyFilterButton;
        }

        private void RenderCommands()
        {
            var editing = presenter.Editor != null;
            newButton.Enabled = !editing;
            editButton.Enabled = !editing && presenter.CanEdit;
            deleteButton.Enabled = !editing && presenter.CanDelete;
            undoButton.Enabled = !editing && presenter.CanUndo;
            redoButton.Enabled = !editing && presenter.CanRedo;
            undoButton.Text = presenter.CanUndo
                ? "&Undo " + presenter.UndoDescription
                : "&Undo";
            redoButton.Text = presenter.CanRedo
                ? "&Redo " + presenter.RedoDescription
                : "&Redo";
            undoButton.AccessibleDescription = presenter.CanUndo
                ? presenter.UndoDescription
                : "There is no command to undo.";
            redoButton.AccessibleDescription = presenter.CanRedo
                ? presenter.RedoDescription
                : "There is no command to redo.";
            documentStatusLabel.Text = workspace.State.EditorDraft.IsDirty
                ? "Editor has unapplied changes"
                : workspace.State.Document.IsDirty
                    ? "Catalogue has unsaved changes"
                    : "Catalogue unchanged";
        }

        private void RenderFeedback()
        {
            if (!presenter.Feedback.IsVisible)
            {
                feedbackStatusLabel.Text = string.Empty;
                feedbackStatusLabel.AccessibleName = "No Brand message";
                return;
            }

            var prefix = presenter.Feedback.Kind == FeedbackKind.Error
                ? "Error: "
                : presenter.Feedback.Kind == FeedbackKind.Warning
                    ? "Warning: "
                    : "Status: ";
            feedbackStatusLabel.Text = prefix + presenter.Feedback.Message;
            feedbackStatusLabel.AccessibleName = prefix.TrimEnd() + " " + presenter.Feedback.Message;
        }

        private void ClearValidationPresentation()
        {
            validationLabel.Text = string.Empty;
            errorProvider.SetError(brandNameTextBox, string.Empty);
            errorProvider.SetError(brandCodeTextBox, string.Empty);
            errorProvider.SetError(brandNotesTextBox, string.Empty);
        }

        private void RaiseCatalogueChanged()
        {
            var handler = CatalogueChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void RequireRuntimeComposition()
        {
            if (presenter == null || workspace == null)
            {
                throw new InvalidOperationException(
                    "BrandWorkspaceForm requires BrandService and WorkspaceController at runtime.");
            }
        }
    }
}
