using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BelTwit_REST_API.Models
{
    [DataContract]
    public class User
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "User must have a login")]
        [StringLength(maximumLength: 20, MinimumLength = 2, 
            ErrorMessage = "Login length should be in diaposon [2;20]")]
        [DataMember]
        public string Login { get; set; }
        

        [Required(ErrorMessage = "User must have a password")]
        [StringLength(maximumLength: 100, MinimumLength = 5,
            ErrorMessage = "Password length should be in diaposon [5;100]")]
        [DataMember]
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        [DataMember]
        public bool IsAdmin { get; set; }


        public virtual IList<RefreshToken> RefreshTokens { get; set; }
        public virtual IList<SubscriberSubscription> Subscribers { get; set; }    //подпісчікі
        public virtual IList<SubscriberSubscription> Subscriptions { get; set; }  //подпіскі
        public virtual IList<Tweet> Tweets { get; set; }  //твіты
        public virtual IList<Comment> TweetComments { get; set; }
        public virtual IList<UserRateState> TweetRateStates { get; set; }


        public User()
        {
            RefreshTokens = new List<RefreshToken>();
            Subscribers= new List<SubscriberSubscription>();
            Subscriptions = new List<SubscriberSubscription>();
            Tweets = new List<Tweet>();
            TweetComments = new List<Comment>();
            TweetRateStates = new List<UserRateState>();
        }
    }
}
