<script>
import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

export default {
  name: 'TransactionsView',
  data() {
    return {
      categories: [],
      transactions: [],
      newTransaction: {
        expenseCategoryId: null,
        amount: null,
        description: '',
        transactionDate: new Date().toISOString().substr(0, 10),
        isExpense: true
      }
    }
  },
  async created() {
    await this.loadCategories()
    await this.loadTransactions()
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
    async loadTransactions() {
      try {
        const config = useAuthStore().authHeader
        const { data } = await axios.get('/api/transactions', config)
        this.transactions = data
      } catch (err) {
        console.error(err)
        alert('Error loading transactions')
      }
    },
    async createTransaction() {
      if (!this.newTransaction.expenseCategoryId || !this.newTransaction.amount) return
      try {
        const config = useAuthStore().authHeader
        const payload = {
          expenseCategoryId: this.newTransaction.expenseCategoryId,
          amount: this.newTransaction.amount,
          description: this.newTransaction.description,
          transactionDate: this.newTransaction.transactionDate,
          isExpense: this.newTransaction.isExpense
        }
        const { data } = await axios.post('/api/transactions', payload, config)
        this.transactions.push(data)
        this.newTransaction = {
          expenseCategoryId: null,
          amount: null,
          description: '',
          transactionDate: new Date().toISOString().substr(0, 10),
          isExpense: true
        }
      } catch (err) {
        console.error(err)
        alert('Error creating transaction')
      }
    }
  }
}
</script>

<template>
  <section class="transactions">
    <h2>Transactions</h2>
    <div class="transactions__form">
      <select v-model="newTransaction.expenseCategoryId">
        <option disabled value="">Select category</option>
        <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
      </select>
      <input type="number" v-model.number="newTransaction.amount" placeholder="Amount" />
      <input type="date" v-model="newTransaction.transactionDate" />
      <input type="text" v-model="newTransaction.description" placeholder="Description" />
      <label>
        <input type="checkbox" v-model="newTransaction.isExpense" /> Expense
      </label>
      <button @click="createTransaction">Add Transaction</button>
    </div>
    <table class="transactions__table">
      <thead>
        <tr>
          <th>Category</th>
          <th>Amount</th>
          <th>Date</th>
          <th>Description</th>
          <th>Expense</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="tx in transactions" :key="tx.id">
          <td>{{ categories.find(c => c.id === tx.expenseCategoryId)?.name }}</td>
          <td :class="{ 'expense-amount': tx.isExpense, 'income-amount': !tx.isExpense }">
            {{ tx.isExpense ? '-' : '+' }}{{ tx.amount }}
          </td>
          <td>{{ new Date(tx.transactionDate).toLocaleDateString() }}</td>
          <td>{{ tx.description }}</td>
          <td>{{ tx.isExpense ? 'Expense' : 'Income' }}</td>
        </tr>
      </tbody>
    </table>
  </section>
</template>

<style scoped lang="scss">
@use "@/assets/styles/variables" as *;
@use "@/assets/styles/mixins" as *;

.transactions {
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
  
  .expense-amount {
    color: #e74c3c;
    font-weight: bold;
  }
  
  .income-amount {
    color: #2ecc71;
    font-weight: bold;
  }
}
</style>
