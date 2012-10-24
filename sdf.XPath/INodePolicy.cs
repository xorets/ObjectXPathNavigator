using System.Xml.XPath;

namespace sdf.XPath
{
	/// <summary>
	/// Defines a contract which must be implemented by any node policy object. 
	/// Node policy controls the mapping of an object to the node of XML tree.
	/// </summary>
	/// <remarks>Almost each <see cref="Node"/> object has node policy attached. 
	/// This policy controls the behavior of the node. The policy of a node
	/// could be changed during the node lifetime.<br/>
	/// It's important that each node policy object must have static GetPolicy method,
	/// which returns an instance of this policy type. It could be either a new or
	/// shared instance - it's up to the policy's factory method.<br/>
	/// Example factory method:
	/// <code>
	/// public class ListNodePolicy : GenericNodePolicy 
	/// {
	///		private static readonly INodePolicy _instance = new ListNodePolicy();
	/// 
	///		public static new INodePolicy GetPolicy()
	///		{
	///			return _instance;
	///		}
	///		
	///		...
	///	}
	/// </code>
	/// </remarks>
	public interface INodePolicy
	{
		/// <summary>
		/// Checks the state of the given node and returns a new node policy object
		/// if that node's policy must be changed.
		/// </summary>
		/// <param name="node">A node to perform operation for.</param>
		/// <returns>A node policy object or <see langword="null"/>.</returns>
		INodePolicy GetNewPolicy( Node node );
		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		/// <param name="node">A node perform operation for.</param>
		/// <returns>Returns the name of the node.</returns>
		/// <remarks>This method will be called only once. The returned value
		/// will be cached in the <see cref="Node"/> object.</remarks>
		string GetName( Node node );
		/// <summary>
		/// Gets the namespace of the node.
		/// </summary>
		/// <param name="node">A node to perform operation for.</param>
		/// <returns>Returns the namespace of the node, or <see langword="null"/> if
		/// this node has no namespace qualified.</returns>
		/// <remarks>This method will be called only once. The returned value
		/// will be cached in the <see cref="Node"/> object.</remarks>
		string GetNamespace( Node node );
		/// <summary>
		/// Gets the node type.
		/// </summary>
		/// <param name="node">A node to perform operation for.</param>
		/// <returns>Returns the type of the node. It could be attribute, element,
		/// text node, comment or root node.</returns>
		/// <remarks>This method will be called only once. The returned value
		/// will be cached in the <see cref="Node"/> object.</remarks>
		XPathNodeType GetNodeType( Node node );
		/// <summary>
		/// Gets the value of the node.
		/// </summary>
		/// <param name="node">A node to perform operation for.</param>
		/// <returns>Returns the value of the node.</returns>
		/// <remarks>This method will be called only once. The returned value
		/// will be cached in the <see cref="Node"/> object.</remarks>
		string GetValue( Node node );
		/// <summary>
		/// Gets the transparency flag for the node.
		/// </summary>
		/// <param name="node">A node to perform operation for.</param>
		/// <returns>Returns <see langword="true"/>, if node must be transparent 
		/// (i.e. skipped during tree traversal), and <see langword="false"/>, if not.</returns>
		/// <remarks>This method will be called only once. The returned value
		/// will be cached in the <see cref="Node"/> object.</remarks>
		bool GetIsTransparent( Node node );
		/// <summary>
		/// Gets the number of the attributes the node has.
		/// </summary>
		/// <param name="node">A node to perform operation for.</param>
		/// <returns>Returns the number of the attributes the node has, or <c>0</c>
		/// if it hasn't.</returns>
		/// <remarks>This method will be called only once. The returned value
		/// will be cached in the <see cref="Node"/> object.</remarks>
		int GetAttributesCount( Node node );
		/// <summary>
		/// Gets the node's attribute with specified index.
		/// </summary>
		/// <param name="node">A node to perform operation for.</param>
		/// <param name="index">Zero-based index of the attribute.</param>
		/// <returns>Returns the attribute node or <see langword="null"/>.</returns>
		/// <remarks>This method will be called only once. The returned value
		/// will be cached in the <see cref="Node"/> object.</remarks>
		Node GetAttribute( Node node, int index );
		/// <summary>
		/// Finds the index of attribute with given name and namespace.
		/// </summary>
		/// <param name="node">A node to perform operation for.</param>
		/// <param name="name">The name of the attribute.</param>
		/// <param name="ns">The namespace of the attribute.</param>
		/// <returns>Returns a zero-based index of desired attribute of the node, or
		/// <c>-1</c> if no such attribute is present.</returns>
		/// <remarks>This method will be called only once. The returned value
		/// will be cached in the <see cref="Node"/> object.</remarks>
		int FindAttribute( Node node, string name, string ns );
		/// <summary>
		/// Gets the number of the children the node has .
		/// </summary>
		/// <param name="node">A node to perform operation for.</param>
		/// <returns>Returns a number of the node's children (elements, text nodes,
		/// comments), or <c>0</c> if node has no children.</returns>
		/// <remarks>This method will be called only once. The returned value
		/// will be cached in the <see cref="Node"/> object.</remarks>
		int GetChildrenCount( Node node );
		/// <summary>
		/// Gets the node's child with given index.
		/// </summary>
		/// <param name="node">A node to perform operation for.</param>
		/// <param name="index">Zero-based index of the child.</param>
		/// <returns>Returns a child node or <see langword="null"/> if no child node
		/// with given index is present.</returns>
		/// <remarks>This method will be called only once. The returned value
		/// will be cached in the <see cref="Node"/> object.</remarks>
		Node GetChild( Node node, int index );
	}
}
