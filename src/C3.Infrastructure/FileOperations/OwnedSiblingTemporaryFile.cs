using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;

namespace C3.Infrastructure.FileOperations
{
	internal sealed class OwnedSiblingTemporaryFile : IDisposable
	{
		private const int MaximumCreationAttempts = 32;

		private readonly string _path;

		private readonly FileStream _stream;

		private bool _disposed;

		public string Path => _path;

		public FileStream Stream
		{
			get
			{
				if (_disposed)
				{
					throw new ObjectDisposedException("OwnedSiblingTemporaryFile");
				}
				return _stream;
			}
		}

		private OwnedSiblingTemporaryFile(string pathValue, FileStream streamValue)
		{
			_path = pathValue;
			_stream = streamValue;
		}

		public static OwnedSiblingTemporaryFile Create(string destinationPath)
		{
			if (string.IsNullOrWhiteSpace(destinationPath))
			{
				throw new ArgumentException("A destination path is required.", "destinationPath");
			}
			string directoryName = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(destinationPath));
			if (string.IsNullOrWhiteSpace(directoryName))
			{
				throw new DirectoryNotFoundException("The destination directory could not be determined.");
			}
			int num = 1;
			do
			{
				string text = System.IO.Path.Combine(directoryName, CompactSiblingFileName.CreateTemporary());
				try
				{
					FileStream streamValue = new FileStream(text, FileMode.CreateNew, FileAccess.Write, FileShare.None);
					return new OwnedSiblingTemporaryFile(text, streamValue);
				}
				catch (IOException ex)
				{
					ProjectData.SetProjectError(ex);
					IOException ex2 = ex;
					if (!File.Exists(text) && !Directory.Exists(text))
					{
						throw;
					}
					ProjectData.ClearProjectError();
				}
				num = checked(num + 1);
			}
			while (num <= 32);
			throw new IOException("C3 could not reserve a unique sibling temporary file.");
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				try
				{
					_stream.Dispose();
				}
				finally
				{
					try
					{
						if (File.Exists(_path))
						{
							File.Delete(_path);
						}
					}
					catch (Exception projectError)
					{
						ProjectData.SetProjectError(projectError);
						ProjectData.ClearProjectError();
					}
				}
			}
		}

		void IDisposable.Dispose()
		{
			//ILSpy generated this explicit interface implementation from .override directive in Dispose
			this.Dispose();
		}
	}
}
