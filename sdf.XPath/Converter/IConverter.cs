namespace sdf.XPath
{
	/// <summary>
	/// Allows for converting of an object to <see cref="System.String"/> and back.
	/// </summary>
	/// <remarks>
	/// Generally, a specific converter implementation is intended for converting
	/// values of certain type (or limited number of types).
	/// Converter must know how to convert an object to a string. Implementation of 
	/// reverse operation is optional. If you decide not to implement it, throw
	/// <see cref="System.NotImplementedException"/>.<br/>
	/// <b>Note for implementors</b><br/> 
	/// Your implementation <b>must be thread-safe</b>.<br/>
	/// Your converter class <b>must have</b> a public constructor with argument
	/// of <see cref="System.Type"/> type.</remarks>
	/// <threadsafety static="true" instance="true"/>
	public interface IConverter
	{
		/// <summary>
		/// Converts a given object to string representation.
		/// </summary>
		/// <param name="obj">An object to convert to string.</param>
		/// <returns>Returns string representation of the object.</returns>
		string ToString( object obj );
		/// <summary>
		/// Parses a string representation of an object into an instance of corresponding
		/// type. 
		/// </summary>
		/// <param name="str">A string representing the object.</param>
		/// <returns>Returns an object, which value (or state) corresponds to given
		/// string.<br/>
		/// This method is not necessary to implement. In this case 
		/// throw the <see cref="System.NotImplementedException"/> exception.</returns>
		object ParseString( string str );
	}
}
