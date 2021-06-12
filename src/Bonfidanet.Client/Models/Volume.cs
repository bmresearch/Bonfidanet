namespace Bonfida.Client.Models
{
    /// <summary>
    /// Represents volume information of a certain market in the Serum DEX.
    /// </summary>
    public class VolumeInfo
    {
        /// <summary>
        /// The volume in USD value.
        /// </summary>
        public decimal VolumeUsd { get; set; }
        
        /// <summary>
        /// The volume in the base currency.
        /// </summary>
        public decimal Volume { get; set; }
    }
}