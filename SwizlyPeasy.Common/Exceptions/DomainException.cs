namespace SwizlyPeasy.Common.Exceptions
{
    public abstract class DomainException : ApplicationException
    {
        /// <summary>
        /// Base class for domain exceptions
        /// </summary>
        /// <param name="msg"></param>
        protected DomainException(string msg) : base(msg)
        {
        }

        protected DomainException(string msg, object? context) : base(msg)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public object? Context { get; set; }
    }
}