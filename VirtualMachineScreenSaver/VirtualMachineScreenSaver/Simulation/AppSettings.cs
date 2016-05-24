using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using VirtualMachineScreenSaver.Utility;

namespace VirtualMachineScreenSaver.Simulation
{
	public class AppSettings
	{
		#region Fields

		private NameValueCollection _appSettings;

		#endregion

		#region Constructors

		static AppSettings()
		{
			Instance = new AppSettings();
		}

		private AppSettings(NameValueCollection appSettings)
		{
			_appSettings = appSettings;
		}

		private AppSettings()
			: this(ConfigurationManager.AppSettings)
		{
		}

		#endregion

		#region Properties

		public static AppSettings Instance { get; private set; }

		public int MsPerCycle
		{
			get
			{
				return GetSetting(() => MsPerCycle);
			}
		}

		public int MaxStackSize
		{
			get
			{
				return GetSetting(() => MaxStackSize);
			}
		}

		public int MaxThreadCount
		{
			get
			{
				return GetSetting(() => MaxThreadCount);
			}
		}

		public int RandomNoisePerCycle
		{
			get
			{
				return GetSetting(() => RandomNoisePerCycle);
			}
		}

		public int NewThreadsPerCycle
		{
			get
			{
				return GetSetting(() => NewThreadsPerCycle);
			}
		}

		public int SpawnLifeSpan
		{
			get
			{
				return GetSetting(() => SpawnLifeSpan);
			}
		}

		public int CopySlice
		{
			get
			{
				return GetSetting(() => CopySlice);
			}
		}

		public int HaltSlice
		{
			get
			{
				return GetSetting(() => HaltSlice);
			}
		}

		public int PushSlice
		{
			get
			{
				return GetSetting(() => PushSlice);
			}
		}

		public int SpawnSlice
		{
			get
			{
				return GetSetting(() => SpawnSlice);
			}
		}

		public bool ReverseEnabled
		{
			get
			{
				return GetSetting(() => ReverseEnabled);
			}
		}

		public bool RandomizeMemoryOnInitialize
		{
			get
			{
				return GetSetting(() => RandomizeMemoryOnInitialize);
			}
		}

		public bool UseCopyEffect
		{
			get
			{
				return GetSetting(() => UseCopyEffect);
			}
		}

		public bool UseThreadDeathEffect
		{
			get
			{
				return GetSetting(() => UseThreadDeathEffect);
			}
		}

		public bool UseSpawnExpireEffect
		{
			get
			{
				return GetSetting(() => UseSpawnExpireEffect);
			}
		}

		public bool ThreadReductionEnabled
		{
			get
			{
				return GetSetting(() => ThreadReductionEnabled);
			}
		}

		#endregion

		#region Methods

		protected T GetSetting<T>(Expression<Func<T>> settingProperty, T defaultValue = default(T))
		{
			var accessor = new PropertyAccessor<T>(settingProperty);
			if (_appSettings.AllKeys.Contains(accessor.Name))
			{
				var textValue = _appSettings[accessor.Name];
				return (T)Convert.ChangeType(textValue, typeof(T));
			}
			else
			{
				return defaultValue;
			}
		}

		#endregion
	}
}
