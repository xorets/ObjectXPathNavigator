using System.Xml.XPath;

namespace sdf.XPath.NodePolicy
{
	/// <summary>
	/// An abstact base class for implementation of node policy.
	/// </summary>
	public abstract class NodePolicyBase : INodePolicy
	{
		/// <summary>
		/// See <see cref="INodePolicy.GetNewPolicy"/> for details.
		/// </summary>
		public virtual INodePolicy GetNewPolicy( Node node )
		{
			return null;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetName"/> for details.
		/// </summary>
		public virtual string GetName( Node node )
		{
			return node.Member != null ? node.Member.Name : string.Empty;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetNamespace"/> for details.
		/// </summary>
		public virtual string GetNamespace( Node node )
		{
			return node.Member != null ? node.Member.Namespace : string.Empty;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetNodeType"/> for details.
		/// </summary>
		public virtual XPathNodeType GetNodeType( Node node )
		{
			return node.Member != null ? node.Member.NodeType : XPathNodeType.Element;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetValue"/> for details.
		/// </summary>
		public virtual string GetValue( Node node )
		{
			return string.Empty;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetIsTransparent"/> for details.
		/// </summary>
		public virtual bool GetIsTransparent( Node node )
		{
			return node.Member != null && node.Member.IsTransparent;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetAttributesCount"/> for details.
		/// </summary>
		public virtual int GetAttributesCount( Node node )
		{
			return 0;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetAttribute"/> for details.
		/// </summary>
		public virtual Node GetAttribute( Node node, int index )
		{
			return null;
		}

		/// <summary>
		/// See <see cref="INodePolicy.FindAttribute"/> for details.
		/// </summary>
		public virtual int FindAttribute( Node node, string name, string ns )
		{
			return -1;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetChildrenCount"/> for details.
		/// </summary>
		public virtual int GetChildrenCount( Node node )
		{
			return 0;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetChild"/> for details.
		/// </summary>
		public virtual Node GetChild( Node node, int index )
		{
			return null;
		}

		/// <summary>
		/// Checks if policy was changed.
		/// </summary>
		/// <param name="node">The node to check policy.</param>
		/// <returns><see langword="true"/> if node's policy is not the same as this
		/// one, and <see langword="false"/> otherwise.</returns>
		protected bool PolicyChanged( Node node )
		{
			return node.Policy != this;
		}
	}
}
