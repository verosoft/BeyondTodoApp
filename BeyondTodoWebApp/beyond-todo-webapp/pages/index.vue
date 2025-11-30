<template>
  <div class="ma-16">
    <p class="text-h6">Todo Items Admin</p>
    <div>
      <div v-if="pending">
        Cargando...
      </div>
      <div v-else-if="error">
        Error al cargar los datos {{ error.message }}
      </div>
      <ul v-else>
        <!-- <li v-for="item in data?.data" :key="item.id">
          {{ item.title }}
        </li> -->
        <v-data-table-server :v-model:items-per-page="itemsPerPage" :headers="headers" :items="data?.data"
          :items-length="totalItems">

          <template v-slot:item.actions="{ item }">
            <div class="d-flex ga-2 justify-end">
              <v-icon color="medium-emphasis" icon="mdi-pencil" size="small" @click="edit(item.id)"></v-icon>

              <v-icon color="medium-emphasis" icon="mdi-delete" size="small" @click="remove(item.id)"></v-icon>
            </div>
          </template>

        </v-data-table-server>
      </ul>
    </div>



  </div>
</template>

<script lang="ts" setup>

interface TodoItem {
  id: number;
  title: string;
  description: string;
  category: string;
  isCompleted: boolean;
}

interface TodoResponse {
  data: TodoItem[];
}

interface ApiResponseBody {
  success: boolean;
  message: string;
  data: boolean | null | any;
}

const runtimeConfig = useRuntimeConfig();
const base_url = runtimeConfig.public.baseURL;

const { pending, data, error, refresh } = await useFetch<TodoResponse>('/todos', {
  baseURL: base_url
});

const totalItems = computed(() => data.value?.data?.length || 0);

const itemsPerPage = ref(5)

const headers = ref([
  { title: 'Id', key: 'id', align: 'end' as const, sortable: false },
  { title: 'Title', key: 'title', align: 'start' as const, sortable: true },
  { title: 'Description', key: 'description', align: 'start' as const, sortable: true },
  { title: 'Category', key: 'category', align: 'start' as const, sortable: true },
  { title: 'Completed', key: 'isCompleted', align: 'start' as const, sortable: true },
  { title: 'Action', key: 'actions', align: 'end' as const, sortable: false },

]);

const remove = async (id: number) => {
  try {
    const apiResponse = await $fetch<ApiResponseBody>(`/todos/${id}`, {
      method: 'DELETE',
      baseURL: base_url
    });

    const { success, message, data } = apiResponse;

    console.log(message);

    refresh();
  } catch (error) {
    if (error && typeof error === 'object' && 'response' in error) {

      const response = (error as any).response;
      const statusCode = response?.status;

      const apiResponseBody: ApiResponseBody | undefined = response?._data;

      if (apiResponseBody && typeof apiResponseBody === 'object' && apiResponseBody.message) {
        const message = apiResponseBody.message;
        switch (statusCode) {
          case 400:
            console.error(`400 Bad Request: ${message}`);
            break;
          default:
            console.error(`Error no manejado: ${statusCode}`);
            break;
        }

      }


    } else {
     
      console.error('Error no HTTP/de red:', error);
    }
  }
};

</script>

<style></style>