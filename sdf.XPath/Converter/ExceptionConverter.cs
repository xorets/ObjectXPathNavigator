using System;

namespace sdf.XPath
{
	internal class ExceptionConverter : IConverter
	{
		/// <summary>
		/// Converts an exception into string.
		/// </summary>
		/// <param name="obj">An <see cref="Exception"/> to convert.</param>
		/// <returns>Returns exception info in form "##ExceptionName: Message".</returns>
		public string ToString( object obj )
		{
			var e = (Exception)obj;
			string name = e.GetType().Name;
			if( name.EndsWith( "Exception" ) && name.Length > 9 )
				name = name.Substring( 0, name.Length - 9 );
			return string.Format( "##{0}: {1}", name, e.Message );
		}

		public object ParseString( string str )
		{
			throw new NotImplementedException();
		}
	}
}