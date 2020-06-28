namespace OSharp.Api.V1.User
{
    /// <summary>
    /// User ID.
    /// </summary>
    public sealed class UserId : UserComponent
    {  
        /// <summary>
        /// Initialize a user ID.
        /// </summary>
        /// <param name="id">User ID.</param>
        public UserId(long id) : base(id.ToString(), Type.Id)
        {
        }

        /// <summary>
        /// Initialize a user ID.
        /// </summary>
        /// <param name="id">User ID.</param>
        public UserId(string id) : base(id, Type.Id)
        {
        }
    }

    /// <summary>
    /// User name.
    /// </summary>
    public sealed class UserName : UserComponent
    {
        /// <summary>
        /// Initialize a user name.
        /// </summary>
        /// <param name="name">User name.</param>
        public UserName(string name) : base(name, Type.Name)
        {
        }
    }

    /// <summary>
    /// Presents a user name or a user ID. It can be from user ID and user name.
    /// </summary>
    public class UserComponent
    {
        /// <summary>
        /// Initialize a user name or a user ID.
        /// </summary>
        /// <param name="idOrName">User name or a user ID.</param>
        /// <param name="idType">Specify the ID which is user name or user ID.</param>
        protected UserComponent(string idOrName, Type idType)
        {
            IdOrName = idOrName;
            IdType = idType;
        }

        /// <summary>
        /// User name or a user ID.
        /// </summary>
        public string IdOrName { get; }

        /// <summary>
        /// Get the ID type which is user name or user ID.
        /// </summary>
        public Type IdType { get; }

        /// <summary>
        /// Initialize with unsure identity. It will works with auto identification of the API server.
        /// </summary>
        /// <param name="idOrName">User name or a user ID.</param>
        /// <returns>Unsure identity.</returns>
        public static UserComponent FromAll(string idOrName) => new UserComponent(idOrName, Type.Auto);

        /// <summary>
        /// Initialize a user ID.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>User ID.</returns>
        public static UserId FromUserId(long id) => new UserId(id);

        /// <summary>
        /// Initialize a user ID.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>User ID.</returns>
        public static UserId FromUserId(string id) => new UserId(id);

        /// <summary>
        /// Initialize a user name.
        /// </summary>
        /// <param name="name">User name.</param>
        /// <returns>User name.</returns>
        public static UserName FromUserName(string name) => new UserName(name);

        /// <summary>
        /// User type option.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Represents a user name.
            /// </summary>
            Name,
            /// <summary>
            /// Represents a user ID.
            /// </summary>
            Id,
            /// <summary>
            /// Represents an unsure identity.
            /// </summary>
            Auto
        }

        /// <summary>
        /// Get the ID string value of the user name or user ID.
        /// </summary>
        /// <returns>User string.</returns>
        public override string ToString()
        {
            return IdOrName;
        }
    }
}
