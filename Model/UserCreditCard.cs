namespace DependencyInjectionApp.Model
{
    public class UserCreditCard
    {
        public string CreditCardNumber {get; set;}
        public string CreditCardExpirationDate {get; set;}
        public string CreditCardSecurityCode {get; set;}
        public bool isDefaultPaymentMethod {get; set;}
    }
}