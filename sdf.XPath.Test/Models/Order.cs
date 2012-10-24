using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Xml.Serialization;

namespace sdf.XPath.Test.Models
{
	public class TypeCode
	{
		private short id_;
		private string description_;

		public TypeCode() {}

		public TypeCode(short id, string description)
		{
			this.Value = id;
			this.Description = description;
		}

		[XmlAttribute("tc")]
		public short Value 
		{
			get { return id_; }
			set { id_ = value; }
		}

		[XmlText(DataType="token")]
		public string Description
		{
			get { return description_; }
			set { description_ = value; }
		}
	}

	/// <summary>
	/// A request for products.
	/// </summary>
	[XmlRoot("Order",
		 Namespace="http://emsinet.com/schemas/Order/Data")]
	public class Order
	{	
		private int id_;
		private DateTime submittedDate_;
		private string clientOrderId_;
		private IDictionary requirements_;
		private IDictionary transactions_;

		public Order()
		{}

		public Order( bool init )
		{
			ClientOrderId = "1234";
			SubmittedDate = new DateTime(2004, 1, 19, 3, 50, 0, 0);
			Requirement aps = new Requirement();
			aps.ClientUniqueId = "2468";
			aps.ProcessingUniqueId = "923232";
			aps.Type = new RequirementType(11, "APS");
			aps.RequestedDate = new DateTime(2005, 1, 29, 0, 56, 0, 0);
			aps.CompletedDate = aps.RequestedDate.AddDays(3);
			aps.State = new RequirementState();
			aps.State.StateCode = new RequirementStateCode(75, "Accepted");
			aps.State.SubStateCode = new RequirementSubStateCode(3, "Reviewing");
			aps.Order = this;
			Requirement mvr = new Requirement();
			mvr.ClientUniqueId = "124692";
			mvr.RequestedDate = new DateTime(2004, 1, 1, 19, 20, 0, 0);
			mvr.ProcessingUniqueId = "18342";
			mvr.Type = new RequirementType(4, "MVR");
			mvr.State = new RequirementState();
			mvr.State.StateCode = new RequirementStateCode(3, "Completed");
			mvr.Order = this;
			OrderTransaction tx = new OrderTransaction();
			tx.Type = new OrderTransactionType(1, "Create");
			tx.Timestamp = new DateTime(2004, 1, 1, 19, 46, 13);
			tx.ConfirmationNumber = "123456";
			XmlDocument clientRequest = new XmlDocument();
			clientRequest.LoadXml(@"<AnyClientRequest foo='bar' xmlns='kaitlyn'>Brenda</AnyClientRequest>");
			tx.ClientRequest = clientRequest.DocumentElement;
			tx.Order = this;
		}

		/// <summary>
		/// Gets/sets the order id.
		/// </summary>
		[XmlElement("OrderId",
			 Namespace="http://emsinet.com/schemas/Order/Data")]
		public int Id
		{
			get { return id_; }
			set { id_ = value; }
		}

		/// <summary>
		/// Gets/set the client supplied order id.
		/// </summary>
		[XmlElement("ClientOrderId",
			 Namespace="http://emsinet.com/schemas/Order/Data")] 
		public string ClientOrderId 
		{
			get { return clientOrderId_; }
			set { clientOrderId_ = value; }
		}

		/// <summary>
		/// Gets/set the date the order was submitted.
		/// </summary>
		[XmlElement("SubmittedDate",
			 Namespace="http://emsinet.com/schemas/Order/Data")]
		public DateTime SubmittedDate
		{
			get { return submittedDate_; } 
			set { submittedDate_ = value; }
		}

		/// <summary>
		/// Gets/sets the requirements of this order.
		/// </summary>
		[XmlAnyElement("Requirement",
			 Namespace="http://emsinet.com/schemas/Order/Data")]
		public IDictionary Requirements
		{
			get
			{
				if (requirements_ == null)
					requirements_ = new ListDictionary();
				return requirements_;
			}
			set { requirements_ = value; }
		}

