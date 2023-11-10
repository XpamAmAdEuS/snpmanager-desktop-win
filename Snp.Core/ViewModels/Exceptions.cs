using System;

namespace Snp.Core.ViewModels
{
    /// <summary>
    /// Represents an exception that occurs when there's an error saving an order.
    /// </summary>
    public class SiteSavingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the OrderSavingException class with a default error message.
        /// </summary>
        public SiteSavingException() : base("Error saving an order.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the OrderSavingException class with the specified error message.
        /// </summary>
        public SiteSavingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OrderSavingException class with 
        /// the specified error message and inner exception.
        /// </summary>
        public SiteSavingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Represents an exception that occurs when there's an error deleting an order.
    /// </summary>
    public class SiteDeletionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the OrderDeletionException class with a default error message.
        /// </summary>
        public SiteDeletionException() : base("Error deleting an order.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the OrderDeletionException class with the specified error message.
        /// </summary>
        public SiteDeletionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OrderDeletionException class with 
        /// the specified error message and inner exception.
        /// </summary>
        public SiteDeletionException(string message,
            Exception innerException) : base(message, innerException)
        {
        }
    }
}
