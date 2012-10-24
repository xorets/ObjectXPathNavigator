using System.Xml.XPath;

namespace sdf.XPath.NodePolicy
{
	/// <summary>
	/// Node policy for handling member nodes resolved to <see langword="null"/>.
	/// </summary>
	public class NullValueNodePolicy : NodePolicyBase
	{
		private static readonly INodePolicy _instance = new NullValueNodePolicy();

		/// <summary>
		/// Gets the node policy object.
		/// </summary>
		/// <returns>Returns an instance of this node policy.</returns>
		/// <remarks>This node policy object is stateless so all nodes shares
		/// the same instance.</remarks>
		public static INodePolicy GetPolicy()
		{
			return _instance;
		}

		/// <summary>
		/// Creates a new <see cref="NullValueNodePolicy"/> instance.
		/// </summary>
		protected NullValueNodePolicy() { }

		/// <summary>
		/// See <see cref="INodePolicy.GetIsTransparent"/> for details.
		/// </summary>
		public override bool GetIsTransparent( Node node )
		{
			return node.NodeType == XPathNodeType.Attribute || !node.Member.IsNullable;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetAttributesCount"/> for details.
		/// </summary>
		public override int GetAttributesCount( Node node )
		{
			return node.Member.IsNullable ? 1 : 0;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetAttribute"/> for details.
		/// </summary>
		public override Node GetAttribute( Node node, int index )
		{
			return node.Member.IsNullable && index == 0
				? new Node( node.Context, null )
					{
						NodeType = XPathNodeType.Attribute,
						Name = "nil",
						Namespace = ObjectXPathContext.Xsi,
						Value = "true"
					}
				: null;
		}

		/// <summary>
		/// See <see cref="INodePolicy.FindAttribute"/> for details.
		/// </summary>
		public override int FindAttribute( Node node, string name, string ns )
		{
			return node.Member.IsNullable && name == "nil" && ns == ObjectXPathContext.Xsi ? 0 : -1;
		}
	}
}
