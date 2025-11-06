using System;

namespace RefactorThis.Persistence
{

	public class Payment
	{

		public decimal Amount { get; set; }


		public string Reference { get; set; }


		public bool IsValid()
		{
			return Amount >= 0;
		}


		public bool HasReference => !string.IsNullOrWhiteSpace(Reference);
	}
}