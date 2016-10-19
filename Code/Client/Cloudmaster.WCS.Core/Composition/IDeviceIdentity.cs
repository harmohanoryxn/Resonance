using System;
using System.ComponentModel.Composition;

namespace WCS.Core.Composition
{
	public interface IDeviceIdentity
	{
		string DeviceName { get; }
		string ApplicationVersion { get; }
	}
}