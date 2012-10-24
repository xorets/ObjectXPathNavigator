using System;
using System.Collections.Concurrent;

namespace sdf.XPath
{
	/// <summary>
	/// A cache of type descriptors.
	/// </summary>
	/// <remarks>
	/// Take in attention that there should be only one instance of the cache in an
	/// application. This would guarantee that each type will be described (via
	/// <see cref="TypeInfo"/> class) only once, reducing memeory usage and improving
	/// performance. <br/>
	/// To get the instance of TypeInfoCache class use <see cref="TypeInfoCache.Instance"/>
	/// property.
	/// </remarks>
	/// <threadsafety static="true" instance="true"/>
	public class TypeInfoCache
	{
		private readonly ConcurrentDictionary<Type, TypeInfo> _cache;
		private readonly TypeInfo _string;

		/// <summary>
		/// Static constructor. Initializes Instance field.
		/// </summary>
		static TypeInfoCache()
		{
			Instance = new TypeInfoCache();
		}

		/// <summary>
		/// Constructs an empty TypeInfoCache.
		/// </summary>
		private TypeInfoCache()
		{
			_cache = new ConcurrentDictionary<Type, TypeInfo>();
			_string = GetTypeInfo( typeof( string ) );
		}

		/// <summary>
		/// Provides access to TypeInfoCache singleton instance. 
		/// </summary>
		/// <value>
		/// The instance of <see cref="TypeInfoCache"/> class.
		/// </value>
		public static TypeInfoCache Instance { get; private set; }

		/// <summary>
		/// Describe the type of the given object. 
		/// </summary>
		/// <param name="o">The object to get type information for.</param>
		/// <returns>Returns an instance of <see cref="TypeInfo"/> class describing the
		/// type of the given object.</returns>
		public TypeInfo GetTypeInfo( object o )
		{
			return GetTypeInfo( o.GetType() );
		}

		/// <summary>
		/// Describe the given type.
		/// </summary>
		/// <param name="type">The type to get information for.</param>
		/// <returns>Returns an instance of <see cref="TypeInfo"/> class describing the
		/// given type.</returns>
		public TypeInfo GetTypeInfo( Type type )
		{
			return _cache.GetOrAdd( type, t => new TypeInfo( t ) );
		}

		/// <summary>
		/// Get type info for <see cref="System.String"/> type.
		/// </summary>
		/// <remarks>
		/// For performance reasons type information for <see cref="System.String"/>
		/// is cached separately.
		/// </remarks>
		public TypeInfo StringTypeInfo
		{
			get { return _string; }
		}
	}
}