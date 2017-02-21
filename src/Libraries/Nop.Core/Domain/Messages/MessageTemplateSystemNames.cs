namespace Nop.Core.Domain.Messages
{
    /// <summary>
    /// Represents message template system names
    /// </summary>
    public static partial class MessageTemplateSystemNames
    {
        #region Customer

        /// <summary>
        /// Represents system name of notification about new registration
        /// </summary>
        public const string CustomerRegisteredNotification = "NewCustomer.Notification";

        /// <summary>
        /// Represents system name of customer welcome message
        /// </summary>
        public const string CustomerWelcomeMessage = "Customer.WelcomeMessage";

        /// <summary>
        /// Represents system name of email validation message
        /// </summary>
        public const string CustomerEmailValidationMessage = "Customer.EmailValidationMessage";

        /// <summary>
        /// Represents system name of email revalidation message
        /// </summary>
        public const string CustomerEmailRevalidationMessage = "Customer.EmailRevalidationMessage";

        /// <summary>
        /// Represents system name of password recovery message
        /// </summary>
        public const string CustomerPasswordRecoveryMessage = "Customer.PasswordRecovery";

        #endregion

        #region Newsletter

        /// <summary>
        /// Represents system name of subscription activation message
        /// </summary>
        public const string NewsletterSubscriptionActivationMessage = "NewsLetterSubscription.ActivationMessage";

        /// <summary>
        /// Represents system name of subscription deactivation message
        /// </summary>
        public const string NewsletterSubscriptionDeactivationMessage = "NewsLetterSubscription.DeactivationMessage";

        #endregion

        #region To friend

        /// <summary>
        /// Represents system name of 'Email a friend' message
        /// </summary>
        public const string EmailAFriendMessage = "Service.EmailAFriend";

        #endregion

        #region Forum

        /// <summary>
        /// Represents system name of notification about new forum topic
        /// </summary>
        public const string NewForumTopicMessage = "Forums.NewForumTopic";

        /// <summary>
        /// Represents system name of notification about new forum post
        /// </summary>
        public const string NewForumPostMessage = "Forums.NewForumPost";

        /// <summary>
        /// Represents system name of notification about new private message
        /// </summary>
        public const string PrivateMessageNotification = "Customer.NewPM";

        #endregion

        #region Misc

        /// <summary>
        /// Represents system name of notification store owner about new product review
        /// </summary>
        public const string ProductReviewNotification = "Product.ProductReview";

        /// <summary>
        /// Represents system name of notification store owner about new blog comment
        /// </summary>
        public const string BlogCommentNotification = "Blog.BlogComment";

        /// <summary>
        /// Represents system name of notification store owner about new news comment
        /// </summary>
        public const string NewsCommentNotification = "News.NewsComment";

        /// <summary>
        /// Represents system name of 'Contact us' message
        /// </summary>
        public const string ContactUsMessage = "Service.ContactUs";

        #endregion
    }
}