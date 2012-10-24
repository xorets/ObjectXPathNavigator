using System;

namespace sdf.XPath
{
	/// <summary>
	/// Provides data for <see cref="ObjectXPathContext.LoopDetected"/> event.
	/// </summary>
	public class LoopDetectionEventArgs : EventArgs
	{
		private readonly ObjectXPathNavigator _navigator;
		private bool _ignoreLoop;
		private readonly Node _node;

		/// <summary>
		/// Creates a new <see cref="LoopDetectionEventArgs"/> instance.
		/// </summary>
		/// <param name="navigator"><see cref="ObjectXPathNavigator"/> Navigator object</param>
		/// <param name="node"><see cref="Node"/> Node that caused loop appearance</param>
		public LoopDetectionEventArgs( ObjectXPathNavigator navigator, Node node )
		{
			_navigator = navigator;
			_node = node;
			_ignoreLoop = false;
		}

		/// <summary>
		/// Navigator loop to be detected in
		/// </summary>
		public ObjectXPathNavigator Navigator
		{
			get { return _navigator; }
		}

		/// <summary>
		/// If set to true Navigator will ignore loop warning 
		/// </summary>
		public bool IgnoreLoop
		{
			get { return _ignoreLoop; }
			set { _ignoreLoop = value; }
		}

		/// <summary>
		/// Node caused loop appearance
		/// </summary>
		public Node Node
		{
			get { return _node; }
		}
	}
}
