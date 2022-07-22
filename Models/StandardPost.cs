using Piranha.AttributeBuilder;
using Piranha.Models;

namespace HeroesCup.Models
{
    [PostType(Title = "Standard post")]
    public class StandardPost  : Post<StandardPost>
    {
    }
}