using System.Text;

namespace AnikarSalon.MapMethods
{
    public static class AdditionalMethods
    {
        public static string PhoneBuilder(string phoneNumber)
        {
            StringBuilder phoneNumberBuilder = new(phoneNumber);
            while (phoneNumberBuilder.Length != 11)
            {
                char[] symbolsToDelete = { ' ', '-', '+', '(', ')', ':', '_' };
                foreach (char symbol in symbolsToDelete)
                    phoneNumberBuilder.Replace(symbol.ToString(), "");
            }
            phoneNumber = phoneNumberBuilder.ToString();
            phoneNumber = phoneNumber.Remove(0, 1).Insert(0, "8");

            return phoneNumber;
        }
    }
}