		/// <summary>
		/// Gets/sets the transactions applied to this order.
		/// </summary>
		[XmlAnyElement("Transaction",
			 Namespace="http://emsinet.com/schemas/Order/Data")]
		public IDictionary Transactions
		{
			get
			{
				if (transactions_ == null)
					transactions_ = new ListDictionary();
				return transactions_;
			}
			set { transactions_ = value; }
		}
	}

	public class RequirementType : TypeCode
	{
		private TypeCode category_;

		/// Constructs an empty RequirementType.
		/// </summary>
		public RequirementType()
		{
			// empty
		}

		/// <summary>
		/// Constructs an initial RequirementType.
		/// </summary>
		/// <param name="name">The RequirementType name.</param>
		/// <param name="id">The RequirementType id.</param>
		public RequirementType(short id, string description)
			: base(id, description)
		{
			// empty
		}

		[XmlElement("Category",
			 Namespace="http://emsinet.com/schemas/Order/Data")]
		[XmlIgnore]
		public TypeCode Category
		{
			get { return category_; }
			set { category_ = value; }
		}
	}

	public class RequirementStateCode : TypeCode
	{
		/// Constructs an empty RequirementStateCode.
		/// </summary>
		public RequirementStateCode()
		{
			// empty
		}

		/// <summary>
		/// Constructs an initial RequirementStateCode.
		/// </summary>
		/// <param name="name">The RequirementStateCode name.</param>
		/// <param name="id">The RequirementStateCode id.</param>
		public RequirementStateCode(short id, string description)
			: base(id, description)
		{
			// empty
		}
	}

	public class RequirementSubStateCode : TypeCode
	{
		/// Constructs an empty RequirementSubStateCode.
		/// </summary>
		public RequirementSubStateCode()
		{
			// empty
		}

		/// <summary>
		/// Constructs an initial RequirementSubStateCode.
		/// </summary>
		/// <param name="name">The RequirementSubStateCode name.</param>
		/// <param name="id">The RequirementSubStateCode id.</param>
		public RequirementSubStateCode(short id, string description)
			: base(id, description)
		{
			// empty
		}
	}

	/// <summary>
	/// Represents the state of a requirement.
	/// </summary>
	public class RequirementState
	{
		private RequirementStateCode stateCode_;
		private RequirementSubStateCode subStateCode_;

		/// <summary>
		/// Gets/set the requirements state code.
		/// </summary>
		[XmlElement("Code",
			 Namespace="http://emsinet.com/schemas/Order/Data")]
		public RequirementStateCode StateCode 
		{
			get { return stateCode_; }
			set { stateCode_ = value; } 
		}

		/// <summary>
		/// Gets/set the requirements sub state code.
		/// </summary>
		[XmlElement("SubCode",
			 Namespace="http://emsinet.com/schemas/Order/Data",
			 IsNullable=true)]
		public RequirementSubStateCode SubStateCode 
		{ 
			get { return subStateCode_; }
			set { subStateCode_ = value; }
		}
	}

	[XmlRoot("Requirement",
		 Namespace="http://emsinet.com/schemas/Order/Data")]
	public class Requirement
	{
		private int id_;
		private Order order_;
		private RequirementType type_;
		private string clientUniqueId_;
		private string processingUniqueId_;
		private DateTime requestedDate_;
		private DateTime completedDate_;
		private RequirementState state_;
		private IDictionary events_;

		/// <summary>
		/// Gets/sets the requirement type id.
		/// </summary>
		[XmlIgnore]
		public int Id
		{
			get { return id_; }
			set { id_ = value; }
		}

		/// <summary>
		/// Gets/set the order this requirement belongs to.
		/// </summary>
		[XmlIgnore]
		public Order Order 
		{
			get { return order_; }
			set 
			{ 
				order_ = value;
				if (!(order_ == null ||
					order_.Requirements.Contains(this)))
				{
					order_.Requirements.Add(this, this);
				}
			}
		}

		/// <summary>
		/// Gets/sets the requirement type.
		/// </summary>
		[XmlElement("Type",
			 Namespace="http://emsinet.com/schemas/Order/Data")]	
		public RequirementType Type 
		{
			get { return type_; }
			set { type_ = value; }
		}
		
