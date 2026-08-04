using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;

namespace C3.Infrastructure.Diagnostics
{
	public sealed class BufferedLogger
	{
		private const int MaximumEntries = 200;

		private static readonly object SyncRoot = RuntimeHelpers.GetObjectValue(new object());

		private static readonly Queue<string> Entries = new Queue<string>();

		private static string _lastAction = "Application initialization";

		public static string LastAction
		{
			get
			{
				object syncRoot = SyncRoot;
				ObjectFlowControl.CheckForSyncLockOnValueType(syncRoot);
				bool flag = false;
				try
				{
					Monitor.Enter(syncRoot, ref flag);
					return _lastAction;
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(syncRoot);
					}
				}
			}
		}

		private BufferedLogger()
		{
		}

		public static void RecordAction(string action)
		{
			if (!string.IsNullOrWhiteSpace(action))
			{
				object syncRoot = SyncRoot;
				ObjectFlowControl.CheckForSyncLockOnValueType(syncRoot);
				bool flag = false;
				try
				{
					Monitor.Enter(syncRoot, ref flag);
					_lastAction = action.Trim();
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(syncRoot);
					}
				}
				Information("Action: " + action.Trim());
			}
		}

		public static void Information(string message)
		{
			Add("INFO", message);
		}

		public static void Warning(string message)
		{
			Add("WARN", message);
		}

		public static void Error(string message)
		{
			Add("ERROR", message);
		}

		public static ReadOnlyCollection<string> Tail()
		{
			object syncRoot = SyncRoot;
			ObjectFlowControl.CheckForSyncLockOnValueType(syncRoot);
			bool flag = false;
			try
			{
				Monitor.Enter(syncRoot, ref flag);
				return new ReadOnlyCollection<string>(Entries.ToArray());
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(syncRoot);
				}
			}
		}

		private static void Add(string level, string message)
		{
			string text = message ?? string.Empty;
			string item = string.Format(CultureInfo.InvariantCulture, "{0:O} [{1}] {2}", new object[3]
			{
				DateTime.UtcNow,
				level,
				text
			});
			object syncRoot = SyncRoot;
			ObjectFlowControl.CheckForSyncLockOnValueType(syncRoot);
			bool flag = false;
			try
			{
				Monitor.Enter(syncRoot, ref flag);
				Entries.Enqueue(item);
				while (Entries.Count > 200)
				{
					Entries.Dequeue();
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(syncRoot);
				}
			}
		}
	}
}
