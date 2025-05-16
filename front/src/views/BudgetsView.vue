<script>
import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

export default {
  name: 'BudgetsView',
  data() {
    return {
      budgets: [],
      categories: [],
      newBudget: { 
        expenseCategoryId: null,
        amountAssigned: null,
        startDate: new Date().toISOString().substr(0, 10),
        endDate: new Date(new Date().setMonth(new Date().getMonth() + 1)).toISOString().substr(0, 10),
        isActive: true 
      }
    }
  },
  async created() {
    await this.loadCategories()
    await this.loadBudgets()
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
    async loadBudgets() {
      try {
        const config = useAuthStore().authHeader
        const { data } = await axios.get('/api/budgets', config)
        this.budgets = data
      } catch (err) {
        console.error(err)
        alert('Error loading budgets')
      }
    },
    async createBudget() {
      if (!this.newBudget.expenseCategoryId || !this.newBudget.amountAssigned) return
      try {
        const config = useAuthStore().authHeader
        const { data } = await axios.post('/api/budgets', this.newBudget, config)
        this.budgets.push(data)
        this.newBudget = { 
          expenseCategoryId: null,
          amountAssigned: null,
          startDate: new Date().toISOString().substr(0, 10),
          endDate: new Date(new Date().setMonth(new Date().getMonth() + 1)).toISOString().substr(0, 10),
          isActive: true 
        }
      } catch (err) {
        console.error(err)
        alert('Error creating budget')
      }
    },
    async deleteBudget(id) {
      if (!confirm('Delete this budget?')) return
      try {
        const config = useAuthStore().authHeader
        await axios.delete(`/api/budgets/${id}`, config)
        this.budgets = this.budgets.filter(b => b.id !== id)
      } catch (err) {
        console.error(err)
        alert('Error deleting budget')
      }
    },
    getProgressPercentage(budget) {
      if (!budget.amountAssigned) return 0
      const percentage = (budget.amountSpent / budget.amountAssigned) * 100
      return Math.min(percentage, 100)
    },
    formatCurrency(amount) {
      return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(amount)
    }
  }
}
</script>

<template>
  <section class="budgets">
    <h2>Budgets</h2>
    <div class="budgets__form">
      <select v-model="newBudget.expenseCategoryId">
        <option disabled value="" selected>Select Category</option>
        <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
      </select>
      <input type="number" v-model.number="newBudget.amountAssigned" placeholder="Amount" min="0" step="0.01" />
      <div class="date-inputs">
        <label>Start: <input type="date" v-model="newBudget.startDate" /></label>
        <label>End: <input type="date" v-model="newBudget.endDate" /></label>
      </div>
      <label>
        <input type="checkbox" v-model="newBudget.isActive" /> Active
      </label>
      <button @click="createBudget">Add Budget</button>
    </div>
    
    <div class="budgets__grid">
      <div class="budget-card" v-for="budget in budgets" :key="budget.id">
        <div class="budget-card__header">
          <h3>{{ budget.expenseCategoryName }}</h3>
          <button class="delete-btn" @click="deleteBudget(budget.id)">×</button>
        </div>
        <div class="budget-card__dates">
          <span>{{ new Date(budget.startDate).toLocaleDateString() }} - {{ new Date(budget.endDate).toLocaleDateString() }}</span>
        </div>
        <div class="budget-card__amounts">
          <div>Budget: <strong>{{ formatCurrency(budget.amountAssigned) }}</strong></div>
          <div>Spent: <strong>{{ formatCurrency(budget.amountSpent) }}</strong></div>
          <div>Remaining: <strong>{{ formatCurrency(budget.amountAssigned - budget.amountSpent) }}</strong></div>
        </div>
        <div class="budget-card__progress">
          <div class="progress-bar">
            <div 
              class="progress-fill" 
              :style="{ width: `${getProgressPercentage(budget)}%`, backgroundColor: getProgressPercentage(budget) > 100 ? '#e74c3c' : '#42b983' }"
            ></div>
          </div>
          <div class="progress-text">{{ getProgressPercentage(budget).toFixed(0) }}% used</div>
        </div>
        <div class="budget-card__status" :class="{ 'active': budget.isActive, 'inactive': !budget.isActive }">
          {{ budget.isActive ? 'Active' : 'Inactive' }}
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped lang="scss">
@use "@/assets/styles/variables" as *;
@use "@/assets/styles/mixins" as *;

.budgets {
  padding: $spacing-unit;
  background-color: $background-color;
  font-family: $font-family-base;

  &__form {
    display: flex;
    flex-wrap: wrap;
    gap: $spacing-unit;
    margin-bottom: $spacing-unit * 2;
    align-items: center;
    
    select, input[type="number"] {
      padding: 8px;
      border-radius: 4px;
      border: 1px solid #ddd;
    }
    
    .date-inputs {
      display: flex;
      gap: 10px;
    }
  }

  &__grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
    gap: $spacing-unit;
  }

  .budget-card {
    @include card;
    position: relative;
    
    &__header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: $spacing-unit / 2;
      
      h3 {
        margin: 0;
      }
      
      .delete-btn {
        background: none;
        border: none;
        font-size: 20px;
        cursor: pointer;
        color: #888;
        padding: 0 5px;
        
        &:hover {
          color: #e74c3c;
        }
      }
    }
    
    &__dates {
      color: #666;
      font-size: 0.9rem;
      margin-bottom: $spacing-unit / 2;
    }
    
    &__amounts {
      margin-bottom: $spacing-unit / 2;
      
      div {
        margin: 5px 0;
      }
    }
    
    &__progress {
      margin: $spacing-unit / 2 0;
      
      .progress-bar {
        height: 10px;
        background-color: #eee;
        border-radius: 5px;
        overflow: hidden;
        
        .progress-fill {
          height: 100%;
          transition: width 0.3s ease;
        }
      }
      
      .progress-text {
        text-align: right;
        font-size: 0.8rem;
        margin-top: 2px;
      }
    }
    
    &__status {
      position: absolute;
      top: 5px;
      right: 35px;
      font-size: 0.7rem;
      padding: 2px 8px;
      border-radius: 10px;
      
      &.active {
        background-color: #e8f5e9;
        color: #43a047;
      }
      
      &.inactive {
        background-color: #ffebee;
        color: #e53935;
      }
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
