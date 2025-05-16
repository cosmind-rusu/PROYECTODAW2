<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

// Definir la interfaz para una cuenta (Category en el backend)
interface Account {
  id: number
  name: string
  description: string
  isActive: boolean
  balance?: number // Balance calculado
  createdDate: string
}

// Referencias reactivas
const accounts = ref<Account[]>([])
const transactions = ref<any[]>([])
const newAccount = ref({
  name: '',
  description: '',
  isActive: true
})

const editingAccount = ref<Account | null>(null)
const loading = ref({
  accounts: false,
  transactions: false
})
const showForm = ref(false)

// Cargar cuentas al montar el componente
onMounted(async () => {
  await loadAccounts()
  await loadTransactions() // Cargar transacciones para calcular balances
})

// Método para cargar cuentas desde la API
async function loadAccounts() {
  loading.value.accounts = true
  try {
    const config = useAuthStore().authHeader
    const { data } = await axios.get('/api/categories', config)
    accounts.value = data
  } catch (err) {
    console.error('Error loading accounts:', err)
    alert('Error loading accounts')
  } finally {
    loading.value.accounts = false
  }
}

// Método para cargar transacciones (para calcular balances)
async function loadTransactions() {
  loading.value.transactions = true
  try {
    const config = useAuthStore().authHeader
    const { data } = await axios.get('/api/transactions', config)
    transactions.value = data
    calculateBalances()
  } catch (err) {
    console.error('Error loading transactions:', err)
  } finally {
    loading.value.transactions = false
  }
}

// Calcular balance para cada cuenta basado en transacciones
function calculateBalances() {
  accounts.value.forEach(account => {
    const accountTransactions = transactions.value.filter(t => t.categoryId === account.id)
    let balance = 0
    accountTransactions.forEach(t => {
      if (t.isExpense) {
        balance -= t.amount
      } else {
        balance += t.amount
      }
    })
    account.balance = balance
  })
}

// Crear una nueva cuenta
async function createAccount() {
  if (!newAccount.value.name) return
  try {
    const config = useAuthStore().authHeader
    const { data } = await axios.post('/api/categories', newAccount.value, config)
    accounts.value.push({...data, balance: 0})
    newAccount.value = { name: '', description: '', isActive: true }
    showForm.value = false
  } catch (err) {
    console.error('Error creating account:', err)
    alert('Error creating account')
  }
}

// Comenzar a editar una cuenta
function startEdit(account: Account) {
  editingAccount.value = { ...account }
}

// Cancelar edición
function cancelEdit() {
  editingAccount.value = null
}

// Guardar cambios en una cuenta
async function updateAccount() {
  if (!editingAccount.value) return
  try {
    const config = useAuthStore().authHeader
    await axios.put(`/api/categories/${editingAccount.value.id}`, editingAccount.value, config)
    
    // Actualizar la cuenta en el array local
    const index = accounts.value.findIndex(a => a.id === editingAccount.value?.id)
    if (index >= 0) {
      accounts.value[index] = { ...editingAccount.value }
    }
    editingAccount.value = null
  } catch (err) {
    console.error('Error updating account:', err)
    alert('Error updating account')
  }
}

// Eliminar una cuenta
async function deleteAccount(id: number) {
  if (!confirm('Delete this account? This will NOT delete associated transactions.')) return
  try {
    const config = useAuthStore().authHeader
    await axios.delete(`/api/categories/${id}`, config)
    accounts.value = accounts.value.filter(a => a.id !== id)
  } catch (err) {
    console.error('Error deleting account:', err)
    alert('Error deleting account')
  }
}

// Cuentas activas para mostrar
const activeAccounts = computed(() => {
  return accounts.value.filter(a => a.isActive)
})

// Cuentas inactivas para mostrar
const inactiveAccounts = computed(() => {
  return accounts.value.filter(a => !a.isActive)
})

// Total balance
const totalBalance = computed(() => {
  return accounts.value.reduce((total, account) => total + (account.balance || 0), 0)
})
</script>

