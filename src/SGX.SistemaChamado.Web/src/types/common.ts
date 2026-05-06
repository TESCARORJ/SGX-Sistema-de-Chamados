export interface ApiListResponse<T> {
  items: T[]
  total: number
}

export interface ApiError {
  status: number
  message: string
}

export type Nullable<T> = T | null

export type SortDirection = 'asc' | 'desc'

export interface PagedResponse<T> {
  items: T[]
  total: number
  pagina: number
  tamanhoPagina: number
}
