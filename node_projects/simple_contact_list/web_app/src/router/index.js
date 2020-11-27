import Vue from 'vue'
import VueRouter from 'vue-router'

Vue.use(VueRouter)

// const EventBus = new Vue();

const routes = [
  {
    path: '/',
    name: 'Contacts',
    component: () => import('../components/Contacts.vue'),
    meta: {
      requiresAuth: true
    }
  },
  {
    path: '/addContact',
    name: 'Add Contacts',
    component: () => import('../components/AddContacts.vue'),
    meta: {
      requiresAuth: true
    }
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import ('../components/Login.vue'),
    meta: {
      requiresAuth: false
    }
  },
  {
    path: '/register',
    name: 'Register',
    component: () => import ('../components/Register.vue'),
    meta: {
      requiresAuth: false
    }
  },

]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

router.beforeEach((to, from, next) => {
  if (to.matched.some(record => record.meta.requiresAuth)) {
    if (router.app.$session.exists()) {

      next();
    } else {
      next('/login');
    }
  } else {
    next();
  }
})

export default router;  // EventBus}
