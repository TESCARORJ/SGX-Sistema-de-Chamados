using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class CatalogoServicoGrupoTecnicoTests
{
    [Fact]
    public void DevePermitirVinculoOpcionalComGrupoTecnico()
    {
        var grupoTecnicoId = Guid.NewGuid();

        var servico = new CatalogoServico(
            "Servico com grupo",
            "servico-com-grupo",
            "Descricao valida",
            null,
            Guid.NewGuid(),
            null,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Interno,
            true,
            false,
            1,
            Guid.NewGuid(),
            "teste",
            grupoTecnicoId);

        Assert.Equal(grupoTecnicoId, servico.GrupoTecnicoId);
    }

    [Fact]
    public void DevePermitirRemoverVinculoComGrupoTecnico()
    {
        var servico = new CatalogoServico(
            "Servico com grupo",
            "servico-com-grupo",
            "Descricao valida",
            null,
            Guid.NewGuid(),
            null,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Interno,
            true,
            false,
            1,
            Guid.NewGuid(),
            "teste",
            Guid.NewGuid());

        servico.DefinirGrupoTecnico(null);

        Assert.Null(servico.GrupoTecnicoId);
    }
}
