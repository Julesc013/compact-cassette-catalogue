using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;

namespace C3.Infrastructure.Updates
{
	public sealed class HttpUpdateManifestSource : IUpdateManifestSource
	{
		private sealed class HttpRequestDeadline : IDisposable
		{
			private const int ActiveState = 0;

			private const int CompletedState = 1;

			private const int ExpiredState = 2;

			private readonly HttpWebRequest _request;

			private readonly Timer _timer;

			private int _state;

			public bool HasExpired => Interlocked.CompareExchange(ref _state, 0, 0) == 2;

			public HttpRequestDeadline(HttpWebRequest request, int timeoutMilliseconds)
			{
				_request = request;
				_timer = new Timer(Expire, null, timeoutMilliseconds, -1);
			}

			public void Complete()
			{
				int num = Interlocked.CompareExchange(ref _state, 1, 0);
				_timer.Dispose();
				if (num != 2)
				{
					return;
				}
				throw new TimeoutException("The update manifest request exceeded its 15-second deadline.");
			}

			private void Expire(object state)
			{
				if (Interlocked.CompareExchange(ref _state, 2, 0) == 0)
				{
					try
					{
						_request.Abort();
					}
					catch (Exception ex)
					{
						ProjectData.SetProjectError(ex);
						Exception ex2 = ex;
						ProjectData.ClearProjectError();
					}
				}
			}

			public void Dispose()
			{
				Interlocked.CompareExchange(ref _state, 1, 0);
				_timer.Dispose();
			}

			void IDisposable.Dispose()
			{
				//ILSpy generated this explicit interface implementation from .override directive in Dispose
				this.Dispose();
			}
		}

		private sealed class LegacyTls12Scope : IDisposable
		{
			private static readonly object SynchronizationRoot = RuntimeHelpers.GetObjectValue(new object());

		private static readonly SecurityProtocolType Tls12 = (SecurityProtocolType)3072;

			private readonly SecurityProtocolType _originalProtocol;

			private readonly bool _changedProtocol;

			private bool _disposed;

			private LegacyTls12Scope()
			{
				bool flag = false;
				Monitor.Enter(RuntimeHelpers.GetObjectValue(SynchronizationRoot));
				try
				{
					_originalProtocol = ServicePointManager.SecurityProtocol;
					flag = true;
					if (_originalProtocol != Tls12)
					{
						ServicePointManager.SecurityProtocol = Tls12;
						if (ServicePointManager.SecurityProtocol != Tls12)
						{
							throw new InvalidOperationException("The process TLS policy did not accept TLS 1.2.");
						}
						_changedProtocol = true;
					}
				}
				catch (Exception ex)
				{
					ProjectData.SetProjectError(ex);
					Exception innerException = ex;
					try
					{
						if (flag && ServicePointManager.SecurityProtocol == Tls12)
						{
							ServicePointManager.SecurityProtocol = _originalProtocol;
						}
					}
					catch (Exception ex2)
					{
						ProjectData.SetProjectError(ex2);
						Exception ex3 = ex2;
						ProjectData.ClearProjectError();
					}
					finally
					{
						Monitor.Exit(RuntimeHelpers.GetObjectValue(SynchronizationRoot));
					}
					throw new InvalidOperationException("TLS 1.2 compatibility mode could not be activated.", innerException);
				}
			}

			public static IDisposable Enter()
			{
				return new LegacyTls12Scope();
			}

			public void Dispose()
			{
				if (!_disposed)
				{
					try
					{
						if (_changedProtocol && ServicePointManager.SecurityProtocol == Tls12)
						{
							ServicePointManager.SecurityProtocol = _originalProtocol;
						}
					}
					finally
					{
						_disposed = true;
						Monitor.Exit(RuntimeHelpers.GetObjectValue(SynchronizationRoot));
					}
				}
			}

			void IDisposable.Dispose()
			{
				//ILSpy generated this explicit interface implementation from .override directive in Dispose
				this.Dispose();
			}
		}

		private const int RequestDeadlineMilliseconds = 15000;

		private readonly bool _enableLegacyTls12Compatibility;

		private readonly UpdateReleaseManifestReader _reader;

		public bool UsesLegacyTls12Compatibility => _enableLegacyTls12Compatibility;

		public HttpUpdateManifestSource()
			: this(false)
		{
		}

		public HttpUpdateManifestSource(bool enableLegacyTls12Compatibility)
		{
			_enableLegacyTls12Compatibility = enableLegacyTls12Compatibility;
			_reader = new UpdateReleaseManifestReader();
		}

		public UpdateManifestReadResult Read(Uri feedUri, string expectedChannel)
		{
			UpdateEndpointPolicy.Validate(feedUri, expectedChannel);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(feedUri);
			httpWebRequest.Method = "GET";
			httpWebRequest.AllowAutoRedirect = false;
			httpWebRequest.AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate);
			httpWebRequest.Timeout = 15000;
			httpWebRequest.ReadWriteTimeout = 15000;
			httpWebRequest.UserAgent = "C3/2 update manifest client";
			IDisposable disposable = null;
			if (_enableLegacyTls12Compatibility)
			{
				disposable = LegacyTls12Scope.Enter();
			}
			try
			{
				return ReadResponse(httpWebRequest, expectedChannel);
			}
			finally
			{
				disposable?.Dispose();
			}
		}

		UpdateManifestReadResult IUpdateManifestSource.Read(Uri feedUri, string expectedChannel)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Read
			return this.Read(feedUri, expectedChannel);
		}

		private UpdateManifestReadResult ReadResponse(HttpWebRequest request, string expectedChannel)
		{
			using (HttpRequestDeadline httpRequestDeadline = new HttpRequestDeadline(request, 15000))
			{
				try
				{
					using (HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse())
					{
						if (httpWebResponse.StatusCode != HttpStatusCode.OK)
						{
							throw new WebException("The update manifest endpoint returned HTTP " + ((int)httpWebResponse.StatusCode).ToString() + ".");
						}
						if (httpWebResponse.ContentLength > 32768)
						{
							httpRequestDeadline.Complete();
							return UpdateManifestReadResult.Failed(UpdateManifestReadFailure.TooLarge, "The update manifest exceeds the 32 KiB safety limit.", null);
						}
						using (Stream source = httpWebResponse.GetResponseStream())
						{
							byte[] array = ReadBounded(source);
							httpRequestDeadline.Complete();
							if (array == null)
							{
								return UpdateManifestReadResult.Failed(UpdateManifestReadFailure.TooLarge, "The update manifest exceeds the 32 KiB safety limit.", null);
							}
							return _reader.Read(array, expectedChannel);
						}
					}
				}
				catch (Exception ex)
				{
					ProjectData.SetProjectError(ex);
					Exception ex2 = ex;
					if (httpRequestDeadline.HasExpired)
					{
						if (ex2 is TimeoutException)
						{
							throw;
						}
						throw new TimeoutException("The update manifest request exceeded its 15-second deadline.", ex2);
					}
					throw;
				}
			}
		}

		private static byte[] ReadBounded(Stream source)
		{
			if (source == null)
			{
				return new byte[0];
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte[] array = new byte[4096];
				while (true)
				{
					int num = source.Read(array, 0, array.Length);
					if (num == 0)
					{
						break;
					}
					if (checked(memoryStream.Length + num) > 32768)
					{
						return null;
					}
					memoryStream.Write(array, 0, num);
				}
				return memoryStream.ToArray();
			}
		}
	}
}
