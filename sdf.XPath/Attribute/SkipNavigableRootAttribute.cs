using System;
using System.Xml.XPath;

namespace sdf.XPath
{
	/// <summary>
	/// Specifies that <see cref="ObjectXPathNavigator"/> must skip the root node
	/// of the inner navigator if type of member, decorated with this attribute,
	/// supports <see cref="IXPathNavigable"/> and at runtime object returs valid
	/// navigator.
	/// </summary>
	[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field )]
	public sealed class SkipNavigableRootAttribute : Attribute
	{
		/// <summary>
		/// Creates a new <see cref="SkipNavigableRootAttribute"/> instance.
		/// </summary>
		public SkipNavigableRootAttribute()
		{
		}
	}
}