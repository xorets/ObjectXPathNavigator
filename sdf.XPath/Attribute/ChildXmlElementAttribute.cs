using System;

namespace sdf.XPath
{
	/// <summary>
	/// Specifies what name and namespace should collection elements have.
	/// </summary>
	/// <remarks>If name and namespace are not specified then name and namespace
	/// of child object's type will be taken.</remarks>
	[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field )]
	public sealed class ChildXmlElementAttribute : Attribute
	{
		private string _name;
		private string _namespace;

		/// <summary>
		/// Creates a new <see cref="ChildXmlElementAttribute"/> instance.
		/// </summary>
		public ChildXmlElementAttribute()
		{
		}

		/// <summary>
		/// Creates a new <see cref="ChildXmlElementAttribute"/> instance.
		/// </summary>
		/// <param name="name">The name which child nodes will have.</param>
		public ChildXmlElementAttribute( string name )
		{
			_name = name;
		}

		/// <summary>
		/// Gets or sets the name of child nodes.
		/// </summary>
		/// <value>Name of child nodes.</value>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets the namespace of child nodes.
		/// </summary>
		/// <value>Namespace of child nodes.</value>
		/// <remarks>If name is set and namespace is not, then empty namespace  
		/// will be taken, not type's namespace.</remarks>
		public string Namespace
		{
			get { return _namespace; }
			set { _namespace = value; }
		}
	}
}
