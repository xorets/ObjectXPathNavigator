using System;
using System.Collections;

namespace sdf.XPath
{
	/// <summary>
	/// Used to get converters for types involved in tree traversal.
	/// </summary>
	/// <remarks>
	/// To get the instance of
	/// the type use the <see cref="ObjectXPathContext.ConverterFactory"/> property.
	/// </remarks>
	/// <threadsafety static="true" instance="true"/>
	public class ConverterFactory
	{
		private IConverter _genericConverter;
		private IConverter _emptyConverter;
		private Hashtable _cache;

		/// <summary>
		/// Initializes a new instance of <see cref="ConverterFactory"/> class.
		/// </summary>
		/// <remarks>
		/// The converter factory should not be constructed from your code. The 
		/// constructor is public only for test reasons. To get the instance of
		/// the type use the <see cref="ObjectXPathContext.ConverterFactory"/> property.
		/// </remarks>
		public ConverterFactory()
		{
			_genericConverter = new GenericConverter();
			_emptyConverter = new EmptyConverter();
			_cache = new Hashtable();
			AddConverter( typeof( bool ), new BooleanConverter() );
			AddConverter( typeof( DateTime ), new DateTimeConverter() );
			AddConverter( typeof( Single ), new SingleConverter() );
			AddConverter( typeof( Double ), new DoubleConverter() );
			AddConverter( typeof( Decimal ), new DecimalConverter() );
			AddConverter( typeof( Exception ), new ExceptionConverter() );
		}

		/// <summary>
		/// Gets a converter for specified type.
		/// </summary>
		/// <param name="forType">Type of objects the converter will be work with.</param>
		/// <returns>Returns the converter which knows how to convert objects of
		/// specified type.</returns>
		/// <remarks>
		/// The type of converter to create is determined at the following:
		/// <list type="bullet">
		/// <item><description>if <paramref name="forType"/> is enum, then enum conveter
		/// will be returned;</description></item>
		/// <item><description>if it has the <see cref="ConverterAttribute"/> 
		/// specified, then declared converter type will be used;</description></item>
		/// <item><description>if this is primitive type, generic converter;</description></item>
		/// <item><description>if no specific converter found, empty converter will
		/// be returned.</description></item>
		/// </list><br/>
		/// The factory caches creates convertes so several trheads could get the same
		/// converter object simultaneously.
		/// </remarks>
		public IConverter GetConverter( Type forType )
		{
			var converter = (IConverter)_cache[forType];
			if( converter == null )
				if( forType.IsEnum )
					// Build new enum converter
					converter = StoreConverter( forType, new EnumConverter( forType ) );
				else if( typeof( Exception ).IsAssignableFrom( forType ) )
					converter = GetConverter( typeof( Exception ) );
				else
				{
					// Find converter for this type
					var typeInfo = TypeInfoCache.Instance.GetTypeInfo( forType );
					var convType = typeInfo.ConverterType;
					if( convType == null )
						// Look in implemented interfaces
						foreach( var iface in forType.GetInterfaces() )
						{
							convType = TypeInfoCache.Instance.GetTypeInfo( iface ).ConverterType;
							if( convType != null )
								break;
						}
					if( convType != null )
					{
						converter = CreateConverter( convType, forType );
						converter = StoreConverter( forType, converter );
					}
				}

			return converter ?? _genericConverter;
		}

		/// <summary>
		/// Gets an instance of specified converter type initialized for given type.
		/// </summary>
		/// <param name="converterType">Type of converter.</param>
		/// <param name="argumentType">Type of objects that will be converted.</param>
		/// <returns>Returns the converter of given type, that is ready to work
		/// with object of <paramref name="argumentType"/> type.</returns>
		/// <remarks>
		/// Converter type must implement <see cref="IConverter"/> interface and has
		/// public constructor with argument of <see cref="System.Type"/> type.
		/// The factory caches creates convertes so several trheads could get the same
		/// converter object simultaneously.
		/// </remarks>
		public IConverter GetConverter( Type converterType, Type argumentType )
		{
			var converterKey = new ConverterKey( converterType, argumentType );

			var converter = (IConverter)_cache[converterKey];
			if( converter == null )
			{
				converter = CreateConverter( converterType, argumentType );
				converter = StoreConverter( converterKey, converter );
			}
			return converter;
		}

		/// <summary>
		/// Removes a converter for the specified type from cache. 
		/// </summary>
		/// <param name="forType">Converter for which type should be removed.</param>
		/// <returns>Returns the converter instance that was just removed, or 
		/// <see langword="null"/> if converter for this type was not cached.</returns>
		public IConverter RemoveConverter( Type forType )
		{
			var converter = (IConverter)_cache[forType];
			if( converter != null )
			{
				lock( _cache.SyncRoot )
				{
					if( _cache.Contains( forType ) )
						_cache.Remove( forType );
					else
						// If someone has removed converter during evaluation
						converter = null;
				}
			}
			return converter;
		}

		/// <summary>
		/// Adds the converter to cache.
		/// </summary>
		/// <param name="forType">For which type this converter intended.</param>
		/// <param name="converter">The converter object.</param>
		/// <returns>Returns a converter object from cache.</returns>
		/// <remarks>If cache already had the converter for this type, then state
		/// of the cache will not be altered, old cached converter will be returned.
		/// </remarks>
		public IConverter AddConverter( Type forType, IConverter converter )
		{
			return (IConverter)_cache[forType] ?? StoreConverter( forType, converter );
		}

		/// <summary>
		/// Gets the generic converter.
		/// </summary>
		/// <value>Converter which uses <see cref="Object.ToString"/> method.</value>
		public IConverter GenericConverter
		{
			get { return _genericConverter; }
		}

		/// <summary>
		/// Gets the empty converter.
		/// </summary>
		/// <value>Converter which always return <see cref="String.Empty"/>.</value>
		public IConverter EmptyConverter
		{
			get { return _emptyConverter; }
		}

		private static IConverter CreateConverter( Type converterType, Type argumentType )
		{
			IConverter converter;
			try
			{
				converter = (IConverter)Activator.CreateInstance( converterType, new object[] { argumentType } );
			}
			catch( MissingMethodException e )
			{
				throw new ArgumentException(
					string.Format( "Converter type {0} must implement public constructor with argument of System.Type type.",
								   converterType.Name ), "converterType", e );
			}
			return converter;
		}

		private IConverter StoreConverter( object key, IConverter converter )
		{
			IConverter storeConverter;
			lock( _cache.SyncRoot )
			{
				if( !_cache.Contains( key ) )
				{
					storeConverter = converter;
					_cache.Add( key, storeConverter );
				}
				else
					// If someone has added an info during evaluation
					storeConverter = (IConverter)_cache[key];
			}
			return storeConverter;
		}

		private class ConverterKey
		{
			public Type ConverterType;
			public Type ForType;

			public ConverterKey( Type converterType, Type forType )
			{
				ConverterType = converterType;
				ForType = forType;
			}

			public override bool Equals( object obj )
			{
				var other = obj as ConverterKey;
				return other != null && ( other.ConverterType == ConverterType && other.ForType == ForType );
			}

			public override int GetHashCode()
			{
				return (int)( ( (long)ConverterType.GetHashCode() + ForType.GetHashCode() ) % int.MaxValue );
			}
		}
	}
}
