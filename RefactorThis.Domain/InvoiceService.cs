using System;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
	public class InvoiceService
	{
		private const decimal TaxRate = 0.14m;
		private readonly InvoiceRepository _invoiceRepository;

		public InvoiceService(InvoiceRepository invoiceRepository)
		{
			_invoiceRepository = invoiceRepository;
		}

		public string ProcessPayment(Payment payment)
		{
			var invoice = GetInvoiceOrThrow(payment.Reference);

			var responseMessage = ProcessPaymentForInvoice(invoice, payment);

			invoice.Save();

			return responseMessage;
		}

		private Invoice GetInvoiceOrThrow(string paymentReference)
		{
			var invoice = _invoiceRepository.GetInvoice(paymentReference);

			if (invoice == null)
			{
				throw new InvalidOperationException("There is no invoice matching this payment");
			}

			return invoice;
		}

		private string ProcessPaymentForInvoice(Invoice invoice, Payment payment)
		{
			// Guard clause: Handle zero-amount invoices
			if (invoice.Amount == 0)
			{
				return HandleZeroAmountInvoice(invoice);
			}

			// Guard clause: Check if already fully paid - use Invoice's own property
			if (invoice.IsFullyPaid)
			{
				return "invoice was already fully paid";
			}

			// Guard clause: Validate payment amount
			var validationMessage = ValidatePaymentAmount(invoice, payment);
			if (validationMessage != null)
			{
				return validationMessage;
			}

			// Process the payment
			return ApplyPaymentToInvoice(invoice, payment);
		}

		private string HandleZeroAmountInvoice(Invoice invoice)
		{
			// Use Invoice's own property instead of repeating the check
			if (!invoice.HasPayments)
			{
				return "no payment needed";
			}

			throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
		}

		private string ValidatePaymentAmount(Invoice invoice, Payment payment)
		{
			// Use the invoice's own property instead of duplicating the calculation
			if (payment.Amount > invoice.RemainingAmount)
			{
				// Use Invoice's own property instead of our duplicate method
				return invoice.HasPayments
					? "the payment is greater than the partial amount remaining"
					: "the payment is greater than the invoice amount";
			}

			return null;
		}

		private string ApplyPaymentToInvoice(Invoice invoice, Payment payment)
		{
			// Use the invoice's own property - cleaner and no duplication
			var isFullPayment = invoice.RemainingAmount == payment.Amount;
			var hasExistingPayments = invoice.HasPayments;

			ApplyPaymentTransaction(invoice, payment);

			return GetPaymentResponseMessage(isFullPayment, hasExistingPayments);
		}

		private void ApplyPaymentTransaction(Invoice invoice, Payment payment)
		{
			invoice.AmountPaid += payment.Amount;
			invoice.Payments.Add(payment);

			if (invoice.Type == InvoiceType.Commercial)
			{
				invoice.TaxAmount += payment.Amount * TaxRate;
			}
			else if (invoice.Type == InvoiceType.Standard)
			{
				// For first payment on Standard invoices, set tax amount
				if (invoice.TaxAmount == 0)
				{
					invoice.TaxAmount = payment.Amount * TaxRate;
				}
			}
		}

		private string GetPaymentResponseMessage(bool isFullPayment, bool hasExistingPayments)
		{
			if (isFullPayment)
			{
				return hasExistingPayments
					? "final partial payment received, invoice is now fully paid"
					: "invoice is now fully paid";
			}

			return hasExistingPayments
				? "another partial payment received, still not fully paid"
				: "invoice is now partially paid";
		}
	}
}