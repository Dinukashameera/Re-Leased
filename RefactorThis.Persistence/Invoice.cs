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
			Payments = new List<Payment>();
			Type = InvoiceType.Standard;
		}

		public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public decimal TaxAmount { get; set; }
		public List<Payment> Payments { get; set; }
		public InvoiceType Type { get; set; }

		public decimal RemainingAmount => Amount - AmountPaid;
		public bool IsFullyPaid => AmountPaid >= Amount && Amount > 0;
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