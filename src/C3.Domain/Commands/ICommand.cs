namespace C3.Domain.Commands
{
    public interface ICommand
    {
        CommandContext Context { get; }
    }

    public interface IDraft
    {
    }

    public interface ICommandHandler<in TCommand, out TResult>
        where TCommand : ICommand
    {
        TResult Handle(TCommand command);
    }

    public interface IUndoCommand : ICommand
    {
        Identity.EntityId<CommandContext> ReversesCommandId { get; }
    }
}
