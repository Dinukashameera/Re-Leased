using System;
using System.Collections.Generic;
using NUnit.Framework;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
	[TestFixture]
	public class InvoicePaymentProcessorTests
	{
		private InvoiceRepository _repository;
		private InvoiceService _invoiceService;

		[SetUp]
		public void SetUp()
		{
			_repository = new InvoiceRepository();
			_invoiceService = new InvoiceService(_repository);
		}

		#region Exception Tests

		[Test]
		public void ProcessPayment_ThrowsException_WhenInvoiceNotFound()
		{
			// Arrange
			var payment = CreatePayment(amount: 10);

			// Act & Assert
			var exception = Assert.Throws<InvalidOperationException>(() =>
				_invoiceService.ProcessPayment(payment));

			Assert.That(exception.Message, Is.EqualTo("There is no invoice matching this payment"));
		}

		[Test]
		public void ProcessPayment_ThrowsException_WhenZeroAmountInvoiceHasPayments()
		{
			// Arrange
			var invoice = CreateInvoice(amount: 0, amountPaid: 0);
			invoice.Payments = new List<Payment> { CreatePayment(amount: 5) };
			_repository.Add(invoice);

			var payment = CreatePayment(amount: 10);

			// Act & Assert
			var exception = Assert.Throws<InvalidOperationException>(() =>
				_invoiceService.ProcessPayment(payment));

			Assert.That(exception.Message, Is.EqualTo("The invoice is in an invalid state, it has an amount of 0 and it has payments."));
		}

		#endregion

		#region Zero Amount Invoice Tests

		[Test]
		public void ProcessPayment_ReturnsNoPaymentNeeded_ForZeroAmountInvoice()
		{
			// Arrange
			var invoice = CreateInvoice(amount: 0, amountPaid: 0);
			_repository.Add(invoice);

			var payment = CreatePayment(amount: 0);

			// Act
			var result = _invoiceService.ProcessPayment(payment);

			// Assert
			Assert.That(result, Is.EqualTo("no payment needed"));
		}

		#endregion

		#region Already Fully Paid Tests

		[Test]
		public void ProcessPayment_RejectsPayment_WhenAlreadyFullyPaid()
		{
			// Arrange
			var invoice = CreateInvoiceWithPayments(
				invoiceAmount: 10,
				amountPaid: 10,
				existingPayments: new[] { 10m });
			_repository.Add(invoice);

			var payment = CreatePayment(amount: 5);

			// Act
			var result = _invoiceService.ProcessPayment(payment);

			// Assert
			Assert.That(result, Is.EqualTo("invoice was already fully paid"));
		}

		#endregion

		#region Overpayment Validation Tests

		[Test]
		public void ProcessPayment_RejectsOverpayment_WithPartialPayments()
		{
			// Arrange
			var invoice = CreateInvoiceWithPayments(
				invoiceAmount: 10,
				amountPaid: 5,
				existingPayments: new[] { 5m });
			_repository.Add(invoice);

			var payment = CreatePayment(amount: 6);

			// Act
			var result = _invoiceService.ProcessPayment(payment);

			// Assert
			Assert.That(result, Is.EqualTo("the payment is greater than the partial amount remaining"));
		}

		[Test]
		public void ProcessPayment_RejectsOverpayment_WithoutPriorPayments()
		{
			// Arrange
			var invoice = CreateInvoice(amount: 5, amountPaid: 0);
			_repository.Add(invoice);

			var payment = CreatePayment(amount: 6);

			// Act
			var result = _invoiceService.ProcessPayment(payment);

			// Assert
			Assert.That(result, Is.EqualTo("the payment is greater than the invoice amount"));
		}

		#endregion

		#region Full Payment Tests

		[Test]
		public void ProcessPayment_CompletesInvoice_WithFinalPartialPayment()
		{
			// Arrange
			var invoice = CreateInvoiceWithPayments(
				invoiceAmount: 10,
				amountPaid: 5,
				existingPayments: new[] { 5m });
			_repository.Add(invoice);

			var payment = CreatePayment(amount: 5);

			// Act
			var result = _invoiceService.ProcessPayment(payment);

			// Assert
			Assert.That(result, Is.EqualTo("final partial payment received, invoice is now fully paid"));
		}

		[Test]
		public void ProcessPayment_CompletesInvoice_WithSinglePayment()
		{
			// Arrange
			var invoice = CreateInvoice(amount: 10, amountPaid: 0);
			_repository.Add(invoice);

			var payment = CreatePayment(amount: 10);

			// Act
			var result = _invoiceService.ProcessPayment(payment);

			// Assert
			Assert.That(result, Is.EqualTo("invoice is now fully paid"));
		}

		#endregion

		#region Partial Payment Tests

		[Test]
		public void ProcessPayment_AcceptsPartialPayment_WithExistingPayments()
		{
			// Arrange
			var invoice = CreateInvoiceWithPayments(
				invoiceAmount: 10,
				amountPaid: 5,
				existingPayments: new[] { 5m });
			_repository.Add(invoice);

			var payment = CreatePayment(amount: 1);

			// Act
			var result = _invoiceService.ProcessPayment(payment);

			// Assert
			Assert.That(result, Is.EqualTo("another partial payment received, still not fully paid"));
		}

		[Test]
		public void ProcessPayment_AcceptsPartialPayment_AsFirstPayment()
		{
			// Arrange
			var invoice = CreateInvoice(amount: 10, amountPaid: 0);
			_repository.Add(invoice);

			var payment = CreatePayment(amount: 1);

			// Act
			var result = _invoiceService.ProcessPayment(payment);

			// Assert
			Assert.That(result, Is.EqualTo("invoice is now partially paid"));
		}

		#endregion

		#region Helper Methods

		private Invoice CreateInvoice(decimal amount, decimal amountPaid, InvoiceType type = InvoiceType.Standard)
		{
			return new Invoice(_repository)
			{
				Amount = amount,
				AmountPaid = amountPaid,
				Type = type,
				Payments = new List<Payment>()
			};
		}

		private Invoice CreateInvoiceWithPayments(decimal invoiceAmount, decimal amountPaid, decimal[] existingPayments, InvoiceType type = InvoiceType.Standard)
		{
			var payments = new List<Payment>();
			foreach (var paymentAmount in existingPayments)
			{
				payments.Add(CreatePayment(paymentAmount));
			}

			return new Invoice(_repository)
			{
				Amount = invoiceAmount,
				AmountPaid = amountPaid,
				Type = type,
				Payments = payments
			};
		}

		private Payment CreatePayment(decimal amount, string reference = null)
		{
			return new Payment
			{
				Amount = amount,
				Reference = reference
			};
		}

		#endregion
	}
}