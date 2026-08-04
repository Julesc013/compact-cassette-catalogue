using C3.Catalogue.Brands;
using C3.Presentation.WinForms.Workspace;
using System;

namespace C3.Presentation.WinForms.Features.Brands
{
    public sealed class CreateBrandCommand : IReversibleWorkspaceCommand
    {
        private readonly BrandService service;
        private readonly BrandDraft requestedDraft;
        private readonly DateTime addedAt;
        private Brand created;

        public CreateBrandCommand(BrandService service, BrandDraft draft, DateTime addedAt)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            requestedDraft = draft ?? throw new ArgumentNullException(nameof(draft));
            this.addedAt = addedAt;
        }

        public string Description => Describe("Create brand", requestedDraft.Code);

        public Brand Brand => created;

        public WorkspaceCommandResult Execute()
        {
            var draft = created == null
                ? requestedDraft
                : new BrandDraft(created.Name, created.Code, created.Notes);
            var result = service.Create(draft, created == null ? addedAt : created.AddedAt);
            if (result.IsSuccess)
            {
                created = result.Brand;
            }

            return Map(result);
        }

        public WorkspaceCommandResult Undo()
        {
            return created == null
                ? WorkspaceCommandResult.Failed("The brand has not been created.")
                : Map(service.Delete(created.Code));
        }

        internal static WorkspaceCommandResult Map(BrandOperationResult result)
        {
            return result.IsSuccess
                ? WorkspaceCommandResult.Success()
                : WorkspaceCommandResult.Failed(result.Message);
        }

        internal static string Describe(string action, string code)
        {
            var normalized = (code ?? string.Empty).Trim().ToUpperInvariant();
            return normalized.Length == 0 ? action : action + " " + normalized;
        }
    }

    public sealed class UpdateBrandCommand : IReversibleWorkspaceCommand
    {
        private readonly BrandService service;
        private readonly string code;
        private readonly BrandDraft draft;
        private Brand before;
        private Brand updated;

        public UpdateBrandCommand(BrandService service, string code, BrandDraft draft)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.code = code;
            this.draft = draft ?? throw new ArgumentNullException(nameof(draft));
        }

        public string Description => CreateBrandCommand.Describe("Update brand", code);

        public Brand Brand => updated;

        public WorkspaceCommandResult Execute()
        {
            if (before == null)
            {
                before = service.Find(code);
                if (before == null)
                {
                    return WorkspaceCommandResult.Failed("The selected brand no longer exists.");
                }
            }

            var result = service.Update(code, draft);
            if (result.IsSuccess)
            {
                updated = result.Brand;
            }

            return CreateBrandCommand.Map(result);
        }

        public WorkspaceCommandResult Undo()
        {
            if (before == null)
            {
                return WorkspaceCommandResult.Failed("The brand has not been updated.");
            }

            return CreateBrandCommand.Map(service.Update(
                before.Code,
                new BrandDraft(before.Name, before.Code, before.Notes)));
        }
    }

    public sealed class DeleteBrandCommand : IReversibleWorkspaceCommand
    {
        private readonly BrandService service;
        private readonly string code;
        private Brand deleted;

        public DeleteBrandCommand(BrandService service, string code)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.code = code;
        }

        public string Description => CreateBrandCommand.Describe("Delete brand", code);

        public WorkspaceCommandResult Execute()
        {
            var result = service.Delete(code);
            if (result.IsSuccess)
            {
                deleted = result.Brand;
            }

            return CreateBrandCommand.Map(result);
        }

        public WorkspaceCommandResult Undo()
        {
            if (deleted == null)
            {
                return WorkspaceCommandResult.Failed("The brand has not been deleted.");
            }

            return CreateBrandCommand.Map(service.Create(
                new BrandDraft(deleted.Name, deleted.Code, deleted.Notes),
                deleted.AddedAt));
        }
    }
}
