<script setup lang="ts">
import { ref, onMounted } from 'vue';
import axios from 'axios';

const categories = ref<any[]>([]);
const transactions = ref<any[]>([]);

const fetchData = async () => {
  try {
    const resCat = await axios.get('/api/categories');
    categories.value = resCat.data;
    const resTxn = await axios.get('/api/transactions');
    transactions.value = resTxn.data;
  } catch (err) {
    console.error(err);
  }
};

onMounted(fetchData);
</script>

<template>
  <main class="dashboard">
    <h2>Dashboard</h2>
    <section class="stats">
      <div class="stat-card">
        <h3>Categories</h3>
        <p>{{ categories.length }}</p>
      </div>
      <div class="stat-card">
        <h3>Transactions</h3>
        <p>{{ transactions.length }}</p>
      </div>
    </section>
    <section class="lists">
      <div class="list categories-list">
        <h4>Categories</h4>
        <ul>
          <li v-for="cat in categories" :key="cat.id">{{ cat.name }}</li>
        </ul>
      </div>
      <div class="list transactions-list">
        <h4>Transactions</h4>
        <ul>
          <li v-for="tx in transactions" :key="tx.id">
            {{ tx.description }} - {{ tx.amount }}
          </li>
        </ul>
      </div>
    </section>
  </main>
</template>

<style scoped lang="scss">
.dashboard {
  padding: 1rem;
  .stats {
    display: flex;
    gap: 1rem;
    .stat-card {
      padding: 1rem;
      background: #f5f5f5;
      border-radius: 4px;
      flex: 1;
      text-align: center;
    }
  }
  .lists {
    display: flex;
    gap: 2rem;
    margin-top: 2rem;
    .list {
      flex: 1;
      h4 {
        margin-bottom: 0.5rem;
      }
      ul {
        list-style: none;
        padding: 0;
        li {
          padding: 0.25rem 0;
          border-bottom: 1px solid #ddd;
        }
      }
    }
  }
}
</style>