		/// <summary>
		/// Gets/set the clients unique id.
		/// </summary>
		[XmlElement("ClientUniqueId",
			 Namespace="http://emsinet.com/schemas/Order/Data")]
		public string ClientUniqueId
		{
			get { return clientUniqueId_; } 
			set { clientUniqueId_ = value; }
		}

		/// <summary>
		/// Gets/sets the requested date.
		/// </summary>
		[XmlIgnore]
		public DateTime RequestedDate
		{ 
			get { return requestedDate_; }
			set { requestedDate_ = value; }
		}

		/// <summary>
		/// Gets/sets the completed date.
		/// </summary>
		[XmlIgnore]
		public DateTime CompletedDate
		{
			get { return completedDate_; }
			set { completedDate_ = value; }
		}

		/// <summary>
		/// Gets/set the processing unique id.
		/// </summary>
		[XmlElement("ProcessingUniqueId",
			 Namespace="http://emsinet.com/schemas/Order/Data")]
		public string ProcessingUniqueId
		{
			get { return processingUniqueId_; } 
			set { processingUniqueId_ = value; }
		}

		/// <summary>
		/// Gets/set the requirements state.
		/// </summary>
		[XmlElement("State",
			 Namespace="http://emsinet.com/schemas/Order/Data")]
		public RequirementState State 
		{
			get 
			{ 
				if (state_ == null)
					state_ = new RequirementState();
				return state_; 
			} 
			set { state_ = value; }
		}

		/// <summary>
		/// Gets/sets the requirement events.
		/// </summary>
		[XmlIgnore]
		public IDictionary Events 
		{
			get
			{
				if (events_ == null)
					events_ = new Hashtable();
				return events_;
			}
			set { events_ = value; }
		}
	}

	/// <summary>
	/// A transaction applied to an order.
	/// </summary>
	[XmlRoot("OrderTransaction",
		 Namespace="http://emsinet.com/schemas/Order/Data")]
	public class OrderTransaction
	{
		private Order order_;
		private DateTime timestamp_;
		private OrderTransactionType type_;
		private string confirmationNumber_;
		private XmlElement clientRequest_;

		/// Gets/sets the related order.
		/// </summary>
		[XmlIgnore]
		public Order Order
		{
			get { return order_; }
			set 
			{ 
				order_ = value;
				if (!(order_ == null || 
					order_.Transactions.Contains(this)))
				{
					order_.Transactions.Add(order_.Transactions.Count, this);
				}
			}
		}

		/// <summary>
		/// Gets/set the transaction timestamp.
		/// </summary>
		[XmlElement("Timestamp",
			 Namespace="http://emsinet.com/schemas/Order/Data")] 
		public DateTime Timestamp 
		{
			get { return timestamp_; } 
			set { timestamp_ = value; }
		}

		[XmlElement("Type",
			 Namespace="http://emsinet.com/schemas/Order/Data")] 
		public OrderTransactionType Type
		{
			get { return type_; }
			set { type_ = value; }
		}

		/// <summary>
		/// Gets/set the transaction confirmation number.
		/// </summary>
		[XmlElement("ConfirmationNumber",
			 Namespace="http://emsinet.com/schemas/Order/Data")] 	
		public string ConfirmationNumber 
		{
			get { return confirmationNumber_; }
			set { confirmationNumber_ = value; }
		}

		/// <summary>
		/// Gets/sets the client request xml.
		/// </summary>
		[XmlElement("ClientRequest",
			 Namespace="http://emsinet.com/schemas/Order/Data")]		
		public XmlElement ClientRequest
		{
			get { return clientRequest_; }
			set { clientRequest_ = value; }
		}
	}

	public class OrderTransactionType : TypeCode
	{
		/// Constructs an empty OrderTransactionType.
		/// </summary>
		public OrderTransactionType()
		{
			// empty
		}

		/// <summary>
		/// Constructs an initial OrderTransactionType.
		/// </summary>
		/// <param name="name">The OrderTransactionType name.</param>
		/// <param name="id">The OrderTransactionType id.</param>
		public OrderTransactionType(short id, string description)
			: base(id, description)
		{
			// empty
		}
	}
}
