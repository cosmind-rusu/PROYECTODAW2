<script>
import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

export default {
  name: 'ExpenseCategoriesView',
  data() {
    return {
      categories: [],
      newCategory: { 
        name: '', 
        description: '', 
        isExpense: true,
        iconName: '',
        colorCode: '#42b983',
        isActive: true 
      }
    }
  },
  async created() {
    await this.loadCategories()
  },
  methods: {
    async loadCategories() {
      try {
        const config = useAuthStore().authHeader
        const { data } = await axios.get('/api/expensecategories', config)
        this.categories = data
      } catch (err) {
        console.error(err)
        alert('Error loading expense categories')
      }
    },
    async createCategory() {
      if (!this.newCategory.name) return
      try {
        const config = useAuthStore().authHeader
        const { data } = await axios.post('/api/expensecategories', this.newCategory, config)
        this.categories.push(data)
        this.newCategory = { 
          name: '', 
          description: '', 
          isExpense: true,
          iconName: '',
          colorCode: '#42b983',
          isActive: true 
        }
      } catch (err) {
        console.error(err)
        alert('Error creating expense category')
      }
    },
    async deleteCategory(id) {
      if (!confirm('Delete this expense category?')) return
      try {
        const config = useAuthStore().authHeader
        await axios.delete(`/api/expensecategories/${id}`, config)
        this.categories = this.categories.filter(c => c.id !== id)
      } catch (err) {
        console.error(err)
        alert('Error deleting expense category')
      }
    },
    toggleType() {
      this.newCategory.isExpense = !this.newCategory.isExpense
    }
  }
}
</script>

<template>
  <section class="categories">
    <h2>Expense Categories</h2>
    <div class="categories__form">
      <input v-model="newCategory.name" placeholder="Name" />
      <input v-model="newCategory.description" placeholder="Description" />
      <input v-model="newCategory.iconName" placeholder="Icon name (optional)" />
      <input type="color" v-model="newCategory.colorCode" />
      <button @click="toggleType" :class="{ 'expense-btn': newCategory.isExpense, 'income-btn': !newCategory.isExpense }">
        {{ newCategory.isExpense ? 'Expense' : 'Income' }}
      </button>
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
          <th>Type</th>
          <th>Icon</th>
          <th>Color</th>
          <th>Active</th>
          <th>Created</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="cat in categories" :key="cat.id">
          <td>{{ cat.name }}</td>
          <td>{{ cat.description }}</td>
          <td :class="{ 'expense-text': cat.isExpense, 'income-text': !cat.isExpense }">
            {{ cat.isExpense ? 'Expense' : 'Income' }}
          </td>
          <td>{{ cat.iconName || '-' }}</td>
          <td>
            <div class="color-box" :style="{ backgroundColor: cat.colorCode || '#ccc' }"></div>
          </td>
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
    flex-wrap: wrap;
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

  .expense-btn {
    background-color: #e74c3c;
  }

  .income-btn {
    background-color: #2ecc71;
  }

  .expense-text {
    color: #e74c3c;
    font-weight: bold;
  }

  .income-text {
    color: #2ecc71;
    font-weight: bold;
  }

  .color-box {
    width: 20px;
    height: 20px;
    border-radius: 3px;
    margin: 0 auto;
  }
}
</style>
