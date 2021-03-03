namespace DependencyInjectionApp.Model
{
    public class User
    {
        public int AccountId {get; set;}
        public string FirstName {get; set;}
        public string LastName {get; set;}
        public UserCreditCard CreditCard {get; set;}
        public UserGiftCard GiftCard {get; set;}
        public UserPayPal PayPal {get; set;} 

        public void SetDefaultPaymentType()
        {
            // Set default 
        }

        
    }
}