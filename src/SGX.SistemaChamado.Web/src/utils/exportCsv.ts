export interface CsvColumn<T extends Record<string, unknown>> {
  key: keyof T | string
  label: string
}

function escapeCsvValue(value: unknown): string {
  if (value === null || value === undefined) {
    return ''
  }

  const text = String(value)
  const escaped = text.replace(/"/g, '""')
  if (/[",\n;]/.test(escaped)) {
    return `"${escaped}"`
  }

  return escaped
}

export function gerarConteudoCsv<T extends Record<string, unknown>>(
  dados: T[],
  colunas?: CsvColumn<T>[]
): string {
  if (!dados.length && (!colunas || !colunas.length)) {
    return ''
  }

  const colunasDefinidas =
    colunas && colunas.length
      ? colunas
      : (Object.keys(dados[0] ?? {}) as Array<keyof T>).map((key) => ({ key, label: String(key) }))

  const header = colunasDefinidas.map((coluna) => escapeCsvValue(coluna.label)).join(';')
  const linhas = dados.map((item) =>
    colunasDefinidas
      .map((coluna) => escapeCsvValue(item[coluna.key as keyof T]))
      .join(';')
  )

  return [header, ...linhas].join('\n')
}

export function exportarCsv<T extends Record<string, unknown>>(
  nomeArquivo: string,
  dados: T[],
  colunas?: CsvColumn<T>[]
): void {
  if (typeof window === 'undefined' || !dados.length) {
    return
  }

  const conteudo = gerarConteudoCsv(dados, colunas)
  const blob = new Blob([conteudo], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')

  link.href = url
  link.download = nomeArquivo.endsWith('.csv') ? nomeArquivo : `${nomeArquivo}.csv`
  link.style.display = 'none'

  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  URL.revokeObjectURL(url)
}
