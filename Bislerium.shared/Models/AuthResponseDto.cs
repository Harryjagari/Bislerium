
namespace Bislerium.shared.Dtos
{ 
    public record AuthResponseDto(LoggedInUser User, string Token, IList<string> roles); 

}

