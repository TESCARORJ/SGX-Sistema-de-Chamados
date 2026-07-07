using System.Reflection;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Tests;

public sealed class FormularioServicoAdminDtosTests
{
    [Fact]
    public void FormularioDtoDeveConterCamposMinimosEsperados()
    {
        var propriedades = ObterNomesPropriedades(typeof(FormularioServicoAdminDto));

        Assert.Equal(
            ["Id", "CatalogoServicoId", "Nome", "Descricao", "Ativo", "CriadoEm", "AtualizadoEm"],
            propriedades);
    }

    [Fact]
    public void VersaoDtoDeveConterCamposMinimosEsperados()
    {
        var propriedades = ObterNomesPropriedades(typeof(FormularioServicoVersaoAdminDto));

        Assert.Equal(
            ["Id", "FormularioServicoId", "Numero", "Publicada", "PublicadoEm", "Ativo", "Campos"],
            propriedades);
    }

    [Fact]
    public void CampoDtoDeveConterTipoObrigatoriedadeOrdemAjudaEVisibilidade()
    {
        var propriedades = ObterNomesPropriedades(typeof(CampoFormularioServicoAdminDto));

        Assert.Equal(
            ["Id", "FormularioServicoVersaoId", "Nome", "Rotulo", "Tipo", "Obrigatorio", "Ordem", "TextoAjuda", "Visivel", "Ativo", "Opcoes"],
            propriedades);
    }

    [Fact]
    public void OpcaoDtoDeveConterValorRotuloEOrdem()
    {
        var propriedades = ObterNomesPropriedades(typeof(OpcaoCampoFormularioServicoAdminDto));

        Assert.Equal(
            ["Id", "CampoFormularioServicoId", "Valor", "Rotulo", "Ordem", "Ativo"],
            propriedades);
    }

    [Fact]
    public void RequestsNaoDevemExporAuditoria()
    {
        var tiposRequest = new[]
        {
            typeof(CriarFormularioServicoRequest),
            typeof(AtualizarFormularioServicoRequest),
            typeof(CriarFormularioServicoVersaoRequest),
            typeof(AtualizarFormularioServicoVersaoRequest),
            typeof(CriarCampoFormularioServicoRequest),
            typeof(AtualizarCampoFormularioServicoRequest),
            typeof(CriarOpcaoCampoFormularioServicoRequest),
            typeof(AtualizarOpcaoCampoFormularioServicoRequest)
        };

        var proibidas = new[]
        {
            "CriadoEm",
            "CriadoPor",
            "AtualizadoEm",
            "AtualizadoPor",
            "PublicadoPor"
        };

        foreach (var tipo in tiposRequest)
        {
            var propriedades = ObterNomesPropriedades(tipo);
            Assert.DoesNotContain(propriedades, nome => proibidas.Contains(nome, StringComparer.Ordinal));
        }
    }

    [Fact]
    public void ContratosNaoDevemExporEntidadesDeDominioOuEf()
    {
        var tiposContrato = new[]
        {
            typeof(FormularioServicoAdminDto),
            typeof(FormularioServicoDetalheAdminDto),
            typeof(FormularioServicoVersaoAdminDto),
            typeof(CampoFormularioServicoAdminDto),
            typeof(OpcaoCampoFormularioServicoAdminDto),
            typeof(CriarFormularioServicoRequest),
            typeof(AtualizarFormularioServicoRequest),
            typeof(CriarFormularioServicoVersaoRequest),
            typeof(AtualizarFormularioServicoVersaoRequest),
            typeof(CriarCampoFormularioServicoRequest),
            typeof(AtualizarCampoFormularioServicoRequest),
            typeof(CriarOpcaoCampoFormularioServicoRequest),
            typeof(AtualizarOpcaoCampoFormularioServicoRequest)
        };

        foreach (var tipo in tiposContrato)
        {
            foreach (var propriedade in tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var tipoExposto = ExtrairTipoElemento(propriedade.PropertyType);
                var namespaceExposto = tipoExposto.Namespace ?? string.Empty;

                Assert.DoesNotContain(".Domain.Entities", namespaceExposto, StringComparison.Ordinal);
                Assert.DoesNotContain("Microsoft.EntityFrameworkCore", namespaceExposto, StringComparison.Ordinal);
            }
        }
    }

    private static string[] ObterNomesPropriedades(Type tipo)
        => tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(x => x.Name)
            .ToArray();

    private static Type ExtrairTipoElemento(Type tipo)
    {
        if (tipo.IsArray)
        {
            return tipo.GetElementType()!;
        }

        if (tipo.IsGenericType)
        {
            var genericArgument = tipo.GetGenericArguments().SingleOrDefault();
            if (genericArgument is not null)
            {
                return genericArgument;
            }
        }

        return tipo;
    }
}
