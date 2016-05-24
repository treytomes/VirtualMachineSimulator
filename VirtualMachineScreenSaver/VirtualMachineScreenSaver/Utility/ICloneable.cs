using System;

namespace VirtualMachineScreenSaver.Utility
{
	public interface ICloneable<T> : ICloneable
	{
		new T Clone();
	}
}
