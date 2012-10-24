using System.Xml.XPath;

namespace sdf.XPath.NodePolicy
{
	internal class RootNodePolicy : INodePolicy
	{
		private object _rootObject;

		public RootNodePolicy( object rootObject )
		{
			_rootObject = rootObject;
		}

		public INodePolicy GetNewPolicy( Node node )
		{
			return null;
		}

		public string GetName( Node node )
		{
			return "#document";
		}

		public string GetNamespace( Node node )
		{
			return string.Empty;
		}

		public XPathNodeType GetNodeType( Node node )
		{
			return XPathNodeType.Root;
		}

		public string GetValue( Node node )
		{
			return string.Empty;
		}

		public bool GetIsTransparent( Node node )
		{
			return false;
		}

		public int GetAttributesCount( Node node )
		{
			return 0;
		}

		public Node GetAttribute( Node node, int index )
		{
			return null;
		}

		public int FindAttribute( Node node, string name, string ns )
		{
			return -1;
		}

		public int GetChildrenCount( Node node )
		{
			return 1;
		}

		public Node GetChild( Node node, int index )
		{
			return new Node( node.Context, node.Context.GetNodePolicy( _rootObject ) ) { Object = _rootObject };
		}
	}
}
