namespace VirtualMachineScreenSaver.Utility
{
	public class PropertyChangingEventArgs<TValue> : System.ComponentModel.PropertyChangingEventArgs
	{
		public PropertyChangingEventArgs(string propertyName, TValue oldValue, TValue newValue)
			: base(propertyName)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}

		public TValue OldValue { get; private set; }

		public TValue NewValue { get; private set; }
	}
}