namespace sdf.XPath.NodePolicy
{
	/// <summary>
	/// Generic node policy implementation.
	/// </summary>
	public class GenericNodePolicy : NodePolicyBase
	{
		private static readonly INodePolicy _instance = new GenericNodePolicy();

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
		/// Creates a new <see cref="GenericNodePolicy"/> instance.
		/// </summary>
		protected GenericNodePolicy() { }

		/// <summary>
		/// See <see cref="INodePolicy.GetNewPolicy"/> for details.
		/// </summary>
		public override INodePolicy GetNewPolicy( Node node )
		{
			return null;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetName"/> for details.
		/// </summary>
		public override string GetName( Node node )
		{
			return node.ObjectType.Name;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetNamespace"/> for details.
		/// </summary>
		public override string GetNamespace( Node node )
		{
			return node.ObjectType.Namespace;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetValue"/> for details.
		/// </summary>
		public override string GetValue( Node node )
		{
			return node.Object != null
				? node.Context.ConverterFactory.GetConverter( node.ObjectType.Type ).ToString( node.Object )
				: string.Empty;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetAttributesCount"/> for details.
		/// </summary>
		public override int GetAttributesCount( Node node )
		{
			return node.ObjectType.GetAttributes().Length;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetAttribute"/> for details.
		/// </summary>
		public override Node GetAttribute( Node node, int index )
		{
			var attributes = node.ObjectType.GetAttributes();
			return index >= 0 && index < attributes.Length
				? new Node( node.Context, MemberNodePolicy.GetPolicy() ) { Member = attributes[index] }
				: null;
		}

		/// <summary>
		/// See <see cref="INodePolicy.FindAttribute"/> for details.
		/// </summary>
		public override int FindAttribute( Node node, string name, string ns )
		{
			var attrInfo = node.ObjectType.GetAttribute( name, ns );
			return attrInfo != null ? attrInfo.Index : -1;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetChildrenCount"/> for details.
		/// </summary>
		public override int GetChildrenCount( Node node )
		{
			return node.ObjectType.GetElements().Length;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetChild"/> for details.
		/// </summary>
		public override Node GetChild( Node node, int index )
		{
			var elements = node.ObjectType.GetElements();
			if( index >= 0 && index < elements.Length )
			{
				var member = elements[index];
				var policy = member.NodePolicy != null
					? node.Context.CreateNodePolicy( member.NodePolicy.NodePolicyType )
					: MemberNodePolicy.GetPolicy();
				return new Node( node.Context, policy ) { Member = member };
			}
			return null;
		}
	}
}
