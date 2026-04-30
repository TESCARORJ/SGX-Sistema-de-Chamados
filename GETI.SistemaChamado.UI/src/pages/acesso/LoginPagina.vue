<template>
  <q-page class="row items-center justify-center q-pa-md">
    <q-card flat bordered class="card-login">
      <q-card-section>
        <div class="text-overline text-primary">Acesso Corporativo</div>
        <div class="text-h5 q-mt-sm">Sistema de Chamados CREA-RJ</div>
        <p class="q-mt-sm text-grey-8">
          Esta tela e provisoria para desenvolvimento local. Em producao, o acesso sera integrado ao AD/Azure.
        </p>
      </q-card-section>

      <q-separator />

      <q-card-section class="q-gutter-md">
        <q-input v-model="formulario.login" label="Login local (desenvolvimento)" outlined hint="Ex.: nome.sobrenome" />
        <q-input v-model="formulario.nome" label="Nome de exibicao" outlined />
        <q-input v-model="formulario.email" label="Email" outlined />
      </q-card-section>

      <q-card-actions align="between">
        <q-btn flat no-caps label="Limpar dados locais" color="negative" @click="limpar" />
        <div class="row q-gutter-sm">
          <q-btn flat no-caps color="primary" label="Portal" @click="irParaPortal" />
          <q-btn no-caps color="secondary" label="Admin" @click="irParaAdmin" />
        </div>
      </q-card-actions>

      <q-card-section v-if="erro" class="q-pt-none">
        <q-banner rounded class="bg-red-1 text-negative">{{ erro }}</q-banner>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { limparSessaoLocalDesenvolvimento, salvarSessaoLocalDesenvolvimento } from '@/services/sessaoLocalDesenvolvimento';
import { limparCacheUsuarioAtual, obterUsuarioAtual, possuiAcessoAdministrativo } from '@/services/sessaoUsuario';

const router = useRouter();
const route = useRoute();

const erro = ref('');
const formulario = reactive({
  login: '',
  nome: '',
  email: ''
});

function persistirSessaoLocal(): void {
  salvarSessaoLocalDesenvolvimento({
    login: formulario.login,
    nome: formulario.nome,
    email: formulario.email
  });
  limparCacheUsuarioAtual();
}

async function irParaPortal(): Promise<void> {
  erro.value = '';
  persistirSessaoLocal();
  const destino = typeof route.query.redirecionar === 'string' ? route.query.redirecionar : '/portal';
  await router.push(destino.startsWith('/admin') ? '/portal' : destino);
}

async function irParaAdmin(): Promise<void> {
  erro.value = '';
  persistirSessaoLocal();
  try {
    const usuario = await obterUsuarioAtual(true);
    if (!usuario) {
      erro.value = 'Usuario nao autenticado. Verifique credenciais AD/Azure ou autenticacao local.';
      return;
    }
    if (!possuiAcessoAdministrativo(usuario)) {
      erro.value = 'Seu perfil nao possui acesso administrativo.';
      return;
    }
    await router.push('/admin');
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao validar permissao administrativa.';
  }
}

function limpar(): void {
  formulario.login = '';
  formulario.nome = '';
  formulario.email = '';
  erro.value = '';
  limparSessaoLocalDesenvolvimento();
  limparCacheUsuarioAtual();
}
</script>

<style scoped>
.card-login {
  width: 100%;
  max-width: 640px;
}
</style>
