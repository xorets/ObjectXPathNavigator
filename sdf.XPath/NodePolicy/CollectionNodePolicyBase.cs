using System.Xml.XPath;

namespace sdf.XPath.NodePolicy
{
	/// <summary>
	/// Base class from all collection nodes.
	/// </summary>
	public abstract class CollectionNodePolicyBase : GenericNodePolicy
	{
		/// <summary>
		/// Creates the child node.
		/// </summary>
		/// <param name="child">Child object.</param>
		/// <param name="node">The collection node.</param>
		/// <returns>Returns new child node representing given child object.</returns>
		protected static Node CreateChildNode( object child, Node node )
		{
			// Check if object is not null and insert a comment instead if it is
			Node childNode;
			if( child != null )
			{
				childNode = new Node( node.Context, node.Context.GetNodePolicy( child ) ) { Object = child };
				if( node.Member != null )
				{
					if( node.Member.ChildXmlElementName == null )
					{
						childNode.Name = node.Member.Name;
						childNode.Namespace = node.Member.Namespace;
					}
					else
					{
						if( node.Member.ChildXmlElementName.Name != null )
						{
							childNode.Name = node.Member.ChildXmlElementName.Name;
							if( node.Member.ChildXmlElementName.Namespace != null )
								childNode.Namespace = node.Member.ChildXmlElementName.Namespace;
						}
					}
				}
			}
			else
				childNode = new Node( node.Context, null ) { NodeType = XPathNodeType.Comment, Value = "Null reference" };
			return childNode;
		}
	}
}