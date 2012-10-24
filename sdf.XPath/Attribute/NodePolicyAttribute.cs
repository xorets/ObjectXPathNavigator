using System;

namespace sdf.XPath
{
	/// <summary>
	/// Specifies a type that must be used as the policy for a node in object 
	/// navigation tree, which corresponds to an instance of the type decorated 
	/// with this attribute.
	/// </summary>
	/// <remarks>
	/// Node policy type must implement the <see cref="INodePolicy"/> interface.
	/// </remarks>
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field )]
	public sealed class NodePolicyAttribute : Attribute
	{
		private Type _proxyType;

		/// <summary>
		/// Initializes a new instance of NodePolicyAttribute class.
		/// </summary>
		/// <param name="proxyType">The type of node policy.</param>
		/// <remarks>
		/// Node policy type must implement the <see cref="INodePolicy"/> interface.
		/// </remarks>
		public NodePolicyAttribute( Type proxyType ) : base()
		{
			if( ( typeof( INodePolicy )).IsAssignableFrom( proxyType ) )
				_proxyType = proxyType;
			else
				throw new ArgumentException( "Type must implement INodePolicy interface.", "proxyType" );
		}

		/// <summary>
		/// Gets a type of node policy for decorated type.
		/// </summary>
		public Type NodePolicyType
		{
			get { return _proxyType; }
		}
	}

}
