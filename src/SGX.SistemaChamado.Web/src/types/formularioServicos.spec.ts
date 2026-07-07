import { describe, expect, it } from 'vitest'
import {
  TipoCampoFormularioServico,
  type AtualizarCampoFormularioServicoRequest,
  type CriarFormularioServicoRequest,
  type FormularioServicoDetalheAdminDto,
} from './formularioServicos'

describe('types/formularioServicos', () => {
  it('deve mapear os tipos administrativos principais do formulario de servico', () => {
    const criarFormulario: CriarFormularioServicoRequest = {
      catalogoServicoId: 'catalogo-1',
      nome: 'Formulario de Acesso',
      descricao: 'Coleta dados da requisicao',
      ativo: true,
    }

    const atualizarCampo: AtualizarCampoFormularioServicoRequest = {
      nome: 'centroCusto',
      rotulo: 'Centro de custo',
      tipo: TipoCampoFormularioServico.SelecaoUnica,
      obrigatorio: true,
      ordem: 2,
      textoAjuda: 'Selecione o centro de custo solicitante.',
      visivel: true,
      ativo: true,
    }

    const detalhe: FormularioServicoDetalheAdminDto = {
      id: 'form-1',
      catalogoServicoId: 'catalogo-1',
      nome: 'Formulario de Acesso',
      descricao: 'Coleta dados da requisicao',
      ativo: true,
      criadoEm: '2026-06-30T00:00:00Z',
      atualizadoEm: null,
      versoes: [
        {
          id: 'versao-1',
          formularioServicoId: 'form-1',
          numero: 1,
          publicada: false,
          publicadoEm: null,
          ativo: true,
          campos: [
            {
              id: 'campo-1',
              formularioServicoVersaoId: 'versao-1',
              nome: atualizarCampo.nome,
              rotulo: atualizarCampo.rotulo,
              tipo: atualizarCampo.tipo,
              obrigatorio: atualizarCampo.obrigatorio,
              ordem: atualizarCampo.ordem,
              textoAjuda: atualizarCampo.textoAjuda ?? null,
              visivel: atualizarCampo.visivel,
              ativo: atualizarCampo.ativo,
              opcoes: [
                {
                  id: 'opcao-1',
                  campoFormularioServicoId: 'campo-1',
                  valor: 'cc-ti',
                  rotulo: 'TI',
                  ordem: 1,
                  ativo: true,
                },
              ],
            },
          ],
        },
      ],
    }

    expect(criarFormulario.catalogoServicoId).toBe('catalogo-1')
    expect(detalhe.versoes[0]?.campos[0]?.tipo).toBe(TipoCampoFormularioServico.SelecaoUnica)
    expect(detalhe.versoes[0]?.campos[0]?.opcoes[0]?.rotulo).toBe('TI')
  })
})
