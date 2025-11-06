namespace RefactorThis.Persistence
{
	public class InvoiceRepository
	{
		private Invoice _invoice;


		public Invoice GetInvoice(string reference)
		{
			return _invoice;
		}


		public void SaveInvoice(Invoice invoice)
		{
			if (invoice == null)
			{
				return;
			}
			// Need to add Invoice saving logic here in a real implementation

		}


		public void Add(Invoice invoice)
		{
			if (invoice == null)
			{
				return;
			}

			_invoice = invoice;
		}


		public void Clear()
		{
			_invoice = null;
		}


		public int Count => _invoice != null ? 1 : 0;
	}
}