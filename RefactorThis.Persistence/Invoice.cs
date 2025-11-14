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
			_repository = repository;
		}

		public void Save()
		{
			_repository.SaveInvoice(this);
		}

		public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public decimal TaxAmount { get; set; }
		public List<Payment> Payments { get; set; }

		public InvoiceType Type { get; set; }
		public bool InvoiceHasPayments { get => Payments != null && Payments.Any(); }
		public bool InvoiceIsAlreadyFullyPaid { get => Payments.Sum(x => x.Amount) != 0 && Amount == Payments.Sum(x => x.Amount); }
		public bool IsPaymentsGreaterThanThePartial(Payment payment) => Payments.Sum(x => x.Amount) != 0 && payment.Amount > (Amount - AmountPaid);

		public string PartialPay(Payment payment) 
		{
            string responseMessage;

            bool isPaymentFinalPartial = (Amount - AmountPaid) == payment.Amount;

            AmountPaid += payment.Amount;
            Payments.Add(payment);

            switch (Type)
            {
                case InvoiceType.Standard:
                    responseMessage = isPaymentFinalPartial ? "final partial payment received, invoice is now fully paid" : "another partial payment received, still not fully paid";
                    break;
                case InvoiceType.Commercial:
                    TaxAmount += payment.Amount * 0.14m;
                    responseMessage = isPaymentFinalPartial ? "final partial payment received, invoice is now fully paid" : "another partial payment received, still not fully paid";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return responseMessage;
        }

		public string FullPay(Payment payment) {
            string responseMessage;

            bool isFullPayment = Amount == payment.Amount;

            AmountPaid = payment.Amount;
            TaxAmount = payment.Amount * 0.14m;
            Payments.Add(payment);

            switch (Type)
            {
                case InvoiceType.Standard:
                case InvoiceType.Commercial:
                    responseMessage = isFullPayment ?  "invoice is now fully paid" : "invoice is now partially paid";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return responseMessage;
        }
	}

	public enum InvoiceType
	{
		Standard,
		Commercial
	}
}