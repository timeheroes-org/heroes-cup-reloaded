using Piranha.AttributeBuilder;
using Piranha.Models;

namespace heroes_cup_reloaded.Models
{
    [PostType(Title = "Standard post")]
    public class StandardPost  : Post<StandardPost>
    {
    }
}