using System.Diagnostics.CodeAnalysis;

namespace VNTags.Utility.Interfaces
{
    public interface IStringSerializable
    {
        /// <summary>
        ///     Make sure the underlying string value is a [SerializeField]
        /// </summary>
        string StringRepresentation { get; set; }

        string Serialize([AllowNull] object context);

        /// <summary>
        ///     classes implementing this interface will need to use StringRepresentation to return an object,
        ///     this will allow for derived types without [SerializeReference], is also a lot more straightforward
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        object Deserialize([AllowNull] object context);
    }
}