<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

const props = defineProps<{
  accountId: number
}>()

const emit = defineEmits(['update'])

// Interfaces
interface Transaction {
  id: number
  categoryId: number
  amount: number
  description: string
  transactionDate: string
  isExpense: boolean
}

// Estado reactivo
const transactions = ref<Transaction[]>([])
const loading = ref(false)
const showAddForm = ref(false)
const newTransaction = ref({
  amount: 0,
  description: '',
  transactionDate: new Date().toISOString().split('T')[0],
  isExpense: false
})

// Cargar transacciones cuando cambia el accountId
watch(() => props.accountId, async () => {
  if (props.accountId) {
    await loadTransactions()
  }
})

onMounted(async () => {
  if (props.accountId) {
    await loadTransactions()
  }
})

// Cargar transacciones de la cuenta específica
async function loadTransactions() {
  if (!props.accountId) return
  
  loading.value = true
  try {
    const config = useAuthStore().authHeader
    const { data } = await axios.get('/api/transactions', config)
    // Filtrar transacciones por categoryId (accountId)
    transactions.value = data.filter((t: Transaction) => t.categoryId === props.accountId)
  } catch (err) {
    console.error('Error loading transactions:', err)
  } finally {
    loading.value = false
  }
}

// Añadir nueva transacción
async function addTransaction() {
  if (!newTransaction.value.amount || !newTransaction.value.description) return
  
  try {
    const config = useAuthStore().authHeader
    const payload = {
      ...newTransaction.value,
      categoryId: props.accountId,
      amount: Number(newTransaction.value.amount)
    }
    
    const { data } = await axios.post('/api/transactions', payload, config)
    transactions.value.push(data)
    resetForm()
    emit('update') // Notificar al componente padre para actualizar balances
  } catch (err) {
    console.error('Error adding transaction:', err)
    alert('Error adding transaction')
  }
}

// Eliminar transacción
async function deleteTransaction(id: number) {
  if (!confirm('Delete this transaction?')) return
  
  try {
    const config = useAuthStore().authHeader
    await axios.delete(`/api/transactions/${id}`, config)
    transactions.value = transactions.value.filter(t => t.id !== id)
    emit('update') // Notificar al componente padre para actualizar balances
  } catch (err) {
    console.error('Error deleting transaction:', err)
    alert('Error deleting transaction')
  }
}

// Resetear formulario
function resetForm() {
  newTransaction.value = {
    amount: 0,
    description: '',
    transactionDate: new Date().toISOString().split('T')[0],
    isExpense: false
  }
  showAddForm.value = false
}

// Calcular balance total
const balance = computed(() => {
  return transactions.value.reduce((total, t) => {
    return total + (t.isExpense ? -t.amount : t.amount)
  }, 0)
})

// Formatear fecha
function formatDate(dateString: string) {
  return new Date(dateString).toLocaleDateString()
}
</script>

