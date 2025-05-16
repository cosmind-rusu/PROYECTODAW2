<script>
import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

export default {
  name: 'CategoriesView',
  data() {
    return {
      categories: [],
      newCategory: { name: '', description: '', isActive: true }
    }
  },
  async created() {
    await this.loadCategories()
  },
  methods: {
    async loadCategories() {
      try {
        const config = useAuthStore().authHeader
        const { data } = await axios.get('/api/categories', config)
        this.categories = data
      } catch (err) {
        console.error(err)
        alert('Error loading categories')
      }
    },
    async createCategory() {
      if (!this.newCategory.name) return
      try {
        const config = useAuthStore().authHeader
        const { data } = await axios.post('/api/categories', this.newCategory, config)
        this.categories.push(data)
        this.newCategory = { name: '', description: '', isActive: true }
      } catch (err) {
        console.error(err)
        alert('Error creating category')
      }
    },
    async deleteCategory(id) {
      if (!confirm('Delete this category?')) return
      try {
        const config = useAuthStore().authHeader
        await axios.delete(`/api/categories/${id}`, config)
        this.categories = this.categories.filter(c => c.id !== id)
      } catch (err) {
        console.error(err)
        alert('Error deleting category')
      }
    }
  }
}
</script>

<template>
  <section class="categories">
    <h2>Categories</h2>
    <div class="categories__form">
      <input v-model="newCategory.name" placeholder="Name" />
      <input v-model="newCategory.description" placeholder="Description" />
      <label>
        <input type="checkbox" v-model="newCategory.isActive" /> Active
      </label>
      <button @click="createCategory">Add Category</button>
    </div>
    <table class="categories__table">
      <thead>
        <tr>
          <th>Name</th>
          <th>Description</th>
          <th>Active</th>
          <th>Created</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="cat in categories" :key="cat.id">
          <td>{{ cat.name }}</td>
          <td>{{ cat.description }}</td>
          <td>{{ cat.isActive }}</td>
          <td>{{ new Date(cat.createdDate).toLocaleDateString() }}</td>
          <td><button @click="deleteCategory(cat.id)">Delete</button></td>
        </tr>
      </tbody>
    </table>
  </section>
</template>

<style scoped lang="scss">
@use "@/assets/styles/variables" as *;
@use "@/assets/styles/mixins" as *;

.categories {
  padding: $spacing-unit;
  background-color: $background-color;
  font-family: $font-family-base;
  &__form {
    display: flex;
    gap: $spacing-unit;
    margin-bottom: $spacing-unit;
  }
  &__table {
    width: 100%;
    border-collapse: collapse;
    th, td {
      border: 1px solid #ddd;
      padding: $spacing-unit / 2;
    }
    tr:hover {
      background-color: #f1f1f1;
    }
  }
  button {
    background-color: $primary-color;
    color: white;
    border: none;
    padding: $spacing-unit / 2 $spacing-unit;
    border-radius: 4px;
    cursor: pointer;
    &:hover {
      background-color: $secondary-color;
    }
  }
}
</style>
