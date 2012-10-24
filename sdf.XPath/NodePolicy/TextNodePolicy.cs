namespace sdf.XPath.NodePolicy
{
	/// <summary>
	/// Node policy for nodes related to objects of simple type, or nodes with 
	/// converter explicitely defined.
	/// </summary>
	public class TextNodePolicy : NodePolicyBase
	{
		private static readonly INodePolicy _instance = new TextNodePolicy();

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
		/// Creates a new <see cref="TextNodePolicy"/> instance.
		/// </summary>
		protected TextNodePolicy() { }

		/// <summary>
		/// See <see cref="INodePolicy.GetValue"/> for details.
		/// </summary>
		public override string GetValue( Node node )
		{
			var c = node.Member != null && node.Member.ConverterType != null
				? node.Context.ConverterFactory.GetConverter( node.Member.ConverterType, node.Member.Type )
				: node.Context.ConverterFactory.GetConverter( node.ObjectType.Type );
			return c.ToString( node.Object );
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
			return index == 0 ? new Node( node.Context, InnerNodePolicy.GetPolicy() ) : null;
		}
	}
}