<template>
  <div class="account-transactions">
    <div class="header">
      <h3>Account Transactions</h3>
      <div class="actions">
        <button v-if="!showAddForm" @click="showAddForm = true" class="btn-primary btn-small">
          Add Transaction
        </button>
        <button v-else @click="showAddForm = false" class="btn-secondary btn-small">
          Cancel
        </button>
      </div>
    </div>
    
    <!-- Formulario para añadir transacción -->
    <form v-if="showAddForm" @submit.prevent="addTransaction" class="transaction-form">
      <div class="form-row">
        <div class="form-group">
          <label for="amount">Amount</label>
          <input 
            id="amount" 
            v-model.number="newTransaction.amount" 
            type="number" 
            step="0.01" 
            required 
            min="0.01"
          />
        </div>
        
        <div class="form-group">
          <label for="transactionDate">Date</label>
          <input 
            id="transactionDate" 
            v-model="newTransaction.transactionDate" 
            type="date" 
            required
          />
        </div>
        
        <div class="form-group checkbox">
          <input 
            id="isExpense" 
            v-model="newTransaction.isExpense" 
            type="checkbox"
          />
          <label for="isExpense">Is Expense</label>
        </div>
      </div>
      
      <div class="form-group">
        <label for="description">Description</label>
        <input 
          id="description" 
          v-model="newTransaction.description" 
          required 
          placeholder="Transaction description"
        />
      </div>
      
      <div class="form-actions">
        <button type="submit" class="btn-primary btn-small">Save Transaction</button>
        <button type="button" @click="resetForm" class="btn-secondary btn-small">Cancel</button>
      </div>
    </form>
    
    <!-- Lista de transacciones -->
    <div v-if="loading" class="loading">Loading transactions...</div>
    
    <div v-else-if="transactions.length === 0" class="empty-state">
      <p>No transactions found for this account.</p>
    </div>
    
    <div v-else class="transactions-list">
      <table>
        <thead>
          <tr>
            <th>Date</th>
            <th>Description</th>
            <th>Amount</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="transaction in transactions" :key="transaction.id">
            <td>{{ formatDate(transaction.transactionDate) }}</td>
            <td>{{ transaction.description }}</td>
            <td :class="{ 'amount-positive': !transaction.isExpense, 'amount-negative': transaction.isExpense }">
              {{ transaction.isExpense ? '-' : '+' }}{{ transaction.amount.toFixed(2) }}€
            </td>
            <td>
              <button @click="deleteTransaction(transaction.id)" class="btn-danger btn-small">Delete</button>
            </td>
          </tr>
        </tbody>
        <tfoot>
          <tr>
            <td colspan="2"><strong>Balance:</strong></td>
            <td :class="{ 'amount-positive': balance > 0, 'amount-negative': balance < 0 }">
              <strong>{{ balance.toFixed(2) }}€</strong>
            </td>
            <td></td>
          </tr>
        </tfoot>
      </table>
    </div>
  </div>
</template>

<style scoped lang="scss">
@use "@/assets/styles/variables" as *;
@use "@/assets/styles/mixins" as *;

.account-transactions {
  margin-top: $spacing-unit;
  padding: $spacing-unit;
  background-color: lighten($background-color, 3%);
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.05);
  
  .header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: $spacing-unit;
    
    h3 {
      margin: 0;
      color: $secondary-color;
    }
  }
  
  .transaction-form {
    background-color: lighten($background-color, 5%);
    padding: $spacing-unit;
    border-radius: 4px;
    margin-bottom: $spacing-unit;
    
    .form-row {
      display: flex;
      gap: $spacing-unit;
      margin-bottom: $spacing-unit / 2;
    }
    
    .form-group {
      flex: 1;
      
      label {
        display: block;
        margin-bottom: 4px;
        font-size: 0.9rem;
      }
      
      input:not([type="checkbox"]) {
        width: 100%;
        padding: 8px;
        border: 1px solid #ddd;
        border-radius: 4px;
      }
      
      &.checkbox {
        display: flex;
        align-items: center;
        
        input {
          margin-right: 8px;
        }
      }
    }
    
    .form-actions {
      display: flex;
      gap: $spacing-unit / 2;
      justify-content: flex-end;
      margin-top: $spacing-unit / 2;
    }
  }
  
  .transactions-list {
    table {
      width: 100%;
      border-collapse: collapse;
      
      th {
        text-align: left;
        padding: 8px;
        background-color: $primary-color;
        color: white;
        font-size: 0.9rem;
      }
      
      td {
        padding: 8px;
        border-bottom: 1px solid #eee;
        font-size: 0.9rem;
      }
      
      tfoot {
        td {
          border-top: 2px solid #ddd;
          border-bottom: none;
          padding-top: 12px;
        }
      }
      
      .amount-positive {
        color: #28a745;
      }
      
      .amount-negative {
        color: #dc3545;
      }
    }
  }
  
  .empty-state {
    text-align: center;
    padding: $spacing-unit;
    font-style: italic;
    color: #666;
  }
  
  .loading {
    text-align: center;
    padding: $spacing-unit;
    color: $secondary-color;
    font-style: italic;
  }
}

.btn-primary, .btn-secondary, .btn-danger {
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
  
  &.btn-small {
    padding: 4px 8px;
    font-size: 0.85rem;
  }
}

.btn-primary {
  background-color: $primary-color;
  color: white;
  
  &:hover {
    background-color: darken($primary-color, 5%);
  }
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
  
  &:hover {
    background-color: darken(#6c757d, 5%);
  }
}

.btn-danger {
  background-color: #dc3545;
  color: white;
  
  &:hover {
    background-color: darken(#dc3545, 5%);
  }
}
</style>
