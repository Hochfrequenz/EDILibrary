using EDILibrary.Helper;

namespace EDILibrary.Interfaces
{
    /// <summary>
    /// An interface that allows to determine the <see cref="Sparte"/> ("division") of a market partner.
    /// </summary>
    /// <remarks>
    /// The classes implementing this interface might be hardcoded mappings to start with;
    /// But in the future™ there might be another market partner service that holds this information.
    /// </remarks>
    public interface IDivisionResolver
    {
        /// <summary>
        /// Determine the sparte that is relevant for a message which has  <paramref name="senderMarketPartnerId"/> as sender and <paramref name="receiverMarketPartnerId"/> as receiver.
        /// </summary>
        /// <param name="senderMarketPartnerId">the 13 digit market partner ID of the sending marktpartner</param>
        /// <param name="receiverMarketPartnerId">the 13 digit market partner ID of the sending marktpartner</param>
        /// <returns>the sparte</returns>
        public Sparte GetSparte(
            string? senderMarketPartnerId,
            string? receiverMarketPartnerId
        );
    }
}
