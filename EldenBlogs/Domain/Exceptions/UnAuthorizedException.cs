namespace Domain.Exceptions
{
    /// <summary>
    /// Excepción que se lanza cuando un usuario intenta acceder a un recurso para el cual no está autorizado.
    /// </summary>
    public class UnAuthorizedException : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UnAuthorizedException"/>.
        /// </summary>
        public UnAuthorizedException() : base("Usted no está autorizado")
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UnAuthorizedException"/> con un mensaje específico.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        public UnAuthorizedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UnAuthorizedException"/> con un mensaje específico y una excepción interna.
        /// </summary>
        /// <param name="message">El mensaje que describe el error.</param>
        /// <param name="innerException">La excepción que es la causa del error actual, o una referencia nula si no se especifica ninguna excepción interna.</param>
        public UnAuthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
