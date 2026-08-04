using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;
using System.Text;

namespace C3.Infrastructure.Diagnostics
{
	public sealed class CrashReportWriter
	{
		private CrashReportWriter()
		{
		}

		public static string TryWrite(Exception exception, CrashReportContext context)
		{
			if (exception == null)
			{
				return null;
			}
			try
			{
				string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "C3", "CrashReports");
				Directory.CreateDirectory(text);
				string path = "C3-error-" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + ".log";
				string text2 = Path.Combine(text, path);
				File.WriteAllText(text2, BuildReport(exception, context), new UTF8Encoding(false));
				return text2;
			}
			catch (Exception projectError)
			{
				ProjectData.SetProjectError(projectError);
				string result = null;
				ProjectData.ClearProjectError();
				return result;
			}
		}

		private static string BuildReport(Exception exception, CrashReportContext context)
		{
			if (context == null)
			{
				context = new CrashReportContext
				{
					LastAction = BufferedLogger.LastAction
				};
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("C3 crash report");
			stringBuilder.AppendLine("===============");
			stringBuilder.AppendLine("Created (UTC): " + DateTime.UtcNow.ToString("O"));
			stringBuilder.AppendLine("Product version: " + (context.ProductVersion ?? "(unknown)") + " " + (context.ReleaseStage ?? string.Empty));
			stringBuilder.AppendLine("Build lane: " + (context.BuildLane ?? "(unknown)"));
			stringBuilder.AppendLine("Operating system: " + (context.OperatingSystem ?? "(unknown)"));
			stringBuilder.AppendLine("CLR version: " + (context.ClrVersion ?? "(unknown)"));
			stringBuilder.AppendLine("Process bitness: " + (context.ProcessBitness ?? "(unknown)"));
			if (context != null)
			{
				stringBuilder.AppendLine("Catalogue path: " + (context.CataloguePath ?? "(new catalogue)"));
				stringBuilder.AppendLine("Last action: " + (context.LastAction ?? "(unknown)"));
			}
			else
			{
				stringBuilder.AppendLine("Catalogue path: (unknown)");
				stringBuilder.AppendLine("Last action: " + BufferedLogger.LastAction);
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Exception");
			stringBuilder.AppendLine("---------");
			stringBuilder.AppendLine(exception.ToString());
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Recent log");
			stringBuilder.AppendLine("----------");
			foreach (string item in BufferedLogger.Tail())
			{
				stringBuilder.AppendLine(item);
			}
			return stringBuilder.ToString();
		}
	}
}
