namespace TechChallenge.DTO;

public class UsuarioAutenticadoDTO(UsuarioDTO usuarioDTO, string token)
{
    public UsuarioDTO Usuario { get; set; } = usuarioDTO;
    public string Token { get; set; } = token;
}
