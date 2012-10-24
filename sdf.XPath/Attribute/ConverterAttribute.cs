using System;

namespace sdf.XPath
{
	/// <summary>
	/// Indicates that instances of marked type or values of marked members must be 
	/// converted to text representation with an instance of given converter type.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Field )]
	public sealed class ConverterAttribute : Attribute
	{
		private Type _converterType;

		/// <summary>
		/// Initializes a new instance of ConverterAttribute class.
		/// </summary>
		/// <param name="converterType">Type of converter to use.</param>
		/// <remarks>
		/// Type specified in <paramref name="converterType"/> parameter must implement
		/// <see cref="IConverter"/> interface and 	must have a public constructor 
		/// with argument of <see cref="System.Type"/> type.</remarks>
		public ConverterAttribute( Type converterType ) : base()
		{
			if( ( typeof( IConverter )).IsAssignableFrom( converterType ) )
				_converterType = converterType;
			else
				throw new ArgumentException( "Type must implement IConverter interface.", "proxyType" );
		}

		/// <summary>
		/// Gets the type of the converter.
		/// </summary>
		public Type ConverterType
		{
			get { return _converterType; }
		}
	}
}