using System;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		private readonly InvoiceRepository _repository;

		public Invoice(InvoiceRepository repository)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));

			// Initialize Payments list here to avoid null reference exceptions later.
			// Before this, code could crash when trying to add payments or check if any exist.
			Payments = new List<Payment>();

			// Set a sensible default. Most invoices are Standard type, so let's assume that
			// unless explicitly told otherwise. Saves us from having to set it every time.
			Type = InvoiceType.Standard;
		}

		public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public decimal TaxAmount { get; set; }
		public List<Payment> Payments { get; set; }
		public InvoiceType Type { get; set; }

		// Three new computed properties added below.

		// These computed properties make the code way more readable in InvoiceService.
		// Instead of doing "invoice.Amount - invoice.AmountPaid" everywhere, we just say
		// "invoice.RemainingAmount". Much cleaner and if the calculation logic changes,
		// we only update it here.
		public decimal RemainingAmount => Amount - AmountPaid;

		// This makes checking payment status super simple. Before, we had complex logic
		// scattered around checking if payments equal the amount. Now it's just one line
		// and the business rule is clear: fully paid means we've received at least the
		// invoice amount, and it's not a zero-amount invoice.
		public bool IsFullyPaid => AmountPaid >= Amount && Amount > 0;

		// Small helper that prevents null checks all over the place. Instead of writing
		// "if (invoice.Payments != null && invoice.Payments.Any())" repeatedly, we can
		// just use "if (invoice.HasPayments)". Saves typing and makes intent clearer.
		public bool HasPayments => Payments != null && Payments.Any();

		public void Save()
		{
			_repository.SaveInvoice(this);
		}
	}

	public enum InvoiceType
	{
		Standard,
		Commercial
	}
}