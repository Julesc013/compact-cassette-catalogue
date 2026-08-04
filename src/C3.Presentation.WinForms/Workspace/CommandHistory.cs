using System;
using System.Collections.Generic;
using System.Linq;

namespace C3.Presentation.WinForms.Workspace
{
    public sealed class CommandHistory : WorkspaceStateComponent
    {
        private readonly int capacity;
        private readonly List<Entry> undoEntries = new List<Entry>();
        private readonly List<Entry> redoEntries = new List<Entry>();
        private long nextStateId = 1;
        private long currentStateId;
        private long checkpointStateId;

        public CommandHistory(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            this.capacity = capacity;
        }

        public int Capacity => capacity;

        public int UndoCount => undoEntries.Count;

        public int RedoCount => redoEntries.Count;

        public bool CanUndo => undoEntries.Count > 0;

        public bool CanRedo => redoEntries.Count > 0;

        public bool IsAtCheckpoint => checkpointStateId >= 0 && currentStateId == checkpointStateId;

        public string UndoDescription => CanUndo
            ? undoEntries[undoEntries.Count - 1].Command.Description
            : string.Empty;

        public string RedoDescription => CanRedo
            ? redoEntries[redoEntries.Count - 1].Command.Description
            : string.Empty;

        public WorkspaceCommandResult Execute(IReversibleWorkspaceCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (string.IsNullOrWhiteSpace(command.Description))
            {
                throw new ArgumentException(
                    "A workspace command requires a description.",
                    nameof(command));
            }

            var result = RequireResult(command.Execute(), "execute");
            if (!result.IsSuccess)
            {
                return result;
            }

            InvalidateCheckpointOnDiscardedRedo();
            redoEntries.Clear();
            var entry = new Entry(command, currentStateId, nextStateId++);
            undoEntries.Add(entry);
            currentStateId = entry.AfterStateId;
            TrimToCapacity();
            RaiseChanged();
            return result;
        }

        public WorkspaceCommandResult Undo()
        {
            if (!CanUndo)
            {
                return WorkspaceCommandResult.Failed("There is no command to undo.");
            }

            var entry = undoEntries[undoEntries.Count - 1];
            var result = RequireResult(entry.Command.Undo(), "undo");
            if (!result.IsSuccess)
            {
                return result;
            }

            undoEntries.RemoveAt(undoEntries.Count - 1);
            redoEntries.Add(entry);
            currentStateId = entry.BeforeStateId;
            RaiseChanged();
            return result;
        }

        public WorkspaceCommandResult Redo()
        {
            if (!CanRedo)
            {
                return WorkspaceCommandResult.Failed("There is no command to redo.");
            }

            var entry = redoEntries[redoEntries.Count - 1];
            var result = RequireResult(entry.Command.Execute(), "redo");
            if (!result.IsSuccess)
            {
                return result;
            }

            redoEntries.RemoveAt(redoEntries.Count - 1);
            undoEntries.Add(entry);
            currentStateId = entry.AfterStateId;
            TrimToCapacity();
            RaiseChanged();
            return result;
        }

        public void MarkCheckpoint()
        {
            checkpointStateId = currentStateId;
            RaiseChanged();
        }

        public void Clear()
        {
            undoEntries.Clear();
            redoEntries.Clear();
            nextStateId = 1;
            currentStateId = 0;
            checkpointStateId = 0;
            RaiseChanged();
        }

        private static WorkspaceCommandResult RequireResult(
            WorkspaceCommandResult result,
            string operation)
        {
            if (result == null)
            {
                throw new InvalidOperationException(
                    "Workspace command returned no result for " + operation + ".");
            }

            return result;
        }

        private void InvalidateCheckpointOnDiscardedRedo()
        {
            if (checkpointStateId < 0 || checkpointStateId == currentStateId)
            {
                return;
            }

            if (redoEntries.Any(entry =>
                entry.BeforeStateId == checkpointStateId ||
                entry.AfterStateId == checkpointStateId))
            {
                checkpointStateId = -1;
            }
        }

        private void TrimToCapacity()
        {
            while (undoEntries.Count > capacity)
            {
                undoEntries.RemoveAt(0);
            }

            if (checkpointStateId >= 0 && checkpointStateId != currentStateId &&
                !undoEntries.Any(entry =>
                    entry.BeforeStateId == checkpointStateId ||
                    entry.AfterStateId == checkpointStateId) &&
                !redoEntries.Any(entry =>
                    entry.BeforeStateId == checkpointStateId ||
                    entry.AfterStateId == checkpointStateId))
            {
                checkpointStateId = -1;
            }
        }

        private sealed class Entry
        {
            public Entry(
                IReversibleWorkspaceCommand command,
                long beforeStateId,
                long afterStateId)
            {
                Command = command;
                BeforeStateId = beforeStateId;
                AfterStateId = afterStateId;
            }

            public IReversibleWorkspaceCommand Command { get; }

            public long BeforeStateId { get; }

            public long AfterStateId { get; }
        }
    }
}
