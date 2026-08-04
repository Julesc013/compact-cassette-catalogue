using C3.Catalogue.Brands;
using C3.Presentation.WinForms.Interaction;
using C3.Presentation.WinForms.Workspace;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Presentation.WinForms.Features.Brands
{
    /// <summary>
    /// Owns the complete Brands interaction workflow while delegating catalogue
    /// rules to BrandService and document history to WorkspaceController.
    /// </summary>
    public sealed class BrandWorkspacePresenter
    {
        public const string FeatureKey = "brands";
        public const string NameField = "brand.name";
        public const string CodeField = "brand.code";
        public const string NotesField = "brand.notes";

        private static readonly ReadOnlyCollection<FieldDefinition> fieldDefinitions =
            new[]
            {
                new FieldDefinition(NameField, "Brand name", true, 100,
                    "The manufacturer name shown throughout the catalogue."),
                new FieldDefinition(CodeField, "Code", true, 2,
                    "Two letters (A-Z). The code is the stable legacy identity."),
                new FieldDefinition(NotesField, "Notes", false, 0,
                    "Optional private notes about this manufacturer.")
            }.ToList().AsReadOnly();

        private readonly BrandService service;
        private readonly WorkspaceController workspace;
        private readonly Func<DateTime> clock;

        public BrandWorkspacePresenter(
            BrandService service,
            WorkspaceController workspace)
            : this(service, workspace, () => DateTime.Now)
        {
        }

        public BrandWorkspacePresenter(
            BrandService service,
            WorkspaceController workspace,
            Func<DateTime> clock)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public event EventHandler StateChanged;

        public ReadOnlyCollection<FieldDefinition> Fields => fieldDefinitions;

        public ListPresentation<Brand> List { get; } = new ListPresentation<Brand>();

        public InspectorPresentation<Brand> Inspector { get; } =
            new InspectorPresentation<Brand>();

        public ValidationPresentation Validation { get; } =
            new ValidationPresentation();

        public FeedbackPresentation Feedback { get; } = new FeedbackPresentation();

        public BrandEditorModel Editor { get; private set; }

        public string FilterText => workspace.State.View.Feature == FeatureKey
            ? workspace.State.View.FilterText
            : string.Empty;

        public bool CanEdit => Inspector.HasSelection;

        public bool CanDelete => Inspector.HasSelection;

        public bool CanUndo => workspace.History.CanUndo;

        public bool CanRedo => workspace.History.CanRedo;

        public string UndoDescription => workspace.History.UndoDescription;

        public string RedoDescription => workspace.History.RedoDescription;

        public void Refresh(string notesFilter)
        {
            workspace.State.View.Apply(
                FeatureKey,
                notesFilter,
                CodeField,
                SortDirection.Ascending);
            ReplaceListPreservingSelection();
            RaiseStateChanged();
        }

        public void Select(string code)
        {
            var value = service.Find(code);
            if (value == null)
            {
                workspace.State.Selection.Clear();
                Inspector.Clear();
            }
            else
            {
                workspace.State.Selection.SelectOnly(FeatureKey, value.Code);
                Inspector.Select(value);
            }

            Validation.Clear();
            RaiseStateChanged();
        }

        public void BeginCreate()
        {
            workspace.State.EditorDraft.Begin(FeatureKey, null);
            Editor = new BrandEditorModel(true, string.Empty, string.Empty, string.Empty);
            Validation.Clear();
            Feedback.Clear();
            RaiseStateChanged();
        }

        public bool BeginEdit()
        {
            if (!Inspector.HasSelection)
            {
                Feedback.Show(FeedbackKind.Warning, "Select one brand to edit.");
                RaiseStateChanged();
                return false;
            }

            var value = Inspector.Value;
            workspace.State.EditorDraft.Begin(FeatureKey, value.Code);
            Editor = new BrandEditorModel(false, value.Name, value.Code, value.Notes);
            Validation.Clear();
            Feedback.Clear();
            RaiseStateChanged();
            return true;
        }

        public void UpdateDraft(string name, string code, string notes)
        {
            if (Editor == null)
            {
                throw new InvalidOperationException("No brand editor is active.");
            }

            Editor.Update(name, code, notes);
            workspace.State.EditorDraft.MarkChanged();
            Validation.Clear();
            RaiseStateChanged();
        }

        public bool Apply()
        {
            if (Editor == null)
            {
                Feedback.Show(FeedbackKind.Warning, "No brand editor is active.");
                RaiseStateChanged();
                return false;
            }

            var draft = new BrandDraft(Editor.Name, Editor.Code, Editor.Notes);
            Brand value;
            WorkspaceCommandResult result;
            if (Editor.IsNew)
            {
                var command = new CreateBrandCommand(service, draft, clock());
                result = workspace.Execute(command);
                value = command.Brand;
            }
            else
            {
                var command = new UpdateBrandCommand(service, Editor.Code, draft);
                result = workspace.Execute(command);
                value = command.Brand;
            }

            if (!result.IsSuccess)
            {
                PresentFailure(result.Message, Editor.IsNew ? draft.Code : Editor.Code);
                RaiseStateChanged();
                return false;
            }

            workspace.State.EditorDraft.MarkApplied();
            workspace.State.EditorDraft.Clear();
            Editor = null;
            Feedback.Show(
                FeedbackKind.Information,
                "Brand " + value.Code + " was saved. Save the catalogue to keep this change.");
            workspace.State.Selection.SelectOnly(FeatureKey, value.Code);
            ReplaceListPreservingSelection();
            RaiseStateChanged();
            return true;
        }

        public void CancelEditor()
        {
            workspace.State.EditorDraft.Clear();
            Editor = null;
            Validation.Clear();
            RaiseStateChanged();
        }

        public bool DeleteSelected()
        {
            if (!Inspector.HasSelection)
            {
                Feedback.Show(FeedbackKind.Warning, "Select one brand to delete.");
                RaiseStateChanged();
                return false;
            }

            var code = Inspector.Value.Code;
            var result = workspace.Execute(new DeleteBrandCommand(service, code));
            if (!result.IsSuccess)
            {
                PresentFailure(result.Message, code);
                RaiseStateChanged();
                return false;
            }

            workspace.State.Selection.Clear();
            Inspector.Clear();
            Feedback.Show(
                FeedbackKind.Information,
                "Brand " + code + " was deleted. Use Undo to restore it.");
            ReplaceListPreservingSelection();
            RaiseStateChanged();
            return true;
        }

        public bool Undo()
        {
            return CompleteHistoryOperation(workspace.Undo(), "Undo completed.");
        }

        public bool Redo()
        {
            return CompleteHistoryOperation(workspace.Redo(), "Redo completed.");
        }

        private bool CompleteHistoryOperation(WorkspaceCommandResult result, string success)
        {
            if (!result.IsSuccess)
            {
                Feedback.Show(FeedbackKind.Error, result.Message);
                RaiseStateChanged();
                return false;
            }

            Feedback.Show(FeedbackKind.Information, success);
            ReplaceListPreservingSelection();
            RaiseStateChanged();
            return true;
        }

        private void ReplaceListPreservingSelection()
        {
            var values = service.GetAll(FilterText);
            List.Replace(
                values,
                string.IsNullOrWhiteSpace(FilterText) ? "No brands yet" : "No matching brands",
                string.IsNullOrWhiteSpace(FilterText)
                    ? "Create a brand to begin describing cassette models."
                    : "Clear or change the notes filter to see other brands.");

            var selectedCode = workspace.State.Selection.Feature == FeatureKey &&
                workspace.State.Selection.SelectedIds.Count == 1
                    ? workspace.State.Selection.SelectedIds[0]
                    : null;
            var selected = values.FirstOrDefault(value => string.Equals(
                value.Code,
                selectedCode,
                StringComparison.OrdinalIgnoreCase));
            if (selected == null)
            {
                workspace.State.Selection.Clear();
                Inspector.Clear();
            }
            else
            {
                Inspector.Select(selected);
            }
        }

        private void PresentFailure(string message, string code)
        {
            var current = service.Find(code);
            string field = string.Empty;
            if (Editor != null && string.IsNullOrWhiteSpace(Editor.Name))
            {
                field = NameField;
            }
            else if (Editor != null && Editor.IsNew &&
                (!IsValidLegacyCode(Editor.Code) || current != null))
            {
                field = CodeField;
            }

            Validation.Show(new ValidationMessage(field, message));
            Feedback.Show(FeedbackKind.Error, message);
        }

        private static bool IsValidLegacyCode(string value)
        {
            var normalized = (value ?? string.Empty).Trim().ToUpperInvariant();
            return normalized.Length == 2 &&
                normalized.All(character => character >= 'A' && character <= 'Z');
        }

        private void RaiseStateChanged()
        {
            var handler = StateChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
