namespace RefactorThis.Persistence
{
	/// <summary>
	/// In-memory repository for Invoice entities.
	/// Note: This is a simplified implementation for testing purposes.
	/// In a real application, this would interact with a database.
	/// </summary>
	public class InvoiceRepository
	{
		private Invoice _invoice;

		/// <summary>
		/// Retrieves the stored invoice.
		/// </summary>
		/// <param name="reference">Payment reference (currently ignored as the repository stores a single invoice)</param>
		/// <returns>The stored invoice, or null if none exists</returns>
		public Invoice GetInvoice(string reference)
		{
			return _invoice;
		}

		/// <summary>
		/// Persists changes to an invoice.
		/// </summary>
		/// <param name="invoice">The invoice to save</param>
		public void SaveInvoice(Invoice invoice)
		{
			if (invoice == null)
			{
				return;
			}

			// In a real application, this would persist to a database
		}

		/// <summary>
		/// Adds an invoice to the repository.
		/// </summary>
		/// <param name="invoice">The invoice to add</param>
		public void Add(Invoice invoice)
		{
			if (invoice == null)
			{
				return;
			}

			_invoice = invoice;
		}

		/// <summary>
		/// Clears the stored invoice from the repository.
		/// </summary>
		public void Clear()
		{
			_invoice = null;
		}

		/// <summary>
		/// Gets the number of invoices currently stored (0 or 1).
		/// </summary>
		public int Count => _invoice != null ? 1 : 0;
	}
}