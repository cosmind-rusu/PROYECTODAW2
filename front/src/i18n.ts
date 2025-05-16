import { createI18n } from 'vue-i18n'
import en from './en.json'

// Traducciones para español
const es = {
  appName: "VetClinic",
  dashboard: "Dashboard",
  about: "About",
  login: {
    title: "Login",
    email: "Email",
    password: "Password",
    submit: "Login",
    error: "Login failed"
  },
  register: {
    title: "Register",
    email: "Email",
    password: "Password",
    confirmPassword: "Confirm Password",
    submit: "Register",
    error: "Registration failed"
  },
  logout: "Logout",
  categories: "Categories",
  transactions: "Transactions",
  stats: {
    categories: "Categories",
    transactions: "Transactions"
  },
  noData: "No data"
}

const messages = {
  en,
  es
}

const i18n = createI18n({
  legacy: false,         // Usamos la API Composition
  locale: 'en',          // Idioma por defecto
  fallbackLocale: 'en',  // Idioma de respaldo
  messages
})

export default i18n
