using System;

namespace C3.Infrastructure.Updates
{
	public interface IUpdateManifestSource
	{
		UpdateManifestReadResult Read(Uri feedUri, string expectedChannel);
	}
}
