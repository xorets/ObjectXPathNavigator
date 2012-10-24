using System.Xml.XPath;

namespace sdf.XPath.NodePolicy
{
	/// <summary>
	/// Node policy handling inner text or comments node.
	/// </summary>
	public class InnerNodePolicy : INodePolicy
	{
		private static readonly INodePolicy _instance = new InnerNodePolicy();

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
		/// Creates a new <see cref="InnerNodePolicy"/> instance.
		/// </summary>
		protected InnerNodePolicy() { }

		/// <summary>
		/// See <see cref="INodePolicy.GetNewPolicy"/> for details.
		/// </summary>
		public INodePolicy GetNewPolicy( Node node )
		{
			return null;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetName"/> for details.
		/// </summary>
		public string GetName( Node node )
		{
			return string.Empty;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetNamespace"/> for details.
		/// </summary>
		public string GetNamespace( Node node )
		{
			return string.Empty;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetNodeType"/> for details.
		/// </summary>
		public XPathNodeType GetNodeType( Node node )
		{
			return XPathNodeType.Text;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetValue"/> for details.
		/// </summary>
		public string GetValue( Node node )
		{
			return node.Parent.Value;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetIsTransparent"/> for details.
		/// </summary>
		public bool GetIsTransparent( Node node )
		{
			return false;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetAttributesCount"/> for details.
		/// </summary>
		public int GetAttributesCount( Node node )
		{
			return 0;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetAttribute"/> for details.
		/// </summary>
		public Node GetAttribute( Node node, int index )
		{
			return null;
		}

		/// <summary>
		/// See <see cref="INodePolicy.FindAttribute"/> for details.
		/// </summary>
		public int FindAttribute( Node node, string name, string ns )
		{
			return -1;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetChildrenCount"/> for details.
		/// </summary>
		public int GetChildrenCount( Node node )
		{
			return 0;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetChild"/> for details.
		/// </summary>
		public Node GetChild( Node node, int index )
		{
			return null;
		}
	}
}
