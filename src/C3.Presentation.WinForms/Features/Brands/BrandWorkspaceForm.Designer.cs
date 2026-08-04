namespace C3.Presentation.WinForms.Features.Brands
{
    partial class BrandWorkspaceForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
        private System.Windows.Forms.Label headingLabel;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.FlowLayoutPanel filterLayout;
        private System.Windows.Forms.Label filterLabel;
        private System.Windows.Forms.TextBox filterTextBox;
        private System.Windows.Forms.Button applyFilterButton;
        private System.Windows.Forms.Button clearFilterButton;
        private System.Windows.Forms.SplitContainer contentSplit;
        private System.Windows.Forms.TableLayoutPanel listLayout;
        private System.Windows.Forms.Label listHeadingLabel;
        private System.Windows.Forms.ListView brandListView;
        private System.Windows.Forms.ColumnHeader codeColumn;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader notesColumn;
        private System.Windows.Forms.Label emptyStateLabel;
        private System.Windows.Forms.TableLayoutPanel inspectorLayout;
        private System.Windows.Forms.Label inspectorHeadingLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox brandNameTextBox;
        private System.Windows.Forms.Label codeLabel;
        private System.Windows.Forms.TextBox brandCodeTextBox;
        private System.Windows.Forms.Label notesLabel;
        private System.Windows.Forms.TextBox brandNotesTextBox;
        private System.Windows.Forms.Label addedAtLabel;
        private System.Windows.Forms.Label addedAtValueLabel;
        private System.Windows.Forms.Label validationLabel;
        private System.Windows.Forms.FlowLayoutPanel editorActions;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.FlowLayoutPanel commandBar;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button undoButton;
        private System.Windows.Forms.Button redoButton;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel countStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel feedbackStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel documentStatusLabel;
        private System.Windows.Forms.ErrorProvider errorProvider;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.headerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.headingLabel = new System.Windows.Forms.Label();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.filterLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.filterLabel = new System.Windows.Forms.Label();
            this.filterTextBox = new System.Windows.Forms.TextBox();
            this.applyFilterButton = new System.Windows.Forms.Button();
            this.clearFilterButton = new System.Windows.Forms.Button();
            this.contentSplit = new System.Windows.Forms.SplitContainer();
            this.listLayout = new System.Windows.Forms.TableLayoutPanel();
            this.listHeadingLabel = new System.Windows.Forms.Label();
            this.brandListView = new System.Windows.Forms.ListView();
            this.codeColumn = new System.Windows.Forms.ColumnHeader();
            this.nameColumn = new System.Windows.Forms.ColumnHeader();
            this.notesColumn = new System.Windows.Forms.ColumnHeader();
            this.emptyStateLabel = new System.Windows.Forms.Label();
            this.inspectorLayout = new System.Windows.Forms.TableLayoutPanel();
            this.inspectorHeadingLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.brandNameTextBox = new System.Windows.Forms.TextBox();
            this.codeLabel = new System.Windows.Forms.Label();
            this.brandCodeTextBox = new System.Windows.Forms.TextBox();
            this.notesLabel = new System.Windows.Forms.Label();
            this.brandNotesTextBox = new System.Windows.Forms.TextBox();
            this.addedAtLabel = new System.Windows.Forms.Label();
            this.addedAtValueLabel = new System.Windows.Forms.Label();
            this.validationLabel = new System.Windows.Forms.Label();
            this.editorActions = new System.Windows.Forms.FlowLayoutPanel();
            this.applyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.commandBar = new System.Windows.Forms.FlowLayoutPanel();
            this.newButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.undoButton = new System.Windows.Forms.Button();
            this.redoButton = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.countStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.feedbackStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.documentStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.rootLayout.SuspendLayout();
            this.headerLayout.SuspendLayout();
            this.filterLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contentSplit)).BeginInit();
            this.contentSplit.Panel1.SuspendLayout();
            this.contentSplit.Panel2.SuspendLayout();
            this.contentSplit.SuspendLayout();
            this.listLayout.SuspendLayout();
            this.inspectorLayout.SuspendLayout();
            this.editorActions.SuspendLayout();
            this.commandBar.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.headerLayout, 0, 0);
            this.rootLayout.Controls.Add(this.contentSplit, 0, 1);
            this.rootLayout.Controls.Add(this.commandBar, 0, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(12, 12, 12, 0);
            this.rootLayout.RowCount = 3;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.Size = new System.Drawing.Size(884, 517);
            this.rootLayout.TabIndex = 0;
            //
            // headerLayout
            //
            this.headerLayout.AutoSize = true;
            this.headerLayout.ColumnCount = 1;
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Controls.Add(this.headingLabel, 0, 0);
            this.headerLayout.Controls.Add(this.descriptionLabel, 0, 1);
            this.headerLayout.Controls.Add(this.filterLayout, 0, 2);
            this.headerLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerLayout.Location = new System.Drawing.Point(15, 15);
            this.headerLayout.Name = "headerLayout";
            this.headerLayout.RowCount = 3;
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.headerLayout.Size = new System.Drawing.Size(854, 83);
            this.headerLayout.TabIndex = 0;
            //
            // headingLabel
            //
            this.headingLabel.AutoSize = true;
            this.headingLabel.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headingLabel.Location = new System.Drawing.Point(0, 0);
            this.headingLabel.Margin = new System.Windows.Forms.Padding(0);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Size = new System.Drawing.Size(82, 30);
            this.headingLabel.TabIndex = 0;
            this.headingLabel.Text = "Brands";
            //
            // descriptionLabel
            //
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(0, 34);
            this.descriptionLabel.Margin = new System.Windows.Forms.Padding(0, 4, 0, 6);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(378, 13);
            this.descriptionLabel.TabIndex = 1;
            this.descriptionLabel.Text = "Manage cassette manufacturers. Changes remain pending until the catalogue is saved.";
            //
            // filterLayout
            //
            this.filterLayout.AutoSize = true;
            this.filterLayout.Controls.Add(this.filterLabel);
            this.filterLayout.Controls.Add(this.filterTextBox);
            this.filterLayout.Controls.Add(this.applyFilterButton);
            this.filterLayout.Controls.Add(this.clearFilterButton);
            this.filterLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterLayout.Location = new System.Drawing.Point(0, 53);
            this.filterLayout.Margin = new System.Windows.Forms.Padding(0);
            this.filterLayout.Name = "filterLayout";
            this.filterLayout.Size = new System.Drawing.Size(854, 30);
            this.filterLayout.TabIndex = 2;
            this.filterLayout.WrapContents = false;
            //
            // filterLabel
            //
            this.filterLabel.AutoSize = true;
            this.filterLabel.Location = new System.Drawing.Point(0, 7);
            this.filterLabel.Margin = new System.Windows.Forms.Padding(0, 7, 6, 0);
            this.filterLabel.Name = "filterLabel";
            this.filterLabel.Size = new System.Drawing.Size(61, 13);
            this.filterLabel.TabIndex = 0;
            this.filterLabel.Text = "Filter &notes:";
            //
            // filterTextBox
            //
            this.filterTextBox.AccessibleDescription = "Filters brands by text contained in their notes.";
            this.filterTextBox.AccessibleName = "Brand notes filter";
            this.filterTextBox.Location = new System.Drawing.Point(70, 3);
            this.filterTextBox.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(260, 20);
            this.filterTextBox.TabIndex = 1;
            //
            // applyFilterButton
            //
            this.applyFilterButton.AutoSize = true;
            this.applyFilterButton.Location = new System.Drawing.Point(339, 2);
            this.applyFilterButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.applyFilterButton.Name = "applyFilterButton";
            this.applyFilterButton.Size = new System.Drawing.Size(83, 25);
            this.applyFilterButton.TabIndex = 2;
            this.applyFilterButton.Text = "&Apply filter";
            this.applyFilterButton.UseVisualStyleBackColor = true;
            //
            // clearFilterButton
            //
            this.clearFilterButton.AutoSize = true;
            this.clearFilterButton.Location = new System.Drawing.Point(428, 2);
            this.clearFilterButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.clearFilterButton.Name = "clearFilterButton";
            this.clearFilterButton.Size = new System.Drawing.Size(72, 25);
            this.clearFilterButton.TabIndex = 3;
            this.clearFilterButton.Text = "&Clear";
            this.clearFilterButton.UseVisualStyleBackColor = true;
            //
            // contentSplit
            //
            this.contentSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentSplit.Location = new System.Drawing.Point(15, 104);
            this.contentSplit.Name = "contentSplit";
            //
            // contentSplit.Panel1
            //
            this.contentSplit.Panel1.Controls.Add(this.listLayout);
            this.contentSplit.Panel1MinSize = 280;
            //
            // contentSplit.Panel2
            //
            this.contentSplit.Panel2.Controls.Add(this.inspectorLayout);
            this.contentSplit.Panel2MinSize = 270;
            this.contentSplit.Size = new System.Drawing.Size(854, 349);
            this.contentSplit.SplitterDistance = 510;
            this.contentSplit.TabIndex = 1;
            //
            // listLayout
            //
            this.listLayout.ColumnCount = 1;
            this.listLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.listLayout.Controls.Add(this.listHeadingLabel, 0, 0);
            this.listLayout.Controls.Add(this.brandListView, 0, 1);
            this.listLayout.Controls.Add(this.emptyStateLabel, 0, 2);
            this.listLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listLayout.Location = new System.Drawing.Point(0, 0);
            this.listLayout.Name = "listLayout";
            this.listLayout.RowCount = 3;
            this.listLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.listLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.listLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.listLayout.Size = new System.Drawing.Size(510, 349);
            this.listLayout.TabIndex = 0;
            //
            // listHeadingLabel
            //
            this.listHeadingLabel.AutoSize = true;
            this.listHeadingLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listHeadingLabel.Location = new System.Drawing.Point(0, 0);
            this.listHeadingLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.listHeadingLabel.Name = "listHeadingLabel";
            this.listHeadingLabel.Size = new System.Drawing.Size(66, 15);
            this.listHeadingLabel.TabIndex = 0;
            this.listHeadingLabel.Text = "Brand list";
            //
            // brandListView
            //
            this.brandListView.AccessibleDescription = "Available brands. Use Up and Down to change selection, Control to select several, and Enter to edit one.";
            this.brandListView.AccessibleName = "Brands";
            this.brandListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.codeColumn,
            this.nameColumn,
            this.notesColumn});
            this.brandListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brandListView.FullRowSelect = true;
            this.brandListView.HideSelection = false;
            this.brandListView.Location = new System.Drawing.Point(0, 21);
            this.brandListView.Margin = new System.Windows.Forms.Padding(0);
            this.brandListView.MultiSelect = true;
            this.brandListView.Name = "brandListView";
            this.brandListView.Size = new System.Drawing.Size(510, 304);
            this.brandListView.TabIndex = 1;
            this.brandListView.UseCompatibleStateImageBehavior = false;
            this.brandListView.View = System.Windows.Forms.View.Details;
            //
            // codeColumn
            //
            this.codeColumn.Text = "Code";
            this.codeColumn.Width = 62;
            //
            // nameColumn
            //
            this.nameColumn.Text = "Brand";
            this.nameColumn.Width = 170;
            //
            // notesColumn
            //
            this.notesColumn.Text = "Notes";
            this.notesColumn.Width = 250;
            //
            // emptyStateLabel
            //
            this.emptyStateLabel.AutoEllipsis = true;
            this.emptyStateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.emptyStateLabel.Location = new System.Drawing.Point(0, 331);
            this.emptyStateLabel.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.emptyStateLabel.Name = "emptyStateLabel";
            this.emptyStateLabel.Size = new System.Drawing.Size(510, 18);
            this.emptyStateLabel.TabIndex = 2;
            this.emptyStateLabel.Text = "No brands yet. Create a brand to begin.";
            this.emptyStateLabel.Visible = false;
            //
            // inspectorLayout
            //
            this.inspectorLayout.ColumnCount = 2;
            this.inspectorLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.inspectorLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.inspectorLayout.Controls.Add(this.inspectorHeadingLabel, 0, 0);
            this.inspectorLayout.Controls.Add(this.nameLabel, 0, 1);
            this.inspectorLayout.Controls.Add(this.brandNameTextBox, 1, 1);
            this.inspectorLayout.Controls.Add(this.codeLabel, 0, 2);
            this.inspectorLayout.Controls.Add(this.brandCodeTextBox, 1, 2);
            this.inspectorLayout.Controls.Add(this.notesLabel, 0, 3);
            this.inspectorLayout.Controls.Add(this.brandNotesTextBox, 1, 3);
            this.inspectorLayout.Controls.Add(this.addedAtLabel, 0, 4);
            this.inspectorLayout.Controls.Add(this.addedAtValueLabel, 1, 4);
            this.inspectorLayout.Controls.Add(this.validationLabel, 0, 5);
            this.inspectorLayout.Controls.Add(this.editorActions, 0, 6);
            this.inspectorLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inspectorLayout.Location = new System.Drawing.Point(0, 0);
            this.inspectorLayout.Name = "inspectorLayout";
            this.inspectorLayout.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.inspectorLayout.RowCount = 7;
            this.inspectorLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inspectorLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inspectorLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inspectorLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.inspectorLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inspectorLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inspectorLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inspectorLayout.Size = new System.Drawing.Size(340, 349);
            this.inspectorLayout.TabIndex = 0;
            //
            // inspectorHeadingLabel
            //
            this.inspectorHeadingLabel.AutoSize = true;
            this.inspectorLayout.SetColumnSpan(this.inspectorHeadingLabel, 2);
            this.inspectorHeadingLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inspectorHeadingLabel.Location = new System.Drawing.Point(12, 0);
            this.inspectorHeadingLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.inspectorHeadingLabel.Name = "inspectorHeadingLabel";
            this.inspectorHeadingLabel.Size = new System.Drawing.Size(62, 15);
            this.inspectorHeadingLabel.TabIndex = 0;
            this.inspectorHeadingLabel.Text = "Inspector";
            //
            // nameLabel
            //
            this.nameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(12, 29);
            this.nameLabel.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(41, 13);
            this.nameLabel.TabIndex = 1;
            this.nameLabel.Text = "&Name:";
            //
            // brandNameTextBox
            //
            this.brandNameTextBox.AccessibleName = "Brand name";
            this.brandNameTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.brandNameTextBox.Location = new System.Drawing.Point(64, 23);
            this.brandNameTextBox.MaxLength = 100;
            this.brandNameTextBox.Name = "brandNameTextBox";
            this.brandNameTextBox.ReadOnly = true;
            this.brandNameTextBox.Size = new System.Drawing.Size(273, 20);
            this.brandNameTextBox.TabIndex = 2;
            //
            // codeLabel
            //
            this.codeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.codeLabel.AutoSize = true;
            this.codeLabel.Location = new System.Drawing.Point(12, 55);
            this.codeLabel.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            this.codeLabel.Name = "codeLabel";
            this.codeLabel.Size = new System.Drawing.Size(35, 13);
            this.codeLabel.TabIndex = 3;
            this.codeLabel.Text = "&Code:";
            //
            // brandCodeTextBox
            //
            this.brandCodeTextBox.AccessibleDescription = "Two-letter stable legacy brand code.";
            this.brandCodeTextBox.AccessibleName = "Brand code";
            this.brandCodeTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.brandCodeTextBox.Location = new System.Drawing.Point(64, 49);
            this.brandCodeTextBox.MaxLength = 2;
            this.brandCodeTextBox.Name = "brandCodeTextBox";
            this.brandCodeTextBox.ReadOnly = true;
            this.brandCodeTextBox.Size = new System.Drawing.Size(54, 20);
            this.brandCodeTextBox.TabIndex = 4;
            //
            // notesLabel
            //
            this.notesLabel.AutoSize = true;
            this.notesLabel.Location = new System.Drawing.Point(12, 78);
            this.notesLabel.Margin = new System.Windows.Forms.Padding(0, 6, 8, 3);
            this.notesLabel.Name = "notesLabel";
            this.notesLabel.Size = new System.Drawing.Size(38, 13);
            this.notesLabel.TabIndex = 5;
            this.notesLabel.Text = "N&otes:";
            //
            // brandNotesTextBox
            //
            this.brandNotesTextBox.AcceptsReturn = true;
            this.brandNotesTextBox.AccessibleName = "Brand notes";
            this.brandNotesTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brandNotesTextBox.Location = new System.Drawing.Point(64, 75);
            this.brandNotesTextBox.Multiline = true;
            this.brandNotesTextBox.Name = "brandNotesTextBox";
            this.brandNotesTextBox.ReadOnly = true;
            this.brandNotesTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.brandNotesTextBox.Size = new System.Drawing.Size(273, 190);
            this.brandNotesTextBox.TabIndex = 6;
            //
            // addedAtLabel
            //
            this.addedAtLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.addedAtLabel.AutoSize = true;
            this.addedAtLabel.Location = new System.Drawing.Point(12, 275);
            this.addedAtLabel.Margin = new System.Windows.Forms.Padding(0, 4, 8, 4);
            this.addedAtLabel.Name = "addedAtLabel";
            this.addedAtLabel.Size = new System.Drawing.Size(41, 13);
            this.addedAtLabel.TabIndex = 7;
            this.addedAtLabel.Text = "Added:";
            //
            // addedAtValueLabel
            //
            this.addedAtValueLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.addedAtValueLabel.AutoSize = true;
            this.addedAtValueLabel.Location = new System.Drawing.Point(64, 275);
            this.addedAtValueLabel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.addedAtValueLabel.Name = "addedAtValueLabel";
            this.addedAtValueLabel.Size = new System.Drawing.Size(12, 13);
            this.addedAtValueLabel.TabIndex = 8;
            this.addedAtValueLabel.Text = "-";
            //
            // validationLabel
            //
            this.validationLabel.AutoEllipsis = true;
            this.validationLabel.AutoSize = true;
            this.inspectorLayout.SetColumnSpan(this.validationLabel, 2);
            this.validationLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.validationLabel.Location = new System.Drawing.Point(12, 296);
            this.validationLabel.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.validationLabel.MaximumSize = new System.Drawing.Size(320, 42);
            this.validationLabel.Name = "validationLabel";
            this.validationLabel.Size = new System.Drawing.Size(0, 13);
            this.validationLabel.TabIndex = 9;
            //
            // editorActions
            //
            this.editorActions.AutoSize = true;
            this.inspectorLayout.SetColumnSpan(this.editorActions, 2);
            this.editorActions.Controls.Add(this.applyButton);
            this.editorActions.Controls.Add(this.cancelButton);
            this.editorActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorActions.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.editorActions.Location = new System.Drawing.Point(12, 317);
            this.editorActions.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.editorActions.Name = "editorActions";
            this.editorActions.Size = new System.Drawing.Size(328, 32);
            this.editorActions.TabIndex = 10;
            this.editorActions.WrapContents = false;
            //
            // applyButton
            //
            this.applyButton.AutoSize = true;
            this.applyButton.Location = new System.Drawing.Point(247, 3);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(78, 26);
            this.applyButton.TabIndex = 0;
            this.applyButton.Text = "&Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Visible = false;
            //
            // cancelButton
            //
            this.cancelButton.AutoSize = true;
            this.cancelButton.Location = new System.Drawing.Point(163, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(78, 26);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Visible = false;
            //
            // commandBar
            //
            this.commandBar.AutoSize = true;
            this.commandBar.Controls.Add(this.newButton);
            this.commandBar.Controls.Add(this.editButton);
            this.commandBar.Controls.Add(this.deleteButton);
            this.commandBar.Controls.Add(this.undoButton);
            this.commandBar.Controls.Add(this.redoButton);
            this.commandBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commandBar.Location = new System.Drawing.Point(15, 459);
            this.commandBar.Name = "commandBar";
            this.commandBar.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.commandBar.Size = new System.Drawing.Size(854, 43);
            this.commandBar.TabIndex = 2;
            this.commandBar.WrapContents = false;
            //
            // newButton
            //
            this.newButton.AutoSize = true;
            this.newButton.Location = new System.Drawing.Point(3, 7);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(84, 26);
            this.newButton.TabIndex = 0;
            this.newButton.Text = "&New brand";
            this.newButton.UseVisualStyleBackColor = true;
            //
            // editButton
            //
            this.editButton.AutoSize = true;
            this.editButton.Enabled = false;
            this.editButton.Location = new System.Drawing.Point(93, 7);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 26);
            this.editButton.TabIndex = 1;
            this.editButton.Text = "&Edit";
            this.editButton.UseVisualStyleBackColor = true;
            //
            // deleteButton
            //
            this.deleteButton.AutoSize = true;
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(174, 7);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 26);
            this.deleteButton.TabIndex = 2;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            //
            // undoButton
            //
            this.undoButton.AutoSize = true;
            this.undoButton.Enabled = false;
            this.undoButton.Location = new System.Drawing.Point(273, 7);
            this.undoButton.Margin = new System.Windows.Forms.Padding(21, 3, 3, 3);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(75, 26);
            this.undoButton.TabIndex = 3;
            this.undoButton.Text = "&Undo";
            this.undoButton.UseVisualStyleBackColor = true;
            //
            // redoButton
            //
            this.redoButton.AutoSize = true;
            this.redoButton.Enabled = false;
            this.redoButton.Location = new System.Drawing.Point(354, 7);
            this.redoButton.Name = "redoButton";
            this.redoButton.Size = new System.Drawing.Size(75, 26);
            this.redoButton.TabIndex = 4;
            this.redoButton.Text = "&Redo";
            this.redoButton.UseVisualStyleBackColor = true;
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.countStatusLabel,
            this.feedbackStatusLabel,
            this.documentStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 517);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(884, 24);
            this.statusStrip.TabIndex = 1;
            //
            // countStatusLabel
            //
            this.countStatusLabel.Name = "countStatusLabel";
            this.countStatusLabel.Size = new System.Drawing.Size(55, 19);
            this.countStatusLabel.Text = "0 brands";
            //
            // feedbackStatusLabel
            //
            this.feedbackStatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.feedbackStatusLabel.Name = "feedbackStatusLabel";
            this.feedbackStatusLabel.Size = new System.Drawing.Size(694, 19);
            this.feedbackStatusLabel.Spring = true;
            this.feedbackStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // documentStatusLabel
            //
            this.documentStatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.documentStatusLabel.Name = "documentStatusLabel";
            this.documentStatusLabel.Size = new System.Drawing.Size(120, 19);
            this.documentStatusLabel.Text = "Catalogue unchanged";
            //
            // errorProvider
            //
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            //
            // BrandWorkspaceForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(884, 541);
            this.Controls.Add(this.rootLayout);
            this.Controls.Add(this.statusStrip);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(720, 450);
            this.Name = "BrandWorkspaceForm";
            this.ShowIcon = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Brands - C3";
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.headerLayout.ResumeLayout(false);
            this.headerLayout.PerformLayout();
            this.filterLayout.ResumeLayout(false);
            this.filterLayout.PerformLayout();
            this.contentSplit.Panel1.ResumeLayout(false);
            this.contentSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.contentSplit)).EndInit();
            this.contentSplit.ResumeLayout(false);
            this.listLayout.ResumeLayout(false);
            this.listLayout.PerformLayout();
            this.inspectorLayout.ResumeLayout(false);
            this.inspectorLayout.PerformLayout();
            this.editorActions.ResumeLayout(false);
            this.editorActions.PerformLayout();
            this.commandBar.ResumeLayout(false);
            this.commandBar.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
