namespace OSharp.Api.V1
{
    /// <summary>
    /// Osu api key class.
    /// </summary>
    public class Key
    {
        /// <summary>
        /// Initial key class with a key string.
        /// </summary>
        /// <param name="value">osu! api key.</param>
        public Key(string value)
        {
            _value = value;
        }

        private readonly string _value;

        /// <summary />
        public static implicit operator Key(string value) => new Key(value);
        /// <summary />
        public static implicit operator string(Key key) => key.ToString();

        /// <summary>
        /// Get key string value of the class.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value;
        }
    }
}