<template>
  <section class="accounts">
    <h1>Accounts Manager</h1>
    
    <div class="accounts__overview">
      <div class="accounts__summary">
        <h2>Balance Overview</h2>
        <p class="total-balance" :class="{ positive: totalBalance > 0, negative: totalBalance < 0 }">
          Total Balance: {{ totalBalance.toFixed(2) }}€
        </p>
      </div>
    
      <div class="accounts__actions">
        <button v-if="!showForm" @click="showForm = true" class="btn-primary">
          Add New Account
        </button>
        <button v-else @click="showForm = false" class="btn-secondary">
          Cancel
        </button>
      </div>
    </div>

    <!-- Form for adding new accounts -->
    <form v-if="showForm" @submit.prevent="createAccount" class="accounts__form">
      <div class="form-group">
        <label for="name">Account Name</label>
        <input id="name" v-model="newAccount.name" required placeholder="e.g. Savings Account" />
      </div>
      <div class="form-group">
        <label for="description">Description</label>
        <input id="description" v-model="newAccount.description" placeholder="Optional description" />
      </div>
      <div class="form-group checkbox">
        <input type="checkbox" id="isActive" v-model="newAccount.isActive" />
        <label for="isActive">Active Account</label>
      </div>
      <div class="form-actions">
        <button type="submit" class="btn-primary">Save Account</button>
        <button type="button" @click="showForm = false" class="btn-secondary">Cancel</button>
      </div>
    </form>

    <div v-if="loading.accounts" class="loading">Loading accounts...</div>
    
    <!-- Active accounts table -->
    <div v-if="activeAccounts.length > 0" class="accounts__list">
      <h2>Active Accounts</h2>
      <table>
        <thead>
          <tr>
            <th>Account Name</th>
            <th>Description</th>
            <th>Balance</th>
            <th>Created</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="account in activeAccounts" :key="account.id" 
              :class="{ 'being-edited': editingAccount && editingAccount.id === account.id }">
            <template v-if="editingAccount && editingAccount.id === account.id">
              <td><input v-model="editingAccount.name" /></td>
              <td><input v-model="editingAccount.description" /></td>
              <td :class="{ positive: account.balance > 0, negative: account.balance < 0 }">
                {{ account.balance?.toFixed(2) || '0.00' }}€
              </td>
              <td>{{ new Date(account.createdDate).toLocaleDateString() }}</td>
              <td>
                <button @click="updateAccount" class="btn-small">Save</button>
                <button @click="cancelEdit" class="btn-small btn-secondary">Cancel</button>
              </td>
            </template>
            <template v-else>
              <td>{{ account.name }}</td>
              <td>{{ account.description }}</td>
              <td :class="{ positive: account.balance > 0, negative: account.balance < 0 }">
                {{ account.balance?.toFixed(2) || '0.00' }}€
              </td>
              <td>{{ new Date(account.createdDate).toLocaleDateString() }}</td>
              <td>
                <button @click="startEdit(account)" class="btn-small">Edit</button>
                <button @click="deleteAccount(account.id)" class="btn-small btn-danger">Delete</button>
              </td>
            </template>
          </tr>
        </tbody>
      </table>
    </div>
    
    <!-- Inactive accounts table (collapsed by default) -->
    <div v-if="inactiveAccounts.length > 0" class="accounts__list inactive">
      <details>
        <summary>Inactive Accounts ({{ inactiveAccounts.length }})</summary>
        <table>
          <thead>
            <tr>
              <th>Account Name</th>
              <th>Description</th>
              <th>Balance</th>
              <th>Created</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="account in inactiveAccounts" :key="account.id" 
                :class="{ 'being-edited': editingAccount && editingAccount.id === account.id }">
              <template v-if="editingAccount && editingAccount.id === account.id">
                <td><input v-model="editingAccount.name" /></td>
                <td><input v-model="editingAccount.description" /></td>
                <td :class="{ positive: account.balance > 0, negative: account.balance < 0 }">
                  {{ account.balance?.toFixed(2) || '0.00' }}€
                </td>
                <td>{{ new Date(account.createdDate).toLocaleDateString() }}</td>
                <td>
                  <button @click="updateAccount" class="btn-small">Save</button>
                  <button @click="cancelEdit" class="btn-small btn-secondary">Cancel</button>
                </td>
              </template>
              <template v-else>
                <td>{{ account.name }}</td>
                <td>{{ account.description }}</td>
                <td :class="{ positive: account.balance > 0, negative: account.balance < 0 }">
                  {{ account.balance?.toFixed(2) || '0.00' }}€
                </td>
                <td>{{ new Date(account.createdDate).toLocaleDateString() }}</td>
                <td>
                  <button @click="startEdit(account)" class="btn-small">Edit</button>
                  <button @click="deleteAccount(account.id)" class="btn-small btn-danger">Delete</button>
                </td>
              </template>
            </tr>
          </tbody>
        </table>
      </details>
    </div>

    <div v-if="accounts.length === 0 && !loading.accounts" class="empty-state">
      <p>No accounts found. Create your first account to get started!</p>
      <button @click="showForm = true" class="btn-primary">Add New Account</button>
    </div>
  </section>
