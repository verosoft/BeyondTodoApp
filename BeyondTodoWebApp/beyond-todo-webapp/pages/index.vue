<template>
  <div class="ma-16">
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

          <template v-slot:top>
            <v-toolbar flat>
              <v-toolbar-title>
                <v-icon color="medium-emphasis" icon="mdi-format-list-checks" size="x-small" start></v-icon>
                Todo Items Admin
              </v-toolbar-title>
              <v-btn class="me-2" prepend-icon="mdi-plus" rounded="lg" text="Add Item" border
                @click="loadNewDialog"></v-btn>
            </v-toolbar>
          </template>

          <template v-slot:item.currentPercent="{ item }">
            <v-progress-linear :model-value="item.currentPercent" :color="getProgressColor(item.currentPercent)"
              height="20" striped>
              <template v-slot:default="{ value }">
                <strong>{{ value }}%</strong>
              </template>
            </v-progress-linear>
          </template>

          <template v-slot:item.actions="{ item }">
            <div class="d-flex ga-2 justify-end">
              <v-icon color="medium-emphasis" icon="mdi-pencil" size="small" @click="loadEditDailog(item.id)"></v-icon>

              <v-icon color="medium-emphasis" icon="mdi-delete" size="small" @click="remove(item.id)"></v-icon>

              <v-icon color="medium-emphasis" icon="mdi-progress-upload" size="small"
                @click="loadProgressDialog(item.id)"></v-icon>
            </div>
          </template>

        </v-data-table-server>
      </ul>
    </div>

    <v-dialog v-model="editDialog" max-width="500">
      <v-card :title="`${isEdit ? 'Edit Description' : isNew ? 'New Todo' : 'Update Percent'}`">
        <v-card-text>
          <v-text-field label="New Description" v-model="editDescription" v-if="isEdit"></v-text-field>
          <div v-else-if="isNew">
            <v-text-field label="Title" v-model="newTitle"></v-text-field>
            <v-text-field label="Description" v-model="newDescription"></v-text-field>
            <v-combobox label="Category" :items="['Work', 'Personal', 'Hobby']" v-model="newCategory"></v-combobox>
          </div>
          <v-number-input :max="99" :min="1" v-model="percent" v-else></v-number-input>
        </v-card-text>
        <v-divider></v-divider>
        <v-card-actions>
          <v-btn text="Save" v-if="isEdit" @click="edit"></v-btn>
          <v-btn text="New" v-else-if="isNew" @click="newItem"></v-btn>
          <v-btn text="Add Percent" v-else @click="addPercent"></v-btn>
          <v-btn text="Close" @click="editDialog = false"></v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

  </div>
</template>

<script lang="ts" setup>

interface TodoItem {
  id: number;
  title: string;
  description: string;
  category: string;
  isCompleted: boolean;
  progressions: Progessions[],
  currentPercent: number
}

interface Progessions {
  date: Date,
  percent: number
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
  { title: 'Progress', key: 'currentPercent', align: 'start' as const, sortable: true },
  { title: 'Completed', key: 'isCompleted', align: 'start' as const, sortable: true },
  { title: 'Action', key: 'actions', align: 'end' as const, sortable: false },
]);

const getProgressColor = (percent: number) => {
  if (percent === 100) {
    return 'success';
  } else if (percent > 50) {
    return 'info';
  } else if (percent > 0) {
    return 'warning';
  } else {
    return 'error';
  }
};

const editDescription = ref('');
const editDialog = ref(false);
const editId = ref<number>();
const isEdit = ref<boolean>(false);
const isNew = ref<boolean>(false);
const percent = ref<number>(1);

const newTitle = ref('');
const newDescription = ref('');
const newCategory = ref('');


const loadNewDialog = () => {
  newTitle.value = '';
  newDescription.value = '';
  newCategory.value = '';
  isEdit.value = false;
  isNew.value = true;
  editDialog.value = true;
}

const newItem = async () => {

  const apiResponse = await $fetch<ApiResponseBody>(`/todos/`, {
    method: 'POST',
    baseURL: base_url,
    body: {
      title: newTitle.value,
      description: newDescription.value,
      category: newCategory.value

    }
  })

  const { success, message, data } = apiResponse;
  console.log(message);

  editDialog.value = false;
  isEdit.value = false;
  isNew.value = false;
  
  newTitle.value = '';
  newDescription.value = '';
  newCategory.value = '';

  refresh();
}

const loadProgressDialog = (id: number) => {
  isEdit.value = false;
  isNew.value = false;
  editDialog.value = true;
  editId.value = id;
}

const addPercent = async () => {

  const now = new Date();
  const utcDateTime = now.toISOString();

  const apiResponse = await $fetch<ApiResponseBody>(`/todos/${editId.value}/progress`, {
    method: 'PATCH',
    baseURL: base_url,
    body: {
      dateTime: utcDateTime,
      percent: percent.value
    }
  })

  const { success, message, data } = apiResponse;
  console.log(message);

  editDialog.value = false;
  isEdit.value = false;
  isNew.value = false;
  percent.value = 1;

  refresh();
}



const loadEditDailog = (id: number) => {
  isEdit.value = true;
  isNew.value = false;
  editDescription.value = '';
  editDialog.value = true;
  editId.value = id;
}

const edit = async () => {
  const apiResponse = await $fetch<ApiResponseBody>(`/todos/${editId.value}/description`, {
    method: 'PATCH',
    baseURL: base_url,
    body: {
      newDescription: editDescription.value
    }
  })

  const { success, message, data } = apiResponse;
  console.log(message);

  editDialog.value = false;
  isEdit.value = false;
  isNew.value = false;

  refresh();
}

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
}

</script>

<style></style>