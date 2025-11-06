using System;

namespace RefactorThis.Persistence
{
	/// <summary>
	/// Represents a payment transaction made against an invoice.
	/// </summary>
	public class Payment
	{
		/// <summary>
		/// Gets or sets the payment amount.
		/// </summary>
		public decimal Amount { get; set; }

		/// <summary>
		/// Gets or sets the reference identifier for this payment.
		/// </summary>
		public string Reference { get; set; }

		/// <summary>
		/// Validates that the payment has a valid amount.
		/// </summary>
		/// <returns>True if the payment amount is valid (greater than or equal to zero), otherwise false</returns>
		public bool IsValid()
		{
			return Amount >= 0;
		}

		/// <summary>
		/// Gets whether this payment has a reference assigned.
		/// </summary>
		public bool HasReference => !string.IsNullOrWhiteSpace(Reference);
	}
}