</template>

<style scoped lang="scss">
@use "@/assets/styles/variables" as *;
@use "@/assets/styles/mixins" as *;

.accounts {
  padding: $spacing-unit;
  max-width: 1200px;
  margin: 0 auto;
  
  h1 {
    margin-bottom: $spacing-unit;
    color: $primary-color;
  }

  h2 {
    margin-bottom: $spacing-unit / 2;
    color: $secondary-color;
  }

  &__overview {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: $spacing-unit;
    padding: $spacing-unit;
    background-color: lighten($background-color, 2%);
    border-radius: 8px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.05);
  }

  &__summary {
    .total-balance {
      font-size: 1.5rem;
      font-weight: bold;
      
      &.positive {
        color: #28a745; // Verde para balances positivos
      }
      
      &.negative {
        color: #dc3545; // Rojo para balances negativos
      }
    }
  }

  &__actions {
    display: flex;
    gap: $spacing-unit / 2;
  }
  
  &__form {
    background-color: lighten($background-color, 5%);
    padding: $spacing-unit;
    border-radius: 8px;
    margin-bottom: $spacing-unit;
    box-shadow: 0 2px 6px rgba(0,0,0,0.1);
    
    .form-group {
      margin-bottom: $spacing-unit / 2;
      
      label {
        display: block;
        margin-bottom: 4px;
        font-weight: 500;
      }
      
      input[type="text"], 
      input[type="number"],
      input:not([type="checkbox"]) {
        width: 100%;
        padding: $spacing-unit / 2;
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
      justify-content: flex-start;
      gap: $spacing-unit / 2;
      margin-top: $spacing-unit;
    }
  }
  
  &__list {
    margin-top: $spacing-unit;
    
    table {
      width: 100%;
      border-collapse: collapse;
      margin-bottom: $spacing-unit;
      
      th {
        text-align: left;
        padding: $spacing-unit / 2;
        background-color: $primary-color;
        color: white;
      }
      
      td {
        padding: $spacing-unit / 2;
        border-bottom: 1px solid #eee;
        
        &.positive {
          color: #28a745;
          font-weight: bold;
        }
        
        &.negative {
          color: #dc3545;
          font-weight: bold;
        }
      }
      
      tr:hover:not(.being-edited) {
        background-color: lighten($background-color, 3%);
      }
      
      tr.being-edited {
        background-color: lighten($background-color, 5%);
        
        td {
          padding: ($spacing-unit / 4);
          
          input {
            width: 100%;
            padding: ($spacing-unit / 4);
            border: 1px solid #ccc;
            border-radius: 4px;
          }
        }
      }
    }
    
    &.inactive {
      opacity: 0.8;
      
      summary {
        cursor: pointer;
        padding: $spacing-unit / 2;
        background-color: lighten($background-color, 2%);
        border-radius: 4px;
        font-weight: 500;
        margin-bottom: $spacing-unit / 2;
      }
      
      details[open] summary {
        margin-bottom: $spacing-unit;
      }
    }
  }
  
  .empty-state {
    text-align: center;
    padding: $spacing-unit * 2;
    
    p {
      margin-bottom: $spacing-unit;
      font-style: italic;
      color: #666;
    }
  }
  
  .loading {
    text-align: center;
    padding: $spacing-unit;
    color: $secondary-color;
    font-style: italic;
  }
}

// Estilos de botones
.btn-primary {
  background-color: $primary-color;
  color: white;
  border: none;
  padding: ($spacing-unit / 2) $spacing-unit;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
  
  &:hover {
    background-color: darken($primary-color, 5%);
  }
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
  border: none;
  padding: ($spacing-unit / 2) $spacing-unit;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
  
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

.btn-small {
  padding: ($spacing-unit / 4) ($spacing-unit / 2);
  font-size: 0.9rem;
}
</style>
