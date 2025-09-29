
namespace authorization.data.Models
{
    public record RelationshipTuple(string User, string Relation, string Object, DateTimeOffset? Timestamp = null);
}
