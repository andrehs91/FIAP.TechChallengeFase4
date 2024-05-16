using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechChallenge.Dominio.Entities;

namespace TechChallenge.Aplicacao.Controllers;

public class BaseController : ControllerBase
{
    protected Usuario ObterUsuarioAutenticado()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        return new Usuario()
        {
            Id = int.Parse(claimsIdentity!.FindFirst("Id")!.Value),
            Matricula = claimsIdentity!.FindFirst("Matricula")!.Value,
            Nome = claimsIdentity!.FindFirst("Nome")!.Value,
            Departamento = claimsIdentity!.FindFirst("Departamento")!.Value,
            EhGestor = claimsIdentity!.FindFirst("EhGestor")!.Value == "Sim",
        };
    }

    protected string ObterErrosDeValidacao()
    {
        return string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
    }
}
