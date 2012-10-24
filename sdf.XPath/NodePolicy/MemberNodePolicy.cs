using System;
using System.Xml.XPath;

namespace sdf.XPath.NodePolicy
{
	/// <summary>
	/// Node policy for a node created for a still unresolved object's field or 
	/// property.
	/// </summary>
	public class MemberNodePolicy : NodePolicyBase
	{
		private static readonly INodePolicy _instance = new MemberNodePolicy();

		/// <summary>
		/// Creates a new <see cref="MemberNodePolicy"/> instance.
		/// </summary>
		protected MemberNodePolicy() { }

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
		/// See <see cref="INodePolicy.GetNewPolicy"/> for details.
		/// </summary>
		public override INodePolicy GetNewPolicy( Node node )
		{
			// Check if we have to switch the policy
			if( node.State == NodeState.ObjectKnown )
			{
				if( node.Object != null )
				{
					if( !node.ObjectType.IsSimpleType && node.Member.ConverterType == null )
					{
						// Create policy for a node that is not of simple type and
						// without explicitly set converter
						string s = node.Name;
						s = node.Namespace;
						return node.Context.GetNodePolicy( node.Object );
					}
					return TextNodePolicy.GetPolicy();
				}
				return NullValueNodePolicy.GetPolicy();
			}
			return node.State == NodeState.Exception ? ExceptionNodePolicy.GetPolicy() : null;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetName"/> for details.
		/// </summary>
		public override string GetName( Node node )
		{
			return node.Member.Name;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetNamespace"/> for details.
		/// </summary>
		public override string GetNamespace( Node node )
		{
			return node.Member.Namespace;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetNodeType"/> for details.
		/// </summary>
		public override XPathNodeType GetNodeType( Node node )
		{
			return node.Member.NodeType;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetValue"/> for details.
		/// </summary>
		public override string GetValue( Node node )
		{
			// Check if we have to switch policy
			node.ResolveObject();
			return PolicyChanged( node ) ? node.Policy.GetValue( node ) : string.Empty;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetIsTransparent"/> for details.
		/// </summary>
		public override bool GetIsTransparent( Node node )
		{
			// Check if we have to switch policy
			node.ResolveObject();
			return PolicyChanged( node ) ? node.Policy.GetIsTransparent( node ) : node.Member.IsTransparent;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetAttributesCount"/> for details.
		/// </summary>
		public override int GetAttributesCount( Node node )
		{
			node.ResolveObject();
			return PolicyChanged( node ) ? node.Policy.GetAttributesCount( node ) : 0;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetAttribute"/> for details.
		/// </summary>
		public override Node GetAttribute( Node node, int index )
		{
			node.ResolveObject();
			return PolicyChanged( node ) ? node.Policy.GetAttribute( node, index ) : null;
		}

		/// <summary>
		/// See <see cref="INodePolicy.FindAttribute"/> for details.
		/// </summary>
		public override int FindAttribute( Node node, string name, string ns )
		{
			// Check if we have to switch policy
			node.ResolveObject();
			return PolicyChanged( node ) ? node.Policy.FindAttribute( node, name, ns ) : -1;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetChildrenCount"/> for details.
		/// </summary>
		public override int GetChildrenCount( Node node )
		{
			// Check if we have to switch policy
			node.ResolveObject();
			return PolicyChanged( node ) ? node.Policy.GetChildrenCount( node ) : 0;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetChild"/> for details.
		/// </summary>
		public override Node GetChild( Node node, int index )
		{
			// Check if we have to switch policy
			node.ResolveObject();
			return PolicyChanged( node ) ? node.Policy.GetChild( node, index ) : null;
		}
	}
}
