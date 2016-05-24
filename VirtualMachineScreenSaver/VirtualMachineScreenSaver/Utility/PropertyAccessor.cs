using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace VirtualMachineScreenSaver.Utility
{
	/// <summary>
	/// Use expressions to dynamically access information on a field or property.
	/// </summary>
	/// <typeparam name="T">The type of the property being accessed.</typeparam>
	public class PropertyAccessor<T>
	{
		#region Variables

		private MemberInfo _member;
		private Func<T> _getValue;
		private Action<T> _setValue;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyAccessor{T}"/> class.
		/// </summary>
		/// <param name="property">An expression that accesses the property.</param>
		/// <exception cref="System.ArgumentException">Expression must be a member accessor.;property</exception>
		public PropertyAccessor(Expression<Func<T>> property)
		{
			var body = property.Body as MemberExpression;
			if (body == null)
			{
				throw new ArgumentException("Expression must be a member accessor.", "property");
			}

			_member = body.Member;
			Name = _member.Name;

			Description = _member.GetCustomAttributes(true)
				.Where(x => x is DescriptionAttribute)
				.Select(x => (x as DescriptionAttribute).Description)
				.DefaultIfEmpty(Name)
				.FirstOrDefault();

			_getValue = property.Compile();

			var newValue = Expression.Parameter(typeof(T), "newValue");

			if (IsWriteable(body.Member))
			{
				_setValue = Expression.Lambda<Action<T>>(Expression.Assign(body, newValue), newValue).Compile();
			}
			else
			{
				_setValue = null;
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// The type of the property being described.
		/// </summary>
		public Type Type
		{
			get
			{
				return typeof(T);
			}
		}

		/// <summary>
		/// The name of the property being described.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Get or set the value of the property.
		/// </summary>
		public T Value
		{
			get
			{
				return _getValue();
			}
			set
			{
				if (_setValue == null)
				{
					throw new InvalidOperationException("This property is not writeable.");
				}
				else
				{
					_setValue(value);
				}
			}
		}

		/// <summary>
		/// The description of the property.
		/// </summary>
		/// <remarks>
		/// This is pulled from the Description attribute if available; otherwise it is set to the property name.
		/// </remarks>
		public string Description { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether this property is defined on <paramref name="type" />.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns><c>true</c> if the property is defined on <paramref name="type" />, otherwise <c>false</c>.</returns>
		public bool IsDefinedOn(Type type)
		{
			return _member.DeclaringType.Equals(type);
		}

		private static bool IsProperty(MemberInfo member)
		{
			return member.MemberType == MemberTypes.Property;
		}

		private static bool IsField(MemberInfo member)
		{
			return member.MemberType == MemberTypes.Field;
		}

		private static PropertyInfo AsProperty(MemberInfo member)
		{
			if (!IsProperty(member))
			{
				return null;
			}
			else
			{
				return member as PropertyInfo;
			}
		}

		private static bool IsWriteable(MemberInfo member)
		{
			return IsField(member) || AsProperty(member).CanWrite;
		}

		#endregion
	}
}