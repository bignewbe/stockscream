using System;

namespace StockScream.DataModels
{
    public class Token
    {
        public static string CreateRandomCode(int numDigits = 6)
        {
            var rnd = new Random();
            var code = string.Empty;
            for (int i = 0; i < numDigits; i++)
                code += rnd.Next(0, 9);
            return code;
        }
        
        public bool IsExpired { get { return DateTime.UtcNow > ExpiresOn; } }

        //public int TokenId { get; set; }
        //public int UserId { get; set; }
        public string AuthToken { get; set; }
        public System.DateTime IssuedOn { get; set; }
        public System.DateTime ExpiresOn { get; set; }

        //public virtual ApplicationUser User { get; set; }
        public Token()
        {
            AuthToken = string.Empty;
            IssuedOn = DateTime.UtcNow;
            ExpiresOn = DateTime.UtcNow;
        }

        public Token(int numDigits, int seconds)
        {
            AuthToken = CreateRandomCode(numDigits);
            IssuedOn = DateTime.UtcNow;
            ExpiresOn = IssuedOn.AddSeconds(seconds);
        }
        
        public bool CheckConfirmationCode(string code)
        {
            return (code == AuthToken && !IsExpired);
        }
    }
}
