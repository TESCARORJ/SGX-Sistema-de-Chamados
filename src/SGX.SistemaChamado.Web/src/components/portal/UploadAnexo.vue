<script setup lang="ts">
const emit = defineEmits<{
  upload: [file: File]
}>()

function handleFileChange(files: readonly File[] | null) {
  if (!files || files.length === 0) {
    return
  }

  emit('upload', files[0])
}
</script>

<template>
  <q-file
    label="Anexar arquivo"
    outlined
    use-chips
    clearable
    @update:model-value="handleFileChange"
  />
</template>
