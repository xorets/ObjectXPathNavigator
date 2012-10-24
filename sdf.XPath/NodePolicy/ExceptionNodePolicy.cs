using System.Xml.XPath;

namespace sdf.XPath.NodePolicy
{
	/// <summary>
	/// Implementation of node policy for handling nodes with member access 
	/// exceptions.
	/// </summary>
	public class ExceptionNodePolicy : NodePolicyBase
	{
		private static readonly INodePolicy _instance = new ExceptionNodePolicy();

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
		/// Creates a new <see cref="ExceptionNodePolicy"/> instance.
		/// </summary>
		protected ExceptionNodePolicy() { }

		/// <summary>
		/// See <see cref="INodePolicy.GetValue"/> for details.
		/// </summary>
		public override string GetValue( Node node )
		{
			return node.Context.ConverterFactory.GetConverter( node.AccessException.GetType() ).ToString( node.AccessException );
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetChildrenCount"/> for details.
		/// </summary>
		public override int GetChildrenCount( Node node )
		{
			return 1;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetChild"/> for details.
		/// </summary>
		public override Node GetChild( Node node, int index )
		{
			return index == 0 ? new Node( node.Context, InnerNodePolicy.GetPolicy() ) { NodeType = XPathNodeType.Comment } : null;
		}
	}
}
