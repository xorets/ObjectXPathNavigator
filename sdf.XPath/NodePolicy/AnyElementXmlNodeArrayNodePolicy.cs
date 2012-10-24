using System.Xml.Serialization;

namespace sdf.XPath.NodePolicy
{
	/// <summary>
	/// Node policy for properties of type XmlNode[] decorated with
	/// <see cref="XmlAnyElementAttribute"/>.
	/// </summary>
	public class AnyElementXmlNodeArrayNodePolicy : ListNodePolicy
	{
		private static readonly INodePolicy _instance = new AnyElementXmlNodeArrayNodePolicy();

		/// <summary>
		/// Gets the node policy object.
		/// </summary>
		/// <returns>Returns an instance of this node policy.</returns>
		/// <remarks>This node policy object is stateless so all nodes shares
		/// the same instance.</remarks>
		public static new INodePolicy GetPolicy()
		{
			return _instance;
		}

		/// <summary>
		/// Creates a new <see cref="AnyElementXmlNodeArrayNodePolicy"/> instance.
		/// </summary>
		protected AnyElementXmlNodeArrayNodePolicy()
		{
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetChild"/> for details.
		/// </summary>
		public override Node GetChild( Node node, int index )
		{
			var childNode = base.GetChild( node, index );
			if( childNode != null )
				childNode.IsTransparent = true;
			return childNode;
		}
	}
}
