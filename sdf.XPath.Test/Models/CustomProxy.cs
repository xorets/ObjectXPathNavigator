using System.Xml.XPath;
using sdf.XPath.NodePolicy;

namespace sdf.XPath.Test.Models
{
	/// <exclude/>
	public class CustomProxy : GenericNodePolicy
	{
		private static CustomProxy _instance = new CustomProxy();

		public static new INodePolicy GetPolicy()
		{
			return _instance;
		}

		public override int GetChildrenCount( Node node )
		{
			return base.GetChildrenCount( node ) + 1;
		}

		public override Node GetChild( Node node, int index )
		{
			Node n;
			if( index == base.GetChildrenCount( node ) )
			{
				n = new Node( node.Context, null );
				n.Value = "Uhus sux!";
				n.NodeType = XPathNodeType.Text;
			}
			else
				n = base.GetChild( node, index );

			return n;
		}
	}
}